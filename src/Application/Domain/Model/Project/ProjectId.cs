﻿using Slugify;

namespace ScriptBee.Domain.Model.Project;

public sealed record ProjectId
{
    public string Value { get; }

    private ProjectId(string value)
    {
        Value = value;
    }

    public static ProjectId Create(string value)
    {
        var slug = new SlugHelper().GenerateSlug(value);
        return new ProjectId(slug);
    }

    public static ProjectId FromValue(string value)
    {
        return new ProjectId(value);
    }
}
