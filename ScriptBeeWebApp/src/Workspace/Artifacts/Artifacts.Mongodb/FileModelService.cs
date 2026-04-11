using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ScriptBee.Domain.Model.File;

namespace ScriptBee.Artifacts.Mongodb;

public class FileModelService(IMongoDatabase mongoDatabase) : IFileModelService
{
    private readonly GridFSBucket _bucket = new(mongoDatabase);

    public async Task UploadFileAsync<TMetadata>(
        FileId fileId,
        Stream fileStream,
        TMetadata? metadata = null,
        CancellationToken cancellationToken = default
    )
        where TMetadata : class
    {
        var options = GetUploadOptions(metadata);

        await _bucket.UploadFromStreamAsync(
            fileId.ToString(),
            fileStream,
            options,
            cancellationToken: cancellationToken
        );
    }

    public void UploadFile<TMetadata>(FileId fileId, Stream fileStream, TMetadata? metadata = null)
        where TMetadata : class
    {
        var options = GetUploadOptions(metadata);

        _bucket.UploadFromStream(fileId.ToString(), fileStream, options);
    }

    public async Task<Stream> GetFileAsync(
        FileId fileId,
        CancellationToken cancellationToken = default
    )
    {
        return await _bucket.OpenDownloadStreamByNameAsync(
            fileId.ToString(),
            cancellationToken: cancellationToken
        );
    }

    public async Task<TMetadata?> GetFileMetadataAsync<TMetadata>(
        FileId fileId,
        CancellationToken cancellationToken = default
    )
        where TMetadata : class
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq(f => f.Filename, fileId.ToString());
        using var cursor = await _bucket.FindAsync(filter, cancellationToken: cancellationToken);
        var fileInfo = await cursor.FirstOrDefaultAsync(cancellationToken);

        return fileInfo?.Metadata == null
            ? null
            : BsonSerializer.Deserialize<TMetadata>(fileInfo.Metadata);
    }

    public async Task DeleteFileAsync(FileId fileId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq("filename", fileId.ToString());
        var cursor = await _bucket.FindAsync(filter, cancellationToken: cancellationToken);
        var fileInfo = cursor.FirstOrDefault(cancellationToken);
        if (fileInfo != null)
        {
            await _bucket.DeleteAsync(fileInfo.Id, cancellationToken);
        }
    }

    public async Task DeleteFilesAsync(
        IEnumerable<FileId> fileIds,
        CancellationToken cancellationToken = default
    )
    {
        var tasks = fileIds.Select(fileId => DeleteFileAsync(fileId, cancellationToken)).ToList();

        await Task.WhenAll(tasks);
    }

    private static GridFSUploadOptions? GetUploadOptions<TMetadata>(TMetadata? metadata)
        where TMetadata : class
    {
        return metadata == null
            ? null
            : new GridFSUploadOptions { Metadata = metadata.ToBsonDocument() };
    }
}
