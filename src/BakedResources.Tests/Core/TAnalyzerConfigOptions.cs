using BakedResources.Tests.Data;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BakedResources.Tests.Core;

public class TAnalyzerConfigOptions : AnalyzerConfigOptions
{
    private readonly Dictionary<string, string> values;

    public TAnalyzerConfigOptions()
    {
        values = new Dictionary<string, string>();
        values.Add("build_property.projectdir", Path.GetDirectoryName(TestProject.ProjectPath)!);
    }

    public override bool TryGetValue(string key, out string? value)
    {
        return values.TryGetValue(key, out value);
    }
}