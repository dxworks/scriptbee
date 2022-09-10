﻿using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Services;

public interface ILinkersHolder
{
    public void AddLinkerToDictionary(IModelLinker linker);
        
    public IModelLinker? GetModelLinker(string modelName);

    public IEnumerable<IModelLinker> GetAllLinkers();
}
