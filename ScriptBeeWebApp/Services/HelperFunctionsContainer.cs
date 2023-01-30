﻿using System.Linq.Expressions;
using System.Reflection;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;

namespace ScriptBeeWebApp.Services;

// todo add tests
public class HelperFunctionsContainer : IHelperFunctionsContainer
{
    private readonly IEnumerable<IHelperFunctions> _helperFunctions;

    public HelperFunctionsContainer(IEnumerable<IHelperFunctions> helperFunctions)
    {
        _helperFunctions = helperFunctions;
    }

    public Dictionary<string, Delegate> GetFunctionsDictionary()
    {
        Dictionary<string, Delegate> functionsDictionary = new();

        foreach (var helperFunction in _helperFunctions)
        {
            var helperFunctionsType = helperFunction.GetType();
            var helperFunctionsMethods =
                helperFunctionsType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var methodInfo in GetUniqueMethods(helperFunctionsMethods))
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
                    functionsDictionary[methodInfo.Name] =
                        Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
                }
                else
                {
                    try
                    {
                        functionsDictionary[methodInfo.Name] = Delegate.CreateDelegate(getType(types.ToArray()),
                            helperFunction, methodInfo.Name);
                    }
                    catch
                    {
                        // todo log error
                    }
                }
            }
        }

        return functionsDictionary;
    }

    private IEnumerable<MethodInfo> GetUniqueMethods(MethodInfo[] helperFunctionsMethods)
    {
        return helperFunctionsMethods
            .GroupBy(m => m.Name)
            .Select(g =>
            {
                if (g.Count() == 1)
                {
                    return g.First();
                }

                return g.OrderByDescending(m => m.GetParameters().Length).ThenByDescending(m =>
                    m.GetParameters().Count(p => p.ParameterType == typeof(object))).First();
            });
    }

    public IEnumerable<IHelperFunctions> GetFunctions()
    {
        return _helperFunctions;
    }
}
