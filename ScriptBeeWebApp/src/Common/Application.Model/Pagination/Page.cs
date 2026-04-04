namespace ScriptBee.Application.Model.Pagination;

public sealed record Page<T>(IEnumerable<T> Data, long TotalCount, int Offset, int Limit);
