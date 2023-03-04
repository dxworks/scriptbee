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
using ScriptParameter = DxWorks.ScriptBee.Plugin.Api.Model.ScriptParameter;

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

    #region GetSupportedLanguages

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

    #endregion

    #region GetScriptsStructureAsync

    [Fact]
    public async Task GivenMissingProject_WhenGetScriptsStructureAsync_ThenReturnsProjectMissing()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var actual = await _scriptsService.GetScriptsStructureAsync("projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT1);
        Assert.Equal("projectId", actual.AsT1.ProjectId);
    }

    [Fact]
    public async Task GivenMissingProjectStructure_WhenGetScriptsStructureAsync_ThenReturnsProjectMissing()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectFileStructureManagerMock.Setup(x => x.GetSrcStructure("projectId"))
            .Returns((FileTreeNode?)null);

        var actual = await _scriptsService.GetScriptsStructureAsync("projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT1);
        Assert.Equal("projectId", actual.AsT1.ProjectId);
    }

    [Fact]
    public async Task GivenNullChildren_WhenGetScriptsStructureAsync_ThenReturnsEmptyScripts()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectFileStructureManagerMock.Setup(x => x.GetSrcStructure("projectId"))
            .Returns(new FileTreeNode("src", "src", "src", null));

        var actual = await _scriptsService.GetScriptsStructureAsync("projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT0);
        Assert.Empty(actual.AsT0);
    }

    [Fact]
    public async Task GivenEmptyChildren_WhenGetScriptsStructureAsync_ThenReturnsEmptyScripts()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectFileStructureManagerMock.Setup(x => x.GetSrcStructure("projectId"))
            .Returns(new FileTreeNode("src", "src", "src", new List<FileTreeNode>()));

        var actual = await _scriptsService.GetScriptsStructureAsync("projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT0);
        Assert.Empty(actual.AsT0);
    }

    [Fact]
    public async Task GivenSrcFolderWithScriptWithoutModel_WhenGetScriptsAsync_ThenReturnScripts()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectFileStructureManagerMock.Setup(x => x.GetSrcStructure("projectId"))
            .Returns(new FileTreeNode("src", "src", "src", new List<FileTreeNode>
            {
                new("script1.js", "script1.js", "script1.js", null),
            }));
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("script1.js", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ScriptModel?)null);
        _projectFileStructureManagerMock.Setup(x => x.GetAbsoluteFilePath("projectId", "script1.js"))
            .Returns("absolutePath");

        var actual = await _scriptsService.GetScriptsStructureAsync("projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT0);
        Assert.Single(actual.AsT0);
        var node = actual.AsT0.First();
        Assert.False(node.IsDirectory);
        Assert.Equal("script1.js", node.Name);
        Assert.Equal("absolutePath", node.AbsolutePath);
        Assert.Equal("script1.js", node.Path);
        Assert.Null(node.Children);
        Assert.Null(node.ScriptData);
    }

    [Fact]
    public async Task GivenSrcFolderWithOnlyScripts_WhenGetScriptsAsync_ThenReturnScripts()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectFileStructureManagerMock.Setup(x => x.GetSrcStructure("projectId"))
            .Returns(new FileTreeNode("src", "src", "src", new List<FileTreeNode>
            {
                new("script1.js", "script1.js", "script1.js", null),
                new("script2.js", "script2.js", "script2.js", null),
            }));
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("script1.js", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptModel
            {
                Id = "id1",
                Name = "script1.js"
            });
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("script2.js", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptModel
            {
                Id = "id2",
                Name = "script2.js"
            });

        var actual = await _scriptsService.GetScriptsStructureAsync("projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT0);
        Assert.Equal(2, actual.AsT0.Count());
        var scriptFileStructureNode1 = actual.AsT0.First();
        Assert.Equal("script1.js", scriptFileStructureNode1.Name);
        Assert.Equal("id1", scriptFileStructureNode1.ScriptData!.Id);
        Assert.Equal("script1.js", scriptFileStructureNode1.ScriptData.Name);
        Assert.Null(scriptFileStructureNode1.Children);
        var scriptFileStructureNode2 = actual.AsT0.Last();
        Assert.Equal("script2.js", scriptFileStructureNode2.Name);
        Assert.Equal("id2", scriptFileStructureNode2.ScriptData!.Id);
        Assert.Equal("script2.js", scriptFileStructureNode2.ScriptData.Name);
        Assert.Null(scriptFileStructureNode2.Children);
    }

    [Fact]
    public async Task GivenSrcFolderWithOnlyFolders_WhenGetScriptsAsync_ThenReturnScripts()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectFileStructureManagerMock.Setup(x => x.GetSrcStructure("projectId"))
            .Returns(new FileTreeNode("src", "src", "src", new List<FileTreeNode>
            {
                new("folder1", "folder1", "folder1", new List<FileTreeNode>
                {
                    new("script1.js", "folder1/script1.js", "script1.js", null),
                }),
                new("folder2", "folder2", "folder2", new List<FileTreeNode>
                {
                    new("script2.js", "folder2/script2.js", "script2.js", null),
                }),
            }));
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("folder1/script1.js", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptModel
            {
                Id = "id1",
                Name = "script1.js"
            });
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("folder2/script2.js", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptModel
            {
                Id = "id2",
                Name = "script2.js"
            });

        var actual = await _scriptsService.GetScriptsStructureAsync("projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT0);
        Assert.Equal(2, actual.AsT0.Count());
        var scriptFileStructureNode1 = actual.AsT0.First();
        Assert.Equal("folder1", scriptFileStructureNode1.Name);
        Assert.Null(scriptFileStructureNode1.ScriptData);
        Assert.Single(scriptFileStructureNode1.Children!);
        var scriptFileStructureNode2 = actual.AsT0.Last();
        Assert.Equal("folder2", scriptFileStructureNode2.Name);
        Assert.Null(scriptFileStructureNode2.ScriptData);
        Assert.Single(scriptFileStructureNode2.Children!);
        var scriptFileStructureNode3 = scriptFileStructureNode1.Children!.First();
        Assert.Equal("script1.js", scriptFileStructureNode3.Name);
        Assert.Equal("id1", scriptFileStructureNode3.ScriptData!.Id);
        Assert.Equal("script1.js", scriptFileStructureNode3.ScriptData.Name);
        Assert.Null(scriptFileStructureNode3.Children);
        var scriptFileStructureNode4 = scriptFileStructureNode2.Children!.First();
        Assert.Equal("script2.js", scriptFileStructureNode4.Name);
        Assert.Equal("id2", scriptFileStructureNode4.ScriptData!.Id);
        Assert.Equal("script2.js", scriptFileStructureNode4.ScriptData.Name);
        Assert.Null(scriptFileStructureNode4.Children);
    }

    [Fact]
    public async Task GivenSrcFolderWithScriptsAndFolders_WhenGetScriptsAsync_ThenReturnScripts()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _projectFileStructureManagerMock.Setup(x => x.GetSrcStructure("projectId"))
            .Returns(new FileTreeNode("src", "src", "src", new List<FileTreeNode>
            {
                new("script1.js", "script1.js", "script1.js", null),
                new("folder1", "folder1", "folder1", new List<FileTreeNode>
                {
                    new("script2.js", "folder1/script2.js", "script2.js", null),
                }),
                new("script3.js", "script3.js", "script3.js", null),
                new("folder2", "folder2", "folder2", new List<FileTreeNode>
                {
                    new("script4.js", "folder2/script4.js", "script4.js", null),
                }),
            }));
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("script1.js", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptModel
            {
                Id = "id1",
                Name = "script1.js"
            });
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("folder1/script2.js", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptModel
            {
                Id = "id2",
                Name = "script2.js"
            });
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("script3.js", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptModel
            {
                Id = "id3",
                Name = "script3.js"
            });
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("folder2/script4.js", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptModel
            {
                Id = "id4",
                Name = "script4.js"
            });

        var actual = await _scriptsService.GetScriptsStructureAsync("projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT0);
        Assert.Equal(4, actual.AsT0.Count());
        var scriptFileStructureNode1 = actual.AsT0.First();
        Assert.Equal("script1.js", scriptFileStructureNode1.Name);
        Assert.Equal("id1", scriptFileStructureNode1.ScriptData!.Id);
        Assert.Equal("script1.js", scriptFileStructureNode1.ScriptData.Name);
        Assert.Null(scriptFileStructureNode1.Children);
        var scriptFileStructureNode2 = actual.AsT0.Skip(1).First();
        Assert.Equal("folder1", scriptFileStructureNode2.Name);
        Assert.Null(scriptFileStructureNode2.ScriptData);
        Assert.Single(scriptFileStructureNode2.Children!);
        var scriptFileStructureNode3 = actual.AsT0.Skip(2).First();
        Assert.Equal("script3.js", scriptFileStructureNode3.Name);
        Assert.Equal("id3", scriptFileStructureNode3.ScriptData!.Id);
        Assert.Equal("script3.js", scriptFileStructureNode3.ScriptData.Name);
        Assert.Null(scriptFileStructureNode3.Children);
        var scriptFileStructureNode4 = actual.AsT0.Skip(3).First();
        Assert.Equal("folder2", scriptFileStructureNode4.Name);
        Assert.Null(scriptFileStructureNode4.ScriptData);
        Assert.Single(scriptFileStructureNode4.Children!);
        var scriptFileStructureNode5 = scriptFileStructureNode2.Children!.First();
        Assert.Equal("script2.js", scriptFileStructureNode5.Name);
        Assert.Equal("id2", scriptFileStructureNode5.ScriptData!.Id);
        Assert.Equal("script2.js", scriptFileStructureNode5.ScriptData.Name);
        Assert.Null(scriptFileStructureNode5.Children);
        var scriptFileStructureNode6 = scriptFileStructureNode4.Children!.First();
        Assert.Equal("script4.js", scriptFileStructureNode6.Name);
        Assert.Equal("id4", scriptFileStructureNode6.ScriptData!.Id);
        Assert.Equal("script4.js", scriptFileStructureNode6.ScriptData.Name);
        Assert.Null(scriptFileStructureNode6.Children);
    }

    #endregion

    #region GetScriptByFilePathAsync

    [Fact]
    public async Task GivenMissingProject_WhenGetScriptByFilePathAsync_ThenReturnsProjectMissing()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var actual =
            await _scriptsService.GetScriptByFilePathAsync("filepath", "projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT1);
        Assert.Equal("projectId", actual.AsT1.ProjectId);
    }

    [Fact]
    public async Task GivenMissingFilePath_WhenGetScriptByFilePathAsync_ThenReturnsScriptMissing()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("filepath", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ScriptModel?)null);

        var actual =
            await _scriptsService.GetScriptByFilePathAsync("filepath", "projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT2);
        Assert.Equal("filepath", actual.AsT2.ScriptId);
    }

    [Fact]
    public async Task GivenExistingFilePath_WhenGetScriptByFilePathAsync_ThenReturnsScript()
    {
        var scriptModel = new ScriptModel
        {
            Id = "id",
            Name = "script",
            AbsoluteFilePath = "absolutePath",
            FilePath = "filepath",
            ProjectId = "projectId",
            ScriptLanguage = "scriptLanguage",
            Parameters = new List<ScriptParameter>
            {
                new()
                {
                    Name = "name",
                    Type = "type",
                    Value = "value",
                },
            },
        };
        var expectedScriptResponse = new ScriptDataResponse("id", "projectId", "script", "filepath", "absolutePath",
            "scriptLanguage", new List<ScriptParameterResponse>
            {
                new("name", "type", "value"),
            });
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _scriptModelServiceMock.Setup(x =>
                x.GetScriptModelByFilePathAsync("filepath", "projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(scriptModel);

        var actual =
            await _scriptsService.GetScriptByFilePathAsync("filepath", "projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT0);
        Assert.True(VerifyScriptDataResponse(expectedScriptResponse, actual.AsT0));
    }

    #endregion

    #region GetScriptContentAsync

    [Fact]
    public async Task GivenMissingProject_WhenGetScriptContentAsync_ThenReturnsProjectMissing()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var actual =
            await _scriptsService.GetScriptContentAsync("filepath", "projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT1);
        Assert.Equal("projectId", actual.AsT1.ProjectId);
    }

    [Fact]
    public async Task GivenMissingScriptId_WhenGetScriptContentAsync_ThenReturnsScriptMissing()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _scriptModelServiceMock.Setup(x => x.GetDocument("scriptId", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ScriptModel?)null);

        var actual =
            await _scriptsService.GetScriptContentAsync("scriptId", "projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT2);
        Assert.Equal("scriptId", actual.AsT2.ScriptId);
    }

    [Fact]
    public async Task GivenMissingFilePath_WhenGetScriptContentAsync_ThenReturnsScriptMissing()
    {
        var scriptModel = new ScriptModel
        {
            Id = "scriptId",
            Name = "script",
            AbsoluteFilePath = "srcPath",
            FilePath = "filepath",
            ProjectId = "projectId",
            ScriptLanguage = "scriptLanguage",
            Parameters = new List<ScriptParameter>
            {
                new()
                {
                    Name = "name",
                    Type = "type",
                    Value = "value",
                },
            },
        };
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _scriptModelServiceMock.Setup(x => x.GetDocument("scriptId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(scriptModel);
        _projectFileStructureManagerMock.Setup(x => x.GetFileContentAsync("projectId", "filepath"))
            .ReturnsAsync((string?)null);

        var actual =
            await _scriptsService.GetScriptContentAsync("scriptId", "projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT2);
        Assert.Equal("scriptId", actual.AsT2.ScriptId);
    }


    [Fact]
    public async Task GivenExistingFilePath_WhenGetScriptContentAsync_ThenReturnsScriptContent()
    {
        var scriptModel = new ScriptModel
        {
            Id = "id",
            Name = "script",
            AbsoluteFilePath = "srcPath",
            FilePath = "filepath",
            ProjectId = "projectId",
            ScriptLanguage = "scriptLanguage",
            Parameters = new List<ScriptParameter>
            {
                new()
                {
                    Name = "name",
                    Type = "type",
                    Value = "value",
                },
            },
        };
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _scriptModelServiceMock.Setup(x => x.GetDocument("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(scriptModel);
        _projectFileStructureManagerMock.Setup(x => x.GetFileContentAsync("projectId", "filepath"))
            .ReturnsAsync("content");

        var actual =
            await _scriptsService.GetScriptContentAsync("id", "projectId", It.IsAny<CancellationToken>());

        Assert.True(actual.IsT0);
        Assert.Equal("content", actual.AsT0);
    }

    #endregion

    #region CreateScriptAsync

    [Fact]
    public async Task GivenEmptySupportedLanguages_WhenCreateScriptAsync_ThenReturnsInvalidScriptType()
    {
        var createScript = new CreateScript("projectId", "filePath", "scriptType",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>());

        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>());

        var actual = await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        Assert.True(actual.IsT3);
    }

    [Fact]
    public async Task GivenInvalidScriptType_WhenCreateScriptAsync_ThenReturnsInvalidScriptType()
    {
        var createScript = new CreateScript("projectId", "filePath", "scriptType",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>());

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
        var createScript = new CreateScript("projectId", "filePath", "scriptType",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>());

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
        var createScript = new CreateScript("projectId", "filePath", "scriptType",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>());

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
        var createScript = new CreateScript("projectId", "filePath.js", "scriptType",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>
            {
                new("name", "type", "value")
            });
        var fileTreeNode = new FileTreeNode("filePath", "fileId", "srcPath", null);
        var createScriptResponse = new ScriptDataResponse(null!, "projectId", "filePath", "filePath.js", "absolutePath",
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
        _projectFileStructureManagerMock.Setup(x => x.GetAbsoluteFilePath(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("absolutePath");
        _projectFileStructureManagerMock
            .Setup(x => x.CreateSrcFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(fileTreeNode);

        var actual = await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        Assert.True(actual.IsT0);
        Assert.Equal(createScriptResponse.ProjectId, actual.AsT0.ProjectId);
        Assert.Equal(createScriptResponse.FilePath, actual.AsT0.FilePath);
        Assert.Equal(createScriptResponse.AbsolutePath, actual.AsT0.AbsolutePath);
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
        var createScript = new CreateScript("projectId", "filePath", "scriptType",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>());
        var fileTreeNode = new FileTreeNode("filePath", "fileId", "srcPath", null);

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
        var createScript = new CreateScript("projectId", "filePath.js", "scriptType",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>());
        var fileTreeNode = new FileTreeNode("filePath", "fileId", "srcPath", null);

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
        var createScript = new CreateScript("projectId", "filePath.js", "scriptType",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>());
        var scriptModel = new ScriptModel
        {
            Id = "filePath.js",
            ProjectId = "projectId",
            Name = "filePath",
            FilePath = "filePath.js",
            AbsoluteFilePath = "absoluteFilePath",
            ScriptLanguage = "scriptType",
            Parameters = new List<ScriptParameter>(),
        };
        var fileTreeNode = new FileTreeNode("filePath", "fileId", "srcPath", null);


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
        _projectFileStructureManagerMock.Setup(x => x.GetAbsoluteFilePath("projectId", "filePath.js"))
            .Returns("absoluteFilePath");
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
        var createScript = new CreateScript("projectId", "filePath.js", "scriptType",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>
            {
                new("parameterName", "parameterType", "parameterValue"),
            });
        var scriptModel = new ScriptModel
        {
            Id = "filePath.js",
            ProjectId = "projectId",
            Name = "filePath",
            FilePath = "filePath.js",
            AbsoluteFilePath = "absoluteFilePath",
            ScriptLanguage = "scriptType",
            Parameters = new List<ScriptParameter>
            {
                new()
                {
                    Name = "parameterName",
                    Type = "parameterType",
                    Value = "parameterValue",
                },
            },
        };
        var fileTreeNode = new FileTreeNode("filePath", "filePath.js", "srcPath", null);

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
        _projectFileStructureManagerMock.Setup(x => x.GetAbsoluteFilePath("projectId", "filePath.js"))
            .Returns("absoluteFilePath");
        _projectFileStructureManagerMock
            .Setup(x => x.CreateSrcFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(fileTreeNode);

        await _scriptsService.CreateScriptAsync(createScript, It.IsAny<CancellationToken>());

        _scriptModelServiceMock.Verify(
            x => x.CreateDocument(It.Is<ScriptModel>(s => VerifyScriptModel(scriptModel, s)),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateScriptAsync

    [Fact]
    public async Task GivenMissingProject_WhenUpdateScriptAsync_ThenProjectMissingIsReturned()
    {
        var updateScript = new UpdateScript("scriptId", "projectId",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>());

        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _scriptsService.UpdateScriptAsync(updateScript, It.IsAny<CancellationToken>());

        Assert.True(result.IsT1);
        Assert.Equal("projectId", result.AsT1.ProjectId);
    }

    [Fact]
    public async Task GivenMissingScript_WhenUpdateScriptAsync_ThenScriptMissingIsReturned()
    {
        var updateScript = new UpdateScript("scriptId", "projectId",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>());

        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _scriptModelServiceMock.Setup(x => x.GetDocument("scriptId", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ScriptModel?)null);

        var result = await _scriptsService.UpdateScriptAsync(updateScript, It.IsAny<CancellationToken>());

        Assert.True(result.IsT2);
        Assert.Equal("scriptId", result.AsT2.ScriptId);
    }

    [Fact]
    public async Task GivenValidScript_WhenUpdateScriptAsync_ThenScriptInformationIsStored()
    {
        var updateScript = new UpdateScript("scriptId", "projectId",
            new List<ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter>
            {
                new("parameterName", "parameterType", "parameterValue"),
            });
        var scriptModel = new ScriptModel
        {
            Id = "scriptId",
            ProjectId = "projectId",
            Name = "filePath",
            FilePath = "filePath.js",
            AbsoluteFilePath = "absoluteFilePath",
            ScriptLanguage = "scriptType",
            Parameters = new List<ScriptParameter>(),
        };
        var updatedScriptModel = new ScriptModel
        {
            Id = "scriptId",
            ProjectId = "projectId",
            Name = "filePath",
            FilePath = "filePath.js",
            AbsoluteFilePath = "absoluteFilePath",
            ScriptLanguage = "scriptType",
            Parameters = new List<ScriptParameter>
            {
                new()
                {
                    Name = "parameterName",
                    Type = "parameterType",
                    Value = "parameterValue",
                },
            },
        };

        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _scriptModelServiceMock.Setup(x => x.GetDocument("scriptId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(scriptModel);

        await _scriptsService.UpdateScriptAsync(updateScript, It.IsAny<CancellationToken>());

        _scriptModelServiceMock.Verify(
            x => x.UpdateDocument(It.Is<ScriptModel>(s => VerifyScriptModel(updatedScriptModel, s)),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteScriptAsync

    [Fact]
    public async Task GivenMissingProject_WhenDeleteScriptAsync_ThenProjectMissingIsReturned()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _scriptsService.DeleteScriptAsync("scriptId", "projectId", It.IsAny<CancellationToken>());

        Assert.True(result.IsT1);
        Assert.Equal("projectId", result.AsT1.ProjectId);
    }

    [Fact]
    public async Task GivenMissingScript_WhenDeleteScriptAsync_ThenScriptMissingIsReturned()
    {
        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _scriptModelServiceMock.Setup(x => x.GetDocument("scriptId", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ScriptModel?)null);

        var result = await _scriptsService.DeleteScriptAsync("scriptId", "projectId", It.IsAny<CancellationToken>());

        Assert.True(result.IsT2);
        Assert.Equal("scriptId", result.AsT2.ScriptId);
    }
    
    [Fact]
    public async Task GivenValidScript_WhenDeleteScriptAsync_ThenScriptIsDeleted()
    {
        var scriptModel = new ScriptModel
        {
            Id = "scriptId",
            ProjectId = "projectId",
            Name = "filePath",
            FilePath = "filePath.js",
            AbsoluteFilePath = "absoluteFilePath",
            ScriptLanguage = "scriptType",
            Parameters = new List<ScriptParameter>(),
        };

        _projectModelServiceMock.Setup(x => x.DocumentExists("projectId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _scriptModelServiceMock.Setup(x => x.GetDocument("scriptId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(scriptModel);

        await _scriptsService.DeleteScriptAsync("scriptId", "projectId", It.IsAny<CancellationToken>());

        _scriptModelServiceMock.Verify(
            x => x.DeleteDocument("scriptId", It.IsAny<CancellationToken>()), Times.Once);
    }
    
    #endregion

    private static bool VerifyScriptDataResponse(ScriptDataResponse expected, ScriptDataResponse actual)
    {
        return expected.Id == actual.Id &&
               expected.ProjectId == actual.ProjectId &&
               expected.Name == actual.Name &&
               expected.FilePath == actual.FilePath &&
               expected.AbsolutePath == actual.AbsolutePath &&
               expected.ScriptLanguage == actual.ScriptLanguage &&
               VerifyScriptParameters(expected.Parameters.ToList(), actual.Parameters.ToList());
    }

    private static bool VerifyScriptModel(ScriptModel expected, ScriptModel actual)
    {
        return expected.Id == actual.Id &&
               expected.ProjectId == actual.ProjectId &&
               expected.Name == actual.Name &&
               expected.FilePath == actual.FilePath &&
               expected.AbsoluteFilePath == actual.AbsoluteFilePath &&
               expected.ScriptLanguage == actual.ScriptLanguage &&
               VerifyScriptParameters(expected.Parameters, actual.Parameters);
    }

    private static bool VerifyScriptParameters(IReadOnlyList<ScriptParameterResponse> expectedParameters,
        IReadOnlyList<ScriptParameterResponse> actualParameters)
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

    private static bool VerifyScriptParameters(IReadOnlyList<ScriptParameter> expectedParameters,
        IReadOnlyList<ScriptParameter> actualParameters)
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
