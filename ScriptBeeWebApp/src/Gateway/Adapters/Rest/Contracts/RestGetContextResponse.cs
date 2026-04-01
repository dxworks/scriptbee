namespace ScriptBee.Rest.Contracts;

public record RestGetContextResponse(IEnumerable<RestContextSlice> Data);
