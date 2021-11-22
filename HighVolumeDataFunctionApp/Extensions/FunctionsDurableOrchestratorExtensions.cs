using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace HighVolumeDataFunctionApp.Extensions
{
    public static class FunctionsDurableOrchestratorExtensions
    {
        public static Task<TResult> CallActivityWithRetryAsync<TResult>(
            this IDurableOrchestrationContext context,
            string activityName,
            double secondsBetweenRetry,
            int numberOfRetries,
            object input)
        {
            return context.CallActivityWithRetryAsync<TResult>(activityName,
                new RetryOptions(
                    TimeSpan.FromSeconds(secondsBetweenRetry),
                    numberOfRetries
                ),
                input);
        }

        public static Task CallActivityWithRetryAsync(
            this IDurableOrchestrationContext context,
            string activityName,
            double secondsBetweenRetry,
            int numberOfRetries,
            object input)
        {
            return context.CallActivityWithRetryAsync(activityName,
                new RetryOptions(
                    TimeSpan.FromSeconds(secondsBetweenRetry),
                    numberOfRetries
                ),
                input);
        }
    }
}
