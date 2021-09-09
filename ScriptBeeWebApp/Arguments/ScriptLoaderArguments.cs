using System.Collections.Generic;

namespace ScriptBeeWebApp.Arguments
{
    public class ScriptLoaderArguments
    {
        public List<string> paths { get; set; }
        public string loaderName { get; set; }
        public string projectId { get; set; }
    }
}