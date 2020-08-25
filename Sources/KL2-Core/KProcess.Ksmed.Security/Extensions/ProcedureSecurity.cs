using KProcess.Ksmed.Models;
using System.Linq;

namespace KProcess.Ksmed.Security.Extensions
{
    public static class ProcedureSecurity
    {
        public static bool CanAdd() =>
            SecurityContext.CurrentUser != null
            && !SecurityContext.HasCurrentLicenseFeature(Activation.ActivationFeatures.ReadOnly)
            && (SecurityContext.HasCurrentUserRole(KnownRoles.Administrator) || SecurityContext.HasCurrentUserRole(KnownRoles.Analyst));

        public static bool CanDelete(this Procedure process) =>
            process != null
            && SecurityContext.CurrentUser != null
            && !SecurityContext.HasCurrentLicenseFeature(Activation.ActivationFeatures.ReadOnly)
            && (SecurityContext.HasCurrentUserRole(KnownRoles.Administrator)
                || (SecurityContext.HasCurrentUserRole(KnownRoles.Analyst)
                    && process.OwnerId == SecurityContext.CurrentUser.User.UserId));

        public static bool CanUpdate(this Procedure process) =>
            process != null
            && SecurityContext.CurrentUser != null
            && !SecurityContext.HasCurrentLicenseFeature(Activation.ActivationFeatures.ReadOnly)
            && (SecurityContext.HasCurrentUserRole(KnownRoles.Administrator)
                || (SecurityContext.HasCurrentUserRole(KnownRoles.Analyst)
                    && process.OwnerId == SecurityContext.CurrentUser.User.UserId));

        public static bool CanRead(this Procedure process, User user) =>
            process != null
            && user != null
            && (user.Roles.Any(r => r.RoleCode == KnownRoles.Administrator) == true
                || process.UserRoleProcesses.Any(urp => urp.UserId == user.UserId) == true);

        public static bool CanWrite(this Procedure process, User user) =>
            process != null
            && user != null
            && (process.IsMarkedAsAdded
                || user.Roles.Any(r => r.RoleCode == KnownRoles.Administrator)
                || process.UserRoleProcesses.Any(urp => urp.UserId == user.UserId && urp.RoleCode == KnownRoles.Analyst));
    }
}
