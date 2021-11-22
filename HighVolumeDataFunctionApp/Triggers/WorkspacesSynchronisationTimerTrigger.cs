using HighVolumeDataFunctionApp.Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace HighVolumeDataFunctionApp.Triggers
{
    public class WorkspacesSynchronisationTimerTrigger
    {
        public WorkspacesSynchronisationTimerTrigger()
        {

        }

        [FunctionName(Constants.AzureFunctions.Triggers.WorkspacesSynchronisationTimerTrigger)]
        public async Task Run(
            [TimerTrigger("%SynchroniseWorkspacesTimerCron%", RunOnStartup = false)]
            TimerInfo myTimer,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger logger)
        {
            // Singleton pattern. Code is copied from MS official docs
            // Check if an instance with the specified ID already exists or an existing one stopped running(completed/failed/terminated).
            var existingInstance = await starter.GetStatusAsync(Constants.AzureFunctions.Orchestrators.WorkspacesSynchronisationOrchestratorInstanceId);
            if (SyncOrchestratorIsNotRunning(existingInstance))
            {
                // An instance with the specified ID doesn't exist or an existing one stopped running, create one.
                LogStartingMessage(myTimer, logger);

                await starter.StartNewAsync(
                    Constants.AzureFunctions.Orchestrators.WorkspacesSynchronisationOrchestrator,
                    Constants.AzureFunctions.Orchestrators.WorkspacesSynchronisationOrchestratorInstanceId);

                LogOrchestratorStarted(logger, Constants.AzureFunctions.Orchestrators.WorkspacesSynchronisationOrchestratorInstanceId);

                LogTimerTriggerDone(starter, logger, Constants.AzureFunctions.Orchestrators.WorkspacesSynchronisationOrchestratorInstanceId);
            }
            else
            {
                // An instance with the specified ID exists or an existing one still running, don't create one.
                LogOrchestratorDuplicate(logger, Constants.AzureFunctions.Orchestrators.WorkspacesSynchronisationOrchestratorInstanceId);
            }
        }

        private static bool SyncOrchestratorIsNotRunning(DurableOrchestrationStatus existingInstance)
        {
            return existingInstance == null
                        || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Completed
                        || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Failed
                        || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Terminated;
        }

        private static void LogTimerTriggerDone(IDurableOrchestrationClient starter, ILogger log, string instanceId)
        {
            var payload = starter.CreateHttpManagementPayload(instanceId);

            var payloadSerialized = JsonSerializer.Serialize(payload);

            log.LogInformation($"Orchestrator_started InstanceId {instanceId}. OrchestratorManagementPayload: {payloadSerialized}");
        }

        private static void LogOrchestratorStarted(ILogger log, string instanceId)
        {
            log.LogInformation(
                $"{Constants.AzureFunctions.Triggers.WorkspacesSynchronisationTimerTrigger} started orchestration with ID = '{instanceId}'.");
        }

        private static void LogStartingMessage(TimerInfo myTimer, ILogger log)
        {
            var now = DateTime.Now;
            log.LogInformation(
                $"{Constants.AzureFunctions.Triggers.WorkspacesSynchronisationTimerTrigger} Timer trigger executed at: {now}. Next scheduled execution: {myTimer.Schedule.GetNextOccurrence(now)}");
        }

        private static void LogOrchestratorDuplicate(ILogger log, string instanceId)
        {
            log.LogWarning(
                $"{Constants.AzureFunctions.Triggers.WorkspacesSynchronisationTimerTrigger} skipped duplicate sync orchestration with ID = '{instanceId}'."
                );
        }
    }
}
