using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ScriptBeeWebApp.FolderManager
{
    public interface IFolderWriter
    {
        public void Initialize();

        public void DeleteFile(string pathToFile);

        public Task WriteToFile(string filePath, IFormFile file);
    }
}