using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using ScriptBee.Models;
using ScriptBeeWebApp.EndpointDefinitions;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions;

// todo to be replaced by Pact tests
public class ProjectsEndpointDefinitionTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IProjectModelService> _projectModelServiceMock;

    public ProjectsEndpointDefinitionTests()
    {
        _projectModelServiceMock = new Mock<IProjectModelService>();

        _fixture = new Fixture();
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenGetProject_ThenBadRequestIsReturned()
    {
        var result =
            await ProjectsEndpointDefinition.GetProject("", _projectModelServiceMock.Object,
                It.IsAny<CancellationToken>());

        Assert.Equal(400, result.GetStatusCode());
        Assert.Equal("You must provide a projectId for this operation", result.GetValue<string>());
    }

    [Fact]
    public async Task GivenInvalidProjectId_WhenGetProject_ThenNotFoundIsReturned()
    {
        _projectModelServiceMock.Setup(service => service.GetDocument("invalid", It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((ProjectModel?)null));

        var result = await ProjectsEndpointDefinition.GetProject("invalid", _projectModelServiceMock.Object,
            It.IsAny<CancellationToken>());

        Assert.Equal(404, result.GetStatusCode());
        Assert.Equal("Could not find project with id: invalid", result.GetValue<string>());
    }

    [Fact]
    public async Task GivenValidProjectId_WhenGetProject_ThenOkIsReturned()
    {
        var projectModel = _fixture.Create<ProjectModel>();

        // var projModel = _fixture.Build<ProjectModel>()
        //     .With(p => p.Id, "my-id")
        //     .With(p => p.Linker, "linker")
        //     .Create();

        _projectModelServiceMock.Setup(service => service.GetDocument("valid", It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((ProjectModel?)projectModel));

        var result = await ProjectsEndpointDefinition.GetProject("valid", _projectModelServiceMock.Object,
            It.IsAny<CancellationToken>());

        var returnedProject = result.GetValue<ReturnedProject>();

        Assert.Equal(200, result.GetStatusCode());
        Assert.Equal(projectModel.Id, returnedProject.Id);
        Assert.Equal(projectModel.Name, returnedProject.Name);
        Assert.Equal(projectModel.Linker, returnedProject.Linker);
        Assert.Equal(projectModel.CreationDate, returnedProject.CreationDate);
        Assert.Equal(projectModel.SavedFiles.Count, returnedProject.SavedFiles.Count);
        // todo test mai in detaliu saved files
    }
}
