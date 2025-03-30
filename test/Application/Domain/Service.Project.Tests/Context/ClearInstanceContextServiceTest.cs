using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using static ScriptBee.Tests.Common.InstanceInfoFixture;

namespace ScriptBee.Service.Project.Tests.Context;

public class ClearInstanceContextServiceTest
{
    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly IClearInstanceContext _clearInstanceContext =
        Substitute.For<IClearInstanceContext>();

    private readonly ClearInstanceContextService _clearInstanceContextService;

    public ClearInstanceContextServiceTest()
    {
        _clearInstanceContextService = new ClearInstanceContextService(
            _getProjectInstance,
            _clearInstanceContext
        );
    }

    [Fact]
    public async Task GivenInstance_ExpectContextToBeCleared()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("2e179101-9195-4bf0-8e06-e171912df595");
        var command = new ClearContextCommand(projectId, instanceId);
        var instanceInfo = BasicInstanceInfo(projectId);
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );

        var result = await _clearInstanceContextService.Clear(command);

        result.AsT0.ShouldBe(new Unit());
        await _clearInstanceContext.Received(1).Clear(instanceInfo, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenNoInstanceForInstanceId_ExpectInstanceDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("2e179101-9195-4bf0-8e06-e171912df595");
        var command = new ClearContextCommand(projectId, instanceId);
        var instanceDoesNotExistsError = new InstanceDoesNotExistsError(instanceId);
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    instanceDoesNotExistsError
                )
            );

        var result = await _clearInstanceContextService.Clear(command);

        result.AsT1.ShouldBe(instanceDoesNotExistsError);
    }
}
