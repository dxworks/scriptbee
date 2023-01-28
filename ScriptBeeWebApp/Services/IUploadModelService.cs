using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ScriptBee.Models;

namespace ScriptBeeWebApp.Services;

public interface IUploadModelService
{
    Task<List<FileData>> UploadFilesAsync(ProjectModel projectModel, string loaderName, IFormFileCollection files,
        CancellationToken cancellationToken = default);
}
