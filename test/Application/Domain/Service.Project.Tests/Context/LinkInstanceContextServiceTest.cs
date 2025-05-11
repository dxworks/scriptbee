﻿using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using static ScriptBee.Tests.Common.InstanceInfoFixture;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Project.Tests.Context;

public class LinkInstanceContextServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();

    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly ILinkInstanceContext _linkInstanceContext =
        Substitute.For<ILinkInstanceContext>();

    private readonly IUpdateProject _updateProject = Substitute.For<IUpdateProject>();

    private readonly LinkInstanceContextService _linkInstanceContextService;

    public LinkInstanceContextServiceTest()
    {
        _linkInstanceContextService = new LinkInstanceContextService(
            _getProject,
            _getProjectInstance,
            _linkInstanceContext,
            _updateProject
        );
    }

    [Fact]
    public async Task GivenInstance_ExpectContextToBeLinked()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("3cf3006c-234f-4d7b-951d-b17b5226020e");
        List<string> linkerIds = ["linker-id"];
        var command = new LinkContextCommand(projectId, instanceId, linkerIds);
        var projectDetails = BasicProjectDetails(ProjectId.Create("id"));
        var instanceInfo = BasicInstanceInfo(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );

        var result = await _linkInstanceContextService.Link(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(new Unit());
        await _linkInstanceContext
            .Received(1)
            .Link(
                instanceInfo,
                Arg.Is<IEnumerable<string>>(linkers =>
                    linkerIds.Single().Equals(linkerIds.Single())
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenInstance_ExpectProjectLinkersToBeUpdated()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("3cf3006c-234f-4d7b-951d-b17b5226020e");
        List<string> linkerIds = ["linker-id"];
        var command = new LinkContextCommand(projectId, instanceId, linkerIds);
        var projectDetails = BasicProjectDetails(ProjectId.Create("id"));
        var instanceInfo = BasicInstanceInfo(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );

        await _linkInstanceContextService.Link(command, TestContext.Current.CancellationToken);

        await _updateProject
            .Received(1)
            .Update(
                Arg.Is<ProjectDetails>(details => details.Linkers.Single().Equals("linker-id")),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenNoProjectForProjectId_ExpectProjectDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("3cf3006c-234f-4d7b-951d-b17b5226020e");
        var command = new LinkContextCommand(projectId, instanceId, ["linker-id"]);
        var projectDoesNotExistsError = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    projectDoesNotExistsError
                )
            );

        var result = await _linkInstanceContextService.Link(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(projectDoesNotExistsError);
    }

    [Fact]
    public async Task GivenNoInstanceForInstanceId_ExpectInstanceDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("3cf3006c-234f-4d7b-951d-b17b5226020e");
        var command = new LinkContextCommand(projectId, instanceId, ["linker-id"]);
        var instanceDoesNotExistsError = new InstanceDoesNotExistsError(instanceId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(ProjectId.Create("id"))
                )
            );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    instanceDoesNotExistsError
                )
            );

        var result = await _linkInstanceContextService.Link(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT2.ShouldBe(instanceDoesNotExistsError);
    }
}
