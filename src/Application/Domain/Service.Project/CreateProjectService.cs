﻿using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project;

namespace ScriptBee.Service.Project;

public class CreateProjectService(ICreateProject createProject, IDateTimeProvider dateTimeProvider)
    : ICreateProjectUseCase
{
    public async Task<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>> CreateProject(
        CreateProjectCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var projectDetails = new ProjectDetails(
            ProjectId.Create(command.Id),
            command.Name,
            dateTimeProvider.UtcNow(),
            new Dictionary<string, List<FileData>>(),
            new Dictionary<string, List<FileData>>(),
            []
        );

        var result = await createProject.Create(projectDetails, cancellationToken);

        return result.Match<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>>(
            _ => projectDetails,
            error => error
        );
    }
}
