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
    public static class PersistWorkspaceActivityExtensions
    {
        public static Task<bool> CallPersistWorkspaceActivity(
            this IDurableOrchestrationContext context, 
            SyncWorkspaceDto workspace)
        {
            return context.CallActivityWithRetryAsync<bool>(
                Constants.AzureFunctions.Activities.Synchronisation.PersistWorkspaceActivity.Name,
                Constants.AzureFunctions.Activities.Synchronisation.PersistWorkspaceActivity.SecondsBetweenRetries,
                Constants.AzureFunctions.Activities.Synchronisation.PersistWorkspaceActivity.MaxNumberOfRetries,
                workspace);
        }
    }

    public class PersistWorkspaceActivity
    {
        [FunctionName(Constants.AzureFunctions.Activities.Synchronisation.PersistWorkspaceActivity.Name)]
        public async Task<bool> Run(
            [ActivityTrigger] SyncWorkspaceDto workspace, 
            ILogger log)
        {
            // Simulates storing the workspace in DB and Azure Search. 
            var delay = int.Parse(Environment.GetEnvironmentVariable("PersistWorkspaceActivityInMiliseconds") ?? "10");
            await Task.Delay(delay);

            log.LogWarning($"PersistWorkspaceActivityDelay: {delay}");

            return true;
        }
    }
}
