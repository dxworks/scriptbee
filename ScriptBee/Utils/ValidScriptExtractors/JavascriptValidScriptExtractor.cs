namespace ScriptBee.Utils.ValidScriptExtractors
{
    public class JavascriptValidScriptExtractor : ValidScriptExtractor
    {
        protected override string StartComment => ValidScriptDelimiters.JavascriptStartComment;

        protected override string EndComment => ValidScriptDelimiters.JavascriptEndComment;
    }
}