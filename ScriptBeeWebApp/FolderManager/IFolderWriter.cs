using Microsoft.AspNetCore.Http;

namespace ScriptBeeWebApp.FolderManager
{
    public interface IFolderWriter
    {
        public void Initialize();

        public void DeleteFile(string pathToFile);

        public void WriteToFile(string folderName, IFormFile file);

        public string GetPathToModelsFolder();

        public string GetPathToResultsFolder();
    }
}