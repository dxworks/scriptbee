using System.IO;
using Microsoft.Extensions.Options;
using ScriptBee.Config;
using ScriptBee.Services.Config;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Services;

public class ConfigFoldersServiceTests
{
    [Fact]
    public void GivenUserFolderVariable_WhenGetAbsoluteFilePath_ThenUserFolderVariableIsUsed()
    {
        var configFoldersService = new ConfigFoldersService(Options.Create(new UserFolderSettings
        {
            UserFolderPath = "user_folder\\file/path"
        }));

        var absoluteFilePath = configFoldersService.GetAbsoluteFilePath("project", "file");

        Assert.Equal("user_folder/file/path/projects/project/src/file", absoluteFilePath);
    }

    [Fact]
    public void NotGivenUserFolderVariable_WhenGetAbsoluteFilePath_ThenUserFolderVariableIsNotUsed()
    {
        var configFoldersService = new ConfigFoldersService(Options.Create(new UserFolderSettings()));

        var absoluteFilePath = configFoldersService.GetAbsoluteFilePath("project", "file");

        Assert.Equal(Path.Combine(ConfigFolders.PathToProjects, "project", ConfigFolders.SrcFolder, "file"),
            absoluteFilePath);
    }

    [Fact]
    public void GivenUserFolderVariable_WhenGetProjectAbsolutePath_ThenUserFolderVariableIsUsed()
    {
        var configFoldersService = new ConfigFoldersService(Options.Create(new UserFolderSettings
        {
            UserFolderPath = "user_folder\\file/path"
        }));

        var absoluteFilePath = configFoldersService.GetProjectAbsolutePath("project");

        Assert.Equal("user_folder/file/path/projects/project", absoluteFilePath);
    }

    [Fact]
    public void NotGivenUserFolderVariable_WhenGetProjectAbsolutePath_ThenUserFolderVariableIsNotUsed()
    {
        var configFoldersService = new ConfigFoldersService(Options.Create(new UserFolderSettings()));

        var absoluteFilePath = configFoldersService.GetProjectAbsolutePath("project");

        Assert.Equal(Path.Combine(ConfigFolders.PathToProjects, "project"), absoluteFilePath);
    }
}
