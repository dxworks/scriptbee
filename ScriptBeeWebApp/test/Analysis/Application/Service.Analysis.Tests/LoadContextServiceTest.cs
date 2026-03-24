using NSubstitute;
using ScriptBee.Domain.Model.File;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class LoadContextServiceTest
{
    private readonly ILoadModelFilesService _loadModelFilesService =
        Substitute.For<ILoadModelFilesService>();

    private readonly LoadContextService _loadContextService;

    public LoadContextServiceTest()
    {
        _loadContextService = new LoadContextService(_loadModelFilesService);
    }

    [Fact]
    public async Task LoadModelFiles()
    {
        var filesToLoad = new Dictionary<string, IEnumerable<FileId>> { { "loader", [] } };

        await _loadContextService.Load(filesToLoad, CancellationToken.None);

        await _loadModelFilesService
            .Received(1)
            .LoadModelFiles(filesToLoad, Arg.Any<CancellationToken>());
    }
}
