using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Domain.Model.Tests.ProjectStructure;

public class ProjectStructureFileTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenBlankName_PathShouldRemainUnchanged(string? name)
    {
        var file = new ProjectStructureFile("path");

        var updatedFile = file.UpdateName(name);

        updatedFile.ShouldBe(file);
        updatedFile.Path.ShouldBe("path");
    }

    [Theory]
    [InlineData(".")]
    [InlineData("..")]
    public void GivenInvalidName_PathShouldRemainUnchanged(string name)
    {
        var file = new ProjectStructureFile("path");

        var updatedFile = file.UpdateName(name);

        updatedFile.ShouldBe(file);
        updatedFile.Path.ShouldBe("path");
    }

    [Theory]
    [InlineData("new-name")]
    [InlineData("\\new-name")]
    [InlineData(".\\new-name")]
    [InlineData(@"..\..\new-name")]
    [InlineData(@"..\folder\new-name")]
    [InlineData("/new-name")]
    [InlineData("../new-name")]
    [InlineData("./new-name")]
    [InlineData("../../new-name")]
    [InlineData("../folder/new-name")]
    public void GivenName_WhenPathIsNonRoot_PathFileNameShouldBeChanged(string? name)
    {
        var file = new ProjectStructureFile("path/file.ext");

        var updatedFile = file.UpdateName(name);

        updatedFile.Path.ShouldBe("path/new-name");
    }

    [Theory]
    [InlineData("new-name")]
    [InlineData("\\new-name")]
    [InlineData(".\\new-name")]
    [InlineData(@"..\..\new-name")]
    [InlineData(@"..\folder\new-name")]
    [InlineData("/new-name")]
    [InlineData("../new-name")]
    [InlineData("./new-name")]
    [InlineData("../../new-name")]
    [InlineData("../folder/new-name")]
    public void GivenName_WhenPathIsFromRoot_PathFileNameShouldBeChanged(string? name)
    {
        var file = new ProjectStructureFile("file.ext");

        var updatedFile = file.UpdateName(name);

        updatedFile.Path.ShouldBe("new-name");
    }
}
