using System;
using System.Collections.Generic;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public interface IStrategyGenerator
    {
        public string GenerateClassName(Type classType);

        public string GenerateClassName(Type classType, Type baseClassType, out HashSet<Type> baseClassGenericTypes);

        public string GenerateClassStart();

        public string GenerateClassEnd();

        public string GenerateField(string fieldModifier, Type fieldType, string fieldName,
            out HashSet<Type> genericTypes);

        public string GenerateProperty(string propertyModifier, Type propertyType, string propertyName,
            out HashSet<Type> genericTypes);

        public string GenerateMethod(string methodModifier, Type methodType, string methodName,
            List<Tuple<Type, string>> methodParams, out HashSet<Type> genericTypes);

        public string GenerateModelDeclaration(string modelType);

        public string GenerateSampleCode();

        public string GenerateEmptyClass();

        public string GenerateImports();

        public string GetStartComment();

        public string GetEndComment();
    }
}