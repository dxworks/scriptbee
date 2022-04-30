using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelperFunctions;

public interface IHelperFunctionsWithResults : IHelperFunctions
{
    public Task<List<RunResult>> GetResults();
}