namespace ScriptBee.Domain.Model.Authorization;

[Serializable]
public class UnknownUserRoleException(string type) : Exception(type);
