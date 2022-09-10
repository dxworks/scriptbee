namespace ScriptBeeWebApp.Tests.Unit.Services;

public class RunScriptServiceTests
{
    // todo add tests
//         private readonly Mock<IProjectManager> _projectManagerMock;
//     private readonly Mock<IProjectFileStructureManager> _projectFileStructureManagerMock;
//     private readonly Mock<IFileNameGenerator> _fileNameGeneratorMock;
//     private readonly Mock<IFileModelService> _fileModelServiceMock;
//     private readonly Mock<IRunModelService> _runModelServiceMock;
//     private readonly Mock<IProjectModelService> _projectModelServiceMock;
//     private readonly Mock<IHelperFunctionsFactory> _helperFunctionsFactoryMock;
//     private readonly Mock<IHelperFunctionsMapper> _helperFunctionsMapperMock;
//     private readonly Fixture _fixture;
//
//     private readonly RunScriptController _runScriptController;
//
//     public RunScriptControllerTests()
//     {
//         _projectManagerMock = new Mock<IProjectManager>();
//         _projectFileStructureManagerMock = new Mock<IProjectFileStructureManager>();
//         _fileNameGeneratorMock = new Mock<IFileNameGenerator>();
//         _fileModelServiceMock = new Mock<IFileModelService>();
//         _runModelServiceMock = new Mock<IRunModelService>();
//         _projectModelServiceMock = new Mock<IProjectModelService>();
//         _helperFunctionsFactoryMock = new Mock<IHelperFunctionsFactory>();
//         _helperFunctionsMapperMock = new Mock<IHelperFunctionsMapper>();
//         _fixture = new Fixture();
//
//         _runScriptController = new RunScriptController(_projectManagerMock.Object,
//             _projectFileStructureManagerMock.Object,
//             _fileNameGeneratorMock.Object, _fileModelServiceMock.Object, _runModelServiceMock.Object,
//             _projectModelServiceMock.Object, _helperFunctionsFactoryMock.Object, _helperFunctionsMapperMock.Object);
//     }
//
//     [Theory]
//     [InlineData("script.py", @"project: Project")]
//     [InlineData("script.js", @"let project = new Project();")]
//     public async Task GivenValidRunScript_WhenRunScriptFromPath_ThenReturnedRunIsReturned(string filePath,
//         string scriptContent)
//     {
//         var runScriptArg = new RunScript("id1", filePath);
//         var project = _fixture.Build<Project>()
//             .With(p => p.Id, "id1")
//             .With(p => p.Name, "name1")
//             .Create();
//         var projectModel = _fixture.Build<ProjectModel>()
//             .With(p => p.Id, "id1")
//             .With(p => p.Name, "name1")
//             .Create();
//
//         _projectManagerMock.Setup(manager => manager.GetProject("id1"))
//             .Returns(project);
//
//         _projectFileStructureManagerMock.Setup(manager => manager.GetFileContentAsync("id1", filePath))
//             .Returns(Task.FromResult<string?>(scriptContent));
//
//         _fileNameGeneratorMock.Setup(generator => generator.GenerateScriptName("id1", filePath))
//             .Returns("scriptName");
//
//         _projectModelServiceMock.Setup(service => service.GetDocument("id1", It.IsAny<CancellationToken>()))
//             .Returns(Task.FromResult<ProjectModel?>(projectModel));
//
//         var helperFunctionsMock = new Mock<IHelperFunctionsWithResults>();
//         helperFunctionsMock.Setup(functions => functions.GetResults())
//             .Returns(Task.FromResult(new List<RunResult>
//             {
//                 new("Console", "path1"),
//                 new("File", "path2-database"),
//             }));
//
//         _helperFunctionsFactoryMock.Setup(factory => factory.Create("id1", null))
//             .Returns(helperFunctionsMock.Object);
//         _helperFunctionsMapperMock.Setup(mapper => mapper.GetFunctionsDictionary(helperFunctionsMock.Object))
//             .Returns(new Dictionary<string, Delegate>());
//
//         _fileNameGeneratorMock.Setup(generator => generator.ExtractOutputFileNameComponents("path2-database"))
//             .Returns((It.IsAny<string>(), It.IsAny<string>(), "File", "path2"));
//
//
//         var response = await _runScriptController.RunScriptFromPath(runScriptArg, It.IsAny<CancellationToken>());
//         var result = Assert.IsType<OkObjectResult>(response);
//         var returnedRun = (result.Value as ReturnedRun)!;
//
//         MakeAssertions(returnedRun, projectModel);
//     }
//
//     [Fact]
//     public async Task GivenValidCSharpRunScript_WhenRunScriptFromPath_ThenReturnedRunIsReturned()
//     {
//         var runScriptArg = new RunScript("id1", "script.cs");
//         var project = _fixture.Build<Project>()
//             .With(p => p.Id, "id1")
//             .With(p => p.Name, "name1")
//             .Create();
//         var projectModel = _fixture.Build<ProjectModel>()
//             .With(p => p.Id, "id1")
//             .With(p => p.Name, "name1")
//             .Create();
//
//         _projectManagerMock.Setup(manager => manager.GetProject("id1"))
//             .Returns(project);
//
//         _projectFileStructureManagerMock.Setup(manager => manager.GetFileContentAsync("id1", "script.cs"))
//             .Returns(Task.FromResult<string?>(@"using ScriptBee.ProjectContext;
// using HelperFunctions;
// public class ScriptContent
// {
//     public void ExecuteScript(Project project, IHelperFunctions helperFunctions)
//     {
//     }
// }
// "));
//
//         _fileNameGeneratorMock.Setup(generator => generator.GenerateScriptName("id1", "script.cs"))
//             .Returns("scriptName");
//
//         _projectModelServiceMock.Setup(service => service.GetDocument("id1", It.IsAny<CancellationToken>()))
//             .Returns(Task.FromResult<ProjectModel?>(projectModel));
//
//         var helperFunctionsMock = new Mock<IHelperFunctionsWithResults>();
//         helperFunctionsMock.Setup(functions => functions.GetResults())
//             .Returns(Task.FromResult(new List<RunResult>
//             {
//                 new("Console", "path1"),
//                 new("File", "path2-database"),
//             }));
//         _helperFunctionsFactoryMock.Setup(factory => factory.Create("id1", null))
//             .Returns(helperFunctionsMock.Object);
//
//         _fileNameGeneratorMock.Setup(generator => generator.ExtractOutputFileNameComponents("path2-database"))
//             .Returns((It.IsAny<string>(), It.IsAny<string>(), "File", "path2"));
//
//
//         var response = await _runScriptController.RunScriptFromPath(runScriptArg, It.IsAny<CancellationToken>());
//         var result = Assert.IsType<OkObjectResult>(response);
//         var returnedRun = (result.Value as ReturnedRun)!;
//
//         MakeAssertions(returnedRun, projectModel);
//     }
//
//     private void MakeAssertions(ReturnedRun returnedRun, ProjectModel projectModel)
//     {
//         Assert.Null(returnedRun.Errors);
//         Assert.Equal("id1", returnedRun.ProjectId);
//         Assert.Equal(projectModel.LastRunIndex, returnedRun.RunIndex);
//         Assert.Equal("path1", returnedRun.ConsoleOutputName);
//         Assert.Single(returnedRun.OutputFiles);
//         Assert.Equal("path2", returnedRun.OutputFiles[0].FileName);
//         Assert.Equal("path2-database", returnedRun.OutputFiles[0].FilePath);
//         Assert.Equal("File", returnedRun.OutputFiles[0].FileType);
//
//         _fileModelServiceMock.Verify(service =>
//             service.UploadFileAsync("scriptName", It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
//
//         _projectModelServiceMock.Verify(service => service.UpdateDocument(projectModel, It.IsAny<CancellationToken>()),
//             Times.Once);
//
//         _runModelServiceMock.Verify(
//             service => service.CreateDocument(It.IsAny<RunModel>(), It.IsAny<CancellationToken>()), Times.Once);
//
//         _runModelServiceMock.Verify(
//             service => service.UpdateDocument(It.IsAny<RunModel>(), It.IsAny<CancellationToken>()), Times.Once);
//     }
}
