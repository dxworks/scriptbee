using DxWorks.ScriptBee.Plugin.Api.Model;
using NSubstitute;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class ClearContextServiceTest
{
    private readonly IProjectManager _projectManager = Substitute.For<IProjectManager>();
    private readonly IContext _context = Substitute.For<IContext>();

    private readonly ClearContextService _clearContextService;

    public ClearContextServiceTest()
    {
        _clearContextService = new ClearContextService(_projectManager);
    }

    [Fact]
    public void ClearContext()
    {
        _projectManager.GetProject().Returns(new Project { Context = _context });

        _clearContextService.Clear();

        _context.Received(1).Clear();
    }
}
