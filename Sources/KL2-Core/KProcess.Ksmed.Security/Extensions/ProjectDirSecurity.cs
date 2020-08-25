namespace KProcess.Ksmed.Security.Extensions
{
    public static class ProjectDirSecurity
    {
        public static bool CanAdd() =>
            !SecurityContext.HasCurrentLicenseFeature(Activation.ActivationFeatures.ReadOnly)
            && (SecurityContext.HasCurrentUserRole(KnownRoles.Administrator) || SecurityContext.HasCurrentUserRole(KnownRoles.Analyst));

        public static bool CanDelete() =>
            !SecurityContext.HasCurrentLicenseFeature(Activation.ActivationFeatures.ReadOnly)
            && (SecurityContext.HasCurrentUserRole(KnownRoles.Administrator) || SecurityContext.HasCurrentUserRole(KnownRoles.Analyst));

        public static bool CanUpdate() =>
            !SecurityContext.HasCurrentLicenseFeature(Activation.ActivationFeatures.ReadOnly)
            && (SecurityContext.HasCurrentUserRole(KnownRoles.Administrator) || SecurityContext.HasCurrentUserRole(KnownRoles.Analyst));
    }
}
