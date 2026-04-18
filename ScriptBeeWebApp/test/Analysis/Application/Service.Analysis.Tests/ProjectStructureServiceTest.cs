using DxWorks.ScriptBee.Plugin.Api;
using NSubstitute;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins.Loader;
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
                        new Dictionary<string, ScriptBeeModel> { { "class-id", new TestClass() } }
                    },
                },
            },
        };
        var dummyStrategy = Substitute.For<IScriptGeneratorStrategy>();
        dummyStrategy.Language.Returns("language");
        dummyStrategy.Extension.Returns(".ext");

        _projectManager.GetProject().Returns(project);
        _pluginRepository.GetPlugins<IScriptGeneratorStrategy>().Returns([dummyStrategy]);
        _pluginRepository.GetPlugins<IModelLoader>().Returns([new TestModelLoader()]);

        // Act
        var result = await _projectStructureService.GenerateModelClasses(
            [],
            TestContext.Current.CancellationToken
        );

        // Assert
        var sampleCodeFile = result.ToList().Single();
        sampleCodeFile.Name.ShouldBe(Path.Combine("language", "TestClass.ext"));
        sampleCodeFile.Content.ShouldBeNullOrWhiteSpace();
    }

    private class TestClass : ScriptBeeModel;

    private class TestModelLoader : IModelLoader
    {
        public Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>> LoadModel(
            List<Stream> fileStreams,
            Dictionary<string, object>? configuration = null,
            CancellationToken cancellationToken = default
        ) => Task.FromResult(new Dictionary<string, Dictionary<string, ScriptBeeModel>>());

        public string GetName() => "test-loader";
    }
}
