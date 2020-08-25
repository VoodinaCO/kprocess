using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using KProcess.Ksmed.Data;
using System.Data.Entity;

namespace KProcess.KL2.WebAdmin.Identity
{
    public class RoleStore : IRoleStore<KL2IdentityRole, string>
    {
        public Task CreateAsync(KL2IdentityRole role)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                context.Roles.AddObject(role);
                return context.SaveChangesAsync();
            }
        }

        public Task DeleteAsync(KL2IdentityRole role)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                context.Roles.DeleteObject(role);
                return context.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
        }

        public async Task<KL2IdentityRole> FindByIdAsync(string roleId)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                var role = await context.Roles.Where(u => u.RoleCode == roleId).FirstOrDefaultAsync();
                return role as KL2IdentityRole;
            }
        }

        public async Task<KL2IdentityRole> FindByNameAsync(string roleName)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                var user = await context.Roles.Where(u => u.RoleCode == roleName).FirstOrDefaultAsync();
                return user as KL2IdentityRole;
            }
        }

        public Task UpdateAsync(KL2IdentityRole role)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                context.Roles.Attach(role);
                context.ObjectStateManager.ChangeObjectState(role, EntityState.Modified);
                return context.SaveChangesAsync();
            }
        }
    }
}