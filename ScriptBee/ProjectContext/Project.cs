using System;
using System.Collections.Generic;
using ScriptBeePlugin;

namespace ScriptBee.ProjectContext
{
    public class Project
    {
        public Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> Context { get; set; } = new();

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
    }
}