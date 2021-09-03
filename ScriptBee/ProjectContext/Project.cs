using System;
using System.Collections.Generic;
using ScriptBeePlugin;

namespace ScriptBee.ProjectContext
{
    public class Project
    {
        public Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> Context { get; set; } = new();
    }
}