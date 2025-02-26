// using ScriptBee.UseCases.Analysis;
// using ScriptBee.Domain.Model.Analysis;
// using ScriptBee.Domain.Model.Project;
// using ScriptBee.Ports.Driving.UseCases.Analysis;
//
// namespace ScriptBee.Service.Analysis;
//
// public class CalculationService(
//     IGetProjectInstancesUseCase getProjectInstancesService,
//     ICreateProjectInstance createProjectInstance,
//     HttpClient httpClient
// ) : ICalculationUseCase
// {
//     public async Task<InstanceInfo> Run(
//         ProjectId projectId,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var calculationInstanceInfo = await GetFirstCalculationInstanceInfo(
//             projectId,
//             cancellationToken
//         );
//         var response = await httpClient.GetStringAsync(
//             $"{calculationInstanceInfo.Url}/api/analysis",
//             cancellationToken
//         );
//
//         return calculationInstanceInfo;
//     }
//
//     private async Task<InstanceInfo> GetFirstCalculationInstanceInfo(
//         ProjectId projectId,
//         CancellationToken cancellationToken
//     )
//     {
//         var allInstances = await getProjectInstancesService.GetAllInstances(
//             projectId,
//             cancellationToken
//         );
//
//         var calculationInstanceInfo = allInstances.FirstOrDefault();
//
//         if (calculationInstanceInfo == null)
//         {
//             return await createProjectInstance.Create(projectId, cancellationToken);
//         }
//
//         return calculationInstanceInfo;
//     }
// }
