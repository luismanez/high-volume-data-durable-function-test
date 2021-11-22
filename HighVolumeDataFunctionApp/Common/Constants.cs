using System;
using System.Collections.Generic;
using System.Text;

namespace HighVolumeDataFunctionApp.Common
{
    public static class Constants
    {
        public static class AzureFunctions
        {
            public static class Triggers
            {
                public const string WorkspacesSynchronisationTimerTrigger = nameof(WorkspacesSynchronisationTimerTrigger);
            }

            public static class Orchestrators
            {
                public const string WorkspacesSynchronisationOrchestratorInstanceId = "88ECDF24-9D6B-495F-B10B-87632AA0577C";

                public const string WorkspacesSynchronisationOrchestrator = nameof(WorkspacesSynchronisationOrchestrator);
                public const string GetWorkspacesToSyncSubOrchestrator = nameof(GetWorkspacesToSyncSubOrchestrator);
                public const string GetWorkspacesDetailedInfoSubOrchestrator = nameof(GetWorkspacesDetailedInfoSubOrchestrator);
            }

            public static class Activities
            {
                public static class Synchronisation
                {
                    public static class GetGroupWorkspacesFromGraphActivity
                    {
                        public const string Name = nameof(GetGroupWorkspacesFromGraphActivity);
                        public const double SecondsBetweenRetries = 5;
                        public const int MaxNumberOfRetries = 2;
                    }                    

                    public static class GetWorkspaceDetailedInfoFromGraphActivity
                    {
                        public const string Name = nameof(GetWorkspaceDetailedInfoFromGraphActivity);
                        public const double SecondsBetweenRetries = 5;
                        public const int MaxNumberOfRetries = 2;
                    }

                    public static class GetWorkspaceDetailedInfoFromSharePointActivity
                    {
                        public const string Name = nameof(GetWorkspaceDetailedInfoFromSharePointActivity);
                        public const double SecondsBetweenRetries = 5;
                        public const int MaxNumberOfRetries = 2;
                    }

                    public static class PersistWorkspaceActivity
                    {
                        public const string Name = nameof(PersistWorkspaceActivity);
                        public const double SecondsBetweenRetries = 5;
                        public const int MaxNumberOfRetries = 2;
                    }
                }
            }
        }
    }
}
