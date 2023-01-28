using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using DxWorks.ScriptBee.Plugin.Api;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions;

// todo to be replaced by Pact tests
public class LinkersEndpointDefinitionTests
{
    private readonly Mock<IProjectModelService> _projectModelServiceMock;
    private readonly Mock<ILinkersService> _linkersServiceMock;
    private readonly Mock<IProjectManager> _projectManagerMock;
    private readonly Mock<IValidator<LinkProject>> _linkProjectValidatorMock;
    private readonly Fixture _fixture;

    public LinkersEndpointDefinitionTests()
    {
        _projectModelServiceMock = new Mock<IProjectModelService>();
        _linkersServiceMock = new Mock<ILinkersService>();
        _projectManagerMock = new Mock<IProjectManager>();
        _linkProjectValidatorMock = new Mock<IValidator<LinkProject>>();

        _fixture = new Fixture();
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

        var linkers = LinkersEndpointDefinition.GetAllLinkers(_linkersServiceMock.Object);

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

        var result = await LinkersEndpointDefinition.Link(linkProject, _linkProjectValidatorMock.Object,
            _linkersServiceMock.Object, _projectModelServiceMock.Object, _projectManagerMock.Object,
            It.IsAny<CancellationToken>());

        Assert.Equal(400, result.GetStatusCode());
        Assert.Equal(expectedValidationErrorsResponse.Errors, result.GetValue<ValidationErrorsResponse>().Errors);
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

        var result = await LinkersEndpointDefinition.Link(linkProject, _linkProjectValidatorMock.Object,
            _linkersServiceMock.Object, _projectModelServiceMock.Object, _projectManagerMock.Object,
            It.IsAny<CancellationToken>());

        Assert.Equal(404, result.GetStatusCode());
        Assert.Equal($"Could not find linker with name: {linkProject.LinkerName}", result.GetValue<string>());
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

        var result = await LinkersEndpointDefinition.Link(linkProject, _linkProjectValidatorMock.Object,
            _linkersServiceMock.Object, _projectModelServiceMock.Object, _projectManagerMock.Object,
            It.IsAny<CancellationToken>());

        Assert.Equal(404, result.GetStatusCode());
        Assert.Equal($"Could not find project model with id: {linkProject.ProjectId}", result.GetValue<string>());
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

        var result = await LinkersEndpointDefinition.Link(linkProject, _linkProjectValidatorMock.Object,
            _linkersServiceMock.Object, _projectModelServiceMock.Object, _projectManagerMock.Object,
            It.IsAny<CancellationToken>());

        Assert.Equal(404, result.GetStatusCode());
        Assert.Equal($"Project with id = {linkProject.ProjectId} does not have its context loaded",
            result.GetValue<string>());
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

        var result = await LinkersEndpointDefinition.Link(linkProject, _linkProjectValidatorMock.Object,
            _linkersServiceMock.Object, _projectModelServiceMock.Object, _projectManagerMock.Object,
            It.IsAny<CancellationToken>());

        Assert.Equal(200, result.GetStatusCode());
    }
}
