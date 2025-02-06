using ScriptBee.Domain.Model.Authorization;
using Shouldly;

namespace ScriptBee.Domain.Model.Tests.Authorization;

public class RolesTests
{
    [Theory]
    [InlineData("guest")]
    [InlineData("Guest")]
    [InlineData("GUEST")]
    public void CreateGuestRole(string type)
    {
        var role = UserRole.FromType(type);

        role.ShouldBe(UserRole.Guest);
    }

    [Theory]
    [InlineData("analyst")]
    [InlineData("Analyst")]
    [InlineData("ANALYST")]
    public void CreateAnalystRole(string type)
    {
        var role = UserRole.FromType(type);

        role.ShouldBe(UserRole.Analyst);
    }

    [Theory]
    [InlineData("maintainer")]
    [InlineData("Maintainer")]
    [InlineData("MAINTAINER")]
    public void CreateMaintainerRole(string type)
    {
        var role = UserRole.FromType(type);

        role.ShouldBe(UserRole.Maintainer);
    }

    [Theory]
    [InlineData("administrator")]
    [InlineData("Administrator")]
    [InlineData("ADMINISTRATOR")]
    public void CreateAdministratorRole(string type)
    {
        var role = UserRole.FromType(type);

        role.ShouldBe(UserRole.Administrator);
    }

    [Fact]
    public void GivenUnknownType_ExpectUnknownUserRoleException()
    {
        Should.Throw<UnknownUserRoleException>(() => UserRole.FromType("other"));
    }
}
