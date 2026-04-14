using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using static ScriptBee.Tests.Common.InstanceInfoFixture;

namespace ScriptBee.Service.Project.Tests.Context;

public class GenerateInstanceClassesServiceTest
{
    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly IGenerateInstanceClasses _generateInstanceClasses =
        Substitute.For<IGenerateInstanceClasses>();

    private readonly GenerateInstanceClassesService _generateInstanceClassesService;

    public GenerateInstanceClassesServiceTest()
    {
        _generateInstanceClassesService = new GenerateInstanceClassesService(
            _getProjectInstance,
            _generateInstanceClasses
        );
    }

    [Fact]
    public async Task GivenInstance_ExpectClassesToBeGenerated()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("2e179101-9195-4bf0-8e06-e171912df595");
        var languages = new List<string> { "csharp" };
        var command = new GenerateClassesCommand(projectId, instanceId, languages, "format");
        var instanceInfo = BasicInstanceInfo(projectId);
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );
        using var stream = new MemoryStream();
        _generateInstanceClasses
            .Generate(instanceInfo, languages, "format", Arg.Any<CancellationToken>())
            .Returns(stream);

        var result = await _generateInstanceClassesService.Generate(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(stream);
        await _generateInstanceClasses
            .Received(1)
            .Generate(instanceInfo, languages, "format", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenNoInstanceForInstanceId_ExpectInstanceDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("2e179101-9195-4bf0-8e06-e171912df595");
        var command = new GenerateClassesCommand(projectId, instanceId, []);
        var instanceDoesNotExistsError = new InstanceDoesNotExistsError(instanceId);
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    instanceDoesNotExistsError
                )
            );

        var result = await _generateInstanceClassesService.Generate(
            command,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(instanceDoesNotExistsError);
    }
}
