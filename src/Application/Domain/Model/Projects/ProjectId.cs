using Slugify;

namespace ScriptBee.Domain.Model.Projects;

public sealed record ProjectId(string Value)
{
    public static ProjectId FromName(string name)
    {
        var slug = new SlugHelper().GenerateSlug(name);
        return new ProjectId(slug);
    }
}
