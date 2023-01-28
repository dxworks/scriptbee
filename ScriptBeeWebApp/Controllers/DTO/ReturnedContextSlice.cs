using System.Collections.Generic;

namespace ScriptBeeWebApp.Controllers.DTO;

public record ReturnedContextSlice(string Name, IEnumerable<string> Models);
