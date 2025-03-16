namespace ScriptBee.Ports.Files;

public interface IFileModelService
{
    public Task UploadFileAsync(
        string fileName,
        Stream fileStream,
        CancellationToken cancellationToken = default
    );

    public void UploadFile(string fileName, Stream fileStream);

    public Task<Stream> GetFileAsync(
        string fileName,
        CancellationToken cancellationToken = default
    );

    public Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);

    public Task DeleteFilesAsync(
        IEnumerable<string> fileNames,
        CancellationToken cancellationToken = default
    );
}
