using System;
using System.Collections.Generic;

namespace ScriptBee.FileManagement;

public interface IPluginDiscriminatorHolder
{
    Dictionary<string, Type> GetDiscriminatedTypes();
}
