using ScriptBee.Domain.Model.File;

namespace ScriptBee.Domain.Model.Errors;

public record ProjectFileDoesNotExistsError(FileId FileId);
