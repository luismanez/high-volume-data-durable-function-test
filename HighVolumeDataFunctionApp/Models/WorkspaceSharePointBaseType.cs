using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HighVolumeDataFunctionApp.Models
{
    public class WorkspaceSharePointBaseType : Enumeration
    {
        public static WorkspaceSharePointBaseType ModernTeamSite
            = new WorkspaceSharePointBaseType(0, nameof(ModernTeamSite));
        public static WorkspaceSharePointBaseType ModernCommunicationSite
            = new WorkspaceSharePointBaseType(1, nameof(ModernCommunicationSite));
        public static WorkspaceSharePointBaseType ModernTeamSiteNoGroup
            = new WorkspaceSharePointBaseType(2, nameof(ModernTeamSiteNoGroup));
        public static WorkspaceSharePointBaseType MsTeamsTeam
            = new WorkspaceSharePointBaseType(3, nameof(MsTeamsTeam));
        public static WorkspaceSharePointBaseType ClassicSite
            = new WorkspaceSharePointBaseType(4, nameof(ClassicSite));

        public WorkspaceSharePointBaseType() : base() { }
        [JsonConstructor]//Needed for serialization in durable functions
        public WorkspaceSharePointBaseType(int id, string name)
            : base(id, name)
        {
        }

        public static string GetGroupTypeLabel(WorkspaceSharePointBaseType workspaceSharePointBaseType)
        {
            if (workspaceSharePointBaseType == null)
            {
                throw new ArgumentNullException(nameof(workspaceSharePointBaseType));
            }

            if (Equals(workspaceSharePointBaseType, MsTeamsTeam))
                return WorkspaceGroupType.Teams.ToString();
            return WorkspaceGroupType.Microsoft365.ToString();
        }

        public static bool IsGroupTypeBased(WorkspaceSharePointBaseType workspaceSharePointBaseType)
        {
            if (workspaceSharePointBaseType == null)
            {
                throw new ArgumentNullException(nameof(workspaceSharePointBaseType));
            }

            if (
                Equals(workspaceSharePointBaseType, ModernTeamSite)
                ||
                Equals(workspaceSharePointBaseType, MsTeamsTeam))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
