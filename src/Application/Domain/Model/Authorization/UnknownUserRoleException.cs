namespace ScriptBee.Domain.Model.Authorization;

[Serializable]
public sealed class UnknownUserRoleException(string type) : Exception(type);
