using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Persistence.Mongodb.Entity.Analysis;

public class MongodbAnalysisError
{
    public required string Message { get; set; }

    public AnalysisError ToAnalysisError()
    {
        return new AnalysisError(Message);
    }

    public static MongodbAnalysisError From(AnalysisError analysisError)
    {
        return new MongodbAnalysisError { Message = analysisError.Message };
    }
}
