using DxWorks.ScriptBee.Plugin.Api;
using NSubstitute;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Ports.Plugins;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class LinkContextServiceTest
{
    private readonly IProjectManager _projectManager = Substitute.For<IProjectManager>();
    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();
    private readonly LinkContextService _linkContextService;

    public LinkContextServiceTest()
    {
        _linkContextService = new LinkContextService(_projectManager, _pluginRepository);
    }

    [Fact]
    public async Task Link_ValidLinkerIds_LinksModels()
    {
        var modelLinker1 = Substitute.For<IModelLinker>();
        modelLinker1.GetName().Returns("linker1");
        var modelLinker2 = Substitute.For<IModelLinker>();
        modelLinker2.GetName().Returns("linker2");
        var project = new Project();
        _projectManager.GetProject().Returns(project);

        _pluginRepository
            .GetPlugin(Arg.Is<Func<IModelLinker, bool>>(x => x(modelLinker1)))
            .Returns(modelLinker1);
        _pluginRepository
            .GetPlugin(Arg.Is<Func<IModelLinker, bool>>(x => x(modelLinker2)))
            .Returns(modelLinker2);

        await _linkContextService.Link(
            new List<string> { "linker1", "linker2" },
            CancellationToken.None
        );

        await modelLinker1
            .Received(1)
            .LinkModel(project.Context.Models, cancellationToken: CancellationToken.None);
        await modelLinker2
            .Received(1)
            .LinkModel(project.Context.Models, cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task Link_InvalidLinkerId_DoesNotLinkModels()
    {
        var modelLinker = Substitute.For<IModelLinker>();
        modelLinker.GetName().Returns("linker1");
        _projectManager.GetProject().Returns(new Project());
        _pluginRepository
            .GetPlugin(Arg.Any<Func<IModelLinker, bool>>())
            .Returns(null as IModelLinker);

        await _linkContextService.Link(
            new List<string> { "invalidLinker" },
            CancellationToken.None
        );

        await modelLinker
            .DidNotReceive()
            .LinkModel(
                Arg.Any<Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>>>(),
                Arg.Any<Dictionary<string, object>?>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Link_MixedValidAndInvalidLinkerIds_LinksValidOnly()
    {
        var modelLinker1 = Substitute.For<IModelLinker>();
        modelLinker1.GetName().Returns("linker1");
        var project = new Project();

        _projectManager.GetProject().Returns(project);
        _pluginRepository
            .GetPlugin(Arg.Is<Func<IModelLinker, bool>>(x => x(modelLinker1)))
            .Returns(modelLinker1);
        _pluginRepository
            .GetPlugin(Arg.Is<Func<IModelLinker, bool>>(x => x.GetName() == "invalidLinker"))
            .Returns(null as IModelLinker);

        await _linkContextService.Link(
            new List<string> { "linker1", "invalidLinker" },
            CancellationToken.None
        );

        await modelLinker1
            .Received(1)
            .LinkModel(project.Context.Models, cancellationToken: CancellationToken.None);
    }
}
