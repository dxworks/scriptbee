using System.Collections.Generic;
using System.IO;

namespace HelperFunctions;

public interface IFileOutputWriter
{
    void FileWrite(string fileName, string fileContent);

    void FileWriteStream(string fileName, Stream stream);

    void ExportJson<T>(string fileName, T obj);

    void ExportCsv<T>(string fileName, List<T> records);
}