using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorTemplate.SourceGenerator;

[Generator]
public class DemoSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var objects = context.CompilationProvider
            .Select((o, _) => o.GetSymbolsWithName(O => true).ToArray());

        context.RegisterSourceOutput(objects, Generate);
    }

    private void Generate(SourceProductionContext context, ISymbol[] input)
    {
        var symbolNames = input.Select(o => o.Name).ToArray();
        var source = $$"""
            using System;

            namespace SourceGeneratorTemplate.SourceGenerator
            {
                public static class DemoSourceGenerator
                {
                    {{symbolNames.Select(o => $@"       public static string {o} => ""{o}"";").JoinWithNewLine()}}
                }
            }
            """;

        context.AddSource("DemoSourceGenerator.cs", source);
    }
}