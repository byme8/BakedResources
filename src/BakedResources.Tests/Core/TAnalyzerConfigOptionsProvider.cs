using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BakedResources.Tests.Core;

public class TAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
    {
        return new TAnalyzerConfigOptions();
    }

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        return new TAnalyzerConfigOptions();
    }

    public override AnalyzerConfigOptions GlobalOptions { get; } = new TAnalyzerConfigOptions();
}