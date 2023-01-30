namespace ScriptBeeWebApp.EndpointDefinitions.Arguments;

public record UploadModelFormData(
    string LoaderName,
    string ProjectId,
    List<IFormFile> Files
);
