namespace ScriptBee.Web.EndpointDefinitions.Loaders.Contracts;

public record WebUploadLoaderFilesResponse(string LoaderId, IEnumerable<string> FileNames);
