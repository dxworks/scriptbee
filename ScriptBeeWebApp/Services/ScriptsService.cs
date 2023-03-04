using OneOf;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Data;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Repository;
using ScriptParameter = DxWorks.ScriptBee.Plugin.Api.Model.ScriptParameter;

namespace ScriptBeeWebApp.Services;

public sealed class ScriptsService : IScriptsService
{
    private readonly IGenerateScriptService _generateScriptService;
    private readonly IProjectFileStructureManager _projectFileStructureManager;
    private readonly IProjectStructureService _projectStructureService;
    private readonly IProjectModelService _projectModelService;
    private readonly IScriptModelService _scriptModelService;

    public ScriptsService(IGenerateScriptService generateScriptService,
        IProjectFileStructureManager projectFileStructureManager, IProjectStructureService projectStructureService,
        IProjectModelService projectModelService, IScriptModelService scriptModelService)
    {
        _generateScriptService = generateScriptService;
        _projectFileStructureManager = projectFileStructureManager;
        _projectStructureService = projectStructureService;
        _projectModelService = projectModelService;
        _scriptModelService = scriptModelService;
    }

    public IEnumerable<ScriptLanguage> GetSupportedLanguages()
    {
        return _generateScriptService.GetSupportedLanguages();
    }

    public async Task<OneOf<IEnumerable<ScriptFileStructureNode>, ProjectMissing>> GetScriptsStructureAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        var documentExists = await _projectModelService.DocumentExists(projectId, cancellationToken);
        if (!documentExists)
        {
            return new ProjectMissing(projectId);
        }

        var fileTreeNode = _projectFileStructureManager.GetSrcStructure(projectId);
        if (fileTreeNode is null)
        {
            return new ProjectMissing(projectId);
        }

        var structure = await GetScriptFileStructureAsync(projectId, fileTreeNode, 0, cancellationToken);
        return OneOf<IEnumerable<ScriptFileStructureNode>, ProjectMissing>.FromT0(structure);
    }

    public async Task<OneOf<ScriptDataResponse, ProjectMissing, ScriptMissing>> GetScriptByFilePathAsync(
        string filepath, string projectId, CancellationToken cancellationToken = default)
    {
        var documentExists = await _projectModelService.DocumentExists(projectId, cancellationToken);
        if (!documentExists)
        {
            return new ProjectMissing(projectId);
        }

        var scriptModel =
            await _scriptModelService.GetScriptModelByFilePathAsync(filepath, projectId, cancellationToken);
        if (scriptModel is null)
        {
            return new ScriptMissing(filepath);
        }

        return CreateScriptResponse(scriptModel);
    }

    public async Task<OneOf<ScriptDataResponse, ProjectMissing, ScriptMissing>> GetScriptByIdAsync(string scriptId,
        string projectId, CancellationToken cancellationToken = default)
    {
        var documentExists = await _projectModelService.DocumentExists(projectId, cancellationToken);
        if (!documentExists)
        {
            return new ProjectMissing(projectId);
        }

        var scriptModel =
            await _scriptModelService.GetDocument(scriptId, cancellationToken);
        if (scriptModel is null)
        {
            return new ScriptMissing(scriptId);
        }

        return CreateScriptResponse(scriptModel);
    }

    public async Task<OneOf<string, ProjectMissing, ScriptMissing>> GetScriptContentAsync(string scriptId,
        string projectId, CancellationToken cancellationToken = default)
    {
        var documentExists = await _projectModelService.DocumentExists(projectId, cancellationToken);
        if (!documentExists)
        {
            return new ProjectMissing(projectId);
        }

        var scriptModel = await _scriptModelService.GetDocument(scriptId, cancellationToken);
        if (scriptModel is null)
        {
            return new ScriptMissing(scriptId);
        }

        var content = await _projectFileStructureManager.GetFileContentAsync(projectId, scriptModel.FilePath);
        if (content is null)
        {
            return new ScriptMissing(scriptId);
        }

        return content;
    }

    public async Task<OneOf<ScriptDataResponse, ProjectMissing, ScriptConflict, InvalidScriptType>> CreateScriptAsync(
        CreateScript createScript, CancellationToken cancellationToken = default)
    {
        if (!IsScriptTypeSupported(createScript.ScriptLanguage))
        {
            return new InvalidScriptType(createScript.ScriptLanguage);
        }

        var documentExists = await _projectModelService.DocumentExists(createScript.ProjectId, cancellationToken);
        if (!documentExists)
        {
            return new ProjectMissing(createScript.ProjectId);
        }

        var (extension, content) =
            await _projectStructureService.GetSampleCodeAsync(createScript.ScriptLanguage, cancellationToken);

        if (!createScript.FilePath.EndsWith(extension))
        {
            createScript.FilePath += extension;
        }

        if (_projectFileStructureManager.FileExists(createScript.ProjectId, createScript.FilePath))
        {
            return new ScriptConflict();
        }

        var node = _projectFileStructureManager.CreateSrcFile(createScript.ProjectId, createScript.FilePath, content);

        var scriptModel = CreateScriptModel(createScript, node);
        await _scriptModelService.CreateDocument(scriptModel, cancellationToken);

        return CreateScriptResponse(scriptModel);
    }

    public async Task<OneOf<ScriptDataResponse, ProjectMissing, ScriptMissing>> UpdateScriptAsync(
        UpdateScript updateScript, CancellationToken cancellationToken = default)
    {
        var documentExists = await _projectModelService.DocumentExists(updateScript.ProjectId, cancellationToken);
        if (!documentExists)
        {
            return new ProjectMissing(updateScript.ProjectId);
        }

        var scriptModel = await _scriptModelService.GetDocument(updateScript.Id, cancellationToken);
        if (scriptModel is null)
        {
            return new ScriptMissing(updateScript.Id);
        }

        scriptModel.Parameters = updateScript.Parameters
            .Select(p => new ScriptParameter
            {
                Name = p.Name,
                Type = p.Type,
                Value = p.Value
            })
            .ToList();

        await _scriptModelService.UpdateDocument(scriptModel, cancellationToken);

        return CreateScriptResponse(scriptModel);
    }

    public async Task<OneOf<ScriptDataResponse, ProjectMissing, ScriptMissing>> DeleteScriptAsync(string scriptId,
        string projectId, CancellationToken cancellationToken = default)
    {
        var documentExists = await _projectModelService.DocumentExists(projectId, cancellationToken);
        if (!documentExists)
        {
            return new ProjectMissing(projectId);
        }

        var scriptModel = await _scriptModelService.GetDocument(scriptId, cancellationToken);
        if (scriptModel is null)
        {
            _projectFileStructureManager.DeleteFile(projectId, scriptId);

            return new ScriptMissing(scriptId);
        }

        await _scriptModelService.DeleteDocument(scriptId, cancellationToken);
        _projectFileStructureManager.DeleteFile(projectId, scriptId);

        return CreateScriptResponse(scriptModel);
    }

    private async Task<IEnumerable<ScriptFileStructureNode>> GetScriptFileStructureAsync(string projectId,
        FileTreeNode fileTreeNode, int level, CancellationToken cancellationToken)
    {
        var scriptFileStructureNodes = new List<ScriptFileStructureNode>();

        if (fileTreeNode.Children is null)
        {
            return scriptFileStructureNodes;
        }

        foreach (var treeNode in fileTreeNode.Children)
        {
            if (treeNode.Children is null)
            {
                var node = await AddScriptToFileStructure(projectId, treeNode, level, cancellationToken);
                scriptFileStructureNodes.Add(node);
            }
            else
            {
                scriptFileStructureNodes.Add(new ScriptFileStructureNode
                {
                    IsDirectory = true,
                    Children = await GetScriptFileStructureAsync(projectId, treeNode, level + 1, cancellationToken),
                    Name = treeNode.Name,
                    Path = treeNode.SrcPath,
                    AbsolutePath = _projectFileStructureManager.GetAbsoluteFilePath(projectId, treeNode.FilePath),
                    Level = level,
                    ScriptData = null
                });
            }
        }

        return scriptFileStructureNodes;
    }

    private async Task<ScriptFileStructureNode> AddScriptToFileStructure(string projectId, FileTreeNode treeNode,
        int level, CancellationToken cancellationToken)
    {
        string absoluteFilePath;
        ScriptDataResponse? scriptData = null;

        var scriptModel = await _scriptModelService.GetScriptModelByFilePathAsync(treeNode.FilePath, projectId,
            cancellationToken);
        if (scriptModel is not null)
        {
            absoluteFilePath = scriptModel.AbsoluteFilePath;
            scriptData = CreateScriptResponse(scriptModel);
        }
        else
        {
            absoluteFilePath = _projectFileStructureManager.GetAbsoluteFilePath(projectId, treeNode.FilePath);
        }

        return new ScriptFileStructureNode
        {
            IsDirectory = false,
            Children = null,
            Name = treeNode.Name,
            Path = treeNode.SrcPath,
            AbsolutePath = absoluteFilePath,
            Level = level,
            ScriptData = scriptData
        };
    }

    private bool IsScriptTypeSupported(string scriptType)
    {
        return _generateScriptService.GetSupportedLanguages().Any(language => language.Name == scriptType);
    }

    private ScriptModel CreateScriptModel(CreateScript createScript, FileTreeNode fileTreeNode)
    {
        return new ScriptModel
        {
            Id = createScript.FilePath,
            ProjectId = createScript.ProjectId,
            Name = fileTreeNode.Name,
            FilePath = createScript.FilePath,
            AbsoluteFilePath =
                _projectFileStructureManager.GetAbsoluteFilePath(createScript.ProjectId, createScript.FilePath),
            ScriptLanguage = createScript.ScriptLanguage,
            Parameters = createScript.Parameters.Select(CreateScriptParameterModel).ToList()
        };
    }

    private static ScriptParameter CreateScriptParameterModel(EndpointDefinitions.Arguments.ScriptParameter parameter)
    {
        return new ScriptParameter
        {
            Name = parameter.Name,
            Type = parameter.Type,
            Value = parameter.Value
        };
    }

    private static ScriptDataResponse CreateScriptResponse(ScriptModel scriptModel)
    {
        return new ScriptDataResponse(
            scriptModel.Id,
            scriptModel.ProjectId,
            scriptModel.Name,
            scriptModel.FilePath,
            scriptModel.AbsoluteFilePath,
            scriptModel.ScriptLanguage,
            scriptModel.Parameters.Select(CreateScriptParameterResponse).ToList()
        );
    }

    private static ScriptParameterResponse CreateScriptParameterResponse(ScriptParameter parameter)
    {
        return new ScriptParameterResponse(parameter.Name, parameter.Type, parameter.Value);
    }
}
