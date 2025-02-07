using Slugify;

namespace ScriptBee.Domain.Model.Project;

public sealed record ProjectId
{
    public string Value { get; }

    private ProjectId(string value)
    {
        Value = value;
    }

    public static ProjectId FromValue(string value)
    {
        return new ProjectId(value);
    }

    public static ProjectId FromName(string name)
    {
        var slug = new SlugHelper().GenerateSlug(name);
        return new ProjectId(slug);
    }
}
