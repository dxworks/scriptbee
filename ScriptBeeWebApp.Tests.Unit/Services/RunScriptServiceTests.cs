using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using DxWorks.ScriptBee.Plugin.Api;
using Moq;
using ScriptBee.Models;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.Data.Exceptions;
using ScriptBeeWebApp.Repository;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Services;

public class RunScriptServiceTests
{
    private readonly Mock<IFileModelService> _fileModelServiceMock;
    private readonly Mock<IGuidGenerator> _guidGeneratorMock;
    private readonly Mock<IPluginRepository> _pluginRepositoryMock;
    private readonly Mock<IProjectFileStructureManager> _projectFileStructureManagerMock;
    private readonly Mock<IProjectModelService> _projectModelServiceMock;
    private readonly Mock<IRunModelService> _runModelServiceMock;
    private readonly Fixture _fixture;

    private readonly RunScriptService _runScriptService;

    public RunScriptServiceTests()
    {
        _fileModelServiceMock = new Mock<IFileModelService>();
        _guidGeneratorMock = new Mock<IGuidGenerator>();
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _projectFileStructureManagerMock = new Mock<IProjectFileStructureManager>();
        _projectModelServiceMock = new Mock<IProjectModelService>();
        _runModelServiceMock = new Mock<IRunModelService>();
        _fixture = new Fixture();

        _runScriptService = new RunScriptService(_fileModelServiceMock.Object, _guidGeneratorMock.Object,
            _pluginRepositoryMock.Object, _projectFileStructureManagerMock.Object, _projectModelServiceMock.Object,
            _runModelServiceMock.Object);
    }


    [Fact]
    public void GivenLoadedPluginManifests_WhenGetSupportedLanguages_ThenReturnSupportedLanguages()
    {
        var pluginExtensionPoints = new List<ScriptRunnerPluginExtensionPoint>
        {
            new()
            {
                Language = "C#"
            },
            new()
            {
                Language = "Python"
            }
        };
        _pluginRepositoryMock.Setup(x => x.GetLoadedPluginExtensionPoints<ScriptRunnerPluginExtensionPoint>())
            .Returns(pluginExtensionPoints);

        var supportedLanguages = _runScriptService.GetSupportedLanguages().ToList();

        Assert.Equal(2, supportedLanguages.Count);
        Assert.Contains("C#", supportedLanguages);
        Assert.Contains("Python", supportedLanguages);
    }

    [Fact]
    public async Task GivenMissingScript_WhenRunScript_ThenScriptFileNotFoundExceptionIsThrown()
    {
        var projectModel = _fixture.Create<ProjectModel>();

        _projectFileStructureManagerMock.Setup(m => m.GetFileContentAsync(projectModel.Id, "scriptPath"))
            .ReturnsAsync((string?)null);

        await Assert.ThrowsAsync<ScriptFileNotFoundException>(() => _runScriptService.RunAsync(It.IsAny<Project>(),
            projectModel, It.IsAny<string>(), "scriptPath", It.IsAny<CancellationToken>()));
    }

    [Fact(Skip = "Unsupported expression: x => (x.Language == \"language\")")]
    public async Task GivenValidScriptPath_WhenRunScript_ThenScriptContentIsSavedInDatabase()
    {
        var projectModel = _fixture.Create<ProjectModel>();
        const string language = "language";

        _guidGeneratorMock.Setup(g => g.GenerateGuid())
            .Returns(Guid.Parse("eee8d702-8065-4db6-b553-d99c4bbd8cbc"));
        _projectFileStructureManagerMock.Setup(m => m.GetFileContentAsync(projectModel.Id, "scriptPath"))
            .ReturnsAsync("content");
        _pluginRepositoryMock.Setup(r =>
                r.GetPlugin<IScriptRunner>(x => x.Language == language, new List<(Type @interface, object instance)>()))
            .Returns(It.IsAny<IScriptRunner>());
        _pluginRepositoryMock
            .Setup(r => r.GetPlugin<IScriptGeneratorStrategy>(x => x.Language == language,
                new List<(Type @interface, object instance)>()))
            .Returns(It.IsAny<IScriptGeneratorStrategy>());

        await _runScriptService.RunAsync(It.IsAny<Project>(), projectModel, language, "scriptPath",
            It.IsAny<CancellationToken>());

        _fileModelServiceMock.Verify(m => m.UploadFileAsync("eee8d702-8065-4db6-b553-d99c4bbd8cbc", It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()));
    }
}
