using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BakedResources.SourceGenerator;

[Generator]
public class BakedResourcesSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var options = context.AnalyzerConfigOptionsProvider
            .Select((o, c) => o.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir)
                ? projectDir
                : Environment.CurrentDirectory);

        var additionalText = context.AdditionalTextsProvider
            .Select((o, _) => o)
            .Where(o => (o.Path.Contains(".baked.") || o.Path.Contains(".b.")) 
                        && !o.Path.EndsWith(".g.cs"));
    
        var fileAndOptions = additionalText
            .Combine(options);

        var filesAndOptions = additionalText.Collect()
            .Combine(options);

        context.RegisterSourceOutput(fileAndOptions,
            (ctx, i) => Generate(ctx, i.Left, i.Right));

        context.RegisterImplementationSourceOutput(filesAndOptions,
            (ctx, i) => GenerateMapping(ctx, i.Left, i.Right));
    }

    private void GenerateMapping(SourceProductionContext ctx, ImmutableArray<AdditionalText> additionalTexts,
        string projectPath)
    {

        var initializers = new List<string>();
        foreach (var additionalFile in additionalTexts)
        {
            if (ctx.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var (filePath, fileExtension, propertyPath) = GetFilePath(additionalFile, projectPath);
            var content = additionalFile.GetText(ctx.CancellationToken)!
                .ToString()
                .Replace("\r\n", "\n");

            var first = propertyPath.First();
            if (first is "bin" or "obj")
            {
                continue;
            }

            var literal = SyntaxFactory
                .Literal(content)
                .ToFullString();

            var initializer = $"{fileExtension.UpperFirstChar()}Files.{propertyPath.Join(".")} = {literal};";
            initializers.Add(initializer);
        }

        var projectName = projectPath
            .ToPathSegments()
            .Last();

        var source = $$"""
            using System;

            namespace BakedResources.{{projectName}}.Internal
            {
                [global::System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public static class BakedResourcesInitializer
                {
                    [global::System.Runtime.CompilerServices.ModuleInitializerAttribute]
                    public static void Initialize()
                    {
                        {{initializers.JoinWithNewLine()}}
                    }
                }
            }
            """;

        if (ctx.CancellationToken.IsCancellationRequested)
        {
            return;
        }

        var formattedSource = SyntaxFactory.ParseCompilationUnit(source)
            .NormalizeWhitespace()
            .ToFullString();

        if (ctx.CancellationToken.IsCancellationRequested)
        {
            return;
        }

        ctx.AddSource("BakedResources.g.cs", formattedSource);
    }

    private void Generate(
        SourceProductionContext context,
        AdditionalText additionalFile,
        string projectPath)
    {
        var (filePath, fileExtension, propertyPath) = GetFilePath(additionalFile, projectPath);

        var classHierarchy = CreateHierarchy(propertyPath, 0);

        var projectName = projectPath
            .ToPathSegments()
            .Last();

        var source = $$"""
            using System;

            namespace BakedResources.{{projectName}}
            {
                public static partial class {{fileExtension.UpperFirstChar()}}Files
                {
                    {{classHierarchy}}
                }
            }
            """;

        var formattedSource = SyntaxFactory.ParseCompilationUnit(source)
            .NormalizeWhitespace()
            .ToFullString();

        var sourceGeneratedFileName = filePath
            .Replace(Path.DirectorySeparatorChar, '.')
            .Trim('.');

        context.AddSource(sourceGeneratedFileName + ".g.cs", formattedSource);
    }

    private (string FilePath, string FileExtensions, string[] properyPath) GetFilePath(
        AdditionalText additionalFile,
        string projectPath)
    {
        var filePath = additionalFile.Path.Replace(projectPath, "");
        var fileExtension = Path.GetExtension(filePath).Trim('.');
        var filePathWithoutExtension = Path.ChangeExtension(filePath, null); // remove extension
        filePathWithoutExtension = Path.ChangeExtension(filePathWithoutExtension, null); // remove .baked.

        var propertyPath = filePathWithoutExtension.ToPathSegments();

        return (filePath, fileExtension, propertyPath);
    }

    private string CreateHierarchy(string[] propertyPath, int current)
    {
        var name = propertyPath[current];
        var isLast = current == propertyPath.Length - 1;
        var next = current + 1;

        if (isLast)
        {
            return $@"public static string {name} {{ get; set; }}";
        }

        return $$"""
                public static partial class {{name.UpperFirstChar()}}
                {
                    {{CreateHierarchy(propertyPath, next)}}
                }
                """;
    }
}