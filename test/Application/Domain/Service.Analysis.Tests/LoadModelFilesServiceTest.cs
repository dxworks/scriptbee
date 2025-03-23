using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using NSubstitute;
using ScriptBee.Domain.Model.File;
using ScriptBee.Ports.Files;
using ScriptBee.Ports.Plugins;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class LoadModelFilesServiceTest
{
    private readonly IProjectManager _projectManager;
    private readonly IPluginRepository _pluginRepository;
    private readonly IFileModelService _fileModelService;
    private readonly LoadModelFilesService _loadModelFilesService;

    public LoadModelFilesServiceTest()
    {
        _projectManager = Substitute.For<IProjectManager>();
        _pluginRepository = Substitute.For<IPluginRepository>();
        _fileModelService = Substitute.For<IFileModelService>();
        _loadModelFilesService = new LoadModelFilesService(
            _projectManager,
            _pluginRepository,
            _fileModelService
        );
    }

    [Fact]
    public async Task LoadModelFiles_ValidLoader_LoadsModelsAndAddsToProject()
    {
        // Arrange
        const string loaderName = "TestLoader";
        var fileIds = new List<FileId>
        {
            new("7134d66b-ce62-47a2-a9ed-e760b5a36271"),
            new("d3ccb098-63d4-4a1b-a006-89dc3ff40b38"),
        };
        var loadedFiles = new Dictionary<string, IEnumerable<FileId>> { { loaderName, fileIds } };

        var modelLoader = Substitute.For<IModelLoader>();
        modelLoader.GetName().Returns(loaderName);
        _pluginRepository.GetPlugin(Arg.Any<Func<IModelLoader, bool>>()).Returns(modelLoader);

        var fileStream1 = new MemoryStream();
        var fileStream2 = new MemoryStream();
        _fileModelService
            .GetFileAsync(fileIds[0], Arg.Any<CancellationToken>())
            .Returns(fileStream1);
        _fileModelService
            .GetFileAsync(fileIds[1], Arg.Any<CancellationToken>())
            .Returns(fileStream2);

        var loadedModels = new Dictionary<string, Dictionary<string, ScriptBeeModel>>
        {
            {
                "Model1",
                new Dictionary<string, ScriptBeeModel> { { "key", new ScriptBeeModel() } }
            },
            {
                "Model2",
                new Dictionary<string, ScriptBeeModel> { { "key", new ScriptBeeModel() } }
            },
        };

        modelLoader
            .LoadModel(
                Arg.Any<List<NamedFileStream>>(),
                Arg.Any<Dictionary<string, object>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(loadedModels);

        // Act
        await _loadModelFilesService.LoadModelFiles(loadedFiles);

        // Assert
        await modelLoader
            .Received()
            .LoadModel(
                Arg.Any<List<NamedFileStream>>(),
                Arg.Any<Dictionary<string, object>>(),
                Arg.Any<CancellationToken>()
            );
        _projectManager.Received().AddToGivenProject(loadedModels, loaderName);
    }

    [Fact]
    public async Task LoadModelFiles_InvalidLoader_SkipsLoading()
    {
        // Arrange
        const string loaderName = "InvalidLoader";
        var fileIds = new List<FileId> { new("8d7971ec-5b55-400e-ab44-537e592d0129") };
        var loadedFiles = new Dictionary<string, IEnumerable<FileId>> { { loaderName, fileIds } };

        _pluginRepository
            .GetPlugin(Arg.Any<Func<IModelLoader, bool>>())
            .Returns((IModelLoader?)null);

        // Act
        await _loadModelFilesService.LoadModelFiles(loadedFiles);

        // Assert
        _projectManager
            .DidNotReceive()
            .AddToGivenProject(
                Arg.Any<Dictionary<string, Dictionary<string, ScriptBeeModel>>>(),
                Arg.Any<string>()
            );
    }

    [Fact]
    public async Task LoadModelFiles_AddsToLoadedModelsCorrectly()
    {
        // Arrange
        const string loaderName = "TestLoader";
        var fileIds = new List<FileId> { new("acf98324-87ef-4977-bd25-97487ebbf1e5") };
        var loadedFiles = new Dictionary<string, IEnumerable<FileId>> { { loaderName, fileIds } };

        var modelLoader = Substitute.For<IModelLoader>();
        modelLoader.GetName().Returns(loaderName);
        _pluginRepository.GetPlugin(Arg.Any<Func<IModelLoader, bool>>()).Returns(modelLoader);

        var fileStream = new MemoryStream();
        _fileModelService
            .GetFileAsync(fileIds[0], Arg.Any<CancellationToken>())
            .Returns(fileStream);

        var loadedModels = new Dictionary<string, Dictionary<string, ScriptBeeModel>>
        {
            {
                "Model1",
                new Dictionary<string, ScriptBeeModel> { { "key", new ScriptBeeModel() } }
            },
            {
                "Model2",
                new Dictionary<string, ScriptBeeModel> { { "key", new ScriptBeeModel() } }
            },
        };

        modelLoader
            .LoadModel(
                Arg.Any<List<NamedFileStream>>(),
                Arg.Any<Dictionary<string, object>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(loadedModels);

        // Act
        await _loadModelFilesService.LoadModelFiles(loadedFiles);

        // Assert
        _projectManager.Received().AddToGivenProject(loadedModels, loaderName);
    }
}
