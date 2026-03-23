using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Tests.Project;

public class ProjectIdTests
{
    [Theory]
    [InlineData("SimpleName", "simplename")]
    [InlineData("Name With Spaces", "name-with-spaces")]
    [InlineData("NameWith123Numbers", "namewith123numbers")]
    [InlineData("Special!@#$%^&*()Characters", "specialcharacters")]
    [InlineData("   LeadingAndTrailing Spaces   ", "leadingandtrailing-spaces")]
    [InlineData("MixeD Case StriNG", "mixed-case-string")]
    [InlineData("", "")]
    public void Create_ShouldSlugifyId(string input, string expectedSlug)
    {
        var projectId = ProjectId.Create(input);

        projectId.Value.ShouldBe(expectedSlug);
    }
}
