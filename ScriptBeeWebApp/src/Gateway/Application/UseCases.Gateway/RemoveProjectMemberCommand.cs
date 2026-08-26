using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway;

public sealed record RemoveProjectMemberCommand(
    ProjectId ProjectId,
    string MemberId,
    string MemberType
);
