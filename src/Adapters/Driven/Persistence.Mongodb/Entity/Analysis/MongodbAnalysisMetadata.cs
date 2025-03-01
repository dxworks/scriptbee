using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Persistence.Mongodb.Entity.Analysis;

public class MongodbAnalysisMetadata
{
    public required IEnumerable<string> Loaders { get; set; }
    public required IEnumerable<string> Linkers { get; set; }

    public AnalysisMetadata ToAnalysisMetadata()
    {
        return new AnalysisMetadata(Loaders, Linkers);
    }

    public static MongodbAnalysisMetadata From(AnalysisMetadata analysisInfoMetadata)
    {
        return new MongodbAnalysisMetadata
        {
            Loaders = analysisInfoMetadata.Loaders,
            Linkers = analysisInfoMetadata.Linkers,
        };
    }
}
