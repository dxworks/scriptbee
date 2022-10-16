using System;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

internal sealed class GuidGenerator : IGuidGenerator
{
    public Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }
}
