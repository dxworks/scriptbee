using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HelperFunctions;

namespace ScriptBeeWebApp.Services;

public class HelperFunctionsMapper : IHelperFunctionsMapper
{
    public Dictionary<string, Delegate> GetFunctionsDictionary(IHelperFunctions helperFunctions)
    {
        Type helperFunctionsType = helperFunctions.GetType();
        MethodInfo[] helperFunctionsMethods =
            helperFunctionsType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        Dictionary<string, Delegate> functionsDictionary = new();

        foreach (var methodInfo in helperFunctionsMethods)
        {
            Func<Type[], Type> getType;
            var isAction = methodInfo.ReturnType == (typeof(void));
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);

            if (isAction)
            {
                getType = Expression.GetActionType;
            }
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[] { methodInfo.ReturnType });
            }

            if (methodInfo.IsStatic)
            {
                functionsDictionary[methodInfo.Name] = Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
            }
            else
            {
                try
                {
                    functionsDictionary[methodInfo.Name] =
                        Delegate.CreateDelegate(getType(types.ToArray()), helperFunctions, methodInfo.Name);
                }
                catch
                {
                    
                }
            }
        }

        return functionsDictionary;
    }
}