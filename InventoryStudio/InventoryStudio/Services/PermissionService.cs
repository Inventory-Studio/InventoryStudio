using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using InventoryStudio.Data;
using ISLibrary;

namespace InventoryStudio.Services
{
    public class PermissionService
    {
    
        private readonly IAuthorizationPolicyProvider _policyProvider;

        public PermissionService( IAuthorizationPolicyProvider policyProvider)
        {
            _policyProvider = policyProvider;
        }

      
        public List<string> GetPermissions()
        {
            var permissions = new List<string>();

            // 获取所有继承 Controller 的类型
            var controllerTypes = Assembly.GetEntryAssembly().GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Controller)));

            foreach (var controllerType in controllerTypes)
            {
                var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);

                foreach (var method in methods)
                {
                    var authorizeAttributes = method.GetCustomAttributes<AuthorizeAttribute>();
                    foreach (var authorizeAttribute in authorizeAttributes)
                    {
                        permissions.Add(authorizeAttribute.Policy);
                    }
                }
            }

            return permissions.Distinct().ToList();
        }

        public void InitializePermissions()
        {
            // 获取权限点
            var systemPermissions = GetPermissions();
            List<Permission> savedPermissions = Permission.GetPermissions();

            // 找出需要添加的权限
            var permissionsToAdd = systemPermissions.Except(savedPermissions.Select(p => p.Name)).ToList();

            // 找出需要删除的权限
            var permissionsToRemove = savedPermissions.Where(p => !systemPermissions.Contains(p.Name)).ToList();

            // 添加新权限
            foreach (var permissionName in permissionsToAdd)
            {
                var newPermission = new Permission
                {
                    Name = permissionName
                };
                newPermission.Create();
            }

            // 删除不再需要的权限
            foreach (var oldPermission in permissionsToRemove)
            {
                oldPermission.Delete();
            }

        }

        //public async Task<AuthorizationPolicy> GetOrCreatePolicyAsync(string policyName)
        //{
        //    // 尝试从缓存中获取策略
        //    if (_cache.TryGetValue(policyName, out AuthorizationPolicy policy))
        //    {
        //        return policy;
        //    }

        //    // 从数据库加载权限节点
        //    var permission = await _dbContext.Permissions.SingleOrDefaultAsync(p => p.Identifier == policyName);

        //    if (permission != null)
        //    {
        //        // 构建权限策略
        //        policy = new AuthorizationPolicyBuilder()
        //            .RequireClaim("Permission", policyName)
        //            .Build();

        //        // 将权限策略添加到缓存
        //        _cache.Set(policyName, policy);

        //        return policy;
        //    }

        //    return null;
        //}

        //public async Task<List<string>> GetPermissionsForPolicyAsync(string policyName)
        //{
        //    var permissions = await _dbContext.Permissions
        //        .Where(p => p.Identifier == policyName)
        //        .Select(p => p.Identifier)
        //        .ToListAsync();

        //    return permissions;
        //}

    }


}