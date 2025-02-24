using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Calculation;

public record CalculationInstanceInfo(
    CalculationInstanceId Id,
    ProjectId ProjectId,
    string Url,
    DateTimeOffset CreationDate
);
