using System.Collections.Generic;
using DummyPlugin;
using ScriptBeePlugin;
using Xunit;

namespace ScriptBeeTests
{
    public class ModelLoaderTests
    {
        [Fact]
        public void Load_Test()
        {
            string json = @"{""DummyNumber"":10,""DummyString"":""dummy"",""IsDummy"":true}";
            List<string> fileContents = new List<string>();
            fileContents.Add(json);

            DummyModelLoader dummyModelLoader = new DummyModelLoader();
            var dictionary = dummyModelLoader.LoadModel(fileContents);
            
            Assert.Single(dictionary);
            Assert.True(dictionary.ContainsKey("DummyModel"));

            Dictionary<string, ScriptBeeModel> objects = dictionary["DummyModel"];
            
            Assert.Single(objects);
            
            DummyModel dummyModel = (DummyModel) objects["0"];

            Assert.Equal(10, dummyModel.DummyNumber);
            Assert.Equal("dummy", dummyModel.DummyString);
            Assert.True(dummyModel.IsDummy);
        }
    }
}