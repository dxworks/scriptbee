using DxWorks.ScriptBee.Plugin.Api;
using NSubstitute;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class GetContextGraphServiceTests
{
    private readonly IProjectManager _projectManager = Substitute.For<IProjectManager>();
    private readonly GetContextGraphService _getContextGraphService;

    public GetContextGraphServiceTests()
    {
        _getContextGraphService = new GetContextGraphService(_projectManager);
    }

    [Fact]
    public void SearchNodes_ReturnsNodesMatchedByQuery()
    {
        // Arrange
        var project = new Project();
        var model1 = new ScriptBeeModel { ["Name"] = "ClassA", ["Description"] = "DescA" };
        var model2 = new ScriptBeeModel { ["Name"] = "ClassB", ["Description"] = "DescB" };

        project.Context.Models.Add(
            Tuple.Create("Class", "Plugin1"),
            new Dictionary<string, ScriptBeeModel> { ["1"] = model1, ["2"] = model2 }
        );
        _projectManager.GetProject().Returns(project);

        // Act
        var result = _getContextGraphService.SearchNodes("ClassA", 0, 10);

        // Assert
        result.Nodes.Count().ShouldBe(1);
        result.Nodes.First().Label.ShouldBe("ClassA");
    }

    [Fact]
    public void GetNeighbors_ReturnsImmediateNeighborsAndEdges()
    {
        // Arrange
        var project = new Project();
        var modelA = new ScriptBeeModel { ["Name"] = "NodeA" };
        var modelB = new ScriptBeeModel { ["Name"] = "NodeB" };
        modelA["Dependency"] = modelB;

        project.Context.Models.Add(
            Tuple.Create("Type", "Plugin"),
            new Dictionary<string, ScriptBeeModel> { ["A"] = modelA, ["B"] = modelB }
        );
        _projectManager.GetProject().Returns(project);

        // Act
        var result = _getContextGraphService.GetNeighbors("Type_Plugin_A");

        // Assert
        result.Nodes.Count().ShouldBe(1);
        result.Nodes.First().Id.ShouldBe("Type_Plugin_B");
        result.Edges.Count().ShouldBe(1);
        result.Edges.First().Source.ShouldBe("Type_Plugin_A");
        result.Edges.First().Target.ShouldBe("Type_Plugin_B");
        result.Edges.First().Label.ShouldBe("Dependency");
    }

    [Fact]
    public void GetNeighbors_HandlesListsOfModels()
    {
        // Arrange
        var project = new Project();
        var modelA = new ScriptBeeModel { ["Name"] = "NodeA" };
        var modelB = new ScriptBeeModel { ["Name"] = "NodeB" };
        var modelC = new ScriptBeeModel { ["Name"] = "NodeC" };
        modelA["Dependencies"] = new List<ScriptBeeModel> { modelB, modelC };

        project.Context.Models.Add(
            Tuple.Create("Type", "Plugin"),
            new Dictionary<string, ScriptBeeModel>
            {
                ["A"] = modelA,
                ["B"] = modelB,
                ["C"] = modelC,
            }
        );
        _projectManager.GetProject().Returns(project);

        // Act
        var result = _getContextGraphService.GetNeighbors("Type_Plugin_A");

        // Assert
        result.Nodes.Count().ShouldBe(2);
        result.Edges.Count().ShouldBe(2);
        result.Edges.Any(e => e.Target == "Type_Plugin_B").ShouldBeTrue();
        result.Edges.Any(e => e.Target == "Type_Plugin_C").ShouldBeTrue();
    }
}
