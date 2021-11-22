using System;
using System.Collections.Generic;
using System.Text;
using HighVolumeDataFunctionApp.Common;
using Newtonsoft.Json;

namespace HighVolumeDataFunctionApp.Models
{
    public class IdentityPrincipal : IEquatable<IdentityPrincipal>
    {
        public static readonly string GraphSecurityPrincipalSelectFields =
            "id,mail,displayName,userPrincipalName,jobTitle,officeLocation,userType,securityEnabled";

        public string Id { get; set; }
        public string UserPrincipalName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string JobTitle { get; set; }
        public string Location { get; set; }
        public bool IsGuest { get; set; } = false;
        public string DirectoryClaim => SecurityEnabled ? $"c:0t.c|tenant|{Id}" : $"c:0o.c|federateddirectoryclaimprovider|{Id}"; //When a group is a Security group, the id is composed with tenant... when it's a unified group, federateddirectoryclaimprovider
        public bool IsGroup { get; set; }
        public bool SecurityEnabled { get; set; }
        [JsonIgnore]
        public bool IsSuperAdmin { get; set; } = false;

        public IdentityPrincipal() { }

        public IdentityPrincipal(string id, string userPrincipalName, string displayName)
        {
            Id = id;
            UserPrincipalName = userPrincipalName;
            DisplayName = displayName;
            IsGroup = userPrincipalName.Contains("c:0o.c|federateddirectoryclaimprovider");
            SecurityEnabled = userPrincipalName.Contains("c:0t.c|tenant");
        }

        public static IdentityPrincipal CreateFakeIdentity()
        {
            var randomTextGenerator = new RandomGenerator();

            var id = Guid.NewGuid().ToString();
            var upn = $"{randomTextGenerator.RandomString(8)}@marvel.onmicrosoft.com";
            var displayName = $"{randomTextGenerator.RandomString(4)} {randomTextGenerator.RandomString(8)}";

            var identity = new IdentityPrincipal(id, upn, displayName)
            {
                IsGuest = false,
                Email = upn,
                JobTitle = randomTextGenerator.RandomString(8),
                Location = randomTextGenerator.RandomString(10),
                IsSuperAdmin = false
            };

            return identity;
        }

        /// <summary>
        /// Returns the IdentityPrincipal ID required by SharePoint when calling EnsureUser
        /// For users, their UPN, for Groups, the Claim format
        /// </summary>
        /// <returns></returns>
        public string GetSharePointAccount() => IsGroup ? DirectoryClaim : UserPrincipalName;
        public string GetIdentityIndexKey()
        {
            var userKey = "";
            if (!string.IsNullOrEmpty(Id) &&
                !string.IsNullOrEmpty(DisplayName))
                userKey = $"{Id}##{GetSharePointAccount()}##{DisplayName}";
            return userKey;
        }
        public string GetKeyForUsersIndex(Guid workspaceId)
        {
            if (!string.IsNullOrEmpty(Id) && workspaceId != Guid.Empty)
            {
                return $"{workspaceId}_{Id}";
            }
            return null;
        }

        public IdentityPrincipal Clone()
        {
            return new IdentityPrincipal
            {
                Id = Id,
                UserPrincipalName = UserPrincipalName,
                Email = Email,
                DisplayName = DisplayName,
                JobTitle = JobTitle,
                Location = Location,
                IsGuest = IsGuest,
                IsSuperAdmin = IsSuperAdmin,
                IsGroup = IsGroup,
                SecurityEnabled = SecurityEnabled
            };
        }

        public override int GetHashCode() => Id.GetHashCode();

        public bool Equals(IdentityPrincipal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other == null)
            {
                return false;
            }
            var idMatches = Id.Equals(other.Id);

            return idMatches;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IdentityPrincipal)obj);
        }

        public static bool operator ==(IdentityPrincipal left, IdentityPrincipal right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IdentityPrincipal left, IdentityPrincipal right)
        {
            return !Equals(left, right);
        }
    }
}
