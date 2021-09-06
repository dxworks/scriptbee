using System.Collections.Generic;

namespace ScriptBee.Scripts.ScriptSampleGenerators
{
    public interface ISampleCodeGenerator
    {
        public IList<SampleCodeFile> GetSampleCode(IEnumerable<object> obj);
        
        public IList<SampleCodeFile> GetSampleCode(object obj);
    }
}