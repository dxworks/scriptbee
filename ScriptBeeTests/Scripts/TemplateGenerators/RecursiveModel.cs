﻿using ScriptBee.Models.Dummy;

namespace ScriptBeeTests.Scripts.TemplateGenerators
{
    public class RecursiveModel
    {
        public long longField;

        public DummyModel dummyField1;
        
        public DummyModel dummyField2 { get; set; }

        public RecursiveModel recursiveModel;
    }

    public class RecursiveModel2
    {
        public DummyModel dummyField1;
        
        public DummyModel dummyField2;

        public char value;
    }
}