﻿using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Domain.Model.Instance;

public record InstanceDoesNotExistsError(InstanceId InstanceId);
