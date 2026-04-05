using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class ProjectManagerTests
{
    private readonly ProjectManager _projectManager = new(
        new Project
        {
            Id = "projectId",
            Name = "projectName",
            CreationDate = DateTime.UtcNow,
        }
    );

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
        var project = _projectManager.GetProject();

        project.Id.ShouldBe("projectId");
        project.Name.ShouldBe("projectName");
    }

    [Fact]
    public void RemoveSourceEntries_RemovesEntriesFromContext()
    {
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
