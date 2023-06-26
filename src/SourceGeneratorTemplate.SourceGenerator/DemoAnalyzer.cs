using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using LanguageNames = Microsoft.CodeAnalysis.LanguageNames;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace SourceGeneratorTemplate.SourceGenerator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DemoAnalyzer : DiagnosticAnalyzer
{
#pragma warning disable RS1026
    public override void Initialize(AnalysisContext context)
#pragma warning restore RS1026
    {
#if !DEBUG
            context.EnableConcurrentExecution();
#endif
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(Handle, SyntaxKind.InvocationExpression);
    }

    private void Handle(SyntaxNodeAnalysisContext context)
    {

    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = ImmutableArray.Create(Descriptors.UnexpectedFail);
}