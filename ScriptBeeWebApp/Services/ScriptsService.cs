using OneOf;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Data;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Repository;

namespace ScriptBeeWebApp.Services;

public class ScriptsService : IScriptsService
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

    public async Task<OneOf<CreateScriptResponse, ProjectMissing, ScriptConflict, InvalidScriptType>> CreateScriptAsync(
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

    private bool IsScriptTypeSupported(string scriptType)
    {
        return _generateScriptService.GetSupportedLanguages().Any(language => language.Name == scriptType);
    }

    private static ScriptModel CreateScriptModel(CreateScript createScript, FileTreeNode fileTreeNode)
    {
        return new ScriptModel
        {
            ProjectId = createScript.ProjectId,
            Name = fileTreeNode.name,
            FilePath = createScript.FilePath,
            SrcPath = fileTreeNode.srcPath,
            ScriptLanguage = createScript.ScriptLanguage,
            Parameters = createScript.Parameters.Select(CreateScriptParameterModel).ToList()
        };
    }

    private static ScriptParameterModel CreateScriptParameterModel(ScriptParameter parameter)
    {
        return new ScriptParameterModel
        {
            Name = parameter.Name,
            Type = parameter.Type,
            Value = parameter.Value
        };
    }

    private static CreateScriptResponse CreateScriptResponse(ScriptModel scriptModel)
    {
        return new CreateScriptResponse(
            scriptModel.Id,
            scriptModel.ProjectId,
            scriptModel.Name,
            scriptModel.FilePath,
            scriptModel.SrcPath,
            scriptModel.ScriptLanguage,
            scriptModel.Parameters.Select(CreateScriptParameterResponse).ToList()
        );
    }

    private static ScriptParameterResponse CreateScriptParameterResponse(ScriptParameterModel parameter)
    {
        return new ScriptParameterResponse(parameter.Name, parameter.Type, parameter.Value);
    }
}
