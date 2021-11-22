using HighVolumeDataFunctionApp.Common;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace HighVolumeDataFunctionApp.Models
{
    public class SyncWorkspaceDto
    {
        public Guid WorkspaceId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SiteId { get; set; }
        public string SiteAbsoluteUrl { get; set; }
        public Guid GroupId { get; set; }
        public string GroupType { get; set; }
        public string Mail { get; set; }
        public string MailNickname { get; set; }
        public bool IsPublic { get; set; }
        public string TeamsInternalId { get; set; }
        public string MainLink { get; set; }
        public Guid ObjectStateVersion { get; set; }
        public IdentityPrincipal CreatedByUser { get; set; }
        public bool IsArchived { get; set; }
        public List<IdentityPrincipal> Owners { get; set; }
        public List<IdentityPrincipal> Members { get; set; }
        public List<IdentityPrincipal> Visitors { get; set; }
        public bool IsReadyToSync { get; set; } = true;
        public bool IsReadyToRecover { get; set; } = false;
        public bool HasToBeDeleted { get; set; } = false;
        public WorkspaceSharePointBaseType SharePointBaseType { get; set; }
        public Guid CategoryTemplateId { get; set; }
        public Guid TemplateId { get; set; }
        public string ObjectStateHash { get; set; }
        public bool IsConnectedToTeams => GroupType == WorkspaceGroupType.Teams.ToString();
        public bool IsConnectedToYammer => GroupType == WorkspaceGroupType.Yammer.ToString();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public void UpdateWorkspaceWithGraphInfo()
        {
            var random = new Random();

            var randomTextGenerator = new RandomGenerator();

            GroupId = Guid.NewGuid();
            LastUpdatedDateTime = DateTime.Now;
            MailNickname = randomTextGenerator.RandomString(5);
            Mail = $"{MailNickname}@onmicrosoft.com";
            Description = randomTextGenerator.RandomString(3); 
            Title = randomTextGenerator.RandomString(10);
            CreatedByUser = IdentityPrincipal.CreateFakeIdentity();
            TeamsInternalId = randomTextGenerator.RandomString(10);
            MainLink =
                "https://marvel.sharepoint.com/:i:/r/sites/SpanishTeam/Shared%20Documents/General/20201211_095016523_iOS.jpg?csf=1&web=1&e=eH4VPx";
            CreatedByUserId = Guid.Parse(CreatedByUser.Id);
            Owners = GenerateListOfFakeUsers(random.Next(20));
            Members = GenerateListOfFakeUsers(random.Next(65));
            Visitors = GenerateListOfFakeUsers(random.Next(100));
        }

        private List<IdentityPrincipal> GenerateListOfFakeUsers(int size)
        {
            var users = new List<IdentityPrincipal>();
            for (var i = 0; i < size; i++)
            {
                users.Add(IdentityPrincipal.CreateFakeIdentity());
            }

            return users;
        }

        public void UpdateWorkspaceWithSharePointInfo()
        {
            SiteId = Guid.NewGuid().ToString();
            LastUpdatedDateTime = DateTime.Now;
            SiteAbsoluteUrl = $"https://marvel.sharepoint.com/sites/{MailNickname}";
            SharePointBaseType = WorkspaceSharePointBaseType.MsTeamsTeam;
        }

        public static SyncWorkspaceDto NewFakeWorkspaceDto()
        {
            return new SyncWorkspaceDto
            {
                WorkspaceId = Guid.NewGuid(),
                IsPublic = true,
                CreatedDateTime = DateTime.Now,
                LastUpdatedDateTime = DateTime.MinValue,
                ObjectStateVersion = Guid.NewGuid(), 
                IsArchived = false, 
                CategoryTemplateId = Guid.NewGuid(),
                TemplateId = Guid.NewGuid(), 
                ObjectStateHash = "fake_hash"
            };
        }
    }
}
