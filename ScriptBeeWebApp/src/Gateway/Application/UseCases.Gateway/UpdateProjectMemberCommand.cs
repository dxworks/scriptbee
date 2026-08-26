using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public sealed record UpdateProjectMemberCommand(
    ProjectId ProjectId,
    string MemberId,
    string MemberType,
    UserRole Role
);
