using System.Threading.Tasks;
using ISLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace InventoryStudio.Authorization
{
    public class DynamicAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {

        public DynamicAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {

        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {                   

            var policy = await base.GetPolicyAsync(policyName);

            // 如果策略不存在，则动态创建策略
            if (policy == null)
            {
                var permissionRequirment = new PermissionRequirement(policyName);
                policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(permissionRequirment)
                    .Build();               
            }

            return policy;
        }
    }

}
