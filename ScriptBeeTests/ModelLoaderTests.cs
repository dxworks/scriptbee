using ScriptBee.Models.Dummy;
using Xunit;

namespace ScriptBeeTests
{
    public class ModelLoaderTests
    {
        [Fact]
        public void Load_Test()
        {
            string json = @"{""DummyNumber"":10,""DummyString"":""dummy"",""IsDummy"":true}";

            DummyModelLoader dummyModelLoader = new DummyModelLoader();
            DummyModel dummyModel = dummyModelLoader.LoadModel(json);

            Assert.Equal(10, dummyModel.DummyNumber);
            Assert.Equal("dummy", dummyModel.DummyString);
            Assert.True(dummyModel.IsDummy);
        }
    }
}