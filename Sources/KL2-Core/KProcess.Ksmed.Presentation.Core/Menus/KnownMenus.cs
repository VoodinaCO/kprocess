using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Contient les codes des menus connus.
    /// </summary>
    public static class KnownMenus
    {
        public const string Prepare = "Prepare";
        public const string PrepareProject = "PrepareProject";
        public const string PrepareMembers = "PrepareMembers";
        public const string PrepareReferentials = "PrepareReferentials";
        public const string PrepareVideos = "PrepareVideos";
        public const string PrepareScenarios = "PrepareScenarios";

        public const string Analyze = "Analyze";
        public const string AnalyzeAcquire = "AnalyzeAcquire";
        public const string AnalyzeBuild = "AnalyzeBuild";
        public const string AnalyzeSimulate = "AnalyzeSimulate";
        public const string AnalyzeRestore = "AnalyzeRestore";
        public const string AnalyzeValidate = "AnalyzeValidate";

        public const string Validate = "Validate";

        public const string Capitalize = "Capitalize";
        public const string ValidateAcquire = "ValidateAcquire";
        public const string ValidateBuild = "ValidateBuild";
        public const string ValidateSimulate = "ValidateSimulate";
        public const string ValidateRestore = "ValidateRestore";

        public const string Publish = "Publish";
        public const string PublishSummary = "PublishSummary";
        public const string PublishScenario = "PublishScenario";
        public const string PublishFormat = "PublishFormat";
        public const string PublishVideos = "PublishVideos";

        //public const string AdminBackupRestore = "AdminBackupRestore";
        //public const string AdminBackupRestoreBackupRestore = "AdminBackupRestoreBackupRestore";

        public const string AdminReferentials = "AdminReferentials";
        public const string AdminReferentialsReferentials = "AdminReferentialsReferentials";

        //public const string AdminUsers = "AdminUsers";
        //public const string AdminUsersUsers = "AdminUsersUsers";

        public const string AdminActivation = "AdminActivation";
        public const string AdminActivationActivation = "AdminActivationActivation";

        public const string Extensions = "Extensions";
        public const string ExtensionsConfiguration = "ExtensionsConfiguration";

    }
}
