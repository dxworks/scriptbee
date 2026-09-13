using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Files;

public record UploadSavedFilesCommand(
    ProjectId ProjectId,
    IEnumerable<UploadFileInformation> UploadFiles
);
