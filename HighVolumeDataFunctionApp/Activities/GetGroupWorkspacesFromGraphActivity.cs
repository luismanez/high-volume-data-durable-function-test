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
    public static class GetGroupWorkspacesFromGraphActivityExtensions
    {
        public static Task<IEnumerable<SyncWorkspaceDto>> CallGetGroupWorkspacesFromGraphActivity(
            this IDurableOrchestrationContext context)
        {
            return context.CallActivityWithRetryAsync<IEnumerable<SyncWorkspaceDto>>(
                Constants.AzureFunctions.Activities.Synchronisation.GetGroupWorkspacesFromGraphActivity.Name,
                Constants.AzureFunctions.Activities.Synchronisation.GetGroupWorkspacesFromGraphActivity.SecondsBetweenRetries,
                Constants.AzureFunctions.Activities.Synchronisation.GetGroupWorkspacesFromGraphActivity.MaxNumberOfRetries, null);
        }
    }

    public class GetGroupWorkspacesFromGraphActivity
    {        
        [FunctionName(Constants.AzureFunctions.Activities.Synchronisation.GetGroupWorkspacesFromGraphActivity.Name)]
        public async Task<IEnumerable<SyncWorkspaceDto>> Run(
            [ActivityTrigger] IDurableActivityContext activityContext,
            ILogger log)
        {
            var totalWorkspacesToProcessCount = int.Parse(Environment.GetEnvironmentVariable("NumberOfItemsToProcess") ?? "3");
            log.LogWarning($"GetGroupWorkspacesFromGraphActivity_ItemsToProcess: {totalWorkspacesToProcessCount}. InstanceId: {activityContext.InstanceId}");

            var delay = int.Parse(Environment.GetEnvironmentVariable("GetGroupWorkspacesFromGraphActivityDelayInMiliseconds") ?? "10");
            await Task.Delay(delay);
            log.LogWarning($"GetGroupWorkspacesFromGraphActivityDelay: {delay}");

            var highVolumeList = new List<SyncWorkspaceDto>();

            // Simulate calling an external API and return a High volume list of data
            // The issue happens with about 25k items. Tests were done with 30k.
            // (we have other production environments with about 10k items and are working fine)
            for (var i = 0; i < totalWorkspacesToProcessCount; i++)
            {
                highVolumeList.Add(SyncWorkspaceDto.NewFakeWorkspaceDto());
            }

            return highVolumeList;
        }
    }
}
