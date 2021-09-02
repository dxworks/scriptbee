using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ScriptBeeWebApp.FolderManager
{
    public class FolderWriter : IFolderWriter
    {
        private const string Root = ".scriptbee";

        private const string ModelsFolder = "models";

        private const string ResultsFolder = "results";

        private const string PluginsFolder = "plugins";

        private readonly string _pathToRoot;

        private readonly string _pathToModels;

        private readonly string _pathToResults;

        private readonly string _pathToPlugins;

        public FolderWriter()
        {
            _pathToRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Root);

            _pathToModels = Path.Combine(_pathToRoot, ModelsFolder);

            _pathToResults = Path.Combine(_pathToRoot, ResultsFolder);

            _pathToPlugins = Path.Combine(_pathToRoot, PluginsFolder);
        }

        public void Initialize()
        {
            if (!Directory.Exists(_pathToRoot))
            {
                Directory.CreateDirectory(_pathToRoot);
                Directory.CreateDirectory(_pathToModels);
                Directory.CreateDirectory(_pathToResults);
                Directory.CreateDirectory(_pathToPlugins);
            }
        }

        public void DeleteFile(string pathToFile)
        {
            throw new NotImplementedException();
        }

        public void WriteToFile(string folderName, IFormFile file)
        {
            string outputPath = Path.Combine(_pathToModels, folderName, file.FileName);

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            using (var fileStream = new FileStream(outputPath, FileMode.Create))
            {
                file.CopyToAsync(fileStream);
            }
        }

        public string GetPathToModelsFolder()
        {
            return _pathToModels;
        }

        public string GetPathToResultsFolder()
        {
            return _pathToResults;
        }
    }
}