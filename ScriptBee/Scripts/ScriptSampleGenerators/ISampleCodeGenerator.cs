using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptBee.Scripts.ScriptSampleGenerators;

public interface ISampleCodeGenerator
{
    public Task<IList<SampleCodeFile>> GetSampleCode(IEnumerable<object> obj,
        CancellationToken cancellationToken = default);

    public Task<string> GenerateSampleCode(CancellationToken cancellationToken = default);
}
