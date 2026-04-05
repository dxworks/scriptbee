using DxWorks.ScriptBee.Plugin.Api;
using NSubstitute;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Plugins;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class ProjectStructureServiceTest
{
    private readonly IProjectManager _projectManager = Substitute.For<IProjectManager>();
    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();

    private readonly IDeleteFileOrFolder _deleteFileOrFolder =
        Substitute.For<IDeleteFileOrFolder>();

    private readonly ICreateFile _createFile = Substitute.For<ICreateFile>();

    private readonly ProjectStructureService _projectStructureService;

    public ProjectStructureServiceTest()
    {
        _projectStructureService = new ProjectStructureService(
            _projectManager,
            _pluginRepository,
            _deleteFileOrFolder,
            _createFile
        );
    }

    [Fact]
    public async Task GenerateModelClasses()
    {
        // Arrange
        var projectId = ProjectId.FromValue("id");
        var project = new Project
        {
            Id = projectId.ToString(),
            Context = new Context
            {
                Models =
                {
                    {
                        Tuple.Create("loader", "class-name"),
                        new Dictionary<string, ScriptBeeModel>
                        {
                            { "class-id", new ScriptBeeModel() },
                        }
                    },
                },
            },
        };
        var dummyStrategy = Substitute.For<IScriptGeneratorStrategy>();
        dummyStrategy.Language.Returns("language");
        dummyStrategy.Extension.Returns(".ext");

        _projectManager.GetProject().Returns(project);
        _pluginRepository.GetPlugins<IScriptGeneratorStrategy>().Returns([dummyStrategy]);

        // Act
        await _projectStructureService.GenerateModelClasses(TestContext.Current.CancellationToken);

        // Assert
        _deleteFileOrFolder
            .Received(1)
            .Delete(projectId, Path.Combine(ConfigFolders.GeneratedFolder, "language"));
        await _createFile
            .Received(1)
            .Create(
                projectId,
                Path.Combine(ConfigFolders.GeneratedFolder, "language", "script.ext"),
                "",
                Arg.Any<CancellationToken>()
            );
    }
}
