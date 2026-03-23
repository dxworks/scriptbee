namespace ScriptBee.Domain.Model.File;

public record UploadFileInformation(string FileName, long Length, Stream FileStream);
