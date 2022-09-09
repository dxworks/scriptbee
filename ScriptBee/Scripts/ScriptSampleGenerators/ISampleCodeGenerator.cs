using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScriptBee.Scripts.ScriptSampleGenerators;

public interface ISampleCodeGenerator
{
    public Task<IList<SampleCodeFile>> GetSampleCode(IEnumerable<object> obj);

    public Task<string> GenerateSampleCode();
}
