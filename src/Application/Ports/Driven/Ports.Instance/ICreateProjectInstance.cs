﻿using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Instance;

public interface ICreateProjectInstance
{
    Task<InstanceInfo> Create(ProjectId projectId, CancellationToken cancellationToken = default);
}
