namespace ScriptBee.Application.Model.Sorting;

public record Sort<TField>(TField Field, SortOrder Direction)
    where TField : struct, Enum;
