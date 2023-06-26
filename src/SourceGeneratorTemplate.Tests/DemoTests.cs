using SourceGeneratorTemplate.Tests.Data;

namespace SourceGeneratorTemplate.Tests;

[UsesVerify]
public class DemoTests
{
    [Fact]
    public async Task CompilationWorks()
    {
        var result = await TestProject.Project.Execute("var result = 42;");

        await Verify(result);
    }
}