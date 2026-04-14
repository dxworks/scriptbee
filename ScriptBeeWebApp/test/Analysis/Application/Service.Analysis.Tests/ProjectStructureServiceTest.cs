using DxWorks.ScriptBee.Plugin.Api;
using NSubstitute;
using ScriptBee.Common.CodeGeneration;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Plugins;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class ProjectStructureServiceTest
{
    private readonly IProjectManager _projectManager = Substitute.For<IProjectManager>();
    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();

    private readonly ProjectStructureService _projectStructureService;

    public ProjectStructureServiceTest()
    {
        _projectStructureService = new ProjectStructureService(_projectManager, _pluginRepository);
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
        var result = await _projectStructureService.GenerateModelClasses(
            [],
            TestContext.Current.CancellationToken
        );

        // Assert
        var files = result.ToList();
        files
            .Single()
            .ShouldBe(
                new SampleCodeFile { Name = Path.Combine("language", "script.ext"), Content = "" }
            );
    }
}
