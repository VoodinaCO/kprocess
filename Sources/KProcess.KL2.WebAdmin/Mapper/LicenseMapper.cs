using Antlr.Runtime.Misc;
using KProcess.KL2.APIClient;
using KProcess.KL2.JWT;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Activation;
using KProcess.Ksmed.Security.Activation.Providers;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Mapper
{
    public static class LicenseMapper
    {
        public static async Task<bool> CheckLicenseStatus(params WebLicenseStatus[] status)
        {
            var license = await GetLicense();
            return status.Contains(license.Status);
        }

        public static async Task<bool> CheckUserCanAccess(UserModel user)
        {
            var license = await GetLicense();

            if (user.Roles.Any(r => r == KnownRoles.Administrator))
                return true;
            else if (license.UsersPool == null)
                return false;
            else
                return license.Status != WebLicenseStatus.Expired && license.UsersPool.Any(u => u == user.UserId) && license.Status != WebLicenseStatus.NotFound;
        }

        public static async Task<UsersPool> GetUserPools()
        {
            var license = await GetLicense();

            return license.UsersPool;
        }
        
        public static async Task<bool> CheckLicenseIsExpired()
        {
            var license = await GetLicense();
            if (license.Status == WebLicenseStatus.Expired)
                return true;
            else
                return false;
        }
        public static async Task<IEnumerable<T>> FilterByActivatedUser<T>(IEnumerable<T> collection, Func<T, int> getUserId)
        {
            var license = await GetLicense();
            return collection.Where(_ => license.UsersPool.Any(u => u == getUserId(_)));
        }

        public static Task<string> GetMachineHash()
        {
            var apiClient = DependencyResolver.Current.GetService<IAPIHttpClient>();
            return apiClient.ServiceAsync<string>(KL2_Server.API, "LicenseService", "GetMachineHash");
        }

        public static Task<WebProductLicense> GetLicense()
        {
            var apiClient = DependencyResolver.Current.GetService<IAPIHttpClient>();
            return apiClient.ServiceAsync<WebProductLicense>(KL2_Server.API, "LicenseService", "GetLicense");
        }

        public static Task SetLicense(WebProductLicense productLicense)
        {
            var apiClient = DependencyResolver.Current.GetService<IAPIHttpClient>();
            dynamic param = new ExpandoObject();
            param.productLicense = productLicense;
            return apiClient.ServiceAsync(KL2_Server.API, "LicenseService", "SetLicense", param);
        }

        public static Task<WebProductLicense> ActivateLicense(ProductLicenseInfo productLicenseInfo)
        {
            var apiClient = DependencyResolver.Current.GetService<IAPIHttpClient>();
            dynamic param = new ExpandoObject();
            param.productLicenseInfo = productLicenseInfo;
            return apiClient.ServiceAsync<WebProductLicense>(KL2_Server.API, "LicenseService", "ActivateLicense", param);
        }

        public static Task<UserInformationProvider> GetUserInformation()
        {
            var apiClient = DependencyResolver.Current.GetService<IAPIHttpClient>();
            return apiClient.ServiceAsync<UserInformationProvider>(KL2_Server.API, "LicenseService", "GetUserInformation");
        }

        public static Task SetUserInformation(string name, string company, string email)
        {
            var apiClient = DependencyResolver.Current.GetService<IAPIHttpClient>();
            dynamic param = new ExpandoObject();
            param.name = name;
            param.company = company;
            param.email = email;
            return apiClient.ServiceAsync(KL2_Server.API, "LicenseService", "SetUserInformation", param);
        }
    }
}