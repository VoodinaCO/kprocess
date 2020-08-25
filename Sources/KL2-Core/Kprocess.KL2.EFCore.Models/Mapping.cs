using KProcess.Ksmed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kprocess.KL2.EFCore.Models
{
    // KAction
    public class KActionConfiguration : IEntityTypeConfiguration<KAction>
    {
        string _schema;

        public KActionConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<KAction> builder)
        {
            builder.ToTable("Action");
            builder.HasKey(x => x.ActionId);

            builder.Property(x => x.ActionId).HasColumnName(@"ActionId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.ScenarioId).HasColumnName(@"ScenarioId").HasColumnType("int").IsRequired();
            builder.Property(x => x.ResourceId).HasColumnName(@"ResourceId").HasColumnType("int");
            builder.Property(x => x.CategoryId).HasColumnName(@"ActionCategoryId").HasColumnType("int");
            builder.Property(x => x.OriginalActionId).HasColumnName(@"OriginalActionId").HasColumnType("int");
            builder.Property(x => x.VideoId).HasColumnName(@"VideoId").HasColumnType("int");
            builder.Property(x => x.WBS).HasColumnName(@"WBS").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.Start).HasColumnName(@"Start").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.Finish).HasColumnName(@"Finish").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.BuildStart).HasColumnName(@"BuildStart").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.BuildFinish).HasColumnName(@"BuildFinish").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.IsRandom).HasColumnName(@"IsRandom").HasColumnType("bit").IsRequired();
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.CustomNumericValue).HasColumnName(@"CustomNumericValue").HasPrecision(19, 4);
            builder.Property(x => x.CustomNumericValue2).HasColumnName(@"CustomNumericValue2").HasPrecision(19, 4);
            builder.Property(x => x.CustomNumericValue3).HasColumnName(@"CustomNumericValue3").HasPrecision(19, 4);
            builder.Property(x => x.CustomNumericValue4).HasColumnName(@"CustomNumericValue4").HasPrecision(19, 4);
            builder.Property(x => x.CustomTextValue).HasColumnName(@"CustomTextValue").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CustomTextValue2).HasColumnName(@"CustomTextValue2").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CustomTextValue3).HasColumnName(@"CustomTextValue3").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CustomTextValue4).HasColumnName(@"CustomTextValue4").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.DifferenceReason).HasColumnName(@"DifferenceReason").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.Thumbnail).HasColumnName(@"Thumbnail").HasColumnType("varbinary(max)");
            builder.Property(x => x.IsThumbnailSpecific).HasColumnName(@"IsThumbnailSpecific").HasColumnType("bit").IsRequired();
            builder.Property(x => x.ThumbnailPosition).HasColumnName(@"ThumbnailPosition").HasColumnType("bigint");
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Original).WithMany(b => b.ActionsReduced).HasForeignKey(c => c.OriginalActionId); // FK_Action_Orignal
            builder.HasOne(a => a.Category).WithMany(b => b.Actions).HasForeignKey(c => c.CategoryId); // FK_Action_ActionCategory
            builder.HasOne(a => a.Resource).WithMany(b => b.Actions).HasForeignKey(c => c.ResourceId); // FK_Action_Resource
            builder.HasOne(a => a.Video).WithMany(b => b.Actions).HasForeignKey(c => c.VideoId); // FK_Action_Video
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedActions).HasForeignKey(c => c.CreatedByUserId); // FK_Action_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedActions).HasForeignKey(c => c.ModifiedByUserId); // FK_Action_User_ModifiedBy
            builder.HasOne(a => a.Scenario).WithMany(b => b.Actions).HasForeignKey(c => c.ScenarioId); // FK_Action_Scenario

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Duration);
            builder.Ignore(x => x.BuildDuration);
            builder.Ignore(x => x.IsGroup);
            builder.Ignore(x => x.DifferenceReasonManaged);
            builder.Ignore(x => x.AmeliorationDescription);
            builder.Ignore(x => x.CanModifyFinish);
        }
    }


    // ActionPredecessorSuccessor
    public class ActionPredecessorSuccessorConfiguration : IEntityTypeConfiguration<ActionPredecessorSuccessor>
    {
        string _schema;

        public ActionPredecessorSuccessorConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<ActionPredecessorSuccessor> builder)
        {
            builder.ToTable("ActionPredecessorSuccessor");
            builder.HasKey(x => new { x.ActionPredecessorId, x.ActionSuccessorId });

            builder.HasOne(ps => ps.Successor).WithMany(a => a.ActionPredecessorSuccessors).HasForeignKey(ps => ps.ActionSuccessorId);
            builder.HasOne(ps => ps.Predecessor).WithMany(a => a.ActionPredecessorSuccessors).HasForeignKey(ps => ps.ActionPredecessorId);
        }
    }

    // KActionReduced
    public class KActionReducedConfiguration : IEntityTypeConfiguration<KActionReduced>
    {
        string _schema;

        public KActionReducedConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<KActionReduced> builder)
        {
            builder.ToTable("ActionReduced");
            builder.HasKey(x => x.ActionId);

            builder.Property(x => x.ActionId).HasColumnName(@"ActionId").HasColumnType("int").IsRequired();
            builder.Property(x => x.ActionTypeCode).HasColumnName(@"ActionTypeCode").HasColumnType("nchar").HasMaxLength(6);
            builder.Property(x => x.Solution).HasColumnName(@"Solution").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.ReductionRatio).HasColumnName(@"ReductionRatio").HasColumnType("float").IsRequired();
            builder.Property(x => x.OriginalBuildDuration).HasColumnName(@"OriginalBuildDuration").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.ActionType).WithMany(b => b.ActionsReduced).HasForeignKey(c => c.ActionTypeCode); // FK_ActionReduced_ActionType
            builder.HasOne(a => a.Action).WithOne(b => b.Reduced); // FK_ActionReduced_Action

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Approved);
            builder.Ignore(x => x.Saving);
            builder.Ignore(x => x.Resource);
        }
    }

    // ActionType
    public class ActionTypeConfiguration : IEntityTypeConfiguration<ActionType>
    {
        string _schema;

        public ActionTypeConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<ActionType> builder)
        {
            builder.ToTable("ActionType");
            builder.HasKey(x => x.ActionTypeCode);

            builder.Property(x => x.ActionTypeCode).HasColumnName(@"ActionTypeCode").HasColumnType("nchar").IsRequired().HasMaxLength(6).ValueGeneratedNever();
            builder.Property(x => x.ShortLabelResourceId).HasColumnName(@"ShortLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LongLabelResourceId).HasColumnName(@"LongLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.LongLabelResource).WithMany(b => b.ActionTypesForLongLabel).HasForeignKey(c => c.LongLabelResourceId); // FK_ActionType_AppResourceKey
            builder.HasOne(a => a.ShortLabelResource).WithMany(b => b.ActionTypesForShortLabel).HasForeignKey(c => c.ShortLabelResourceId); // FK_ActionType_AppResourceKey_ShortLabel

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ShortLabel);
            builder.Ignore(x => x.LongLabel);
            builder.Ignore(x => x.ContextualLabel);
        }
    }

    // ActionValue
    public class ActionValueConfiguration : IEntityTypeConfiguration<ActionValue>
    {
        string _schema;

        public ActionValueConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<ActionValue> builder)
        {
            builder.ToTable("ActionValue");
            builder.HasKey(x => x.ActionValueCode);

            builder.Property(x => x.ActionValueCode).HasColumnName(@"ActionValueCode").HasColumnType("nchar").IsRequired().HasMaxLength(6).ValueGeneratedNever();
            builder.Property(x => x.ShortLabelResourceId).HasColumnName(@"ShortLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LongLabelResourceId).HasColumnName(@"LongLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.LongLabelResource).WithMany(b => b.ActionValuesForLongLabel).HasForeignKey(c => c.LongLabelResourceId); // FK_ActionValue_AppResourceKey_LongLabel
            builder.HasOne(a => a.ShortLabelResource).WithMany(b => b.ActionValuesForShortLabel).HasForeignKey(c => c.ShortLabelResourceId); // FK_ActionValue_AppResourceKey_ShortLabel

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ShortLabel);
            builder.Ignore(x => x.LongLabel);
        }
    }

    // AppResourceKey
    public class AppResourceKeyConfiguration : IEntityTypeConfiguration<AppResourceKey>
    {
        string _schema;

        public AppResourceKeyConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<AppResourceKey> builder)
        {
            builder.ToTable("AppResourceKey");
            builder.HasKey(x => x.ResourceId);

            builder.Property(x => x.ResourceId).HasColumnName(@"ResourceId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.ResourceKey).HasColumnName(@"ResourceKey").HasColumnType("nvarchar").IsRequired().HasMaxLength(100);

            builder.Ignore(x => x.ChangeTracker);
        }
    }

    // AppResourceValue
    public class AppResourceValueConfiguration : IEntityTypeConfiguration<AppResourceValue>
    {
        string _schema;

        public AppResourceValueConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<AppResourceValue> builder)
        {
            builder.ToTable("AppResourceValue");
            builder.HasKey(x => new { x.ResourceId, x.LanguageCode });

            builder.Property(x => x.ResourceId).HasColumnName(@"ResourceId").HasColumnType("int").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.LanguageCode).HasColumnName(@"LanguageCode").HasColumnType("char").IsRequired().IsUnicode(false).HasMaxLength(5).ValueGeneratedNever();
            builder.Property(x => x.Value).HasColumnName(@"Value").HasColumnType("nvarchar").IsRequired().HasMaxLength(500);
            builder.Property(x => x.Comment).HasColumnName(@"Comment").HasColumnType("nvarchar").HasMaxLength(500);
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();

            // Foreign keys
            builder.HasOne(a => a.AppResourceKey).WithMany(b => b.AppResourceValues).HasForeignKey(c => c.ResourceId); // FK_AppResourceValue_AppResourceKey
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedAppResourceValues).HasForeignKey(c => c.CreatedByUserId); // FK_AppResourceValue_User_CreatedBy
            builder.HasOne(a => a.Language).WithMany(b => b.AppResourceValues).HasForeignKey(c => c.LanguageCode); // FK_AppResourceValue_Language
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedAppResourceValues).HasForeignKey(c => c.ModifiedByUserId); // FK_AppResourceValue_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
        }
    }

    // AppSetting
    public class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting>
    {
        string _schema;

        public AppSettingConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<AppSetting> builder)
        {
            builder.ToTable("AppSetting");
            builder.HasKey(x => x.Key);

            builder.Property(x => x.Key).HasColumnName(@"Key").HasColumnType("nvarchar").IsRequired().HasMaxLength(50).ValueGeneratedNever();
            builder.Property(x => x.Value).HasColumnName(@"Value").HasColumnType("varbinary(max)").IsRequired();

            builder.Ignore(x => x.ChangeTracker);
        }
    }

    // Language
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        string _schema;

        public LanguageConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Language");
            builder.HasKey(x => x.LanguageCode);

            builder.Property(x => x.LanguageCode).HasColumnName(@"LanguageCode").HasColumnType("char").IsRequired().IsUnicode(false).HasMaxLength(5).ValueGeneratedNever();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);

            builder.Ignore(x => x.ChangeTracker);
        }
    }

    // Objective
    public class ObjectiveConfiguration : IEntityTypeConfiguration<Objective>
    {
        string _schema;

        public ObjectiveConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Objective> builder)
        {
            builder.ToTable("Objective");
            builder.HasKey(x => x.ObjectiveCode);

            builder.Property(x => x.ObjectiveCode).HasColumnName(@"ObjectiveCode").HasColumnType("nchar").IsRequired().HasMaxLength(6).ValueGeneratedNever();
            builder.Property(x => x.ShortLabelResourceId).HasColumnName(@"ShortLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LongLabelResourceId).HasColumnName(@"LongLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.LongLabelResource).WithMany(b => b.ObjectivesForLongLabel).HasForeignKey(c => c.LongLabelResourceId); // FK_Objective_LongLabel
            builder.HasOne(a => a.ShortLabelResource).WithMany(b => b.ObjectivesForShortLabel).HasForeignKey(c => c.ShortLabelResourceId); // FK_Objective_ShortLabel

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ShortLabel);
            builder.Ignore(x => x.LongLabel);
        }
    }

    // Project
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        string _schema;

        public ProjectConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Project");
            builder.HasKey(x => x.ProjectId);

            builder.Property(x => x.ProjectId).HasColumnName(@"ProjectId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.ObjectiveCode).HasColumnName(@"ObjectiveCode").HasColumnType("nchar").HasMaxLength(6);
            builder.Property(x => x.OtherObjectiveLabel).HasColumnName(@"OtherObjectiveLabel").HasColumnType("nvarchar").HasMaxLength(50);
            //builder.Property(x => x.Workshop).HasColumnName(@"Workshop").HasColumnType("nvarchar").IsRequired().HasMaxLength(100);
            builder.Property(x => x.CustomTextLabel).HasColumnName(@"CustomTextLabel").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.CustomTextLabel2).HasColumnName(@"CustomTextLabel2").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.CustomTextLabel3).HasColumnName(@"CustomTextLabel3").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.CustomTextLabel4).HasColumnName(@"CustomTextLabel4").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.CustomNumericLabel).HasColumnName(@"CustomNumericLabel").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.CustomNumericLabel2).HasColumnName(@"CustomNumericLabel2").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.CustomNumericLabel3).HasColumnName(@"CustomNumericLabel3").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.CustomNumericLabel4).HasColumnName(@"CustomNumericLabel4").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.TimeScale).HasColumnName(@"TimeScale").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");
            builder.Property(x => x.ParentProjectId).HasColumnName(@"ParentProjectId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Objective).WithMany(b => b.Projects).HasForeignKey(c => c.ObjectiveCode); // FK_Project_Objective
            builder.HasOne(a => a.ProjectParent).WithMany(b => b.ProjectChilds).HasForeignKey(c => c.ParentProjectId); // FK_Project_Project
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedProjects).HasForeignKey(c => c.CreatedByUserId); // FK_Project_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedProjects).HasForeignKey(c => c.ModifiedByUserId); // FK_Project_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
        }
    }

    // ProjectDir
    public class ProjectDirConfiguration : IEntityTypeConfiguration<ProjectDir>
    {
        string _schema;

        public ProjectDirConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<ProjectDir> builder)
        {
            builder.ToTable("ProjectDir");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.ParentId).HasColumnName(@"ParentId").HasColumnType("int");
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Parent).WithMany(b => b.Childs).HasForeignKey(c => c.ParentId); // FK_ProjectDir_ProjectDir

            builder.Ignore(x => x.ChangeTracker);
        }
    }

    // ProjectReferential
    public class ProjectReferentialConfiguration : IEntityTypeConfiguration<ProjectReferential>
    {
        string _schema;

        public ProjectReferentialConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<ProjectReferential> builder)
        {
            builder.ToTable("ProjectReferential");
            builder.HasKey(x => new { x.ProjectId, x.ReferentialId });

            builder.Property(x => x.ProjectId).HasColumnName(@"ProjectId").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReferentialId).HasColumnName(@"ReferentialId").HasColumnType("tinyint").IsRequired();
            builder.Property(x => x.IsEnabled).HasColumnName(@"IsEnabled").HasColumnType("bit").IsRequired();
            builder.Property(x => x.HasMultipleSelection).HasColumnName(@"HasMultipleSelection").HasColumnType("bit").IsRequired();
            builder.Property(x => x.KeepsSelection).HasColumnName(@"KeepsSelection").HasColumnType("bit").IsRequired();
            builder.Property(x => x.HasQuantity).HasColumnName(@"HasQuantity").HasColumnType("bit").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Project).WithMany(b => b.Referentials).HasForeignKey(c => c.ProjectId); // FK_ProjectReferential_Project

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Label);
        }
    }

    // Ref1
    public class Ref1Configuration : IEntityTypeConfiguration<Ref1>
    {
        string _schema;

        public Ref1Configuration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref1> builder)
        {
            builder.ToTable("Ref1");
            builder.HasKey(x => x.RefId);

            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Color).HasColumnName(@"Color").HasColumnType("varchar").IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.Uri).HasColumnName(@"Uri").HasColumnType("nvarchar").HasMaxLength(255);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Process).WithMany(b => b.Refs1).HasForeignKey(c => c.ProcessId); // FK_Ref1_Project
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedRefs1).HasForeignKey(c => c.CreatedByUserId); // FK_Tool_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedRefs1).HasForeignKey(c => c.ModifiedByUserId); // FK_Tool_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Id);
        }
    }

    // Ref1Action
    public class Ref1ActionConfiguration : IEntityTypeConfiguration<Ref1Action>
    {
        string _schema;

        public Ref1ActionConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref1Action> builder)
        {
            builder.ToTable("Ref1Action");
            builder.HasKey(x => new { x.ActionId, x.RefId });

            builder.Property(x => x.ActionId).HasColumnName(@"ActionId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName(@"Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Action).WithMany(b => b.Ref1).HasForeignKey(c => c.ActionId); // FK_RefToolAction_Action
            builder.HasOne(a => a.Ref1).WithMany(b => b.Ref1Actions).HasForeignKey(c => c.RefId); // FK_RefToolAction_RefTool

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ReferentialId);
        }
    }

    // Ref2
    public class Ref2Configuration : IEntityTypeConfiguration<Ref2>
    {
        string _schema;

        public Ref2Configuration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref2> builder)
        {
            builder.ToTable("Ref2");
            builder.HasKey(x => x.RefId);

            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Color).HasColumnName(@"Color").HasColumnType("varchar").IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.Uri).HasColumnName(@"Uri").HasColumnType("nvarchar").HasMaxLength(255);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Process).WithMany(b => b.Refs2).HasForeignKey(c => c.ProcessId); // FK_Ref2_Project
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedRefs2).HasForeignKey(c => c.CreatedByUserId); // FK_Consumable_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedRefs2).HasForeignKey(c => c.ModifiedByUserId); // FK_Consumable_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Id);
        }
    }

    // Ref2Action
    public class Ref2ActionConfiguration : IEntityTypeConfiguration<Ref2Action>
    {
        string _schema;

        public Ref2ActionConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref2Action> builder)
        {
            builder.ToTable("Ref2Action");
            builder.HasKey(x => new { x.ActionId, x.RefId });

            builder.Property(x => x.ActionId).HasColumnName(@"ActionId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName(@"Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Action).WithMany(b => b.Ref2).HasForeignKey(c => c.ActionId); // FK_RefConsumableAction_Action
            builder.HasOne(a => a.Ref2).WithMany(b => b.Ref2Actions).HasForeignKey(c => c.RefId); // FK_RefConsumableAction_RefConsumable

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ReferentialId);
        }
    }

    // Ref3
    public class Ref3Configuration : IEntityTypeConfiguration<Ref3>
    {
        string _schema;

        public Ref3Configuration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref3> builder)
        {
            builder.ToTable("Ref3");
            builder.HasKey(x => x.RefId);

            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Color).HasColumnName(@"Color").HasColumnType("varchar").IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.Uri).HasColumnName(@"Uri").HasColumnType("nvarchar").HasMaxLength(255);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Process).WithMany(b => b.Refs3).HasForeignKey(c => c.ProcessId); // FK_Ref3_Project
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedRefs3).HasForeignKey(c => c.CreatedByUserId); // FK_Place_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedRefs3).HasForeignKey(c => c.ModifiedByUserId); // FK_Place_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Id);
        }
    }

    // Ref3Action
    public class Ref3ActionConfiguration : IEntityTypeConfiguration<Ref3Action>
    {
        string _schema;

        public Ref3ActionConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref3Action> builder)
        {
            builder.ToTable("Ref3Action");
            builder.HasKey(x => new { x.ActionId, x.RefId });

            builder.Property(x => x.ActionId).HasColumnName(@"ActionId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName(@"Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Action).WithMany(b => b.Ref3).HasForeignKey(c => c.ActionId); // FK_RefPlaceAction_Action
            builder.HasOne(a => a.Ref3).WithMany(b => b.Ref3Actions).HasForeignKey(c => c.RefId); // FK_RefPlaceAction_RefPlace

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ReferentialId);
        }
    }

    // Ref4
    public class Ref4Configuration : IEntityTypeConfiguration<Ref4>
    {
        string _schema;

        public Ref4Configuration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref4> builder)
        {
            builder.ToTable("Ref4");
            builder.HasKey(x => x.RefId);

            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Color).HasColumnName(@"Color").HasColumnType("varchar").IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.Uri).HasColumnName(@"Uri").HasColumnType("nvarchar").HasMaxLength(255);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Process).WithMany(b => b.Refs4).HasForeignKey(c => c.ProcessId); // FK_Ref4_Project
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedRefs4).HasForeignKey(c => c.CreatedByUserId); // FK_Document_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedRefs4).HasForeignKey(c => c.ModifiedByUserId); // FK_Document_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Id);
        }
    }

    // Ref4Action
    public class Ref4ActionConfiguration : IEntityTypeConfiguration<Ref4Action>
    {
        string _schema;

        public Ref4ActionConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref4Action> builder)
        {
            builder.ToTable("Ref4Action");
            builder.HasKey(x => new { x.ActionId, x.RefId });

            builder.Property(x => x.ActionId).HasColumnName(@"ActionId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName(@"Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Action).WithMany(b => b.Ref4).HasForeignKey(c => c.ActionId); // FK_RefDocumentAction_Action
            builder.HasOne(a => a.Ref4).WithMany(b => b.Ref4Actions).HasForeignKey(c => c.RefId); // FK_RefDocumentAction_RefDocument

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ReferentialId);
        }
    }

    // Ref5
    public class Ref5Configuration : IEntityTypeConfiguration<Ref5>
    {
        string _schema;

        public Ref5Configuration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref5> builder)
        {
            builder.ToTable("Ref5");
            builder.HasKey(x => x.RefId);

            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Color).HasColumnName(@"Color").HasColumnType("varchar").IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.Uri).HasColumnName(@"Uri").HasColumnType("nvarchar").HasMaxLength(255);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Process).WithMany(b => b.Refs5).HasForeignKey(c => c.ProcessId); // FK_Ref5_Project
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedRefs5).HasForeignKey(c => c.CreatedByUserId); // FK_RefExtra1_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedRefs5).HasForeignKey(c => c.ModifiedByUserId); // FK_RefExtra1_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Id);
        }
    }

    // Ref5Action
    public class Ref5ActionConfiguration : IEntityTypeConfiguration<Ref5Action>
    {
        string _schema;

        public Ref5ActionConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref5Action> builder)
        {
            builder.ToTable("Ref5Action");
            builder.HasKey(x => new { x.ActionId, x.RefId });

            builder.Property(x => x.ActionId).HasColumnName(@"ActionId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName(@"Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Action).WithMany(b => b.Ref5).HasForeignKey(c => c.ActionId); // FK_RefExtra1Action_Action
            builder.HasOne(a => a.Ref5).WithMany(b => b.Ref5Actions).HasForeignKey(c => c.RefId); // FK_RefExtra1Action_RefExtra1

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ReferentialId);
        }
    }

    // Ref6
    public class Ref6Configuration : IEntityTypeConfiguration<Ref6>
    {
        string _schema;

        public Ref6Configuration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref6> builder)
        {
            builder.ToTable("Ref6");
            builder.HasKey(x => x.RefId);

            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Color).HasColumnName(@"Color").HasColumnType("varchar").IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.Uri).HasColumnName(@"Uri").HasColumnType("nvarchar").HasMaxLength(255);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Process).WithMany(b => b.Refs6).HasForeignKey(c => c.ProcessId); // FK_Ref6_Project
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedRefs6).HasForeignKey(c => c.CreatedByUserId); // FK_RefExtra2_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedRefs6).HasForeignKey(c => c.ModifiedByUserId); // FK_RefExtra2_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Id);
        }
    }

    // Ref6Action
    public class Ref6ActionConfiguration : IEntityTypeConfiguration<Ref6Action>
    {
        string _schema;

        public Ref6ActionConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref6Action> builder)
        {
            builder.ToTable("Ref6Action");
            builder.HasKey(x => new { x.ActionId, x.RefId });

            builder.Property(x => x.ActionId).HasColumnName(@"ActionId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName(@"Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Action).WithMany(b => b.Ref6).HasForeignKey(c => c.ActionId); // FK_RefExtra2Action_Action
            builder.HasOne(a => a.Ref6).WithMany(b => b.Ref6Actions).HasForeignKey(c => c.RefId); // FK_RefExtra2Action_RefExtra2

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ReferentialId);
        }
    }

    // Ref7
    public class Ref7Configuration : IEntityTypeConfiguration<Ref7>
    {
        string _schema;

        public Ref7Configuration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref7> builder)
        {
            builder.ToTable("Ref7");
            builder.HasKey(x => x.RefId);

            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Color).HasColumnName(@"Color").HasColumnType("varchar").IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.Uri).HasColumnName(@"Uri").HasColumnType("nvarchar").HasMaxLength(255);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Process).WithMany(b => b.Refs7).HasForeignKey(c => c.ProcessId); // FK_Ref7_Project
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedRefs7).HasForeignKey(c => c.CreatedByUserId); // FK_RefExtra3_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedRefs7).HasForeignKey(c => c.ModifiedByUserId); // FK_RefExtra3_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Id);
        }
    }

    // Ref7Action
    public class Ref7ActionConfiguration : IEntityTypeConfiguration<Ref7Action>
    {
        string _schema;

        public Ref7ActionConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Ref7Action> builder)
        {
            builder.ToTable("Ref7Action");
            builder.HasKey(x => new { x.ActionId, x.RefId });

            builder.Property(x => x.ActionId).HasColumnName(@"ActionId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RefId).HasColumnName(@"RefId").HasColumnType("int").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName(@"Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Action).WithMany(b => b.Ref7).HasForeignKey(c => c.ActionId); // FK_RefExtra3Action_Action
            builder.HasOne(a => a.Ref7).WithMany(b => b.Ref7Actions).HasForeignKey(c => c.RefId); // FK_RefExtra3Action_RefExtra3

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ReferentialId);
        }
    }

    // ActionCategory
    public class ActionCategoryConfiguration : IEntityTypeConfiguration<ActionCategory>
    {
        string _schema;

        public ActionCategoryConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<ActionCategory> builder)
        {
            builder.ToTable("RefActionCategory");
            builder.HasKey(x => x.ActionCategoryId);

            builder.Property(x => x.ActionCategoryId).HasColumnName(@"ActionCategoryId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Color).HasColumnName(@"Color").HasColumnType("varchar").IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.Uri).HasColumnName(@"Uri").HasColumnType("nvarchar").HasMaxLength(255);
            builder.Property(x => x.ActionTypeCode).HasColumnName(@"ActionTypeCode").HasColumnType("nchar").HasMaxLength(6);
            builder.Property(x => x.ActionValueCode).HasColumnName(@"ActionValueCode").HasColumnType("nchar").IsRequired().HasMaxLength(6);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Type).WithMany(b => b.ActionCategories).HasForeignKey(c => c.ActionTypeCode); // FK_ActionCategory_ActionType
            builder.HasOne(a => a.Process).WithMany(b => b.ActionCategories).HasForeignKey(c => c.ProcessId); // FK_RefActionCategory_Project
            builder.HasOne(a => a.Value).WithMany(b => b.ActionCategories).HasForeignKey(c => c.ActionValueCode); // FK_ActionCategory_ActionValue
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedActionCategories).HasForeignKey(c => c.CreatedByUserId); // FK_ActionCategory_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedActionCategories).HasForeignKey(c => c.ModifiedByUserId); // FK_ActionCategory_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Id);
            builder.Ignore(x => x.HasRelatedActions);
            builder.Ignore(x => x.IsEditable);
            builder.Ignore(x => x.IsSelected);
            builder.Ignore(x => x.Quantity);
        }
    }

    // Equipment
    public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        string _schema;

        public EquipmentConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            builder.ToTable("RefEquipment");
            builder.HasKey(x => x.ResourceId);

            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Process).WithMany(b => b.Equipments).HasForeignKey(c => c.ProcessId); // FK_RefEquipment_Project

            /*builder.Ignore(x => x.EquipmentProject);
            builder.Ignore(x => x.EquipmentStandard);*/
        }
    }

    // Referential
    public class ReferentialConfiguration : IEntityTypeConfiguration<Referential>
    {
        string _schema;

        public ReferentialConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Referential> builder)
        {
            builder.ToTable("Referentials");
            builder.HasKey(x => x.ReferentialId);

            builder.Property(x => x.ReferentialId).HasColumnName(@"ReferentialId").HasColumnType("tinyint").IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);

            builder.Ignore(x => x.ChangeTracker);
        }
    }

    // Operator
    public class OperatorConfiguration : IEntityTypeConfiguration<Operator>
    {
        string _schema;

        public OperatorConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Operator> builder)
        {
            builder.ToTable("RefOperator");
            builder.HasKey(x => x.ResourceId);

            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int");

            // Foreign keys
            builder.HasOne(a => a.Process).WithMany(b => b.Operators).HasForeignKey(c => c.ProcessId); // FK_RefOperator_Project
        }
    }

    // Resource
    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        string _schema;

        public ResourceConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.ToTable("RefResource");
            builder.HasKey(x => x.ResourceId);

            builder.Property(x => x.ResourceId).HasColumnName(@"ResourceId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.PaceRating).HasColumnName(@"PaceRating").HasColumnType("float").IsRequired();
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Color).HasColumnName(@"Color").HasColumnType("varchar").IsUnicode(false).HasMaxLength(10);
            builder.Property(x => x.Uri).HasColumnName(@"Uri").HasColumnType("nvarchar").HasMaxLength(255);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedResources).HasForeignKey(c => c.CreatedByUserId); // FK_Resource_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedResources).HasForeignKey(c => c.ModifiedByUserId); // FK_Resource_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.Id);
        }
    }

    // Role
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        string _schema;

        public RoleConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Role");
            builder.HasKey(x => x.RoleCode);

            builder.Property(x => x.RoleCode).HasColumnName(@"RoleCode").HasColumnType("nchar").IsRequired().HasMaxLength(6).ValueGeneratedNever();
            builder.Property(x => x.ShortLabelResourceId).HasColumnName(@"ShortLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LongLabelResourceId).HasColumnName(@"LongLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.LongLabelResource).WithMany(b => b.RolesForLongLabel).HasForeignKey(c => c.LongLabelResourceId); // FK_Role_AppResourceKey_LongLabel
            builder.HasOne(a => a.ShortLabelResource).WithMany(b => b.RolesForShortLabel).HasForeignKey(c => c.ShortLabelResourceId); // FK_Role_AppResourceKey_ShortLabel

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ShortLabel);
            builder.Ignore(x => x.LongLabel);
            builder.Ignore(x => x.IsChecked);
        }
    }

    // Scenario
    public class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
    {
        string _schema;

        public ScenarioConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Scenario> builder)
        {
            builder.ToTable("Scenario");
            builder.HasKey(x => x.ScenarioId);

            builder.Property(x => x.ScenarioId).HasColumnName(@"ScenarioId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.ProjectId).HasColumnName(@"ProjectId").HasColumnType("int").IsRequired();
            builder.Property(x => x.StateCode).HasColumnName(@"ScenarioStateCode").HasColumnType("nchar").IsRequired().HasMaxLength(6);
            builder.Property(x => x.NatureCode).HasColumnName(@"ScenarioNatureCode").HasColumnType("nchar").IsRequired().HasMaxLength(6);
            builder.Property(x => x.OriginalScenarioId).HasColumnName(@"OriginalScenarioId").HasColumnType("int");
            builder.Property(x => x.Label).HasColumnName(@"Label").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.IsShownInSummary).HasColumnName(@"IsShownInSummary").HasColumnType("bit").IsRequired();
            builder.Property(x => x.CriticalPathIDuration).HasColumnName(@"CriticalPathIDuration").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.WebPublicationGuid).HasColumnName(@"WebPublicationGuid").HasColumnType("uniqueidentifier");
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Original).WithMany(b => b.InheritedScenarios).HasForeignKey(c => c.OriginalScenarioId); // FK_Scenario_Scenario_Original
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedScenarios).HasForeignKey(c => c.CreatedByUserId); // FK_Scenario_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedScenarios).HasForeignKey(c => c.ModifiedByUserId); // FK_Scenario_User_ModifiedBy
            builder.HasOne(a => a.Project).WithMany(b => b.Scenarios).HasForeignKey(c => c.ProjectId); // FK_Scenario_Project
            builder.HasOne(a => a.Nature).WithMany(b => b.Scenarios).HasForeignKey(c => c.NatureCode); // FK_Scenario_ScenarioNature
            builder.HasOne(a => a.State).WithMany(b => b.Scenarios).HasForeignKey(c => c.StateCode); // FK_Scenario_State

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.IsLocked);
        }
    }

    // ScenarioNature
    public class ScenarioNatureConfiguration : IEntityTypeConfiguration<ScenarioNature>
    {
        string _schema;

        public ScenarioNatureConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<ScenarioNature> builder)
        {
            builder.ToTable("ScenarioNature");
            builder.HasKey(x => x.ScenarioNatureCode);

            builder.Property(x => x.ScenarioNatureCode).HasColumnName(@"ScenarioNatureCode").HasColumnType("nchar").IsRequired().HasMaxLength(6).ValueGeneratedNever();
            builder.Property(x => x.ShortLabelResourceId).HasColumnName(@"ShortLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LongLabelResourceId).HasColumnName(@"LongLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.LongLabelResource).WithMany(b => b.ScenarioNaturesForLongLabel).HasForeignKey(c => c.LongLabelResourceId); // FK_ScenarioNature_AppResourceKey_LongLabel
            builder.HasOne(a => a.ShortLabelResource).WithMany(b => b.ScenarioNaturesForShortLabel).HasForeignKey(c => c.ShortLabelResourceId); // FK_ScenarioNature_AppResourceKey_ShortLabel

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ShortLabel);
            builder.Ignore(x => x.LongLabel);
        }
    }

    // ScenarioState
    public class ScenarioStateConfiguration : IEntityTypeConfiguration<ScenarioState>
    {
        string _schema;

        public ScenarioStateConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<ScenarioState> builder)
        {
            builder.ToTable("ScenarioState");
            builder.HasKey(x => x.ScenarioStateCode);

            builder.Property(x => x.ScenarioStateCode).HasColumnName(@"ScenarioStateCode").HasColumnType("nchar").IsRequired().HasMaxLength(6).ValueGeneratedNever();
            builder.Property(x => x.ShortLabelResourceId).HasColumnName(@"ShortLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LongLabelResourceId).HasColumnName(@"LongLabelResourceId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.LongLabelResource).WithMany(b => b.ScenarioStatesForLongLabel).HasForeignKey(c => c.LongLabelResourceId); // FK_State_AppResourceKey_LongLabel
            builder.HasOne(a => a.ShortLabelResource).WithMany(b => b.ScenarioStatesForShortLabel).HasForeignKey(c => c.ShortLabelResourceId); // FK_State_AppResourceKey_ShortLabel

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.ShortLabel);
            builder.Ignore(x => x.LongLabel);
        }
    }

    // Solution
    public class SolutionConfiguration : IEntityTypeConfiguration<Solution>
    {
        string _schema;

        public SolutionConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Solution> builder)
        {
            builder.ToTable("Solution");
            builder.HasKey(x => x.SolutionId);

            builder.Property(x => x.SolutionId).HasColumnName(@"SolutionId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.ScenarioId).HasColumnName(@"ScenarioId").HasColumnType("int").IsRequired();
            builder.Property(x => x.SolutionDescription).HasColumnName(@"SolutionDescription").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.Approved).HasColumnName(@"Approved").HasColumnType("bit").IsRequired();
            builder.Property(x => x.Cost).HasColumnName(@"Cost").HasColumnType("smallint");
            builder.Property(x => x.Difficulty).HasColumnName(@"Difficulty").HasColumnType("smallint");
            builder.Property(x => x.Investment).HasColumnName(@"Investment").HasColumnType("float");
            builder.Property(x => x.Comments).HasColumnName(@"Comments").HasColumnType("nvarchar").HasMaxLength(4000);
            builder.Property(x => x.IsEmpty).HasColumnName(@"IsEmpty").HasColumnType("bit").IsRequired();
            builder.Property(x => x.Who).HasColumnName(@"Who").HasColumnType("nvarchar").HasMaxLength(50);
            builder.Property(x => x.When).HasColumnName(@"When").HasColumnType("date");
            builder.Property(x => x.P).HasColumnName(@"P").IsRequired().HasPrecision(5, 2);
            builder.Property(x => x.D).HasColumnName(@"D").IsRequired().HasPrecision(5, 2);
            builder.Property(x => x.C).HasColumnName(@"C").IsRequired().HasPrecision(5, 2);
            builder.Property(x => x.A).HasColumnName(@"A").IsRequired().HasPrecision(5, 2);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Scenario).WithMany(b => b.Solutions).HasForeignKey(c => c.ScenarioId); // FK_Solution_Scenario

            builder.Ignore(x => x.ChangeTracker);
        }
    }

    // UISetting
    public class UISettingConfiguration : IEntityTypeConfiguration<UISetting>
    {
        string _schema;

        public UISettingConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<UISetting> builder)
        {
            builder.ToTable("UISetting");
            builder.HasKey(x => x.Key);

            builder.Property(x => x.Key).HasColumnName(@"Key").HasColumnType("nvarchar").IsRequired().HasMaxLength(200).ValueGeneratedNever();
            builder.Property(x => x.Value).HasColumnName(@"Value").HasColumnType("varbinary(max)").IsRequired();

            builder.Ignore(x => x.ChangeTracker);
        }
    }

    // User
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        string _schema;

        public UserConfiguration(string schema = "dbo")
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(x => x.UserId);

            builder.Property(x => x.UserId).HasColumnName(@"UserId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.DefaultLanguageCode).HasColumnName(@"DefaultLanguageCode").HasColumnType("char").IsUnicode(false).HasMaxLength(5);
            builder.Property(x => x.Username).HasColumnName(@"Username").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Password).HasColumnName(@"Password").HasColumnType("binary").HasMaxLength(20);
            builder.Property(x => x.Firstname).HasColumnName(@"Firstname").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.Email).HasColumnName(@"Email").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.PhoneNumber).HasColumnName(@"PhoneNumber").HasColumnType("nvarchar").HasMaxLength(20);
            builder.Property(x => x.IsDeleted).HasColumnName(@"IsDeleted").HasColumnType("bit").IsRequired();
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int");
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int");
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedUsers).HasForeignKey(c => c.CreatedByUserId); // FK_User_User_CreatedBy
            builder.HasOne(a => a.DefaultLanguage).WithMany(b => b.Users).HasForeignKey(c => c.DefaultLanguageCode); // FK_User_Language
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedUsers).HasForeignKey(c => c.ModifiedByUserId); // FK_User_User_ModifiedBy

            builder.Ignore(x => x.ChangeTracker);
            builder.Ignore(x => x.NewPassword);
            builder.Ignore(x => x.ConfirmNewPassword);
            builder.Ignore(x => x.ValidateRoleCodes);
            builder.Ignore(x => x.IsUsernameReadOnly);
        }
    }

    // UserRole
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        string _schema;

        public UserRoleConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRole");
            builder.HasKey(x => new { x.UserId, x.RoleCode });

            builder.Property(x => x.RoleCode).HasColumnType("nchar").IsRequired().HasMaxLength(6);

            // Foreign keys
            builder.HasOne(t => t.User).WithMany(t => t.UserRoles).HasForeignKey(t => t.UserId);
            builder.HasOne(t => t.Role).WithMany(t => t.UserRoles).HasForeignKey(t => t.RoleCode);
        }
    }

    // UserRoleProject
    public class UserRoleProjectConfiguration : IEntityTypeConfiguration<UserRoleProject>
    {
        string _schema;

        public UserRoleProjectConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<UserRoleProject> builder)
        {
            builder.ToTable("UserRoleProject");
            builder.HasKey(x => new { x.UserId, x.RoleCode, x.ProjectId });

            builder.Property(x => x.UserId).HasColumnName(@"UserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.RoleCode).HasColumnName(@"RoleCode").HasColumnType("nchar").IsRequired().HasMaxLength(6);
            builder.Property(x => x.ProjectId).HasColumnName(@"ProjectId").HasColumnType("int").IsRequired();

            // Foreign keys
            builder.HasOne(a => a.Project).WithMany(b => b.UserRoleProjects).HasForeignKey(c => c.ProjectId); // FK_UserRoleProject_Project
            builder.HasOne(a => a.Role).WithMany(b => b.UserRoleProjects).HasForeignKey(c => c.RoleCode); // FK_UserRoleProject_Role
            builder.HasOne(a => a.User).WithMany(b => b.UserRoleProjects).HasForeignKey(c => c.UserId); // FK_UserRoleProject_User

            builder.Ignore(x => x.ChangeTracker);
        }
    }

    // Video
    public class VideoConfiguration : IEntityTypeConfiguration<Video>
    {
        string _schema;

        public VideoConfiguration(string schema = null)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.ToTable("Video");
            builder.HasKey(x => x.VideoId);

            builder.Property(x => x.VideoId).HasColumnName(@"VideoId").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.ProcessId).HasColumnName(@"ProcessId").HasColumnType("int").IsRequired();
            builder.Property(x => x.DefaultResourceId).HasColumnName(@"DefaultResourceId").HasColumnType("int");
            builder.Property(x => x.CameraName).HasColumnName(@"CameraName").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.Duration).HasColumnName(@"Duration").HasColumnType("float").IsRequired();
            builder.Property(x => x.Format).HasColumnName(@"Format").HasColumnType("nvarchar").IsRequired().HasMaxLength(100);
            builder.Property(x => x.FilePath).HasColumnName(@"FilePath").HasColumnType("nvarchar").IsRequired().HasMaxLength(255);
            builder.Property(x => x.ShootingDate).HasColumnName(@"ShootingDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.Thumbnail).HasColumnName(@"Thumbnail").HasColumnType("varbinary(max)");
            builder.Property(x => x.CreatedByUserId).HasColumnName(@"CreatedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ModifiedByUserId).HasColumnName(@"ModifiedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.LastModificationDate).HasColumnName(@"LastModificationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Foreign keys
            builder.HasOne(a => a.DefaultResource).WithMany(b => b.Videos).HasForeignKey(c => c.DefaultResourceId); // FK_Video_Resource
            builder.HasOne(a => a.Creator).WithMany(b => b.CreatedVideos).HasForeignKey(c => c.CreatedByUserId); // FK_Video_User_CreatedBy
            builder.HasOne(a => a.LastModifier).WithMany(b => b.LastModifiedVideos).HasForeignKey(c => c.ModifiedByUserId); // FK_Video_User_ModifiedBy
            builder.HasOne(a => a.Process).WithMany(b => b.Videos).HasForeignKey(c => c.ProcessId); // FK_Video_Project

            builder.Ignore(x => x.ChangeTracker);
        }
    }

}
