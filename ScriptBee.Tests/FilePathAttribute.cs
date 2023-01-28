﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace ScriptBee.Tests;

public class FilePathAttribute : DataAttribute
{
    private readonly string _filePath;
    private readonly string? _parameter;

    public FilePathAttribute(string filePath, string? parameter = null, [CallerFilePath] string testFilePath = "")
    {
        var testDataFolder =
            Path.GetRelativePath(Directory.GetCurrentDirectory(), Directory.GetParent(testFilePath)!.FullName)
                .Replace("..\\", "");
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), testDataFolder, filePath);
        _parameter = parameter;
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (_parameter is null)
        {
            yield return new object[] { _filePath };
        }
        else
        {
            yield return new object[] { _filePath, _parameter };
        }
    }
}
