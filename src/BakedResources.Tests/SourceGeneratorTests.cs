using BakedResources.SourceGenerator;
using BakedResources.Tests.Data;

namespace BakedResources.Tests;

[UsesVerify]
public class SourceGeneratorTests
{
    [Fact]
    public async Task CompilationWorks()
    {
        var result =
            await TestProject.Project.Execute("var result = BakedResources.GraphqlFiles.Queries.Users.GetUsers;");

        await Verify(result);
    }

    [Fact]
    public async Task AccessorGenerated()
    {
        var result = await TestProject.Project.ApplyGenerator(new BakedResourcesSourceGenerator());
        await Verify(result.GeneratedTrees
            .ToDictionary(
                o => o.FilePath.Replace("\\", "/"),
                o => o.ToString()));
    }
}