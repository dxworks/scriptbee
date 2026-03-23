using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Files;

public record UploadLoaderFilesCommand(
    ProjectId ProjectId,
    string LoaderId,
    IEnumerable<UploadFileInformation> UploadFiles
);
