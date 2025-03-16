using DxWorks.ScriptBee.Plugin.Api;
using NSubstitute;
using ScriptBee.Common;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class ProjectManagerTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    private readonly ProjectManager _projectManager;

    public ProjectManagerTests()
    {
        _projectManager = new ProjectManager(_dateTimeProvider);
    }

    [Fact]
    public void CreateProject_SetsProjectProperties()
    {
        var now = DateTime.UtcNow;
        _dateTimeProvider.UtcNow().Returns(now);

        var project = _projectManager.CreateProject("projectId", "projectName");

        project.Id.ShouldBe("projectId");
        project.Name.ShouldBe("projectName");
        project.CreationDate.ShouldBe(now);
        project.Context.Models.ShouldBeEmpty();
    }

    [Fact]
    public void AddToGivenProject_AddsModelsToContext()
    {
        var dictionary = new Dictionary<string, Dictionary<string, ScriptBeeModel>>
        {
            {
                "exportedType1",
                new Dictionary<string, ScriptBeeModel>
                {
                    { "model1", new ScriptBeeModel() },
                    { "model2", new ScriptBeeModel() },
                }
            },
            {
                "exportedType2",
                new Dictionary<string, ScriptBeeModel> { { "model3", new ScriptBeeModel() } }
            },
        };
        _projectManager.CreateProject("projectId", "projectName");

        _projectManager.AddToGivenProject(dictionary, "source1");

        var project = _projectManager.GetProject();
        project.Context.Models.Count.ShouldBe(2);
        project.Context.Models.ShouldContainKey(
            new Tuple<string, string>("exportedType1", "source1")
        );
        project.Context.Models.ShouldContainKey(
            new Tuple<string, string>("exportedType2", "source1")
        );
    }

    [Fact]
    public void GetProject_ReturnsCreatedProject()
    {
        _projectManager.CreateProject("projectId", "projectName");

        var project = _projectManager.GetProject();

        project.Id.ShouldBe("projectId");
        project.Name.ShouldBe("projectName");
    }

    [Fact]
    public void RemoveSourceEntries_RemovesEntriesFromContext()
    {
        _projectManager.CreateProject("projectId", "projectName");

        var dictionary = new Dictionary<string, Dictionary<string, ScriptBeeModel>>
        {
            {
                "exportedType1",
                new Dictionary<string, ScriptBeeModel> { { "model1", new ScriptBeeModel() } }
            },
            {
                "exportedType2",
                new Dictionary<string, ScriptBeeModel> { { "model2", new ScriptBeeModel() } }
            },
        };
        _projectManager.AddToGivenProject(dictionary, "source1");

        _projectManager.RemoveSourceEntries("source1");

        var project = _projectManager.GetProject();
        project.Context.Models.ShouldBeEmpty();
    }
}
