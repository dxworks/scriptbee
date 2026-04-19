using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Gateway.Context;

namespace ScriptBee.Service.Gateway.Context;

using GenerateClassesResult = OneOf<Stream, InstanceDoesNotExistsError>;

public class GenerateInstanceClassesService(
    IGetProjectInstance getProjectInstance,
    IGenerateInstanceClasses generateInstanceClasses
) : IGenerateInstanceClassesUseCase
{
    public async Task<GenerateClassesResult> Generate(
        GenerateClassesCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getProjectInstance.Get(command.InstanceId, cancellationToken);

        return await result.Match<Task<GenerateClassesResult>>(
            async instanceInfo =>
            {
                var stream = await generateInstanceClasses.Generate(
                    instanceInfo,
                    command.Languages,
                    command.TransferFormat,
                    cancellationToken
                );
                return OneOf<Stream, InstanceDoesNotExistsError>.FromT0(stream);
            },
            error => Task.FromResult<GenerateClassesResult>(error)
        );
    }
}
