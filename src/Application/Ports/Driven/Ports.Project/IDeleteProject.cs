﻿using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Driven.Project;

public interface IDeleteProject
{
    Task Delete(ProjectId projectId, CancellationToken cancellationToken = default);
}
