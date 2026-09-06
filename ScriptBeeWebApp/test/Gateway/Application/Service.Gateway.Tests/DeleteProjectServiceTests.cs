using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Permissions;
using ScriptBee.Ports.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway;
using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Service.Gateway.Tests;

public class DeleteProjectServiceTests
{
    private readonly IDeleteProject _deleteProject = Substitute.For<IDeleteProject>();

    private readonly IRemoveProjectMember _removeProjectMember =
        Substitute.For<IRemoveProjectMember>();

    private readonly IGetAllProjectInstances _getAllProjectInstances =
        Substitute.For<IGetAllProjectInstances>();

    private readonly IDeallocateProjectInstanceUseCase _deallocateProjectInstance =
        Substitute.For<IDeallocateProjectInstanceUseCase>();

    private readonly DeleteProjectService _deleteProjectService;

    public DeleteProjectServiceTests()
    {
        _deleteProjectService = new DeleteProjectService(
            _deleteProject,
            _removeProjectMember,
            _getAllProjectInstances,
            _deallocateProjectInstance
        );
    }

    [Fact]
    public async Task GivenNoInstances_ExpectProjectDeletedWithoutDeallocation()
    {
        var projectId = ProjectId.Create("id");
        _getAllProjectInstances.GetAll(projectId, Arg.Any<CancellationToken>()).Returns([]);

        await _deleteProjectService.DeleteProject(
            new DeleteProjectCommand(projectId),
            TestContext.Current.CancellationToken
        );

        await _deallocateProjectInstance
            .Received(0)
            .Deallocate(Arg.Any<ProjectId>(), Arg.Any<InstanceId>(), Arg.Any<CancellationToken>());
        await _deleteProject.Received(1).Delete(projectId, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GivenOneInstance_ExpectInstanceDeallocatedBeforeProjectDeleted()
    {
        var projectId = ProjectId.Create("id");
        var instance = InstanceInfoFixture.BasicInstanceInfo(projectId);
        _getAllProjectInstances.GetAll(projectId, Arg.Any<CancellationToken>()).Returns([instance]);
        _deallocateProjectInstance
            .Deallocate(projectId, instance.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Success, ProjectDoesNotExistsError>>(new Success()));

        await _deleteProjectService.DeleteProject(
            new DeleteProjectCommand(projectId),
            TestContext.Current.CancellationToken
        );

        await _deallocateProjectInstance
            .Received(1)
            .Deallocate(projectId, instance.Id, Arg.Any<CancellationToken>());
        await _deleteProject.Received(1).Delete(projectId, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GivenMultipleInstances_ExpectAllInstancesDeallocated()
    {
        var projectId = ProjectId.Create("id");
        var instance1 = InstanceInfoFixture.BasicInstanceInfo(projectId);
        var instance2 = InstanceInfoFixture.BasicInstanceInfo(projectId);
        _getAllProjectInstances
            .GetAll(projectId, Arg.Any<CancellationToken>())
            .Returns([instance1, instance2]);
        _deallocateProjectInstance
            .Deallocate(projectId, Arg.Any<InstanceId>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Success, ProjectDoesNotExistsError>>(new Success()));

        await _deleteProjectService.DeleteProject(
            new DeleteProjectCommand(projectId),
            TestContext.Current.CancellationToken
        );

        await _deallocateProjectInstance
            .Received(2)
            .Deallocate(projectId, Arg.Any<InstanceId>(), Arg.Any<CancellationToken>());
        await _deleteProject.Received(1).Delete(projectId, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GivenDeallocateFails_ExpectProjectNotDeleted()
    {
        var projectId = ProjectId.Create("id");
        var instance = InstanceInfoFixture.BasicInstanceInfo(projectId);
        _getAllProjectInstances.GetAll(projectId, Arg.Any<CancellationToken>()).Returns([instance]);
        _deallocateProjectInstance
            .Deallocate(projectId, instance.Id, Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Docker unavailable"));

        await Assert.ThrowsAsync<Exception>(() =>
            _deleteProjectService.DeleteProject(
                new DeleteProjectCommand(projectId),
                TestContext.Current.CancellationToken
            )
        );

        await _deleteProject.Received(0).Delete(Arg.Any<ProjectId>(), Arg.Any<CancellationToken>());
        await _removeProjectMember
            .Received(0)
            .RemoveAllProjectMembers(Arg.Any<ProjectId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProjectSuccessfully()
    {
        var projectId = ProjectId.Create("id");
        _getAllProjectInstances.GetAll(projectId, Arg.Any<CancellationToken>()).Returns([]);
        _deleteProject
            .Delete(projectId, TestContext.Current.CancellationToken)
            .Returns(Task.CompletedTask);

        await _deleteProjectService.DeleteProject(
            new DeleteProjectCommand(projectId),
            TestContext.Current.CancellationToken
        );

        await _deleteProject.Received(1).Delete(projectId, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ProjectMembersAreRemovedSuccessfully()
    {
        var projectId = ProjectId.Create("id");
        _getAllProjectInstances.GetAll(projectId, Arg.Any<CancellationToken>()).Returns([]);
        _deleteProject
            .Delete(projectId, TestContext.Current.CancellationToken)
            .Returns(Task.CompletedTask);

        await _deleteProjectService.DeleteProject(
            new DeleteProjectCommand(projectId),
            TestContext.Current.CancellationToken
        );

        await _removeProjectMember
            .Received(1)
            .RemoveAllProjectMembers(projectId, TestContext.Current.CancellationToken);
    }
}
