using System.IO;
using Microsoft.Extensions.Options;
using Moq;
using ScriptBee.Config;
using ScriptBee.ProjectContext;
using ScriptBee.Services.Config;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Services;

public class ProjectFileStructureManagerTests
{
    private readonly Mock<IFileWatcherService> _fileWatcherServiceMock = new();

    [Fact]
    public void GivenUserFolderVariable_WhenGetAbsoluteFilePath_ThenUserFolderVariableIsUsed()
    {
        var projectFileStructureManager =
            new ProjectFileStructureManager(Options.Create(new UserFolderSettings
                {
                    UserFolderPath = "user_folder\\file/path"
                }),
                _fileWatcherServiceMock.Object);

        var absoluteFilePath = projectFileStructureManager.GetAbsoluteFilePath("project", "file");

        Assert.Equal("user_folder/file/path/projects/project/src/file", absoluteFilePath);
    }

    [Fact]
    public void NotGivenUserFolderVariable_WhenGetAbsoluteFilePath_ThenUserFolderVariableIsNotUsed()
    {
        var projectFileStructureManager =
            new ProjectFileStructureManager(Options.Create(new UserFolderSettings()),
                _fileWatcherServiceMock.Object);

        var absoluteFilePath = projectFileStructureManager.GetAbsoluteFilePath("project", "file");

        Assert.Equal(Path.Combine(ConfigFolders.PathToProjects, "project", ConfigFolders.SrcFolder, "file"),
            absoluteFilePath);
    }

    [Fact]
    public void GivenUserFolderVariable_WhenGetProjectAbsolutePath_ThenUserFolderVariableIsUsed()
    {
        var projectFileStructureManager =
            new ProjectFileStructureManager(Options.Create(new UserFolderSettings
                {
                    UserFolderPath = "user_folder\\file/path"
                }),
                _fileWatcherServiceMock.Object);

        var absoluteFilePath = projectFileStructureManager.GetProjectAbsolutePath("project");

        Assert.Equal("user_folder/file/path/projects/project", absoluteFilePath);
    }

    [Fact]
    public void NotGivenUserFolderVariable_WhenGetProjectAbsolutePath_ThenUserFolderVariableIsNotUsed()
    {
        var projectFileStructureManager =
            new ProjectFileStructureManager(Options.Create(new UserFolderSettings()),
                _fileWatcherServiceMock.Object);

        var absoluteFilePath = projectFileStructureManager.GetProjectAbsolutePath("project");

        Assert.Equal(Path.Combine(ConfigFolders.PathToProjects, "project"), absoluteFilePath);
    }
}
