using HighVolumeDataFunctionApp.Activities;
using HighVolumeDataFunctionApp.Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HighVolumeDataFunctionApp.Orchestrators
{
    public class WorkspacesSynchronisationOrchestrator
    {
        [FunctionName(Constants.AzureFunctions.Orchestrators.WorkspacesSynchronisationOrchestrator)]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            try
            {
                log = context.CreateReplaySafeLogger(log);

                // This sub-orchestrator just simulates the creation of a very long list of Items (models/SyncWorkspaceDto.cs)
                var workspacesToSync = await context.CallGetWorkspacesToSyncSubOrchestrator();
                log.LogInformation($"{nameof(WorkspacesSynchronisationOrchestrator)} got workspaces to process");

                // This sub-orchestrator iterates the very-long list following fan-out/in pattern. 
                // For each element, calls an Activity that simulates calling an external API to get more info about that specific item
                // Then, the list is iterated again (applying some filter with the data obtained in previous foreach)
                // and for each element, call an Activity that simulates calling another external API to get more info about the item
                workspacesToSync = await context.CallGetWorkspacesDetailedInfoSubOrchestrator(workspacesToSync);
                log.LogInformation($"{nameof(WorkspacesSynchronisationOrchestrator)} got detailed info for all workspaces to process");

                // fan-out/in to Persist Workspaces to DB.
                // Foreach item, it calls another Activity that simulates storing the item info in SQL and Azure Search
                var workspacesToPersist = workspacesToSync; // in real project, we apply some filter here
                var workspacesToPersistCount = workspacesToPersist.Count();

                var workspacesToPersistTasks = new Task<bool>[workspacesToPersistCount];
                var workspacesToPersistTaskNumber = 0;
                foreach (var workspace in workspacesToPersist)
                {
                    workspacesToPersistTasks[workspacesToPersistTaskNumber] =
                                    context.CallPersistWorkspaceActivity(workspace);
                    workspacesToPersistTaskNumber++;
                }
                await Task.WhenAll(workspacesToPersistTasks);

                var workspacesPersistedSuccessfully = workspacesToPersistTasks.Select(t => t.Result == true);

                log.LogWarning($"{nameof(WorkspacesSynchronisationOrchestrator)} finished. Processed a total of {workspacesToPersistCount} workspaces ({workspacesPersistedSuccessfully.Count()}).");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "WorkspacesSynchronisationOrchestrator_Something_Went_Wrong");
            }
        }
    }
}
