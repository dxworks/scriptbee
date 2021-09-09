using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ScriptBee.Config;

namespace ScriptBeeWebApp.FolderManager
{
    public class FolderWriter : IFolderWriter
    {
        public void Initialize()
        {
            if (!Directory.Exists(ConfigFolders.PathToRoot))
            {
                Directory.CreateDirectory(ConfigFolders.PathToRoot);
                Directory.CreateDirectory(ConfigFolders.PathToModels);
                Directory.CreateDirectory(ConfigFolders.PathToResults);
                Directory.CreateDirectory(ConfigFolders.PathToPlugins);
            }
        }

        public void DeleteFile(string pathToFile)
        {
            throw new NotImplementedException();
        }
        
        public Task WriteToFile(string filePath, IFormFile file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                return file.CopyToAsync(fileStream);
            }
        }
    }
}