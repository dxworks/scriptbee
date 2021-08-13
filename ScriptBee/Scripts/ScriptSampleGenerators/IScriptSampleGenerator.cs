using System;

namespace ScriptBee.Scripts.ScriptSampleGenerators
{
    public interface IScriptSampleGenerator
    {
        public string Generate(Type type);
    }
}