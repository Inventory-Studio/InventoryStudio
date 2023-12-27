using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InventoryStudio.Services
{
    public class FilterOrganizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FilterOrganizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<int> GetCurrentOrganizationIds()
        {
            var organizationIds = new List<int>();

            // 获取当前用户的组织ID逻辑
            var claim = _httpContextAccessor.HttpContext.User.FindFirst("OrganizationId");
            if (claim != null && int.TryParse(claim.Value, out var organizationId))
            {
                organizationIds.Add(organizationId);

                // 这里可以根据实际情况添加获取子组织ID的逻辑
            }

            return organizationIds;
        }

    }


}