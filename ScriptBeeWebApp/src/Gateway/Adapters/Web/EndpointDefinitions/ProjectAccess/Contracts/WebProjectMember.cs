using ScriptBee.Domain.Model.User;

namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

public record WebProjectMember(string MemberId, string MemberType, string Role)
{
    public static WebProjectMember Map(ProjectMember member)
    {
        return new WebProjectMember(member.MemberId, member.MemberType, member.Role.Value);
    }
}
