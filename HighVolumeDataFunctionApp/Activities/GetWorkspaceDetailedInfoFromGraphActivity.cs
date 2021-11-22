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
    public static class GetWorkspaceDetailedInfoFromGraphActivityExtensions
    {
        public static Task<SyncWorkspaceDto> CallGetWorkspaceDetailedInfoFromGraphActivity(
            this IDurableOrchestrationContext context, SyncWorkspaceDto workspaceWithGroup)
        {
            return context.CallActivityWithRetryAsync<SyncWorkspaceDto>(
                Constants.AzureFunctions.Activities.Synchronisation.GetWorkspaceDetailedInfoFromGraphActivity.Name,
                Constants.AzureFunctions.Activities.Synchronisation.GetWorkspaceDetailedInfoFromGraphActivity.SecondsBetweenRetries,
                Constants.AzureFunctions.Activities.Synchronisation.GetWorkspaceDetailedInfoFromGraphActivity.MaxNumberOfRetries,
                workspaceWithGroup);
        }
    }

    public class GetWorkspaceDetailedInfoFromGraphActivity
    {
        [FunctionName(Constants.AzureFunctions.Activities.Synchronisation.GetWorkspaceDetailedInfoFromGraphActivity.Name)]
        public async Task<SyncWorkspaceDto> Run(
            [ActivityTrigger] SyncWorkspaceDto workspaceWithGroup, 
            ILogger log)
        {
            // Simulates calling a Graph endpoint to complete Workspace info
            var delay = int.Parse(Environment.GetEnvironmentVariable("GetWorkspaceDetailedInfoFromGraphActivityDelayInMiliseconds") ?? "10");
            await Task.Delay(delay);            

            workspaceWithGroup.UpdateWorkspaceWithGraphInfo();

            log.LogWarning($"GetWorkspaceDetailedInfoFromGraphActivityDelay: {delay}");

            return workspaceWithGroup;
        }
    }
}
