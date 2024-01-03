using InventoryStudio.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Security.Claims;

namespace InventoryStudio.Authorization
{  
    public class PermissionHandler : IAuthorizationHandler
    {

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirements = context.PendingRequirements.ToList();

            foreach (var requirement in pendingRequirements)
            {
                // Check if the requirement is of type PermissionRequirement
                if (requirement is PermissionRequirement permissionRequirement)
                {
                    // Check if the user has the required permission for the policy
                    if (HasPermission(context.User, permissionRequirement.PermissionName)||IsRootUser(context.User))
                    {
                        context.Succeed(requirement); // User has the required permission
                    }
                }
            }

            return Task.CompletedTask;
        }

        private bool HasPermission(ClaimsPrincipal user, string permissionName)
        {
            var permissionClaim = user.Claims.FirstOrDefault(c => c.Type == "Permissions");
            if (permissionClaim == null)
            {
                return false;
            }

            try
            {
                var permissions = System.Text.Json.JsonSerializer.Deserialize<List<string>>(permissionClaim.Value);

                return permissions != null && permissions.Contains(permissionName);
            }
            catch
            {
                return false;
            }
        }


        private bool IsRootUser(ClaimsPrincipal user)
        {
            var organizationClaim = user.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var rootUser = user.Claims.FirstOrDefault(c => c.Type == "RootUser");


            var organizationClaimValue = organizationClaim?.Value;
            var rootUserClaimValue = rootUser?.Value;

            if (!string.IsNullOrEmpty(organizationClaimValue) && !string.IsNullOrEmpty(rootUserClaimValue))
            {
                try
                {
                    var rootUserIds = JsonConvert.DeserializeObject<List<string>>(rootUserClaimValue);
                    return rootUserIds != null && rootUserIds.Contains(organizationClaimValue);
                }
                catch (JsonException)
                {
                    return false;
                }
            }

            return false;
        }

    }


}
