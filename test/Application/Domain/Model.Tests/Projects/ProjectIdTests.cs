using ScriptBee.Domain.Model.Projects;
using Shouldly;

namespace ScriptBee.Domain.Model.Tests.Projects;

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
    public void FromName_ShouldSlugifyName(string input, string expectedSlug)
    {
        var projectId = ProjectId.FromName(input);

        projectId.Value.ShouldBe(expectedSlug);
    }
}
