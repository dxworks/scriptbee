using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Files;

public record DeleteSavedFileCommand(ProjectId ProjectId, FileId FileId);
