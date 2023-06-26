using System.Reflection;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorTemplate.Tests.Data;

public static class TestProject
{
    public const string ResultCalculation = "var result = 0; // execute place";
    public const string AdditionalCode = "// additional code";

    public const string TestAppProjectName = "SourceGeneratorTemplate.TestApp";

    static TestProject()
    {
        var manager = new AnalyzerManager();
        manager.GetProject(@$"../../../../{TestAppProjectName}/{TestAppProjectName}.csproj");
        Workspace = manager.GetWorkspace();

        Project = Workspace.CurrentSolution.Projects.First(o => o.Name == TestAppProjectName);
    }

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