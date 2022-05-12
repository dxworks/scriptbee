using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Controllers;

public class ProjectsControllerTests
{
    private readonly ProjectsController _projectsController;
    private readonly Mock<IProjectManager> _projectManagerMock;
    private readonly Mock<IProjectFileStructureManager> _projectFileStructureManagerMock;
    private readonly Mock<IProjectModelService> _projectModelServiceMock;
    private readonly Mock<IFileNameGenerator> _fileNameGeneratorMock;
    private readonly Mock<IFileModelService> _fileModelServiceMock;
    private readonly Mock<IRunModelService> _runModelServiceMock;
    private readonly Fixture _fixture;

    public ProjectsControllerTests()
    {
        _projectManagerMock = new Mock<IProjectManager>();
        _projectFileStructureManagerMock = new Mock<IProjectFileStructureManager>();
        _projectModelServiceMock = new Mock<IProjectModelService>();
        _fileNameGeneratorMock = new Mock<IFileNameGenerator>();
        _fileModelServiceMock = new Mock<IFileModelService>();
        _runModelServiceMock = new Mock<IRunModelService>();
        _fixture = new Fixture();

        _projectsController = new ProjectsController(_projectManagerMock.Object,
            _projectFileStructureManagerMock.Object, _projectModelServiceMock.Object, _fileNameGeneratorMock.Object,
            _fileModelServiceMock.Object, _runModelServiceMock.Object);
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenGetProject_ThenBadRequestIsReturned()
    {
        var response = await _projectsController.GetProject("", It.IsAny<CancellationToken>());
        var result = Assert.IsType<BadRequestObjectResult>(response);

        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("You must provide a projectId for this operation", result.Value);
    }

    [Fact]
    public async Task GivenInvalidProjectId_WhenGetProject_ThenNotFoundIsReturned()
    {
        _projectModelServiceMock.Setup(service => service.GetDocument("invalid", It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((ProjectModel?)null));

        var response = await _projectsController.GetProject("invalid", It.IsAny<CancellationToken>());
        var result = Assert.IsType<NotFoundObjectResult>(response);

        Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
        Assert.Equal("Could not find project with id: invalid", result.Value);
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

        var response = await _projectsController.GetProject("valid", It.IsAny<CancellationToken>());
        var result = Assert.IsType<OkObjectResult>(response);
        var returnedProject = (result.Value as ReturnedProject)!;

        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(projectModel.Id, returnedProject.Id);
        Assert.Equal(projectModel.Name, returnedProject.Name);
        Assert.Equal(projectModel.Linker, returnedProject.Linker);
        Assert.Equal(projectModel.CreationDate, returnedProject.CreationDate);
        Assert.Equal(projectModel.Loaders, returnedProject.Loaders);
        Assert.Equal(projectModel.SavedFiles.Count, returnedProject.SavedFiles.Count);
        // todo test mai in detaliu saved files
    }
}