using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HighVolumeDataFunctionApp.Models
{
    public class WorkspaceGroupType : Enumeration
    {
        public static WorkspaceGroupType Microsoft365
            = new WorkspaceGroupType(1, "Microsoft 365");
        public static WorkspaceGroupType Teams
            = new WorkspaceGroupType(2, nameof(Teams));
        public static WorkspaceGroupType Yammer
            = new WorkspaceGroupType(3, nameof(Yammer));

        public WorkspaceGroupType() : base() { }
        [JsonConstructor]//Needed for serialization in durable functions
        public WorkspaceGroupType(int id, string name)
            : base(id, name)
        {
        }
    }
}
