namespace ScriptBee.Utils.ValidScriptExtractors
{
    public class PythonValidScriptExtractor : ValidScriptExtractor
    {
        protected override string StartComment => ValidScriptDelimiters.PythonStartComment;
        protected override string EndComment => ValidScriptDelimiters.PythonEndComment;
    }
}