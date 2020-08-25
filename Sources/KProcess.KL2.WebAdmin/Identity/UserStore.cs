using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Models;
using KProcess.Ksmed.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class UserStore : 
        IUserStore<KL2IdentityUser>, 
        IUserRoleStore<KL2IdentityUser>, 
        IUserPasswordStore<KL2IdentityUser>, 
        IUserEmailStore<KL2IdentityUser>, 
        IQueryableUserStore<KL2IdentityUser>,
        IUserTwoFactorStore<KL2IdentityUser, string>,
        IUserLockoutStore<KL2IdentityUser, string>
    {
        public UserStore()
        {
        }

        public UserStore(ApplicationDbContext database)
        {
            var dummy = database;
        }

        public IQueryable<KL2IdentityUser> Users => throw new NotImplementedException();

        #region IUserStore

        public async Task CreateAsync(KL2IdentityUser user)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                context.Users.AddObject(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(KL2IdentityUser user)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                context.Users.DeleteObject(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<KL2IdentityUser> FindByIdAsync(string userId)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                int val = int.Parse(userId);
                var user = await context.Users.Include(u => u.Roles).FirstOrDefaultAsync(n => n.UserId == val);
                if (user == null)
                    return null;
                return new KL2IdentityUser(user);
            }
        }

        public async Task<KL2IdentityUser> FindByNameAsync(string userName)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                var user = await context.Users.Where(u => u.Username == userName).FirstOrDefaultAsync();
                if (user == null) return null;
                return new KL2IdentityUser(user);
            }
        }

        public async Task UpdateAsync(KL2IdentityUser user)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                context.Users.Attach(user);
                context.ObjectStateManager.ChangeObjectState(user, EntityState.Modified);
                await context.SaveChangesAsync();
            }
        }

        #endregion

        #region IUserRoleStore

        public async Task AddToRoleAsync(KL2IdentityUser user, string roleName)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                var model = context.Users.First(u => u.UserId == user.UserId);
                var role = context.Roles.First(u => u.RoleCode == roleName);
                model.Roles.Add(role);
                await context.SaveChangesAsync();
            }
        }

        public Task<IList<string>> GetRolesAsync(KL2IdentityUser user) =>
            Task.FromResult<IList<string>>(user.Roles.Select(u => u.RoleCode).ToList());

        public Task<bool> IsInRoleAsync(KL2IdentityUser user, string roleName) =>
            user.Roles.Any(s => s.RoleCode == roleName) ? Task.FromResult(true) : Task.FromResult(false);

        public Task RemoveFromRoleAsync(KL2IdentityUser user, string roleName)
        {
            user.RoleCodes.Remove(roleName);
            return Task.CompletedTask;
        }

        #endregion

        #region IUserPasswordStore

        public Task<bool> HasPasswordAsync(KL2IdentityUser user) =>
            user.Password != null ? Task.FromResult(true) : Task.FromResult(false);

        public Task SetPasswordHashAsync(KL2IdentityUser user, string passwordHash)
        {
            user.Password = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(passwordHash));
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(KL2IdentityUser user) =>
            Task.FromResult(Convert.ToBase64String(user.Password));

        #endregion

        #region IUserEmailStore

        public async Task<KL2IdentityUser> FindByEmailAsync(string email)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return new KL2IdentityUser(user);
            }
        }

        public Task<string> GetEmailAsync(KL2IdentityUser user) =>
            Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(KL2IdentityUser user) =>
            Task.FromResult(true);

        public Task SetEmailAsync(KL2IdentityUser user, string email)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(KL2IdentityUser user, bool confirmed) =>
            Task.CompletedTask;

        #endregion

        #region IUserLockoutStore

        public Task<bool> GetLockoutEnabledAsync(KL2IdentityUser user) =>
            Task.FromResult(false);

        public Task<DateTimeOffset> GetLockoutEndDateAsync(KL2IdentityUser user) =>
            Task.FromException<DateTimeOffset>(new NotImplementedException());

        public Task SetLockoutEnabledAsync(KL2IdentityUser user, bool enabled) =>
            Task.FromException(new NotImplementedException());

        public Task SetLockoutEndDateAsync(KL2IdentityUser user, DateTimeOffset lockoutEnd) =>
            Task.FromException(new NotImplementedException());

        public Task<int> GetAccessFailedCountAsync(KL2IdentityUser user) =>
            Task.FromResult(0);

        public Task<int> IncrementAccessFailedCountAsync(KL2IdentityUser user) =>
            Task.FromException<int>(new NotImplementedException());

        public Task ResetAccessFailedCountAsync(KL2IdentityUser user) =>
            Task.FromException<int>(new NotImplementedException());

        #endregion

        #region IUserTwoFactorStore

        public Task<bool> GetTwoFactorEnabledAsync(KL2IdentityUser user) =>
            Task.FromResult(false);

        public Task SetTwoFactorEnabledAsync(KL2IdentityUser user, bool enabled) =>
            Task.FromException(new NotImplementedException());

        #endregion

        public void Dispose()
        {
        }
    }

}