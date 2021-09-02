using DummyPlugin;
using Xunit;

namespace ScriptBeeTests
{
    public class ModelUnloaderTests
    {
        [Fact]
        public void Unload_Test()
        {
            DummyModel dummyModel = new DummyModel
            {
                DummyNumber = 10,
                DummyString = "dummy",
                IsDummy = true
            };

            string json = @"{""DummyNumber"":10,""DummyString"":""dummy"",""IsDummy"":true}";

            DummyModelUnloader dummyModelLoader = new DummyModelUnloader();
            string unloadedModel = dummyModelLoader.UnloadModel(dummyModel);

            Assert.Equal(json, unloadedModel);
        }
    }
}