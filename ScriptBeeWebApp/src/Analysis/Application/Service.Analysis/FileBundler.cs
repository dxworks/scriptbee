using System.Buffers.Binary;
using System.Text;
using ScriptBee.Common.CodeGeneration;

namespace ScriptBee.Service.Analysis;

public class FileBundler
{
    public async Task WriteToStream(
        IEnumerable<SampleCodeFile> files,
        Stream outputStream,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var file in files)
        {
            var pathBytes = Encoding.UTF8.GetBytes(file.Name);
            var contentBytes = Encoding.UTF8.GetBytes(file.Content);

            var pathLengthBuffer = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(pathLengthBuffer, pathBytes.Length);
            await outputStream.WriteAsync(pathLengthBuffer, cancellationToken);

            await outputStream.WriteAsync(pathBytes, cancellationToken);

            var contentLengthBuffer = new byte[8];
            BinaryPrimitives.WriteInt64BigEndian(contentLengthBuffer, contentBytes.Length);
            await outputStream.WriteAsync(contentLengthBuffer, cancellationToken);

            await outputStream.WriteAsync(contentBytes, cancellationToken);
        }

        var endBuffer = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(endBuffer, 0);
        await outputStream.WriteAsync(endBuffer, cancellationToken);
    }
}
