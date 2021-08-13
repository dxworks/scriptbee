namespace ScriptBee.Utils.ValidScriptExtractors
{
    public class CSharpValidScriptExtractor : ValidScriptExtractor
    {
        protected override string StartComment => ValidScriptDelimiters.CSharpStartComment;
        protected override string EndComment => ValidScriptDelimiters.CSharpEndComment;
    }
}