namespace ScriptBee.Domain.Model.User;

public record UserPermissions(UserRole Role, List<string> Permissions);
