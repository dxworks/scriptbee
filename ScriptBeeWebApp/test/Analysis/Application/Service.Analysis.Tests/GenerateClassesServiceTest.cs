using NSubstitute;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class GenerateClassesServiceTest
{
    private readonly IProjectStructureService _projectStructureService =
        Substitute.For<IProjectStructureService>();

    private readonly GenerateClassesService _generateClassesService;

    public GenerateClassesServiceTest()
    {
        _generateClassesService = new GenerateClassesService(_projectStructureService);
    }

    [Fact]
    public async Task GenerateClasses_ShouldCallProjectStructureService()
    {
        await _generateClassesService.GenerateClasses([], TestContext.Current.CancellationToken);

        await _projectStructureService
            .Received(1)
            .GenerateModelClasses(Arg.Any<List<string>>(), Arg.Any<CancellationToken>());
    }
}
