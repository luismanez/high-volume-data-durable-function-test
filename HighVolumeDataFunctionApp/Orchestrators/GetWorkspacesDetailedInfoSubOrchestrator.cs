using HighVolumeDataFunctionApp.Activities;
using HighVolumeDataFunctionApp.Common;
using HighVolumeDataFunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HighVolumeDataFunctionApp.Orchestrators
{
    public static class GetWorkspacesDetailedInfoSubOrchestratorExtensions
    {
        public static Task<IEnumerable<SyncWorkspaceDto>> CallGetWorkspacesDetailedInfoSubOrchestrator(this IDurableOrchestrationContext context, IEnumerable<SyncWorkspaceDto> workspacesToSync)
        {
            return context.CallSubOrchestratorAsync<IEnumerable<SyncWorkspaceDto>>(
                Constants.AzureFunctions.Orchestrators.GetWorkspacesDetailedInfoSubOrchestrator,
                workspacesToSync);
        }
    }

    public class GetWorkspacesDetailedInfoSubOrchestrator
    {
        [FunctionName(Constants.AzureFunctions.Orchestrators.GetWorkspacesDetailedInfoSubOrchestrator)]
        public async Task<IEnumerable<SyncWorkspaceDto>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var workspacesToGetDetailsFor = context.GetInput<IEnumerable<SyncWorkspaceDto>>();

            var workspacesToUpdateFromGraphCount = workspacesToGetDetailsFor.Count();
            var workspacesToUpdateFromGraphTasks = new Task<SyncWorkspaceDto>[workspacesToUpdateFromGraphCount];
            var workspacesToUpdateFromGraphTaskNumber = 0;

            foreach (var workspace in workspacesToGetDetailsFor)
            {
                workspacesToUpdateFromGraphTasks[workspacesToUpdateFromGraphTaskNumber] =
                                context.CallGetWorkspaceDetailedInfoFromGraphActivity(workspace);
                workspacesToUpdateFromGraphTaskNumber++;
            }
            await Task.WhenAll(workspacesToUpdateFromGraphTasks);
            var workspacesToUpdateWithDetailedInfo =
                workspacesToUpdateFromGraphTasks.Select(t => t.Result).Where(workspace => workspace != null);

            // repeat the fan out-in. In real project, we first apply a filter over "workspacesToUpdateWithDetailedInfo"
            // that filter returns most of the items in "workspacesToGetDetailsFor", but not all of them
            // that´s why we need a 2nd fan out-in to complete the filtered list items with info from another data provider
            var workspacesToUpdateFromSharePointCount = workspacesToUpdateWithDetailedInfo.Count();
            var workspacesToUpdateFromSharePointTasks = new Task<SyncWorkspaceDto>[workspacesToUpdateFromSharePointCount];
            var workspacesToUpdateFromSharePointTaskNumber = 0;

            foreach (var workspace in workspacesToUpdateWithDetailedInfo)
            {
                workspacesToUpdateFromSharePointTasks[workspacesToUpdateFromSharePointTaskNumber] =
                                context.CallGetWorkspaceDetailedInfoFromSharePointActivity(workspace);
                workspacesToUpdateFromSharePointTaskNumber++;
            }
            await Task.WhenAll(workspacesToUpdateFromSharePointTasks);

            var workspacesWithFullInfo =
                workspacesToUpdateFromSharePointTasks.Select(t => t.Result).Where(workspace => workspace != null);


            return workspacesWithFullInfo;
        }
    }
}
