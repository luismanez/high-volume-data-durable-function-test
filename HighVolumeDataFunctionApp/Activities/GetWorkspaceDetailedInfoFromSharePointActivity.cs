using HighVolumeDataFunctionApp.Common;
using HighVolumeDataFunctionApp.Extensions;
using HighVolumeDataFunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HighVolumeDataFunctionApp.Activities
{
    public static class GetWorkspaceDetailedInfoFromSharePointActivityExtensions
    {
        public static Task<SyncWorkspaceDto> CallGetWorkspaceDetailedInfoFromSharePointActivity(
            this IDurableOrchestrationContext context, 
            SyncWorkspaceDto workspace)
        {
            return context.CallActivityWithRetryAsync<SyncWorkspaceDto>(
                Constants.AzureFunctions.Activities.Synchronisation.GetWorkspaceDetailedInfoFromSharePointActivity.Name,
                Constants.AzureFunctions.Activities.Synchronisation.GetWorkspaceDetailedInfoFromSharePointActivity.SecondsBetweenRetries,
                Constants.AzureFunctions.Activities.Synchronisation.GetWorkspaceDetailedInfoFromSharePointActivity.MaxNumberOfRetries,
                workspace);
        }
    }

    public class GetWorkspaceDetailedInfoFromSharePointActivity
    {
        [FunctionName(Constants.AzureFunctions.Activities.Synchronisation.GetWorkspaceDetailedInfoFromSharePointActivity.Name)]
        public async Task<SyncWorkspaceDto> Run(
            [ActivityTrigger] SyncWorkspaceDto workspace, 
            ILogger log)
        {
            // Simulates calling a SharePoint endpoint to complete Workspace info
            var delay = int.Parse(Environment.GetEnvironmentVariable("GetWorkspaceDetailedInfoFromSharePointActivityInMiliseconds") ?? "10");
            await Task.Delay(delay);            

            workspace.UpdateWorkspaceWithSharePointInfo();

            log.LogWarning($"GetWorkspaceDetailedInfoFromSharePointActivityDelay: {delay}");

            return workspace;
        }
    }
}
