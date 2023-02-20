using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Repository;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Services;

public class ScriptsServiceTests
{
    private readonly Mock<IGenerateScriptService> _generateScriptServiceMock = new();
    private readonly Mock<IProjectFileStructureManager> _projectFileStructureManagerMock = new();
    private readonly Mock<IProjectStructureService> _projectStructureServiceMock = new();
    private readonly Mock<IProjectModelService> _projectModelServiceMock = new();
    private readonly Mock<IScriptModelService> _scriptModelServiceMock = new();
    private readonly ScriptsService _scriptsService;

    public ScriptsServiceTests()
    {
        _scriptsService = new ScriptsService(_generateScriptServiceMock.Object, _projectFileStructureManagerMock.Object,
            _projectStructureServiceMock.Object, _projectModelServiceMock.Object, _scriptModelServiceMock.Object);
    }

    [Fact]
    public void GetSupportedLanguages_ReturnsSupportedLanguages()
    {
        var expected = new List<ScriptLanguage>
        {
            new("Javascript", "js"),
        };
        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(expected);

        var actual = _scriptsService.GetSupportedLanguages();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GivenEmptySupportedLanguages_WhenCreateScriptAsync_ThenReturnsInvalidScriptType()
    {
        var createScript = new CreateScript("projectId", "filePath", "scriptType", new List<ScriptParameter>());

        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>());

        var actual = await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        Assert.True(actual.IsT3);
    }

    [Fact]
    public async Task GivenInvalidScriptType_WhenCreateScriptAsync_ThenReturnsInvalidScriptType()
    {
        var createScript = new CreateScript("projectId", "filePath", "scriptType", new List<ScriptParameter>());

        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>
            {
                new("Javascript", "js"),
            });

        var actual = await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        Assert.True(actual.IsT3);
    }

    [Fact]
    public async Task GivenMissingProject_WhenCreateScriptAsync_ThenReturnsProjectMissing()
    {
        var createScript = new CreateScript("projectId", "filePath", "scriptType", new List<ScriptParameter>());

        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>
            {
                new("scriptType", "st"),
            });
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var actual = await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        Assert.True(actual.IsT1);
    }

    [Fact]
    public async Task GivenExistingScriptPath_WhenCreateScriptAsync_ThenReturnsScriptConflict()
    {
        var createScript = new CreateScript("projectId", "filePath", "scriptType", new List<ScriptParameter>());

        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>
            {
                new("scriptType", "st"),
            });
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectStructureServiceMock.Setup(x => x.GetSampleCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("js", "content"));
        _projectFileStructureManagerMock.Setup(x => x.FileExists(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        var actual = await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        Assert.True(actual.IsT2);
    }

    [Fact]
    public async Task GivenNonExistingScriptPath_WhenCreateScriptAsync_ThenReturnsFileTreeNode()
    {
        var createScript = new CreateScript("projectId", "filePath.js", "scriptType", new List<ScriptParameter>
        {
            new("name", "type", "value")
        });
        var fileTreeNode = new FileTreeNode("filePath", null, "fileId", "srcPath");
        var createScriptResponse = new CreateScriptResponse(null!, "projectId", "filePath", "filePath.js", "srcPath",
            "scriptType", new List<ScriptParameterResponse>
            {
                new("name", "type", "value")
            });

        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>
            {
                new("scriptType", "st"),
            });
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectStructureServiceMock.Setup(x => x.GetSampleCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("js", "content"));
        _projectFileStructureManagerMock.Setup(x => x.FileExists(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);
        _projectFileStructureManagerMock
            .Setup(x => x.CreateSrcFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(fileTreeNode);

        var actual = await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        Assert.True(actual.IsT0);
        Assert.Equal(createScriptResponse.ProjectId, actual.AsT0.ProjectId);
        Assert.Equal(createScriptResponse.FilePath, actual.AsT0.FilePath);
        Assert.Equal(createScriptResponse.SrcPath, actual.AsT0.SrcPath);
        Assert.Equal(createScriptResponse.ScriptLanguage, actual.AsT0.ScriptLanguage);
        var expectedScriptParameterResponses = createScriptResponse.Parameters.ToList();
        var actualScriptParameterResponses = actual.AsT0.Parameters.ToList();
        Assert.Equal(expectedScriptParameterResponses.Count, actualScriptParameterResponses.Count);
        Assert.Equal(expectedScriptParameterResponses[0].Name, actualScriptParameterResponses[0].Name);
        Assert.Equal(expectedScriptParameterResponses[0].Value, actualScriptParameterResponses[0].Value);
        Assert.Equal(expectedScriptParameterResponses[0].Type, actualScriptParameterResponses[0].Type);
    }

    [Fact]
    public async Task GivenFilePathWithoutExtension_WhenCreateScriptAsync_ThenExtensionIsAppended()
    {
        var createScript = new CreateScript("projectId", "filePath", "scriptType", new List<ScriptParameter>());
        var fileTreeNode = new FileTreeNode("filePath", null, "fileId", "srcPath");

        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>
            {
                new("scriptType", "st"),
            });
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectStructureServiceMock.Setup(x => x.GetSampleCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((".js", "content"));
        _projectFileStructureManagerMock.Setup(x => x.FileExists(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);
        _projectFileStructureManagerMock
            .Setup(x => x.CreateSrcFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(fileTreeNode);

        await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        _projectFileStructureManagerMock.Verify(x =>
            x.CreateSrcFile(It.IsAny<string>(), "filePath.js", It.IsAny<string>()));
    }

    [Fact]
    public async Task GivenFilePathWithExtension_WhenCreateScriptAsync_ThenExtensionIsNotAppended()
    {
        var createScript = new CreateScript("projectId", "filePath.js", "scriptType", new List<ScriptParameter>());
        var fileTreeNode = new FileTreeNode("filePath", null, "fileId", "srcPath");

        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>
            {
                new("scriptType", "st"),
            });
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectStructureServiceMock.Setup(x => x.GetSampleCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((".js", "content"));
        _projectFileStructureManagerMock.Setup(x => x.FileExists(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);
        _projectFileStructureManagerMock
            .Setup(x => x.CreateSrcFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(fileTreeNode);

        await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        _projectFileStructureManagerMock.Verify(x =>
            x.CreateSrcFile(It.IsAny<string>(), "filePath.js", It.IsAny<string>()));
    }

    [Fact]
    public async Task GivenValidScriptWithNoParameters_WhenCreateScriptAsync_ThenScriptInformationIsStored()
    {
        var createScript = new CreateScript("projectId", "filePath.js", "scriptType", new List<ScriptParameter>());
        var scriptModel = new ScriptModel
        {
            ProjectId = "projectId",
            Name = "filePath",
            FilePath = "filePath.js",
            SrcPath = "srcPath",
            ScriptLanguage = "scriptType",
            Parameters = new List<ScriptParameterModel>(),
        };
        var fileTreeNode = new FileTreeNode("filePath", null, "fileId", "srcPath");


        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>
            {
                new("scriptType", "st"),
            });
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectStructureServiceMock.Setup(x => x.GetSampleCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((".js", "content"));
        _projectFileStructureManagerMock.Setup(x => x.FileExists(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);
        _projectFileStructureManagerMock
            .Setup(x => x.CreateSrcFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(fileTreeNode);

        await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        _scriptModelServiceMock.Verify(
            x => x.CreateDocument(It.Is<ScriptModel>(s => VerifyScriptModel(scriptModel, s)),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidScriptWithParameters_WhenCreateScriptAsync_ThenScriptInformationIsStored()
    {
        var createScript = new CreateScript("projectId", "filePath.js", "scriptType", new List<ScriptParameter>
        {
            new("parameterName", "parameterType", "parameterValue"),
        });
        var scriptModel = new ScriptModel
        {
            ProjectId = "projectId",
            Name = "filePath",
            FilePath = "filePath.js",
            SrcPath = "srcPath",
            ScriptLanguage = "scriptType",
            Parameters = new List<ScriptParameterModel>
            {
                new()
                {
                    Name = "parameterName",
                    Type = "parameterType",
                    Value = "parameterValue",
                },
            },
        };
        var fileTreeNode = new FileTreeNode("filePath", null, "filePath.js", "srcPath");


        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>
            {
                new("scriptType", "st"),
            });
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectStructureServiceMock.Setup(x => x.GetSampleCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((".js", "content"));
        _projectFileStructureManagerMock.Setup(x => x.FileExists(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);
        _projectFileStructureManagerMock
            .Setup(x => x.CreateSrcFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(fileTreeNode);

        await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        _scriptModelServiceMock.Verify(
            x => x.CreateDocument(It.Is<ScriptModel>(s => VerifyScriptModel(scriptModel, s)),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    private static bool VerifyScriptModel(ScriptModel expected, ScriptModel actual)
    {
        return expected.ProjectId == actual.ProjectId &&
               expected.Name == actual.Name &&
               expected.FilePath == actual.FilePath &&
               expected.SrcPath == actual.SrcPath &&
               expected.ScriptLanguage == actual.ScriptLanguage &&
               VerifyScriptParameters(expected.Parameters, actual.Parameters);
    }

    private static bool VerifyScriptParameters(IReadOnlyList<ScriptParameterModel> expectedParameters,
        IReadOnlyList<ScriptParameterModel> actualParameters)
    {
        if (expectedParameters.Count != actualParameters.Count)
        {
            return false;
        }

        for (var i = 0; i < expectedParameters.Count; i++)
        {
            if (expectedParameters[i].Name != actualParameters[i].Name ||
                expectedParameters[i].Type != actualParameters[i].Type ||
                expectedParameters[i].Value != actualParameters[i].Value)
            {
                return false;
            }
        }

        return true;
    }
}
