using ScriptBee.Web.EndpointDefinitions.Project.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Files.Contracts;

public record WebUploadSavedFilesResponse(IEnumerable<WebFileData> Files);
