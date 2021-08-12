using System;

namespace ScriptBee.Scripts.TemplateGenerators
{
    public interface ITemplateGenerator
    {
        public string Generate(Type type);
    }
}