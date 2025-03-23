using System.Text;
using MongoDB.Driver.GridFS;

namespace ScriptBee.Persistence.Mongodb.Tests;

public class FileModelServiceTest(MongoDbFixture fixture) : IClassFixture<MongoDbFixture>
{
    private readonly FileModelService _sut = new(fixture.Database);
    private readonly GridFSBucket _bucket = new(fixture.Database);

    [Fact]
    public async Task UploadFileAsync_ShouldUploadFile()
    {
        const string fileName = "test.txt";

        await _sut.UploadFileAsync(
            fileName,
            new MemoryStream("test content"u8.ToArray()),
            CancellationToken.None
        );

        var downloadedStream = await _bucket.OpenDownloadStreamByNameAsync(fileName);
        using var reader = new StreamReader(downloadedStream);
        var content = await reader.ReadToEndAsync();
        content.ShouldBe("test content");
    }

    [Fact]
    public void UploadFile_ShouldUploadFileSynchronously()
    {
        const string fileName = "testSync.txt";
        var stream = new MemoryStream("sync test content"u8.ToArray());

        _sut.UploadFile(fileName, stream);

        var downloadedStream = _bucket.OpenDownloadStreamByName(fileName);
        using var reader = new StreamReader(downloadedStream);
        var content = reader.ReadToEnd();
        content.ShouldBe("sync test content");
    }

    [Fact]
    public async Task GetFileAsync_ShouldDownloadFile()
    {
        const string fileName = "download.txt";
        var uploadStream = new MemoryStream("download test content"u8.ToArray());
        await _bucket.UploadFromStreamAsync(fileName, uploadStream);

        var downloadedStream = await _sut.GetFileAsync(fileName);

        using var reader = new StreamReader(downloadedStream);
        var content = await reader.ReadToEndAsync();
        content.ShouldBe("download test content");
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldDeleteFile()
    {
        const string fileName = "delete.txt";
        var uploadStream = new MemoryStream("delete test content"u8.ToArray());
        await _bucket.UploadFromStreamAsync(fileName, uploadStream);

        await _sut.DeleteFileAsync(fileName);

        await Assert.ThrowsAsync<GridFSFileNotFoundException>(
            () => _bucket.OpenDownloadStreamByNameAsync(fileName)
        );
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldNotThrow_WhenFileNotFound()
    {
        const string fileName = "nonexistent.txt";

        await _sut.DeleteFileAsync(fileName);
    }

    [Fact]
    public async Task DeleteFilesAsync_ShouldDeleteMultipleFiles()
    {
        var fileNames = new List<string> { "delete1.txt", "delete2.txt" };
        foreach (var fileName in fileNames)
        {
            var uploadStream = new MemoryStream(
                Encoding.UTF8.GetBytes($"delete {fileName} content")
            );
            await _bucket.UploadFromStreamAsync(fileName, uploadStream);
        }

        await _sut.DeleteFilesAsync(fileNames);

        foreach (var fileName in fileNames)
        {
            await Assert.ThrowsAsync<GridFSFileNotFoundException>(
                () => _bucket.OpenDownloadStreamByNameAsync(fileName)
            );
        }
    }

    [Fact]
    public async Task DeleteFilesAsync_WhenEmptyListIsPassed_ShouldNotThrowError()
    {
        var exception = await Record.ExceptionAsync(async () => await _sut.DeleteFilesAsync([]));

        exception.ShouldBeNull();
    }
}
