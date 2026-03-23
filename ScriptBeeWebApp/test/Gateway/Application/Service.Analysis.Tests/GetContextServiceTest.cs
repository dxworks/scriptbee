using DxWorks.ScriptBee.Plugin.Api;
using NSubstitute;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class GetContextServiceTest
{
    private readonly IProjectManager _projectManager = Substitute.For<IProjectManager>();
    private readonly GetContextService _getContextService;

    public GetContextServiceTest()
    {
        _getContextService = new GetContextService(_projectManager);
    }

    [Fact]
    public void ReturnsEmptyList_WhenProjectContextModelsIsEmpty()
    {
        var project = new Project();
        _projectManager.GetProject().Returns(project);

        var result = _getContextService.Get();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void GroupsContextModelsByType()
    {
        // Arrange
        var project = new Project();
        project.Context.Models.Add(
            Tuple.Create("Class", "Model1"),
            new Dictionary<string, ScriptBeeModel>()
        );
        project.Context.Models.Add(
            Tuple.Create("Class", "Model2"),
            new Dictionary<string, ScriptBeeModel>()
        );
        project.Context.Models.Add(
            Tuple.Create("Interface", "IModel1"),
            new Dictionary<string, ScriptBeeModel>()
        );
        _projectManager.GetProject().Returns(project);

        // Act
        var result = _getContextService.Get().ToList();

        // Assert
        result.Count.ShouldBe(2);

        var classSlice = result.FirstOrDefault(slice => slice.Model == "Class");
        classSlice.ShouldNotBeNull();
        classSlice.PluginIds.ShouldBe(new List<string> { "Model1", "Model2" });

        var interfaceSlice = result.FirstOrDefault(slice => slice.Model == "Interface");
        interfaceSlice.ShouldNotBeNull();
        interfaceSlice.PluginIds.ShouldBe(new List<string> { "IModel1" });
    }

    [Fact]
    public void HandlesSingleContextType()
    {
        var project = new Project();
        project.Context.Models.Add(
            Tuple.Create("Class", "ModelA"),
            new Dictionary<string, ScriptBeeModel>()
        );
        project.Context.Models.Add(
            Tuple.Create("Class", "ModelB"),
            new Dictionary<string, ScriptBeeModel>()
        );
        _projectManager.GetProject().Returns(project);

        var result = _getContextService.Get().ToList();

        result.Count.ShouldBe(1);
        result.First().Model.ShouldBe("Class");
        result.First().PluginIds.ShouldBe(new List<string> { "ModelA", "ModelB" });
    }
}
