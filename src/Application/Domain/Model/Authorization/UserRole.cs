namespace ScriptBee.Domain.Model.Authorization;

public sealed record UserRole(string Type)
{
    public static readonly UserRole Guest = new("GUEST");
    public static readonly UserRole Analyst = new("ANALYST");
    public static readonly UserRole Maintainer = new("MAINTAINER");
    public static readonly UserRole Administrator = new("ADMINISTRATOR");

    public static UserRole FromType(string type)
    {
        return type.ToUpperInvariant() switch
        {
            "GUEST" => Guest,
            "ANALYST" => Analyst,
            "MAINTAINER" => Maintainer,
            "ADMINISTRATOR" => Administrator,
            _ => throw new UnknownUserRoleException(type)
        };
    }
}
