using Westwind.Utilities;

namespace ScriptBee.Models.Dummy
{
    public class DummyModel : Expando
    {
        public int DummyNumber { get; set; }
        public string DummyString { get; set; }
        public bool IsDummy { get; set; }
    }
}