using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Domain.Model.Errors;

public record InstanceDoesNotExistsError(InstanceId InstanceId);
