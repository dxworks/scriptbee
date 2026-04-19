using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Instance.Allocation;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.Tests.Common;

namespace ScriptBee.Service.Gateway.Tests.Analysis;

public class DeallocateProjectInstanceServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();

    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly IDeallocateInstance _deallocateInstance =
        Substitute.For<IDeallocateInstance>();

    private readonly IDeleteProjectInstance _deleteProjectInstance =
        Substitute.For<IDeleteProjectInstance>();

    private readonly DeallocateProjectInstanceService _deallocateProjectInstanceService;

    public DeallocateProjectInstanceServiceTest()
    {
        _deallocateProjectInstanceService = new DeallocateProjectInstanceService(
            _getProject,
            _getProjectInstance,
            _deallocateInstance,
            _deleteProjectInstance
        );
    }

    [Fact]
    public async Task GivenNoProjectForProjectId_ExpectProjectDoesNotExistError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        var result = await _deallocateProjectInstanceService.Deallocate(
            projectId,
            instanceId,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new ProjectDoesNotExistsError(projectId));
    }

    [Fact]
    public async Task GivenNoInstance_ExpectSuccess()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    ProjectDetailsFixture.BasicProjectDetails(projectId)
                )
            );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var result = await _deallocateProjectInstanceService.Deallocate(
            projectId,
            instanceId,
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(new Success());
        await _deallocateInstance
            .Received(0)
            .Deallocate(Arg.Any<InstanceInfo>(), Arg.Any<CancellationToken>());
        await _deleteProjectInstance
            .Received(0)
            .Delete(Arg.Any<InstanceInfo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenInstance_ExpectSuccess()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId(Guid.NewGuid());
        var instanceInfo = InstanceInfoFixture.BasicInstanceInfo(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    ProjectDetailsFixture.BasicProjectDetails(projectId)
                )
            );
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );

        var result = await _deallocateProjectInstanceService.Deallocate(
            projectId,
            instanceId,
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(new Success());
        await _deallocateInstance
            .Received(1)
            .Deallocate(instanceInfo, Arg.Any<CancellationToken>());
        await _deleteProjectInstance.Received(1).Delete(instanceInfo, Arg.Any<CancellationToken>());
    }
}
