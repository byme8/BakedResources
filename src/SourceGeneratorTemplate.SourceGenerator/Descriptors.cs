using Microsoft.CodeAnalysis;

namespace SourceGeneratorTemplate.SourceGenerator;

public class Descriptors
{
    public static DiagnosticDescriptor UnexpectedFail = new(
        nameof(UnexpectedFail),
        "Source generator failed unexpectedly",
        "Source generator failed unexpectedly with exception message:\n{0}",
        "Demo",
        DiagnosticSeverity.Error,
        true);
}