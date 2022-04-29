using System.IO;
using System.Threading.Tasks;

namespace ScriptBeeWebApp.Services;

public interface IFileModelService
{
    public Task UploadFile(string fileName, Stream fileStream);

    public Task<Stream> GetFile(string fileName);
}