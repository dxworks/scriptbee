using ScriptBee.Domain.Model.File;

namespace ScriptBee.Persistence.Mongodb.Entity;

public record MongodbFileData(string Id, string Name)
{
    public FileData ToFileData()
    {
        return new FileData(new FileId(Id), Name);
    }

    public static MongodbFileData From(FileData fileData)
    {
        return new MongodbFileData(fileData.Id.ToString(), fileData.Name);
    }
}
