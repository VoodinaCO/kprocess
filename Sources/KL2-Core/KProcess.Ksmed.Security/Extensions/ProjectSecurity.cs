using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Security.Extensions
{
    public static class ProjectSecurity
    {
        public static bool CanAdd(this Procedure process) =>
            ProcedureSecurity.CanUpdate(process);

        public static bool CanDelete(this Project project) =>
            project != null
            && project.Process != null
            && SecurityContext.CurrentUser != null
            && project.Process.OwnerId == SecurityContext.CurrentUser.User.UserId;

        public static bool CanUpdate(this Project project) =>
            project != null
            && project.Process != null
            && SecurityContext.CurrentUser != null
            && project.Process.CanWrite(SecurityContext.CurrentUser.User);

        public static bool CanRead(this Project project, User user) =>
            project != null
            && project.Process != null
            && user != null
            && project.Process.CanRead(user);

        public static bool CanWrite(this Project project, User user) =>
            project != null
            && project.Process != null
            && user != null
            && project.Process.CanWrite(user);
    }
}
