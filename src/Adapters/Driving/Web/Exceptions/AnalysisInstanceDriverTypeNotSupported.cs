namespace ScriptBee.Web.Exceptions;

[Serializable]
public sealed class AnalysisInstanceDriverTypeNotSupported(string driver)
    : Exception($"{driver} instance type not supported");
