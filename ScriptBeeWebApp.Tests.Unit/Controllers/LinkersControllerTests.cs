using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using DxWorks.ScriptBee.Plugin.Api;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Controllers.Arguments.Validation;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Controllers;

public class LinkersControllerTests
{
    private readonly Mock<IProjectModelService> _projectModelServiceMock;
    private readonly Mock<ILinkersService> _linkersServiceMock;
    private readonly Mock<IProjectManager> _projectManagerMock;
    private readonly Mock<IValidator<LinkProject>> _linkProjectValidatorMock;
    private readonly Fixture _fixture;

    private readonly LinkersController _linkersController;

    public LinkersControllerTests()
    {
        _projectModelServiceMock = new Mock<IProjectModelService>();
        _linkersServiceMock = new Mock<ILinkersService>();
        _projectManagerMock = new Mock<IProjectManager>();
        _linkProjectValidatorMock = new Mock<IValidator<LinkProject>>();

        _fixture = new Fixture();

        _linkersController = new LinkersController(_projectModelServiceMock.Object, _linkersServiceMock.Object,
            _projectManagerMock.Object, _linkProjectValidatorMock.Object);
    }

    [Fact]
    public void GivenSupportedLinkers_WhenGetAllLinkers_ThenAllLinkersAreReturned()
    {
        var expectedLinkers = new List<string>
        {
            "linker1",
            "linker2",
            "linker3"
        };
        _linkersServiceMock.Setup(x => x.GetSupportedLinkers()).Returns(expectedLinkers);

        var actionResult = _linkersController.GetAllLinkers();
        var result = (OkObjectResult)actionResult.Result!;
        var linkers = (List<string>)result.Value!;

        Assert.Equal(expectedLinkers, linkers);
    }

    [Fact]
    public async Task GivenInvalidLinkProject_WhenLink_ThenBadRequestIsReturned()
    {
        var linkProject = _fixture.Create<LinkProject>();
        var expectedValidationErrorsResponse = new ValidationErrorsResponse(new List<ValidationError>
        {
            new("property", "error")
        });

        _linkProjectValidatorMock
            .Setup(v => v.ValidateAsync(linkProject, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("property", "error") }));

        var actionResult = await _linkersController.Link(linkProject, It.IsAny<CancellationToken>());
        var badRequestResult = (BadRequestObjectResult)actionResult;
        var validationErrorResponse = (ValidationErrorsResponse)badRequestResult.Value!;

        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal(expectedValidationErrorsResponse.Errors, validationErrorResponse.Errors);
    }

    [Fact]
    public async Task GivenMissingLinker_WhenLink_ThenNotFoundIsReturned()
    {
        var linkProject = _fixture.Create<LinkProject>();

        _linkProjectValidatorMock
            .Setup(v => v.ValidateAsync(linkProject, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

        _linkersServiceMock.Setup(s => s.GetLinker(It.IsAny<string>()))
            .Returns((IModelLinker?)null);

        var actionResult = await _linkersController.Link(linkProject, It.IsAny<CancellationToken>());
        var notFoundResult = (NotFoundObjectResult)actionResult;

        Assert.Equal(404, notFoundResult.StatusCode);
        Assert.Equal($"Could not find linker with name: {linkProject.LinkerName}", notFoundResult.Value);
    }

    [Fact]
    public async Task GivenMissingProjectModel_WhenLink_ThenNotFoundIsReturned()
    {
        var linkProject = _fixture.Create<LinkProject>();
        var linker = new Mock<IModelLinker>().Object;

        _linkProjectValidatorMock
            .Setup(v => v.ValidateAsync(linkProject, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

        _linkersServiceMock.Setup(s => s.GetLinker(linkProject.LinkerName))
            .Returns(linker);
        _projectModelServiceMock.Setup(s => s.GetDocument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectModel?)null);

        var actionResult = await _linkersController.Link(linkProject, It.IsAny<CancellationToken>());
        var notFoundResult = (NotFoundObjectResult)actionResult;

        Assert.Equal(404, notFoundResult.StatusCode);
        Assert.Equal($"Could not find project model with id: {linkProject.ProjectId}", notFoundResult.Value);
    }

    [Fact]
    public async Task GivenMissingProject_WhenLink_ThenNotFoundIsReturned()
    {
        var linkProject = _fixture.Create<LinkProject>();
        var projectModel = _fixture.Create<ProjectModel>();
        var linker = new Mock<IModelLinker>().Object;

        _linkProjectValidatorMock
            .Setup(v => v.ValidateAsync(linkProject, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

        _linkersServiceMock.Setup(s => s.GetLinker(linkProject.LinkerName))
            .Returns(linker);
        _projectModelServiceMock.Setup(s => s.GetDocument(linkProject.ProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectModel);
        _projectManagerMock.Setup(m => m.GetProject(It.IsAny<string>()))
            .Returns((Project?)null);

        var actionResult = await _linkersController.Link(linkProject, It.IsAny<CancellationToken>());
        var notFoundResult = (NotFoundObjectResult)actionResult;

        Assert.Equal(404, notFoundResult.StatusCode);
        Assert.Equal($"Project with id = {linkProject.ProjectId} does not have its context loaded",
            notFoundResult.Value);
    }

    [Fact]
    public async Task GivenValidLinkProject_WhenLink_ThenOkIsReturned()
    {
        var linkProject = _fixture.Create<LinkProject>();
        var projectModel = _fixture.Create<ProjectModel>();
        var project = new Project();
        var linker = new Mock<IModelLinker>().Object;

        _linkProjectValidatorMock
            .Setup(v => v.ValidateAsync(linkProject, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

        _linkersServiceMock.Setup(s => s.GetLinker(linkProject.LinkerName))
            .Returns(linker);
        _projectModelServiceMock.Setup(s => s.GetDocument(linkProject.ProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectModel);
        _projectManagerMock.Setup(m => m.GetProject(linkProject.ProjectId))
            .Returns(project);

        var actionResult = await _linkersController.Link(linkProject, It.IsAny<CancellationToken>());
        var result = (OkObjectResult)actionResult;

        Assert.Equal(200, result.StatusCode);
    }
}
