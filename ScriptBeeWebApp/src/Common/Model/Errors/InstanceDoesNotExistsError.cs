using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Domain.Model.Errors;

public sealed record InstanceDoesNotExistsError(InstanceId InstanceId);
