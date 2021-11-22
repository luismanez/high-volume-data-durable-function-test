using HighVolumeDataFunctionApp.Activities;
using HighVolumeDataFunctionApp.Common;
using HighVolumeDataFunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HighVolumeDataFunctionApp.Orchestrators
{
    public static class GetWorkspacesToSyncSubOrchestratorExtensions
    {
        public static Task<IEnumerable<SyncWorkspaceDto>> CallGetWorkspacesToSyncSubOrchestrator(this IDurableOrchestrationContext context)
        {
            return context.CallSubOrchestratorAsync<IEnumerable<SyncWorkspaceDto>>(
                Constants.AzureFunctions.Orchestrators.GetWorkspacesToSyncSubOrchestrator,
                null);
        }
    }

    public class GetWorkspacesToSyncSubOrchestrator
    {
        [FunctionName(Constants.AzureFunctions.Orchestrators.GetWorkspacesToSyncSubOrchestrator)]
        public async Task<IEnumerable<SyncWorkspaceDto>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            // Just calls an Activity that simulates the creating of a very long List (see Activity for more info)
            var workspacesToSync = await context.CallGetGroupWorkspacesFromGraphActivity();

            return workspacesToSync;
        }
    }
}
