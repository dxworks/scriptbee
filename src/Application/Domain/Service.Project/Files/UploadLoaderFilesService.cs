using OneOf;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.UseCases.Project.Files;

namespace ScriptBee.Service.Project.Files;

public class UploadLoaderFilesService : IUploadLoaderFilesUseCase
{
    public Task<OneOf<IEnumerable<FileData>, ProjectDoesNotExistsError>> Upload(
        UploadLoaderFilesCommand command,
        CancellationToken cancellationToken = default
    )
    {
        // await DeletePreviousSavedFiles(projectModel, loaderName, cancellationToken);
        //
        // var savedFilesData = await UploadFiles(files, cancellationToken);
        //
        // projectModel.SavedFiles[loaderName] = savedFilesData;
        //
        // await _projectModelService.UpdateDocument(projectModel, cancellationToken);
        //
        // return savedFilesData;

        throw new NotImplementedException();
    }
}
