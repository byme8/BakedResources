using BackedResources.SourceGenerator;
using BackedResources.Tests.Data;

namespace BackedResources.Tests;

[UsesVerify]
public class SourceGeneratorTests
{
    [Fact]
    public async Task CompilationWorks()
    {
        var result =
            await TestProject.Project.Execute("var result = BackedResources.GraphqlFiles.Queries.Users.GetUsers;");

        await Verify(result);
    }

    [Fact]
    public async Task AccessorGenerated()
    {
        var result = await TestProject.Project.ApplyGenerator(new BackedResourcesSourceGenerator());
        await Verify(result.GeneratedTrees
            .ToDictionary(
                o => o.FilePath,
                o => o.ToString()));
    }
}