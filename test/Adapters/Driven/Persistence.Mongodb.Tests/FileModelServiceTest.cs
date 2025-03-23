using System.Text;
using MongoDB.Driver.GridFS;
using ScriptBee.Domain.Model.File;

namespace ScriptBee.Persistence.Mongodb.Tests;

public class FileModelServiceTest(MongoDbFixture fixture) : IClassFixture<MongoDbFixture>
{
    private readonly FileModelService _sut = new(fixture.Database);
    private readonly GridFSBucket _bucket = new(fixture.Database);

    [Fact]
    public async Task UploadFileAsync_ShouldUploadFile()
    {
        const string fileName = "2af40c6f-4a4f-4a3f-892b-5e44a3ebd8dc";

        await _sut.UploadFileAsync(
            new FileId(fileName),
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
        const string fileName = "2af40c6f-4a4f-4a3f-892b-5e44a3ebd8dc";
        var stream = new MemoryStream("sync test content"u8.ToArray());

        _sut.UploadFile(new FileId(fileName), stream);

        var downloadedStream = _bucket.OpenDownloadStreamByName(fileName);
        using var reader = new StreamReader(downloadedStream);
        var content = reader.ReadToEnd();
        content.ShouldBe("sync test content");
    }

    [Fact]
    public async Task GetFileAsync_ShouldDownloadFile()
    {
        const string fileName = "ef90fb36-b635-405d-83c3-36c6aa3abc50";
        var uploadStream = new MemoryStream("download test content"u8.ToArray());
        await _bucket.UploadFromStreamAsync(fileName, uploadStream);

        var downloadedStream = await _sut.GetFileAsync(
            new FileId("ef90fb36-b635-405d-83c3-36c6aa3abc50")
        );

        using var reader = new StreamReader(downloadedStream);
        var content = await reader.ReadToEndAsync();
        content.ShouldBe("download test content");
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldDeleteFile()
    {
        const string fileName = "d8606c7f-9ad0-4751-93c7-b8ece3b3e0fc";
        var uploadStream = new MemoryStream("delete test content"u8.ToArray());
        await _bucket.UploadFromStreamAsync(fileName, uploadStream);

        await _sut.DeleteFileAsync(new FileId("d8606c7f-9ad0-4751-93c7-b8ece3b3e0fc"));

        await Assert.ThrowsAsync<GridFSFileNotFoundException>(
            () => _bucket.OpenDownloadStreamByNameAsync(fileName)
        );
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldNotThrow_WhenFileNotFound()
    {
        var exception = await Record.ExceptionAsync(
            async () =>
                await _sut.DeleteFileAsync(new FileId("bda201ca-27ef-4790-b8e5-cb383dfdd242"))
        );

        exception.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteFilesAsync_ShouldDeleteMultipleFiles()
    {
        var fileIds = new List<FileId>
        {
            new("7eda1e32-f536-4766-85cb-1109b9089ecb"),
            new("eb61a254-71bd-4a1f-b3c3-3ee2c7f9b4d4"),
        };
        foreach (var fileName in fileIds)
        {
            var uploadStream = new MemoryStream(
                Encoding.UTF8.GetBytes($"delete {fileName} content")
            );
            await _bucket.UploadFromStreamAsync(fileName.ToString(), uploadStream);
        }

        await _sut.DeleteFilesAsync(fileIds);

        foreach (var fileId in fileIds)
        {
            await Assert.ThrowsAsync<GridFSFileNotFoundException>(
                () => _bucket.OpenDownloadStreamByNameAsync(fileId.ToString())
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
