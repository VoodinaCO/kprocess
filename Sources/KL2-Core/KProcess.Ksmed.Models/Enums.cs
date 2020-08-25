using System;
using System.Collections.Generic;

namespace KProcess.Ksmed.Models
{
    public static class Enums
    {
        public readonly static Dictionary<PublishModeEnum, PublishMode> PublishModes = new Dictionary<PublishModeEnum, PublishMode>
        {
            [PublishModeEnum.Formation] = new PublishMode { Id = PublishModeEnum.Formation, LabelResourceId = "Common_PublishMode_Training", IsEnabled = true },
            //[PublishModeEnum.Audit] = new PublishMode { Id = PublishModeEnum.Audit, LabelResourceId = "Common_PublishMode_Audit", IsEnabled = false },
            [PublishModeEnum.Inspection] = new PublishMode { Id = PublishModeEnum.Inspection, LabelResourceId = "Common_PublishMode_Inspection", IsEnabled = true }
        };

        public readonly static Dictionary<ResourceViewEnum, ResourceView> ResourceViews = new Dictionary<ResourceViewEnum, ResourceView>
        {
            [ResourceViewEnum.Intern] = new ResourceView { Id = ResourceViewEnum.Intern, LabelResourceId = "Common_ResourceView_Intern" },
            [ResourceViewEnum.Extern] = new ResourceView { Id = ResourceViewEnum.Extern, LabelResourceId = "Common_ResourceView_Extern" }
        };

        public readonly static Dictionary<ChoiceEnum, Choice> Choices = new Dictionary<ChoiceEnum, Choice>
        {
            [ChoiceEnum.No] = new Choice { Id = ChoiceEnum.No, LabelResourceId = "Common_No" },
            [ChoiceEnum.Yes] = new Choice { Id = ChoiceEnum.Yes, LabelResourceId = "Common_Yes" }
        };
    }

    [Flags]
    public enum PublishModeEnum
    {
        Formation = 1,
        Audit = 2,
        Inspection = 4,
        Evaluation = 8
    }

    public enum ChoiceEnum
    {
        No = 0,
        Yes = 1
    }

    public enum EHorizontalAlign
    {
        Left = 0,
        Right,
        Center
    }

    public enum EVerticalAlign
    {
        Top = 0,
        Center,
        Bottom
    }

    public enum AnomalyType
    {
        Security = 0,
        Maintenance,
        Operator
    }

    public enum AnomalyPriority
    {
        A = 1,
        B = 2,
        C = 3
    }

    public enum AnomalyOrigin
    {
        Inspection = 0,
        Audit = 1
    }

    public enum ResourceViewEnum
    {
        Intern = 0,
        Extern = 1
    }

    public enum VideoSyncTask
    {
        NotSync = 0,
        Sync = 1
    }

    public enum NotificationCategory
    {
        Evaluation = 0,
        Inspection = 1,
        Audit = 2,
        Anomaly = 3,
        Formation = 4
    }

    public enum ToType
    {
        Recipients = 1,
        RecipientCcs = 2,
        RecipientBccs = 3
    }

    public enum PublicationStatus
    {
        Waiting = 0,
        InProgress = 1,
        Completed = 2,
        InError = 3,
        Cancelled = 4
    }

    public enum ReferentialCategory
    {
        Referential = 0,
        CustomTextField = 1,
        CustomNumericField = 2,
        Other = 3
    }
}
