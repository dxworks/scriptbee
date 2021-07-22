using System;
using ScriptBee.Models;
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

            ModelUnloader modelLoader = new ModelUnloader();
            string unloadedModel = modelLoader.Unload(dummyModel);
            
            Assert.Equal(json, unloadedModel);
        }
    }
}