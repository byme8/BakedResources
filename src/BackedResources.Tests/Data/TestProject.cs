using System.Reflection;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;

namespace BackedResources.Tests.Data;

public static class TestProject
{
    public const string ResultCalculation = "var result = 0; // execute place";
    public const string AdditionalCode = "// additional code";

    public const string TestAppProjectName = "BackedResources.TestApp";

    static TestProject()
    {
        ProjectPath = Path.GetFullPath(@$"../../../../{TestAppProjectName}/{TestAppProjectName}.csproj");
        var manager = new AnalyzerManager();

        var analyzer = manager.GetProject(ProjectPath);
        Workspace = manager.GetWorkspace();

        var results = analyzer.Build();
        var result = results.First()!;

        Project = Workspace.CurrentSolution.Projects.First(o => o.Name == TestAppProjectName);
        var projectRoot = Path.GetDirectoryName(Project.FilePath)!;
        foreach (var additionalFile in result.Items["BakedFiles"])
        {
            var path = Path.Combine(projectRoot, additionalFile.ItemSpec);
            Project = Project
                .AddAdditionalDocument(path, File.ReadAllText(path))
                .Project;
            
        }
    }

    public static string ProjectPath { get; set; }

    public static Project Project { get; }

    public static AdhocWorkspace Workspace { get; }

    public static async Task<object> Execute(this Project project, string source, string? additional = null)
    {
        var newProject = await Project
            .ReplacePartOfDocumentAsync("Program.cs", (ResultCalculation, source));

        if (!string.IsNullOrEmpty(additional))
        {
            newProject = await Project
                .ReplacePartOfDocumentAsync("Program.cs", (AdditionalCode, additional));
        }

        var assembly = await newProject.CompileToRealAssembly();
        var program = assembly.GetType($"{TestAppProjectName}.Program")!;
        var execute = program
            .GetMethod("Execute", BindingFlags.Static | BindingFlags.Public)!
            .CreateDelegate<Func<object>>();

        return execute.Invoke();
    }
}