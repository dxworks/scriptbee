using System.Buffers.Binary;
using System.Text;
using ScriptBee.Common.CodeGeneration;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class FileBundlerTests
{
    private readonly FileBundler _fileBundler = new();

    [Fact]
    public async Task WriteToStream_ShouldEncodeFilesCorrectly()
    {
        // Arrange
        var files = new List<SampleCodeFile>
        {
            new() { Name = "file1.cs", Content = "content1" },
            new() { Name = "dir/file2.py", Content = "content2" },
        };
        using var stream = new MemoryStream();

        // Act
        await _fileBundler.WriteToStream(files, stream, TestContext.Current.CancellationToken);
        stream.Position = 0;

        // Assert
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        foreach (var file in files)
        {
            var pathLength = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
            Assert.Equal(file.Name.Length, pathLength);

            var path = Encoding.UTF8.GetString(reader.ReadBytes(pathLength));
            Assert.Equal(file.Name, path);

            var contentLength = BinaryPrimitives.ReadInt64BigEndian(reader.ReadBytes(8));
            Assert.Equal(file.Content.Length, contentLength);

            var content = Encoding.UTF8.GetString(reader.ReadBytes((int)contentLength));
            Assert.Equal(file.Content, content);
        }

        var endMarker = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
        Assert.Equal(0, endMarker);
        Assert.Equal(stream.Length, stream.Position);
    }

    [Fact]
    public async Task WriteToStream_EmptyList_ShouldWriteOnlyEndMarker()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        await _fileBundler.WriteToStream([], stream, TestContext.Current.CancellationToken);
        stream.Position = 0;

        // Assert
        var buffer = new byte[4];
        var read = await stream.ReadAsync(buffer, TestContext.Current.CancellationToken);
        Assert.Equal(4, read);
        Assert.Equal(0, BinaryPrimitives.ReadInt32BigEndian(buffer));
        Assert.Equal(stream.Length, stream.Position);
    }
}
