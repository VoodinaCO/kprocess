SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
GO

USE [master];
GO

IF (DB_ID(N'Kprocess.KL2') IS NOT NULL) 
BEGIN
    ALTER DATABASE [Kprocess.KL2]
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [Kprocess.KL2];
END
GO

PRINT N'Création de Kprocess.KL2...'
GO

CREATE DATABASE [Kprocess.KL2] COLLATE SQL_Latin1_General_CP1_CI_AS;
GO

IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'Kprocess.KL2')
    BEGIN
        ALTER DATABASE [Kprocess.KL2]
            SET ANSI_NULLS OFF,
                ANSI_PADDING OFF,
                ANSI_WARNINGS OFF,
                ARITHABORT OFF,
                CONCAT_NULL_YIELDS_NULL OFF,
                NUMERIC_ROUNDABORT OFF,
                QUOTED_IDENTIFIER OFF,
                ANSI_NULL_DEFAULT OFF,
                CURSOR_DEFAULT GLOBAL,
                RECOVERY FULL,
                CURSOR_CLOSE_ON_COMMIT OFF,
                AUTO_CREATE_STATISTICS ON,
                AUTO_SHRINK OFF,
                AUTO_UPDATE_STATISTICS ON,
                RECURSIVE_TRIGGERS OFF 
            WITH ROLLBACK IMMEDIATE;
        ALTER DATABASE [Kprocess.KL2]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END
GO

IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'Kprocess.KL2')
    BEGIN
        ALTER DATABASE [Kprocess.KL2]
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
    END
GO

IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'Kprocess.KL2')
    BEGIN
        ALTER DATABASE [Kprocess.KL2]
            SET READ_COMMITTED_SNAPSHOT OFF 
            WITH ROLLBACK IMMEDIATE;
    END
GO

IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'Kprocess.KL2')
    BEGIN
        ALTER DATABASE [Kprocess.KL2]
            SET AUTO_UPDATE_STATISTICS_ASYNC OFF,
                PAGE_VERIFY CHECKSUM,
                DATE_CORRELATION_OPTIMIZATION OFF,
                DISABLE_BROKER,
                PARAMETERIZATION SIMPLE,
                SUPPLEMENTAL_LOGGING OFF 
            WITH ROLLBACK IMMEDIATE;
    END
GO

IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'Kprocess.KL2')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [Kprocess.KL2]
    SET TRUSTWORTHY OFF,
        DB_CHAINING OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'Impossible de modifier les paramètres de base de données. Vous devez être administrateur système pour appliquer ces paramètres.';
    END
GO

IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'Kprocess.KL2')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [Kprocess.KL2]
    SET HONOR_BROKER_PRIORITY OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'Impossible de modifier les paramètres de base de données. Vous devez être administrateur système pour appliquer ces paramètres.';
    END
GO

USE [Kprocess.KL2];
GO

IF fulltextserviceproperty(N'IsFulltextInstalled') = 1
    EXECUTE sp_fulltext_database 'enable';
GO

USE [Kprocess.KL2]
GO
/****** Object:  Table [dbo].[Action]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Action](
	[ActionId] [int] IDENTITY(1,1) NOT NULL,
	[ScenarioId] [int] NOT NULL,
	[ResourceId] [int] NULL,
	[ActionCategoryId] [int] NULL,
	[OriginalActionId] [int] NULL,
	[VideoId] [int] NULL,
	[WBS] [nvarchar](50) NOT NULL,
	[Label] [nvarchar](100) NULL,
	[Start] [bigint] NOT NULL,
	[Finish] [bigint] NOT NULL,
	[BuildStart] [bigint] NOT NULL,
	[BuildFinish] [bigint] NOT NULL,
	[IsRandom] [bit] NOT NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[CustomNumericValue] [float] NULL,
	[CustomNumericValue2] [float] NULL,
	[CustomNumericValue3] [float] NULL,
	[CustomNumericValue4] [float] NULL,
	[CustomTextValue] [nvarchar](4000) NULL,
	[CustomTextValue2] [nvarchar](4000) NULL,
	[CustomTextValue3] [nvarchar](4000) NULL,
	[CustomTextValue4] [nvarchar](4000) NULL,
	[DifferenceReason] [nvarchar](100) NULL,
	[ThumbnailHash] [nchar](32) NULL,
	[IsThumbnailSpecific] [bit] NOT NULL,
	[ThumbnailPosition] [bigint] NULL,
	[RowVersion] [timestamp] NULL,
	[IsKeyTask] [bit] NOT NULL,
	[LinkedProcessId] [int] NULL,
	[SkillId] [int] NULL,
 CONSTRAINT [PK_Action] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ActionPredecessorSuccessor]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActionPredecessorSuccessor](
	[ActionPredecessorId] [int] NOT NULL,
	[ActionSuccessorId] [int] NOT NULL,
 CONSTRAINT [PK_ActionPredecessorSuccessor] PRIMARY KEY CLUSTERED 
(
	[ActionPredecessorId] ASC,
	[ActionSuccessorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ActionReduced]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActionReduced](
	[ActionId] [int] NOT NULL,
	[ActionTypeCode] [nchar](6) NULL,
	[Solution] [nvarchar](100) NULL,
	[ReductionRatio] [float] NOT NULL,
	[OriginalBuildDuration] [bigint] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_ActionReduced] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ActionType]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActionType](
	[ActionTypeCode] [nchar](6) NOT NULL,
	[ShortLabelResourceId] [int] NOT NULL,
	[LongLabelResourceId] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_ActionType] PRIMARY KEY CLUSTERED 
(
	[ActionTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ActionValue]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActionValue](
	[ActionValueCode] [nchar](6) NOT NULL,
	[ShortLabelResourceId] [int] NOT NULL,
	[LongLabelResourceId] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_ActionValue] PRIMARY KEY CLUSTERED 
(
	[ActionValueCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Anomaly]    Script Date: 17/05/2018 16:55:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Anomaly](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](MAX) NULL,
	[Photo] [varbinary](MAX) NULL,
	[InspectionId] [int] NOT NULL,
	[InspectorId] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Type] [int] NOT NULL,
	[Line] [nvarchar](254) NULL,
	[Machine] [nvarchar](254) NULL,
	[Priority] [int] NULL,
	[Label] [nvarchar](254) NULL,
	[Category] [nvarchar](254) NULL,
	[Origin] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Anomaly] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppResourceKey]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppResourceKey](
	[ResourceId] [int] IDENTITY(1,1) NOT NULL,
	[ResourceKey] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_AppResource] PRIMARY KEY CLUSTERED 
(
	[ResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppResourceValue]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppResourceValue](
	[ResourceId] [int] NOT NULL,
	[LanguageCode] [char](5) NOT NULL,
	[Value] [nvarchar](500) NOT NULL,
	[Comment] [nvarchar](500) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_AppResourceValue] PRIMARY KEY CLUSTERED 
(
	[ResourceId] ASC,
	[LanguageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppSetting]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppSetting](
	[Key] [nvarchar](50) NOT NULL,
	[Value] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_AppSettings] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Audit]    Script Date: 11/06/2018 07:58:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Audit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SurveyId] [int] NOT NULL,
	[AuditorId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[InspectionId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditItem]    Script Date: 07/06/2018 11:15:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditItem](
	[AuditId] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[IsOk] [bit] NULL,
	[Photo] [varbinary](max) NULL,
	[Comment] [nvarchar](max) NULL,
 CONSTRAINT [PK_AuditItem] PRIMARY KEY CLUSTERED 
(
	[AuditId] ASC,
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CloudFile]    Script Date: 26/08/2018 13:40:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CloudFile](
	[Hash] [nchar](32) NOT NULL,
	[Extension] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CutVideo]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CutVideo](
	[Hash] [nchar](32) NOT NULL,
	[HashOriginalVideo] [nchar](32) NOT NULL,
	[Start] [bigint] NOT NULL,
	[End] [bigint] NOT NULL,
	[Watermark] [nvarchar](4000) NULL,
	[MinDuration] [bigint] NOT NULL,
	[Extension] [nvarchar](20) NULL,
 CONSTRAINT [PK_CutVideo] PRIMARY KEY CLUSTERED 
(
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentationReferential]    Script Date: 10/05/2019 13:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentationReferential](
	[ProcessId] [int] NOT NULL,
	[ReferentialId] [tinyint] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[HasMultipleSelection] [bit] NOT NULL,
	[HasQuantity] [bit] NOT NULL,
 CONSTRAINT [PK_DocumentationReferential] PRIMARY KEY CLUSTERED 
(
	[ProcessId] ASC,
	[ReferentialId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentationDraft]    Script Date: 14/02/2019 14:41:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentationDraft](
	[DocumentationDraftId] [int] IDENTITY(1,1) NOT NULL,
	[ScenarioId] [int] NOT NULL,
	[Formation_DispositionAsJson] [nvarchar](max) NULL,
	[Inspection_DispositionAsJson] [nvarchar](max) NULL,
	[Evaluation_DispositionAsJson] [nvarchar](max) NULL,
	[ActiveVideoExport] bit NULL,
	[SlowMotion] bit NULL,
	[SlowMotionDuration] [decimal] NULL,
	[WaterMarking] bit NULL,
	[WaterMarkingText] [nvarchar](255) NULL,
	[WaterMarkingVAlign] [int] NULL,
	[WaterMarkingHAlign] [int] NULL,
	[Formation_IsMajor] [bit] NOT NULL DEFAULT 1,
	[Formation_ReleaseNote] [nvarchar](max) NULL,
	[Inspection_IsMajor] [bit] NOT NULL DEFAULT 1,
	[Inspection_ReleaseNote] [nvarchar](max) NULL,
	[Evaluation_IsMajor] [bit] NOT NULL DEFAULT 1,
	[Evaluation_ReleaseNote] [nvarchar](max) NULL,
	[Formation_ActionDisposition] nvarchar(max) NULL,
	[Inspection_ActionDisposition] nvarchar(max) NULL,
	[Evaluation_ActionDisposition] nvarchar(max) NULL
PRIMARY KEY CLUSTERED 
(
	[DocumentationDraftId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentationDraftLocalization]    Script Date: 12/05/2019 15:34:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentationDraftLocalization](
	[ProcessId] [int] NOT NULL,
	[ReferentialId] tinyint NOT NULL,
	[Value] [nvarchar](500) NULL,
 CONSTRAINT [PK_DocumentationDraftLocalization] PRIMARY KEY CLUSTERED 
(
	[ProcessId] ASC,
	[ReferentialId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentationActionDraft]    Script Date: 14/02/2019 14:41:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentationActionDraft](
	[DocumentationActionDraftId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](100) NULL,
	[Duration] [bigint] NOT NULL,
	[ThumbnailHash] [nchar](32) NULL,
	[IsKeyTask] [bit] NOT NULL,
	[SkillId] [int] NULL,
	[CustomNumericValue] [float] NULL,
	[CustomNumericValue2] [float] NULL,
	[CustomNumericValue3] [float] NULL,
	[CustomNumericValue4] [float] NULL,
	[CustomTextValue] [nvarchar](4000) NULL,
	[CustomTextValue2] [nvarchar](4000) NULL,
	[CustomTextValue3] [nvarchar](4000) NULL,
	[CustomTextValue4] [nvarchar](4000) NULL,
	[ResourceId] [int] NULL,
	[ActionCategoryId] [int] NULL
PRIMARY KEY CLUSTERED 
(
	[DocumentationActionDraftId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReferentialDocumentationActionDraft]    Script Date: 12/05/2019 15:37:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferentialDocumentationActionDraft](
	[DocumentationActionDraftId] [int] NOT NULL,
	[ReferentialId] [int] NOT NULL,
	[RefNumber] [int] NOT NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_ReferentialDocumentationActionDraft] PRIMARY KEY CLUSTERED 
(
	[DocumentationActionDraftId] ASC,
	[ReferentialId] ASC,
	[RefNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentationActionDraftWBS]    Script Date: 14/02/2019 14:41:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentationActionDraftWBS](
	[DocumentationActionDraftWBSId] [int] IDENTITY(1,1) NOT NULL,
	[ActionId] [int] NULL,
	[DocumentationActionDraftId] [int] NULL,
	[WBS] [nvarchar](50) NOT NULL,
	[DocumentationDraftId] [int] NOT NULL,
	[DocumentationPublishMode] [int] NOT NULL
PRIMARY KEY CLUSTERED 
(
	[DocumentationActionDraftWBSId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Timeslot]    Script Date: 09/10/2018 14:25:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Timeslot](
	[TimeslotId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[Color] [varchar](10) NULL,
	[DisplayOrder] [int] NULL,
	[IsDeleted] [bit] NOT NULL
PRIMARY KEY CLUSTERED 
(
	[TimeslotId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Inspection]    Script Date: 23/04/2018 14:35:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Inspection](
	[InspectionId] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[PublicationId] [uniqueidentifier] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsScheduled] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[InspectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InspectionSchedule]    Script Date: 04/10/2018 08:46:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InspectionSchedule](
	[InspectionScheduleId] [int] IDENTITY(1,1) NOT NULL,
	[ProcessId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[TimeslotId] [int] NOT NULL,
	[RecurrenceId] [int] NULL,
	[RecurrenceRule] [nvarchar](max) NULL,
	[RecurrenceException] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[InspectionScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InspectionStep]    Script Date: 23/04/2018 14:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InspectionStep](
	[InspectionStepId] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[InspectionId] [int] NOT NULL,
	[PublishedActionId] [int] NOT NULL,
	[IsOk] [bit] NULL,
	[InspectorId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[AnomalyId] [int] NULL,
	[LinkedInspectionId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[InspectionStepId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Language]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Language](
	[LanguageCode] [char](5) NOT NULL,
	[Label] [nvarchar](50) NOT NULL,
	[FlagName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED 
(
	[LanguageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotificationTypeSetting]    Script Date: 19/10/2018 15:54:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationTypeSetting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Recipients] [nvarchar](max) NULL,
	[RecipientCcs] [nvarchar](max) NULL,
	[RecipientBccs] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[BodyTemplate] [nvarchar](max) NULL,
	[PdfTemplate] [nvarchar](max) NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotificationType]    Script Date: 19/10/2018 15:54:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](max) NULL, /*Report type => Anomaly, Evaluation, Inspection, Audit*/
	[Description] [nvarchar](max) NULL,
	[NotificationTypeSettingId] [int] NOT NULL,
	[NotificationCategory] [int] NOT NULL /*Enum category => Email, Message etc.*/
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 19/10/2018 15:54:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[NotificationId] [int] IDENTITY(1,1) NOT NULL,
	[ActualSendingDate] [datetime2](7) NULL,
	[Body] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[IsProcessed] [bit] NOT NULL,
	[NotificationTypeId] [int] NOT NULL,
	[RecipientBcc] [nvarchar](max) NULL,
	[RecipientCc] [nvarchar](max) NULL,
	[RecipientTo] [nvarchar](max) NULL,
	[ScheduledSendingDate] [datetime2](7) NOT NULL,
	[Subject] [nvarchar](max) NULL
PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotificationAttachment]    Script Date: 30/10/2018 14:54:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationAttachment](
	[NotificationAttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[NotificationId] [int] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Attachment] [varbinary](max) NULL
PRIMARY KEY CLUSTERED 
(
	[NotificationAttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Objective]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Objective](
	[ObjectiveCode] [nchar](6) NOT NULL,
	[ShortLabelResourceId] [int] NOT NULL,
	[LongLabelResourceId] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_Objective] PRIMARY KEY CLUSTERED 
(
	[ObjectiveCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Procedure]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Procedure](
	[ProcessId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](100) NOT NULL,
	[ProjectDirId] [int] NULL,
	[Description] [nvarchar](4000) NULL,
	[IsSkill] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
	[OwnerId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProcessId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Project]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Project](
	[ProjectId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[ObjectiveCode] [nchar](6) NULL,
	[OtherObjectiveLabel] [nvarchar](50) NULL,
	[CustomTextLabel] [nvarchar](100) NULL,
	[CustomTextLabel2] [nvarchar](100) NULL,
	[CustomTextLabel3] [nvarchar](100) NULL,
	[CustomTextLabel4] [nvarchar](100) NULL,
	[CustomNumericLabel] [nvarchar](100) NULL,
	[CustomNumericLabel2] [nvarchar](100) NULL,
	[CustomNumericLabel3] [nvarchar](100) NULL,
	[CustomNumericLabel4] [nvarchar](100) NULL,
	[TimeScale] [bigint] NOT NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[ParentProjectId] [int] NULL,
	[ProcessId] [int] NOT NULL,
	[Workshop] [nvarchar](4000) NULL,
	[StartDate] [datetime] NOT NULL,
	[ForecastEndDate] [datetime] NULL,
	[RealEndDate] [datetime] NULL,
	[Formation_Disposition] [varbinary](MAX), 
	[Inspection_Disposition] [varbinary](MAX), 
	[Audit_Disposition] [varbinary](MAX), 
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
	[IsAbandoned] [bit] NOT NULL DEFAULT ((0)),
	[Other_Disposition] [varbinary](MAX) NULL, 
	[Evaluation_Disposition] [varbinary](MAX) NULL
 CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED 
(
	[ProjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectDir]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectDir](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ParentId] [int] NULL,
	[RowVersion] [timestamp] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_ProjectDir] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectReferential]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectReferential](
	[ProjectId] [int] NOT NULL,
	[ReferentialId] [tinyint] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[HasMultipleSelection] [bit] NOT NULL,
	[KeepsSelection] [bit] NOT NULL,
	[HasQuantity] [bit] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_ProjectReferential] PRIMARY KEY CLUSTERED 
(
	[ProjectId] ASC,
	[ReferentialId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Publication]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Publication](
	[PublicationId] [uniqueidentifier] NOT NULL,
	[ProjectId] [int] NOT NULL,
	[ScenarioId] [int] NOT NULL,
	[Label] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[CriticalPathIDuration] [bigint] NOT NULL,
	[PublishedByUserId] [int] NOT NULL,
	[PublishedDate] [datetime] NOT NULL,
	[Watermark] [nvarchar](4000) NULL,
	[ProcessId] [int] NOT NULL,
	[MinDurationVideo] [bigint] NOT NULL,
	[TimeScale] [bigint] NOT NULL,
	[Formation_Disposition] [varbinary](max) NULL,
	[Inspection_Disposition] [varbinary](max) NULL,
	[Audit_Disposition] [varbinary](max) NULL,
	[Evaluation_Disposition] [varbinary](max) NULL,
	[IsSkill] [bit] NOT NULL,
	[PublishMode] [INT] NOT NULL,
	[IsMajor] [bit] NOT NULL DEFAULT 1,
	[ReleaseNote] [nvarchar](max) NULL,
	[Version] [nvarchar](10) NOT NULL DEFAULT (N'1.0'),
	[Formation_ActionDisposition] nvarchar(max) NULL,
	[Inspection_ActionDisposition] nvarchar(max) NULL,
	[Evaluation_ActionDisposition] nvarchar(max) NULL
 CONSTRAINT [PK_Publication] PRIMARY KEY CLUSTERED 
(
	[PublicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PublicationHistory]    Script Date: 09/05/2019 10:10:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublicationHistory](
	[PublicationHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[ProcessId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[TrainingDocumentationId] [uniqueidentifier] NULL,
	[EvaluationDocumentationId] [uniqueidentifier] NULL,
	[InspectionDocumentationId] [uniqueidentifier] NULL,
	[State] [int] NOT NULL,
	[ErrorMessage] [nvarchar](254) NULL,
	[PublisherId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PublicationHistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PublicationLocalization]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublicationLocalization](
	[PublicationId] [uniqueidentifier] NOT NULL,
	[ResourceKey] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](500) NULL,
 CONSTRAINT [PK_PublicationLocalization] PRIMARY KEY CLUSTERED 
(
	[PublicationId] ASC,
	[ResourceKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PublishedAction]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublishedAction](
	[PublishedActionId] [int] IDENTITY(1,1) NOT NULL,
	[ResourceId] [int] NULL,
	[ActionCategoryId] [int] NULL,
	[CutVideoHash] [nchar](32) NULL,
	[WBS] [nvarchar](50) NOT NULL,
	[Label] [nvarchar](100) NULL,
	[Start] [bigint] NOT NULL,
	[Finish] [bigint] NOT NULL,
	[BuildStart] [bigint] NOT NULL,
	[BuildFinish] [bigint] NOT NULL,
	[IsRandom] [bit] NOT NULL,
	[CustomNumericValue] [float] NULL,
	[CustomNumericValue2] [float] NULL,
	[CustomNumericValue3] [float] NULL,
	[CustomNumericValue4] [float] NULL,
	[CustomTextValue] [nvarchar](4000) NULL,
	[CustomTextValue2] [nvarchar](4000) NULL,
	[CustomTextValue3] [nvarchar](4000) NULL,
	[CustomTextValue4] [nvarchar](4000) NULL,
	[DifferenceReason] [nvarchar](100) NULL,
	[ThumbnailHash] [nchar](32) NULL,
	[PublicationId] [uniqueidentifier] NOT NULL,
	[IsKeyTask] [bit] NOT NULL,
	[SkillId] [int] NULL,
	[LinkedPublicationId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[PublishedActionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PublishedActionCategory]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublishedActionCategory](
	[PublishedActionCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[FileHash] [nchar](32) NULL,
	[ActionTypeCode] [nchar](6) NULL,
	[ActionValueCode] [nchar](6) NULL,
PRIMARY KEY CLUSTERED 
(
	[PublishedActionCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PublishedActionPredecessorSuccessor]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublishedActionPredecessorSuccessor](
	[PublishedActionPredecessorId] [int] NOT NULL,
	[PublishedActionSuccessorId] [int] NOT NULL,
 CONSTRAINT [PK_PublishedActionPredecessorSuccessor] PRIMARY KEY CLUSTERED 
(
	[PublishedActionPredecessorId] ASC,
	[PublishedActionSuccessorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PublishedFile]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublishedFile](
	[Hash] [nchar](32) NOT NULL,
	[Extension] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PublishedReferential]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublishedReferential](
	[PublishedReferentialId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[FileHash] [nchar](32) NULL,
PRIMARY KEY CLUSTERED 
(
	[PublishedReferentialId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PublishedReferentialAction]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublishedReferentialAction](
	[PublishedActionId] [int] NOT NULL,
	[PublishedReferentialId] [int] NOT NULL,
	[RefNumber] [int] NOT NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_PublishedReferentialAction] PRIMARY KEY CLUSTERED 
(
	[PublishedActionId] ASC,
	[PublishedReferentialId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PublishedResource]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublishedResource](
	[PublishedResourceId] [int] IDENTITY(1,1) NOT NULL,
	[PaceRating] [float] NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[FileHash] [nchar](32) NULL,
 CONSTRAINT [PK_PublishedResource] PRIMARY KEY CLUSTERED 
(
	[PublishedResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Qualification]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Qualification](
	[QualificationId] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[PublicationId] [uniqueidentifier] NOT NULL,
	[UserId] [int] NOT NULL,
	[Result] [int] NULL,
	[IsQualified] [bit] NULL,
	[Comment] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[PreviousVersionQualificationId] [int], 
 CONSTRAINT [PK_Qualification] PRIMARY KEY CLUSTERED 
(
	[QualificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QualificationReason]    Script Date: 16/07/2018 08:55:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QualificationReason](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [int] NOT NULL,
	[Comment] [nvarchar](3000) NOT NULL,
	[IsEditable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QualificationStep]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QualificationStep](
	[QualificationStepId] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[QualificationReasonId] [int] NULL,
	[QualificationId] [int] NOT NULL,
	[PublishedActionId] [int] NOT NULL,
	[QualifierId] [int] NOT NULL,
	[IsQualified] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_QualificationStep] PRIMARY KEY CLUSTERED 
(
	[QualificationStepId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref1]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref1](
	[RefId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Color] [varchar](10) NULL,
	[Hash] [nchar](32) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[ProcessId] [int] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Tool] PRIMARY KEY CLUSTERED 
(
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref1Action]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref1Action](
	[ActionId] [int] NOT NULL,
	[RefId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_RefToolAction] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC,
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref2]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref2](
	[RefId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Color] [varchar](10) NULL,
	[Hash] [nchar](32) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[ProcessId] [int] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Consumable] PRIMARY KEY CLUSTERED 
(
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref2Action]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref2Action](
	[ActionId] [int] NOT NULL,
	[RefId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_RefConsumableAction] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC,
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref3]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref3](
	[RefId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Color] [varchar](10) NULL,
	[Hash] [nchar](32) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[ProcessId] [int] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Place] PRIMARY KEY CLUSTERED 
(
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref3Action]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref3Action](
	[ActionId] [int] NOT NULL,
	[RefId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_RefPlaceAction] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC,
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref4]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref4](
	[RefId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Color] [varchar](10) NULL,
	[Hash] [nchar](32) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[ProcessId] [int] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref4Action]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref4Action](
	[ActionId] [int] NOT NULL,
	[RefId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_RefDocumentAction] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC,
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref5]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref5](
	[RefId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Color] [varchar](10) NULL,
	[Hash] [nchar](32) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[ProcessId] [int] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_RefExtra1] PRIMARY KEY CLUSTERED 
(
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref5Action]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref5Action](
	[ActionId] [int] NOT NULL,
	[RefId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_RefExtra1Action] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC,
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref6]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref6](
	[RefId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Color] [varchar](10) NULL,
	[Hash] [nchar](32) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[ProcessId] [int] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_RefExtra2] PRIMARY KEY CLUSTERED 
(
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref6Action]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref6Action](
	[ActionId] [int] NOT NULL,
	[RefId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_RefExtra2Action] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC,
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref7]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref7](
	[RefId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Color] [varchar](10) NULL,
	[Hash] [nchar](32) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[ProcessId] [int] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_RefExtra3] PRIMARY KEY CLUSTERED 
(
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ref7Action]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ref7Action](
	[ActionId] [int] NOT NULL,
	[RefId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_RefExtra3Action] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC,
	[RefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefActionCategory]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefActionCategory](
	[ActionCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Color] [varchar](10) NULL,
	[Hash] [nchar](32) NULL,
	[ActionTypeCode] [nchar](6) NULL,
	[ActionValueCode] [nchar](6) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[ProcessId] [int] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_ActionCategory] PRIMARY KEY CLUSTERED 
(
	[ActionCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefEquipment]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefEquipment](
	[ResourceId] [int] NOT NULL,
	[ProcessId] [int] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Equipment] PRIMARY KEY CLUSTERED 
(
	[ResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Referentials]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Referentials](
	[ReferentialId] [tinyint] NOT NULL,
	[Label] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Referentials] PRIMARY KEY CLUSTERED 
(
	[ReferentialId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Referentials_Label] UNIQUE NONCLUSTERED 
(
	[Label] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefOperator]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefOperator](
	[ResourceId] [int] NOT NULL,
	[ProcessId] [int] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Operator] PRIMARY KEY CLUSTERED 
(
	[ResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefResource]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefResource](
	[ResourceId] [int] IDENTITY(1,1) NOT NULL,
	[PaceRating] [float] NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Color] [varchar](10) NULL,
	[Hash] [nchar](32) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_Resource] PRIMARY KEY CLUSTERED 
(
	[ResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleCode] [nchar](6) NOT NULL,
	[ShortLabelResourceId] [int] NOT NULL,
	[LongLabelResourceId] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Scenario]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Scenario](
	[ScenarioId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NOT NULL,
	[ScenarioStateCode] [nchar](6) NOT NULL,
	[ScenarioNatureCode] [nchar](6) NOT NULL,
	[OriginalScenarioId] [int] NULL,
	[Label] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[IsShownInSummary] [bit] NOT NULL,
	[CriticalPathIDuration] [bigint] NOT NULL,
	[WebPublicationGuid] [uniqueidentifier] NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Scenario] PRIMARY KEY CLUSTERED 
(
	[ScenarioId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScenarioNature]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScenarioNature](
	[ScenarioNatureCode] [nchar](6) NOT NULL,
	[ShortLabelResourceId] [int] NOT NULL,
	[LongLabelResourceId] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_ScenarioNature] PRIMARY KEY CLUSTERED 
(
	[ScenarioNatureCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScenarioState]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScenarioState](
	[ScenarioStateCode] [nchar](6) NOT NULL,
	[ShortLabelResourceId] [int] NOT NULL,
	[LongLabelResourceId] [int] NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED 
(
	[ScenarioStateCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Skill]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Skill](
	[SkillId] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](128) NOT NULL,
	[Color] [varchar](10) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Skill] PRIMARY KEY CLUSTERED 
(
	[SkillId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Solution]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Solution](
	[SolutionId] [int] IDENTITY(1,1) NOT NULL,
	[ScenarioId] [int] NOT NULL,
	[SolutionDescription] [nvarchar](100) NULL,
	[Approved] [bit] NOT NULL,
	[Cost] [smallint] NULL,
	[Difficulty] [smallint] NULL,
	[Investment] [float] NULL,
	[Comments] [nvarchar](4000) NULL,
	[IsEmpty] [bit] NOT NULL,
	[Who] [nvarchar](50) NULL,
	[When] [date] NULL,
	[P] [decimal](5, 2) NOT NULL,
	[D] [decimal](5, 2) NOT NULL,
	[C] [decimal](5, 2) NOT NULL,
	[A] [decimal](5, 2) NOT NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_Solution] PRIMARY KEY CLUSTERED 
(
	[SolutionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Survey]    Script Date: 11/06/2018 08:00:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Survey](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](254) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SurveyItem]    Script Date: 07/06/2018 11:15:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SurveyItem](
	[SurveyId] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[Query] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_SurveyItem] PRIMARY KEY CLUSTERED 
(
	[SurveyId] ASC,
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Team]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Team](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Team] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Training]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Training](
	[TrainingId] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[PublicationId] [uniqueidentifier] NOT NULL,
	[UserId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[PreviousVersionTrainingId] [int],
 CONSTRAINT [PK_Training] PRIMARY KEY CLUSTERED 
(
	[TrainingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UISetting]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UISetting](
	[Key] [nvarchar](200) NOT NULL,
	[Value] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_UISettings] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[DefaultLanguageCode] [char](5) NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [binary](20) NULL,
	[Firstname] [nvarchar](100) NULL,
	[Name] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NULL,
	[Tenured] [bit] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserReadPublication]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserReadPublication](
	[UserId] [int] NOT NULL,
	[PublicationId] [uniqueidentifier] NOT NULL,
	[ReadDate] [datetime] NULL,
	[PreviousVersionPublicationId] [uniqueidentifier],
 CONSTRAINT [PK_UserReadPublication] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[PublicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[UserId] [int] NOT NULL,
	[RoleCode] [nchar](6) NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoleProcess]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoleProcess](
	[UserId] [int] NOT NULL,
	[RoleCode] [nchar](6) NOT NULL,
	[ProcessId] [int] NOT NULL,
 CONSTRAINT [PK_UserRoleProcess] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleCode] ASC,
	[ProcessId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserTeam]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTeam](
	[TeamId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_UserTeam] PRIMARY KEY CLUSTERED 
(
	[TeamId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ValidationTraining]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ValidationTraining](
	[ValidationTrainingId] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[TrainingId] [int] NOT NULL,
	[PublishedActionId] [int] NOT NULL,
	[TrainerId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ValidationTraining] PRIMARY KEY CLUSTERED 
(
	[ValidationTrainingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Video]    Script Date: 04/04/2018 09:46:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Video](
	[VideoId] [int] IDENTITY(1,1) NOT NULL,
	[ProcessId] [int] NOT NULL,
	[DefaultResourceId] [int] NULL,
	[CameraName] [nvarchar](50) NULL,
	[NumSeq] [int] NOT NULL DEFAULT ((1)),
	[Duration] [float] NOT NULL,
	[Format] [nvarchar](100) NOT NULL,
	[FilePath] [nvarchar](255) NULL,
	[OriginalHash] [nchar](32) NULL,
	[ShootingDate] [datetime] NOT NULL,
	[Thumbnail] [varbinary](max) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[ResourceView] [int] NULL,
	[Hash] [nchar](32) NULL,
	[OnServer] [bit] NOT NULL DEFAULT ((0)), 
	[Extension] [nvarchar](20) NULL,
	[Sync] [bit] NOT NULL DEFAULT ((0)),
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_Video] PRIMARY KEY CLUSTERED 
(
	[VideoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VideoSync]    Script Date: 31/07/2018 08:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VideoSync](
	[UserId] [int] NOT NULL,
	[ProcessId] [int] NOT NULL,
	[SyncValue] [bit] NOT NULL,
 CONSTRAINT [PK_VideoSync] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[ProcessId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Action] ADD  CONSTRAINT [DF_Action_IsRandom]  DEFAULT ((0)) FOR [IsRandom]
GO
ALTER TABLE [dbo].[Action] ADD  CONSTRAINT [DF_Action_IsThumbnailSpecific]  DEFAULT ((0)) FOR [IsThumbnailSpecific]
GO
ALTER TABLE [dbo].[Action] ADD  CONSTRAINT [DF_Action_IsKeyTask]  DEFAULT ((0)) FOR [IsKeyTask]
GO
ALTER TABLE [dbo].[ActionReduced] ADD  CONSTRAINT [DF_ActionReduced_ReductionRatio]  DEFAULT ((0)) FOR [ReductionRatio]
GO
ALTER TABLE [dbo].[CutVideo] ADD  DEFAULT ((0)) FOR [MinDuration]
GO
ALTER TABLE [dbo].[Procedure] ADD  CONSTRAINT [DF_Procedure_IsSkill]  DEFAULT ((0)) FOR [IsSkill]
GO
ALTER TABLE [dbo].[Publication] ADD  DEFAULT ((0)) FOR [CriticalPathIDuration]
GO
ALTER TABLE [dbo].[Publication] ADD  CONSTRAINT [DF_Publication_MinDurationVideo]  DEFAULT ((0)) FOR [MinDurationVideo]
GO
ALTER TABLE [dbo].[Publication] ADD  CONSTRAINT [DF_Publication_IsSkill]  DEFAULT ((0)) FOR [IsSkill]
GO
ALTER TABLE [dbo].[PublicationHistory] ADD  DEFAULT (getdate()) FOR [Timestamp]
GO
ALTER TABLE [dbo].[PublishedAction] ADD  DEFAULT ((0)) FOR [IsRandom]
GO
ALTER TABLE [dbo].[PublishedAction] ADD  CONSTRAINT [DF_PublishedAction_IsKeyTask]  DEFAULT ((0)) FOR [IsKeyTask]
GO
ALTER TABLE [dbo].[PublishedResource] ADD  DEFAULT ((1)) FOR [PaceRating]
GO
ALTER TABLE [dbo].[Ref1Action] ADD  CONSTRAINT [DF_RefToolAction_Quantity]  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Ref2Action] ADD  CONSTRAINT [DF_RefConsumableAction_Quantity]  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Ref3Action] ADD  CONSTRAINT [DF_RefPlaceAction_Quantity]  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Ref4Action] ADD  CONSTRAINT [DF_RefDocumentAction_Quantity]  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Ref5Action] ADD  CONSTRAINT [DF_RefExtra1Action_Quantity]  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Ref6Action] ADD  CONSTRAINT [DF_RefExtra2Action_Quantity]  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Ref7Action] ADD  CONSTRAINT [DF_RefExtra3Action_Quantity]  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[RefResource] ADD  CONSTRAINT [DF_Resource_PaceRating]  DEFAULT ((1)) FOR [PaceRating]
GO
ALTER TABLE [dbo].[Scenario] ADD  CONSTRAINT [DF_Scenario_IsHoswnInSummary]  DEFAULT ((0)) FOR [IsShownInSummary]
GO
ALTER TABLE [dbo].[Scenario] ADD  CONSTRAINT [DF_Scenario_CriticalPathIDuration]  DEFAULT ((0)) FOR [CriticalPathIDuration]
GO
ALTER TABLE [dbo].[Solution] ADD  CONSTRAINT [DF_Solution_Approved]  DEFAULT ((1)) FOR [Approved]
GO
ALTER TABLE [dbo].[Solution] ADD  CONSTRAINT [DF_Solution_IsEmpty]  DEFAULT ((0)) FOR [IsEmpty]
GO
ALTER TABLE [dbo].[Solution] ADD  DEFAULT ((0)) FOR [P]
GO
ALTER TABLE [dbo].[Solution] ADD  DEFAULT ((0)) FOR [D]
GO
ALTER TABLE [dbo].[Solution] ADD  DEFAULT ((0)) FOR [C]
GO
ALTER TABLE [dbo].[Solution] ADD  DEFAULT ((0)) FOR [A]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_ActionCategory] FOREIGN KEY([ActionCategoryId])
REFERENCES [dbo].[RefActionCategory] ([ActionCategoryId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_ActionCategory]
GO
ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_CloudFile] FOREIGN KEY([ThumbnailHash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_CloudFile]
GO
ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_LinkedProcess] FOREIGN KEY([LinkedProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_LinkedProcess]
GO
ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_Orignal] FOREIGN KEY([OriginalActionId])
REFERENCES [dbo].[Action] ([ActionId])
GO
ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_Orignal]
GO
ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[RefResource] ([ResourceId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_Resource]
GO
ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_Scenario] FOREIGN KEY([ScenarioId])
REFERENCES [dbo].[Scenario] ([ScenarioId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_Scenario]
GO
ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_Skill] FOREIGN KEY([SkillId])
REFERENCES [dbo].[Skill] ([SkillId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_Skill]
GO
ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_User_CreatedBy]
GO
ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_Video] FOREIGN KEY([VideoId])
REFERENCES [dbo].[Video] ([VideoId])
GO
ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_Video]
GO
ALTER TABLE [dbo].[ActionPredecessorSuccessor]  WITH CHECK ADD  CONSTRAINT [FK_ActionPredecessorSuccessor_Predecessor] FOREIGN KEY([ActionPredecessorId])
REFERENCES [dbo].[Action] ([ActionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ActionPredecessorSuccessor] CHECK CONSTRAINT [FK_ActionPredecessorSuccessor_Predecessor]
GO
ALTER TABLE [dbo].[ActionPredecessorSuccessor]  WITH CHECK ADD  CONSTRAINT [FK_ActionPredecessorSuccessor_Successor] FOREIGN KEY([ActionSuccessorId])
REFERENCES [dbo].[Action] ([ActionId])
GO
ALTER TABLE [dbo].[ActionPredecessorSuccessor] CHECK CONSTRAINT [FK_ActionPredecessorSuccessor_Successor]
GO
ALTER TABLE [dbo].[ActionReduced]  WITH CHECK ADD  CONSTRAINT [FK_ActionReduced_Action] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Action] ([ActionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ActionReduced] CHECK CONSTRAINT [FK_ActionReduced_Action]
GO
ALTER TABLE [dbo].[ActionReduced]  WITH CHECK ADD  CONSTRAINT [FK_ActionReduced_ActionType] FOREIGN KEY([ActionTypeCode])
REFERENCES [dbo].[ActionType] ([ActionTypeCode])
GO
ALTER TABLE [dbo].[ActionReduced] CHECK CONSTRAINT [FK_ActionReduced_ActionType]
GO
ALTER TABLE [dbo].[ActionType]  WITH CHECK ADD  CONSTRAINT [FK_ActionType_AppResourceKey] FOREIGN KEY([LongLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[ActionType] CHECK CONSTRAINT [FK_ActionType_AppResourceKey]
GO
ALTER TABLE [dbo].[ActionType]  WITH CHECK ADD  CONSTRAINT [FK_ActionType_AppResourceKey_ShortLabel] FOREIGN KEY([ShortLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[ActionType] CHECK CONSTRAINT [FK_ActionType_AppResourceKey_ShortLabel]
GO
ALTER TABLE [dbo].[ActionValue]  WITH CHECK ADD  CONSTRAINT [FK_ActionValue_AppResourceKey_LongLabel] FOREIGN KEY([LongLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[ActionValue] CHECK CONSTRAINT [FK_ActionValue_AppResourceKey_LongLabel]
GO
ALTER TABLE [dbo].[ActionValue]  WITH CHECK ADD  CONSTRAINT [FK_ActionValue_AppResourceKey_ShortLabel] FOREIGN KEY([ShortLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[ActionValue] CHECK CONSTRAINT [FK_ActionValue_AppResourceKey_ShortLabel]
GO
ALTER TABLE [dbo].[Anomaly]  WITH CHECK ADD  CONSTRAINT [FK_Anomaly_Inspection] FOREIGN KEY([InspectionId])
REFERENCES [dbo].[Inspection] ([InspectionId])
GO
ALTER TABLE [dbo].[Anomaly] CHECK CONSTRAINT [FK_Anomaly_Inspection]
GO
ALTER TABLE [dbo].[Anomaly]  WITH CHECK ADD  CONSTRAINT [FK_Anomaly_User] FOREIGN KEY([InspectorId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Anomaly] CHECK CONSTRAINT [FK_Anomaly_User]
GO
ALTER TABLE [dbo].[AppResourceValue]  WITH CHECK ADD  CONSTRAINT [FK_AppResourceValue_AppResourceKey] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[AppResourceValue] CHECK CONSTRAINT [FK_AppResourceValue_AppResourceKey]
GO
ALTER TABLE [dbo].[AppResourceValue]  WITH CHECK ADD  CONSTRAINT [FK_AppResourceValue_Language] FOREIGN KEY([LanguageCode])
REFERENCES [dbo].[Language] ([LanguageCode])
GO
ALTER TABLE [dbo].[AppResourceValue] CHECK CONSTRAINT [FK_AppResourceValue_Language]
GO
ALTER TABLE [dbo].[AppResourceValue]  WITH CHECK ADD  CONSTRAINT [FK_AppResourceValue_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[AppResourceValue] CHECK CONSTRAINT [FK_AppResourceValue_User_CreatedBy]
GO
ALTER TABLE [dbo].[AppResourceValue]  WITH CHECK ADD  CONSTRAINT [FK_AppResourceValue_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[AppResourceValue] CHECK CONSTRAINT [FK_AppResourceValue_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Audit]  WITH CHECK ADD  CONSTRAINT [FK_Audit_Inspection] FOREIGN KEY([InspectionId])
REFERENCES [dbo].[Inspection] ([InspectionId])
GO
ALTER TABLE [dbo].[Audit] CHECK CONSTRAINT [FK_Audit_Inspection]
GO
ALTER TABLE [dbo].[Audit]  WITH CHECK ADD  CONSTRAINT [FK_Audit_Survey] FOREIGN KEY([SurveyId])
REFERENCES [dbo].[Survey] ([Id])
GO
ALTER TABLE [dbo].[Audit] CHECK CONSTRAINT [FK_Audit_Survey]
GO
ALTER TABLE [dbo].[Audit]  WITH CHECK ADD  CONSTRAINT [FK_Audit_User] FOREIGN KEY([AuditorId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Audit] CHECK CONSTRAINT [FK_Audit_User]
GO
ALTER TABLE [dbo].[AuditItem]  WITH CHECK ADD  CONSTRAINT [FK_AuditItem_Audit] FOREIGN KEY([AuditId])
REFERENCES [dbo].[Audit] ([Id])
GO
ALTER TABLE [dbo].[AuditItem] CHECK CONSTRAINT [FK_AuditItem_Audit]
GO
ALTER TABLE [dbo].[NotificationType]  WITH CHECK ADD  CONSTRAINT [FK_NotificationType_NotificationTypeSetting] FOREIGN KEY([NotificationTypeSettingId]) 
REFERENCES [dbo].[NotificationTypeSetting] ([Id])
GO
ALTER TABLE [dbo].[NotificationType] CHECK CONSTRAINT [FK_NotificationType_NotificationTypeSetting]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_NotificationType] FOREIGN KEY([NotificationTypeId]) 
REFERENCES [dbo].[NotificationType] ([Id])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_NotificationType]
GO
ALTER TABLE [dbo].[NotificationAttachment]  WITH CHECK ADD  CONSTRAINT [FK_NotificationAttachment_Notification] FOREIGN KEY([NotificationId]) 
REFERENCES [dbo].[Notification] ([NotificationId])
GO
ALTER TABLE [dbo].[NotificationAttachment] CHECK CONSTRAINT [FK_NotificationAttachment_Notification];
GO
ALTER TABLE [dbo].[Inspection]  WITH CHECK ADD CONSTRAINT [FK_Inspection_Publication] FOREIGN KEY([PublicationId])
REFERENCES [dbo].[Publication] ([PublicationId])
GO
ALTER TABLE [dbo].[InspectionSchedule]  WITH CHECK ADD  CONSTRAINT [FK_InspectionSchedule_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[InspectionSchedule] CHECK CONSTRAINT [FK_InspectionSchedule_Procedure]
GO
ALTER TABLE [dbo].[InspectionSchedule]  WITH CHECK ADD  CONSTRAINT [FK_InspectionSchedule_Timeslot] FOREIGN KEY([TimeslotId])
REFERENCES [dbo].[Timeslot] ([TimeslotId])
GO
ALTER TABLE [dbo].[InspectionSchedule] CHECK CONSTRAINT [FK_InspectionSchedule_Timeslot]
GO
ALTER TABLE [dbo].[InspectionStep]  WITH CHECK ADD  CONSTRAINT [FK_InspectionStep_Anomaly] FOREIGN KEY([AnomalyId])
REFERENCES [dbo].[Anomaly] ([Id])
GO
ALTER TABLE [dbo].[InspectionStep] CHECK CONSTRAINT [FK_InspectionStep_Anomaly]
GO
ALTER TABLE [dbo].[InspectionStep]  WITH CHECK ADD  CONSTRAINT [FK_InspectionStep_LinkedInspection] FOREIGN KEY([LinkedInspectionId])
REFERENCES [dbo].[Inspection] ([InspectionId])
GO
ALTER TABLE [dbo].[InspectionStep] CHECK CONSTRAINT [FK_InspectionStep_LinkedInspection]
GO
ALTER TABLE [dbo].[InspectionStep]  WITH CHECK ADD CONSTRAINT [FK_InspectionStep_User] FOREIGN KEY([InspectorId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[InspectionStep]  WITH CHECK ADD CONSTRAINT [FK_InspectionStep_Inspection] FOREIGN KEY([InspectionId])
REFERENCES [dbo].[Inspection] ([InspectionId])
GO
ALTER TABLE [dbo].[InspectionStep]  WITH CHECK ADD CONSTRAINT [FK_InspectionStep_PublishedAction] FOREIGN KEY([PublishedActionId])
REFERENCES [dbo].[PublishedAction] ([PublishedActionId])
GO
ALTER TABLE [dbo].[Objective]  WITH CHECK ADD  CONSTRAINT [FK_Objective_LongLabel] FOREIGN KEY([LongLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[Objective] CHECK CONSTRAINT [FK_Objective_LongLabel]
GO
ALTER TABLE [dbo].[Objective]  WITH CHECK ADD  CONSTRAINT [FK_Objective_ShortLabel] FOREIGN KEY([ShortLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[Objective] CHECK CONSTRAINT [FK_Objective_ShortLabel]
GO
ALTER TABLE [dbo].[Procedure]  WITH CHECK ADD  CONSTRAINT [FK_Procedure_ProjectDir] FOREIGN KEY([ProjectDirId])
REFERENCES [dbo].[ProjectDir] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Procedure] CHECK CONSTRAINT [FK_Procedure_ProjectDir]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Objective] FOREIGN KEY([ObjectiveCode])
REFERENCES [dbo].[Objective] ([ObjectiveCode])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Objective]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Process] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Process]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Project] FOREIGN KEY([ParentProjectId])
REFERENCES [dbo].[Project] ([ProjectId])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Project]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_User_CreatedBy]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Procedure]  WITH CHECK ADD  CONSTRAINT [FK_Procedure_Owner] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Procedure] CHECK CONSTRAINT [FK_Procedure_Owner]
GO
ALTER TABLE [dbo].[ProjectDir]  WITH CHECK ADD  CONSTRAINT [FK_ProjectDir_ProjectDir] FOREIGN KEY([ParentId])
REFERENCES [dbo].[ProjectDir] ([Id])
GO
ALTER TABLE [dbo].[ProjectDir] CHECK CONSTRAINT [FK_ProjectDir_ProjectDir]
GO
ALTER TABLE [dbo].[ProjectReferential]  WITH CHECK ADD  CONSTRAINT [FK_ProjectReferential_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([ProjectId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectReferential] CHECK CONSTRAINT [FK_ProjectReferential_Project]
GO
ALTER TABLE [dbo].[Publication]  WITH CHECK ADD  CONSTRAINT [FK_Publication_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[Publication] CHECK CONSTRAINT [FK_Publication_Procedure]
GO
ALTER TABLE [dbo].[Publication]  WITH CHECK ADD  CONSTRAINT [FK_Publication_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([ProjectId])
GO
ALTER TABLE [dbo].[Publication] CHECK CONSTRAINT [FK_Publication_Project]
GO
ALTER TABLE [dbo].[Publication]  WITH CHECK ADD  CONSTRAINT [FK_Publication_Scenario] FOREIGN KEY([ScenarioId])
REFERENCES [dbo].[Scenario] ([ScenarioId])
GO
ALTER TABLE [dbo].[Publication] CHECK CONSTRAINT [FK_Publication_Scenario]
GO
ALTER TABLE [dbo].[Publication]  WITH CHECK ADD  CONSTRAINT [FK_Publication_User] FOREIGN KEY([PublishedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Publication] CHECK CONSTRAINT [FK_Publication_User]
GO
ALTER TABLE [dbo].[PublicationHistory]  WITH CHECK ADD  CONSTRAINT [FK_PublicationHistory_EvaluationPublication] FOREIGN KEY([EvaluationDocumentationId])
REFERENCES [dbo].[Publication] ([PublicationId])
GO
ALTER TABLE [dbo].[PublicationHistory] CHECK CONSTRAINT [FK_PublicationHistory_EvaluationPublication]
GO
ALTER TABLE [dbo].[PublicationHistory]  WITH CHECK ADD  CONSTRAINT [FK_PublicationHistory_InspectionPublication] FOREIGN KEY([InspectionDocumentationId])
REFERENCES [dbo].[Publication] ([PublicationId])
GO
ALTER TABLE [dbo].[PublicationHistory] CHECK CONSTRAINT [FK_PublicationHistory_InspectionPublication]
GO
ALTER TABLE [dbo].[PublicationHistory]  WITH CHECK ADD  CONSTRAINT [FK_PublicationHistory_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PublicationHistory] CHECK CONSTRAINT [FK_PublicationHistory_Procedure]
GO
ALTER TABLE [dbo].[PublicationHistory]  WITH CHECK ADD  CONSTRAINT [FK_PublicationHistory_TrainingPublication] FOREIGN KEY([TrainingDocumentationId])
REFERENCES [dbo].[Publication] ([PublicationId])
GO
ALTER TABLE [dbo].[PublicationHistory] CHECK CONSTRAINT [FK_PublicationHistory_TrainingPublication]
GO
ALTER TABLE [dbo].[PublicationHistory]  WITH CHECK ADD  CONSTRAINT [FK_PublicationHistory_User] FOREIGN KEY([PublisherId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PublicationHistory] CHECK CONSTRAINT [FK_PublicationHistory_User]
GO
ALTER TABLE [dbo].[DocumentationReferential]  WITH CHECK ADD  CONSTRAINT [FK_DocumentationReferential_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DocumentationReferential] CHECK CONSTRAINT [FK_DocumentationReferential_Procedure]
GO
ALTER TABLE [dbo].[DocumentationDraft]  WITH CHECK ADD  CONSTRAINT [FK_DocumentationDraft_Scenario] FOREIGN KEY([ScenarioId])
REFERENCES [dbo].[Scenario] ([ScenarioId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DocumentationDraftLocalization]  WITH CHECK ADD  CONSTRAINT [FK_DocumentationDraftLocalization_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DocumentationDraftLocalization] CHECK CONSTRAINT [FK_DocumentationDraftLocalization_Procedure]
GO
ALTER TABLE [dbo].[DocumentationActionDraft]  WITH CHECK ADD  CONSTRAINT [FK_DocumentationDraft_Skill] FOREIGN KEY([SkillId])
REFERENCES [dbo].[Skill] ([SkillId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[DocumentationActionDraft]  WITH CHECK ADD  CONSTRAINT [FK_DocumentationActionDraft_RefResource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[RefResource] ([ResourceId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[DocumentationActionDraft] CHECK CONSTRAINT [FK_DocumentationActionDraft_RefResource]
GO
ALTER TABLE [dbo].[DocumentationActionDraft]  WITH CHECK ADD  CONSTRAINT [FK_DocumentationActionDraft_RefActionCategory] FOREIGN KEY([ActionCategoryId])
REFERENCES [dbo].[RefActionCategory] ([ActionCategoryId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[DocumentationActionDraft] CHECK CONSTRAINT [FK_DocumentationActionDraft_RefActionCategory]
GO
ALTER TABLE [dbo].[ReferentialDocumentationActionDraft]  WITH CHECK ADD  CONSTRAINT [FK_ReferentialDocumentationActionDraft_DocumentationActionDraft] FOREIGN KEY([DocumentationActionDraftId])
REFERENCES [dbo].[DocumentationActionDraft] ([DocumentationActionDraftId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReferentialDocumentationActionDraft] CHECK CONSTRAINT [FK_ReferentialDocumentationActionDraft_DocumentationActionDraft]
GO
ALTER TABLE [dbo].[DocumentationActionDraftWBS]  WITH CHECK ADD  CONSTRAINT [FK_DocumentationActionDraftWBS_Action] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Action] ([ActionId])
GO
ALTER TABLE [dbo].[DocumentationActionDraftWBS]  WITH CHECK ADD  CONSTRAINT [FK_DocumentationActionDraftWBS_DocumentationAction] FOREIGN KEY([DocumentationActionDraftId])
REFERENCES [dbo].[DocumentationActionDraft] ([DocumentationActionDraftId])
GO
ALTER TABLE [dbo].[DocumentationActionDraftWBS]  WITH CHECK ADD  CONSTRAINT [FK_DocumentationActionDraftWBS_DocumentationActionDraft] FOREIGN KEY([DocumentationDraftId])
REFERENCES [dbo].[DocumentationDraft] ([DocumentationDraftId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PublicationLocalization]  WITH CHECK ADD  CONSTRAINT [FK_PublicationLocalization_Publication] FOREIGN KEY([PublicationId])
REFERENCES [dbo].[Publication] ([PublicationId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PublicationLocalization] CHECK CONSTRAINT [FK_PublicationLocalization_Publication]
GO
ALTER TABLE [dbo].[PublishedAction]  WITH CHECK ADD  CONSTRAINT [FK_PublishedAction_CutVideo] FOREIGN KEY([CutVideoHash])
REFERENCES [dbo].[CutVideo] ([Hash])
GO
ALTER TABLE [dbo].[PublishedAction] CHECK CONSTRAINT [FK_PublishedAction_CutVideo]
GO
ALTER TABLE [dbo].[PublishedAction]  WITH CHECK ADD  CONSTRAINT [FK_PublishedAction_LinkedPublication] FOREIGN KEY([LinkedPublicationId])
REFERENCES [dbo].[Publication] ([PublicationId])
GO
ALTER TABLE [dbo].[PublishedAction] CHECK CONSTRAINT [FK_PublishedAction_LinkedPublication]
GO
ALTER TABLE [dbo].[PublishedAction]  WITH CHECK ADD  CONSTRAINT [FK_PublishedAction_Publication] FOREIGN KEY([PublicationId])
REFERENCES [dbo].[Publication] ([PublicationId])
GO
ALTER TABLE [dbo].[PublishedAction] CHECK CONSTRAINT [FK_PublishedAction_Publication]
GO
ALTER TABLE [dbo].[PublishedAction]  WITH CHECK ADD  CONSTRAINT [FK_PublishedAction_PublishedActionCategory] FOREIGN KEY([ActionCategoryId])
REFERENCES [dbo].[PublishedActionCategory] ([PublishedActionCategoryId])
GO
ALTER TABLE [dbo].[PublishedAction] CHECK CONSTRAINT [FK_PublishedAction_PublishedActionCategory]
GO
ALTER TABLE [dbo].[PublishedAction]  WITH CHECK ADD  CONSTRAINT [FK_PublishedAction_PublishedFile] FOREIGN KEY([ThumbnailHash])
REFERENCES [dbo].[PublishedFile] ([Hash])
GO
ALTER TABLE [dbo].[PublishedAction] CHECK CONSTRAINT [FK_PublishedAction_PublishedFile]
GO
ALTER TABLE [dbo].[PublishedAction]  WITH CHECK ADD  CONSTRAINT [FK_PublishedAction_PublishedResource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[PublishedResource] ([PublishedResourceId])
GO
ALTER TABLE [dbo].[PublishedAction] CHECK CONSTRAINT [FK_PublishedAction_PublishedResource]
GO
ALTER TABLE [dbo].[PublishedAction]  WITH CHECK ADD  CONSTRAINT [FK_PublishedAction_Skill] FOREIGN KEY([SkillId])
REFERENCES [dbo].[Skill] ([SkillId])
GO
ALTER TABLE [dbo].[PublishedAction] CHECK CONSTRAINT [FK_PublishedAction_Skill]
GO
ALTER TABLE [dbo].[PublishedActionCategory]  WITH CHECK ADD  CONSTRAINT [FK_PublishedActionCategory_ActionType] FOREIGN KEY([ActionTypeCode])
REFERENCES [dbo].[ActionType] ([ActionTypeCode])
GO
ALTER TABLE [dbo].[PublishedActionCategory] CHECK CONSTRAINT [FK_PublishedActionCategory_ActionType]
GO
ALTER TABLE [dbo].[PublishedActionCategory]  WITH CHECK ADD  CONSTRAINT [FK_PublishedActionCategory_ActionValue] FOREIGN KEY([ActionValueCode])
REFERENCES [dbo].[ActionValue] ([ActionValueCode])
GO
ALTER TABLE [dbo].[PublishedActionCategory] CHECK CONSTRAINT [FK_PublishedActionCategory_ActionValue]
GO
ALTER TABLE [dbo].[PublishedActionCategory]  WITH CHECK ADD  CONSTRAINT [FK_PublishedActionCategory_PublishedFile] FOREIGN KEY([FileHash])
REFERENCES [dbo].[PublishedFile] ([Hash])
GO
ALTER TABLE [dbo].[PublishedActionCategory] CHECK CONSTRAINT [FK_PublishedActionCategory_PublishedFile]
GO
ALTER TABLE [dbo].[PublishedActionPredecessorSuccessor]  WITH CHECK ADD  CONSTRAINT [FK_PublishedActionPredecessor_PublishedAction] FOREIGN KEY([PublishedActionPredecessorId])
REFERENCES [dbo].[PublishedAction] ([PublishedActionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PublishedActionPredecessorSuccessor] CHECK CONSTRAINT [FK_PublishedActionPredecessor_PublishedAction]
GO
ALTER TABLE [dbo].[PublishedActionPredecessorSuccessor]  WITH CHECK ADD  CONSTRAINT [FK_PublishedActionSuccessor_PublishedAction] FOREIGN KEY([PublishedActionSuccessorId])
REFERENCES [dbo].[PublishedAction] ([PublishedActionId])
GO
ALTER TABLE [dbo].[PublishedActionPredecessorSuccessor] CHECK CONSTRAINT [FK_PublishedActionSuccessor_PublishedAction]
GO
ALTER TABLE [dbo].[PublishedReferential]  WITH CHECK ADD  CONSTRAINT [FK_PublishedReferential_PublishedFile] FOREIGN KEY([FileHash])
REFERENCES [dbo].[PublishedFile] ([Hash])
GO
ALTER TABLE [dbo].[PublishedReferential] CHECK CONSTRAINT [FK_PublishedReferential_PublishedFile]
GO
ALTER TABLE [dbo].[PublishedReferentialAction]  WITH CHECK ADD  CONSTRAINT [FK_PublishedReferentialAction_PublishedAction] FOREIGN KEY([PublishedActionId])
REFERENCES [dbo].[PublishedAction] ([PublishedActionId])
GO
ALTER TABLE [dbo].[PublishedReferentialAction] CHECK CONSTRAINT [FK_PublishedReferentialAction_PublishedAction]
GO
ALTER TABLE [dbo].[PublishedReferentialAction]  WITH CHECK ADD  CONSTRAINT [FK_PublishedReferentialAction_PublishedReferential] FOREIGN KEY([PublishedReferentialId])
REFERENCES [dbo].[PublishedReferential] ([PublishedReferentialId])
GO
ALTER TABLE [dbo].[PublishedReferentialAction] CHECK CONSTRAINT [FK_PublishedReferentialAction_PublishedReferential]
GO
ALTER TABLE [dbo].[PublishedResource]  WITH CHECK ADD  CONSTRAINT [FK_PublishedResource_PublishedFile] FOREIGN KEY([FileHash])
REFERENCES [dbo].[PublishedFile] ([Hash])
GO
ALTER TABLE [dbo].[PublishedResource] CHECK CONSTRAINT [FK_PublishedResource_PublishedFile]
GO
ALTER TABLE [dbo].[Qualification]  WITH CHECK ADD  CONSTRAINT [FK_QUALIFICATION_PUBLICATION] FOREIGN KEY([PublicationId])
REFERENCES [dbo].[Publication] ([PublicationId])
GO
ALTER TABLE [dbo].[Qualification] CHECK CONSTRAINT [FK_QUALIFICATION_PUBLICATION]
GO
ALTER TABLE [dbo].[Qualification]  WITH CHECK ADD  CONSTRAINT [FK_QUALIFICATION_USER] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Qualification] CHECK CONSTRAINT [FK_QUALIFICATION_USER]
GO
ALTER TABLE [dbo].[Qualification]  WITH CHECK ADD  CONSTRAINT [FK_Qualification_PreviousVersionQualification] FOREIGN KEY([PreviousVersionQualificationId])
REFERENCES [dbo].[Qualification] ([QualificationId])
GO
ALTER TABLE [dbo].[Qualification] CHECK CONSTRAINT [FK_Qualification_PreviousVersionQualification]
GO
ALTER TABLE [dbo].[QualificationReason] ADD  DEFAULT ((0)) FOR [IsEditable]
GO
ALTER TABLE [dbo].[QualificationStep]  WITH CHECK ADD  CONSTRAINT [FK_QualificationStep_QualificationReason] FOREIGN KEY([QualificationReasonId])
REFERENCES [dbo].[QualificationReason] ([Id])
GO
ALTER TABLE [dbo].[QualificationStep] CHECK CONSTRAINT [FK_QualificationStep_QualificationReason]
GO
CREATE INDEX [IX_FK_QualificationStep_QualificationReason] ON [dbo].[QualificationStep] ([QualificationReasonId]);
GO
ALTER TABLE [dbo].[QualificationStep]  WITH CHECK ADD  CONSTRAINT [FK_QUALIFICATION_STEP_PUBLISHED_ACTION] FOREIGN KEY([PublishedActionId])
REFERENCES [dbo].[PublishedAction] ([PublishedActionId])
GO
ALTER TABLE [dbo].[QualificationStep] CHECK CONSTRAINT [FK_QUALIFICATION_STEP_PUBLISHED_ACTION]
GO
ALTER TABLE [dbo].[QualificationStep]  WITH CHECK ADD  CONSTRAINT [FK_QUALIFICATION_STEP_QUALIFICATION] FOREIGN KEY([QualificationId])
REFERENCES [dbo].[Qualification] ([QualificationId])
GO
ALTER TABLE [dbo].[QualificationStep] CHECK CONSTRAINT [FK_QUALIFICATION_STEP_QUALIFICATION]
GO
ALTER TABLE [dbo].[QualificationStep]  WITH CHECK ADD  CONSTRAINT [FK_QUALIFICATION_STEP_QUALIFIER] FOREIGN KEY([QualifierId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[QualificationStep] CHECK CONSTRAINT [FK_QUALIFICATION_STEP_QUALIFIER]
GO
ALTER TABLE [dbo].[Ref1]  WITH CHECK ADD  CONSTRAINT [FK_Ref1_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[Ref1] CHECK CONSTRAINT [FK_Ref1_Procedure]
GO
ALTER TABLE [dbo].[Ref1]  WITH CHECK ADD  CONSTRAINT [FK_Tool_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref1] CHECK CONSTRAINT [FK_Tool_User_CreatedBy]
GO
ALTER TABLE [dbo].[Ref1]  WITH CHECK ADD  CONSTRAINT [FK_Tool_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref1] CHECK CONSTRAINT [FK_Tool_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Ref1Action]  WITH CHECK ADD  CONSTRAINT [FK_RefToolAction_Action] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Action] ([ActionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref1Action] CHECK CONSTRAINT [FK_RefToolAction_Action]
GO
ALTER TABLE [dbo].[Ref1Action]  WITH CHECK ADD  CONSTRAINT [FK_RefToolAction_RefTool] FOREIGN KEY([RefId])
REFERENCES [dbo].[Ref1] ([RefId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref1Action] CHECK CONSTRAINT [FK_RefToolAction_RefTool]
GO
ALTER TABLE [dbo].[Ref2]  WITH CHECK ADD  CONSTRAINT [FK_Consumable_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref2] CHECK CONSTRAINT [FK_Consumable_User_CreatedBy]
GO
ALTER TABLE [dbo].[Ref2]  WITH CHECK ADD  CONSTRAINT [FK_Consumable_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref2] CHECK CONSTRAINT [FK_Consumable_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Ref2]  WITH CHECK ADD  CONSTRAINT [FK_Ref2_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[Ref2] CHECK CONSTRAINT [FK_Ref2_Procedure]
GO
ALTER TABLE [dbo].[Ref2Action]  WITH CHECK ADD  CONSTRAINT [FK_RefConsumableAction_Action] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Action] ([ActionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref2Action] CHECK CONSTRAINT [FK_RefConsumableAction_Action]
GO
ALTER TABLE [dbo].[Ref2Action]  WITH CHECK ADD  CONSTRAINT [FK_RefConsumableAction_RefConsumable] FOREIGN KEY([RefId])
REFERENCES [dbo].[Ref2] ([RefId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref2Action] CHECK CONSTRAINT [FK_RefConsumableAction_RefConsumable]
GO
ALTER TABLE [dbo].[Ref3]  WITH CHECK ADD  CONSTRAINT [FK_Place_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref3] CHECK CONSTRAINT [FK_Place_User_CreatedBy]
GO
ALTER TABLE [dbo].[Ref3]  WITH CHECK ADD  CONSTRAINT [FK_Place_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref3] CHECK CONSTRAINT [FK_Place_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Ref3]  WITH CHECK ADD  CONSTRAINT [FK_Ref3_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[Ref3] CHECK CONSTRAINT [FK_Ref3_Procedure]
GO
ALTER TABLE [dbo].[Ref3Action]  WITH CHECK ADD  CONSTRAINT [FK_RefPlaceAction_Action] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Action] ([ActionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref3Action] CHECK CONSTRAINT [FK_RefPlaceAction_Action]
GO
ALTER TABLE [dbo].[Ref3Action]  WITH CHECK ADD  CONSTRAINT [FK_RefPlaceAction_RefPlace] FOREIGN KEY([RefId])
REFERENCES [dbo].[Ref3] ([RefId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref3Action] CHECK CONSTRAINT [FK_RefPlaceAction_RefPlace]
GO
ALTER TABLE [dbo].[Ref4]  WITH CHECK ADD  CONSTRAINT [FK_Document_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref4] CHECK CONSTRAINT [FK_Document_User_CreatedBy]
GO
ALTER TABLE [dbo].[Ref4]  WITH CHECK ADD  CONSTRAINT [FK_Document_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref4] CHECK CONSTRAINT [FK_Document_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Ref4]  WITH CHECK ADD  CONSTRAINT [FK_Ref4_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[Ref4] CHECK CONSTRAINT [FK_Ref4_Procedure]
GO
ALTER TABLE [dbo].[Ref4Action]  WITH CHECK ADD  CONSTRAINT [FK_RefDocumentAction_Action] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Action] ([ActionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref4Action] CHECK CONSTRAINT [FK_RefDocumentAction_Action]
GO
ALTER TABLE [dbo].[Ref4Action]  WITH CHECK ADD  CONSTRAINT [FK_RefDocumentAction_RefDocument] FOREIGN KEY([RefId])
REFERENCES [dbo].[Ref4] ([RefId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref4Action] CHECK CONSTRAINT [FK_RefDocumentAction_RefDocument]
GO
ALTER TABLE [dbo].[Ref5]  WITH CHECK ADD  CONSTRAINT [FK_Ref5_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[Ref5] CHECK CONSTRAINT [FK_Ref5_Procedure]
GO
ALTER TABLE [dbo].[Ref5]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra1_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref5] CHECK CONSTRAINT [FK_RefExtra1_User_CreatedBy]
GO
ALTER TABLE [dbo].[Ref5]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra1_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref5] CHECK CONSTRAINT [FK_RefExtra1_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Ref5Action]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra1Action_Action] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Action] ([ActionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref5Action] CHECK CONSTRAINT [FK_RefExtra1Action_Action]
GO
ALTER TABLE [dbo].[Ref5Action]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra1Action_RefExtra1] FOREIGN KEY([RefId])
REFERENCES [dbo].[Ref5] ([RefId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref5Action] CHECK CONSTRAINT [FK_RefExtra1Action_RefExtra1]
GO
ALTER TABLE [dbo].[Ref6]  WITH CHECK ADD  CONSTRAINT [FK_Ref6_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[Ref6] CHECK CONSTRAINT [FK_Ref6_Procedure]
GO
ALTER TABLE [dbo].[Ref6]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra2_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref6] CHECK CONSTRAINT [FK_RefExtra2_User_CreatedBy]
GO
ALTER TABLE [dbo].[Ref6]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra2_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref6] CHECK CONSTRAINT [FK_RefExtra2_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Ref6Action]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra2Action_Action] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Action] ([ActionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref6Action] CHECK CONSTRAINT [FK_RefExtra2Action_Action]
GO
ALTER TABLE [dbo].[Ref6Action]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra2Action_RefExtra2] FOREIGN KEY([RefId])
REFERENCES [dbo].[Ref6] ([RefId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref6Action] CHECK CONSTRAINT [FK_RefExtra2Action_RefExtra2]
GO
ALTER TABLE [dbo].[Ref7]  WITH CHECK ADD  CONSTRAINT [FK_Ref7_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[Ref7] CHECK CONSTRAINT [FK_Ref7_Procedure]
GO
ALTER TABLE [dbo].[Ref7]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra3_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref7] CHECK CONSTRAINT [FK_RefExtra3_User_CreatedBy]
GO
ALTER TABLE [dbo].[Ref7]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra3_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Ref7] CHECK CONSTRAINT [FK_RefExtra3_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Ref7Action]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra3Action_Action] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Action] ([ActionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref7Action] CHECK CONSTRAINT [FK_RefExtra3Action_Action]
GO
ALTER TABLE [dbo].[Ref7Action]  WITH CHECK ADD  CONSTRAINT [FK_RefExtra3Action_RefExtra3] FOREIGN KEY([RefId])
REFERENCES [dbo].[Ref7] ([RefId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ref7Action] CHECK CONSTRAINT [FK_RefExtra3Action_RefExtra3]
GO
ALTER TABLE [dbo].[RefActionCategory]  WITH CHECK ADD  CONSTRAINT [FK_ActionCategory_ActionType] FOREIGN KEY([ActionTypeCode])
REFERENCES [dbo].[ActionType] ([ActionTypeCode])
GO
ALTER TABLE [dbo].[RefActionCategory] CHECK CONSTRAINT [FK_ActionCategory_ActionType]
GO
ALTER TABLE [dbo].[RefActionCategory]  WITH CHECK ADD  CONSTRAINT [FK_ActionCategory_ActionValue] FOREIGN KEY([ActionValueCode])
REFERENCES [dbo].[ActionValue] ([ActionValueCode])
GO
ALTER TABLE [dbo].[RefActionCategory] CHECK CONSTRAINT [FK_ActionCategory_ActionValue]
GO
ALTER TABLE [dbo].[RefActionCategory]  WITH CHECK ADD  CONSTRAINT [FK_ActionCategory_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[RefActionCategory] CHECK CONSTRAINT [FK_ActionCategory_User_CreatedBy]
GO
ALTER TABLE [dbo].[RefActionCategory]  WITH CHECK ADD  CONSTRAINT [FK_ActionCategory_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[RefActionCategory] CHECK CONSTRAINT [FK_ActionCategory_User_ModifiedBy]
GO
ALTER TABLE [dbo].[RefActionCategory]  WITH CHECK ADD  CONSTRAINT [FK_RefActionCategory_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[RefActionCategory] CHECK CONSTRAINT [FK_RefActionCategory_Procedure]
GO
ALTER TABLE [dbo].[RefEquipment]  WITH CHECK ADD  CONSTRAINT [FK_Equipment_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[RefResource] ([ResourceId])
GO
ALTER TABLE [dbo].[RefEquipment] CHECK CONSTRAINT [FK_Equipment_Resource]
GO
ALTER TABLE [dbo].[RefEquipment]  WITH CHECK ADD  CONSTRAINT [FK_RefEquipment_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[RefEquipment] CHECK CONSTRAINT [FK_RefEquipment_Procedure]
GO
ALTER TABLE [dbo].[RefOperator]  WITH CHECK ADD  CONSTRAINT [FK_Operator_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[RefResource] ([ResourceId])
GO
ALTER TABLE [dbo].[RefOperator] CHECK CONSTRAINT [FK_Operator_Resource]
GO
ALTER TABLE [dbo].[RefOperator]  WITH CHECK ADD  CONSTRAINT [FK_RefOperator_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[RefOperator] CHECK CONSTRAINT [FK_RefOperator_Procedure]
GO
ALTER TABLE [dbo].[RefResource]  WITH CHECK ADD  CONSTRAINT [FK_RefResource_CloudFile] FOREIGN KEY([Hash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[RefResource] CHECK CONSTRAINT [FK_RefResource_CloudFile]
GO
ALTER TABLE [dbo].[RefResource]  WITH CHECK ADD  CONSTRAINT [FK_Resource_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[RefResource] CHECK CONSTRAINT [FK_Resource_User_CreatedBy]
GO
ALTER TABLE [dbo].[RefResource]  WITH CHECK ADD  CONSTRAINT [FK_Resource_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[RefResource] CHECK CONSTRAINT [FK_Resource_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Ref1]  WITH CHECK ADD  CONSTRAINT [FK_Ref1_CloudFile] FOREIGN KEY([Hash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Ref1] CHECK CONSTRAINT [FK_Ref1_CloudFile]
GO
ALTER TABLE [dbo].[Ref2]  WITH CHECK ADD  CONSTRAINT [FK_Ref2_CloudFile] FOREIGN KEY([Hash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Ref2] CHECK CONSTRAINT [FK_Ref2_CloudFile]
GO
ALTER TABLE [dbo].[Ref3]  WITH CHECK ADD  CONSTRAINT [FK_Ref3_CloudFile] FOREIGN KEY([Hash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Ref3] CHECK CONSTRAINT [FK_Ref3_CloudFile]
GO
ALTER TABLE [dbo].[Ref4]  WITH CHECK ADD  CONSTRAINT [FK_Ref4_CloudFile] FOREIGN KEY([Hash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Ref4] CHECK CONSTRAINT [FK_Ref4_CloudFile]
GO
ALTER TABLE [dbo].[Ref5]  WITH CHECK ADD  CONSTRAINT [FK_Ref5_CloudFile] FOREIGN KEY([Hash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Ref5] CHECK CONSTRAINT [FK_Ref5_CloudFile]
GO
ALTER TABLE [dbo].[Ref6]  WITH CHECK ADD  CONSTRAINT [FK_Ref6_CloudFile] FOREIGN KEY([Hash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Ref6] CHECK CONSTRAINT [FK_Ref6_CloudFile]
GO
ALTER TABLE [dbo].[Ref7]  WITH CHECK ADD  CONSTRAINT [FK_Ref7_CloudFile] FOREIGN KEY([Hash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Ref7] CHECK CONSTRAINT [FK_Ref7_CloudFile]
GO
ALTER TABLE [dbo].[RefActionCategory]  WITH CHECK ADD  CONSTRAINT [FK_RefActionCategory_CloudFile] FOREIGN KEY([Hash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[RefActionCategory] CHECK CONSTRAINT [FK_RefActionCategory_CloudFile]
GO
ALTER TABLE [dbo].[Skill] ADD [Hash] NCHAR(32) NULL;
GO
ALTER TABLE [dbo].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_Skill_CloudFile] FOREIGN KEY([Hash])
REFERENCES [dbo].[CloudFile] ([Hash])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Skill] CHECK CONSTRAINT [FK_Skill_CloudFile]
GO
ALTER TABLE [dbo].[Role]  WITH CHECK ADD  CONSTRAINT [FK_Role_AppResourceKey_LongLabel] FOREIGN KEY([LongLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[Role] CHECK CONSTRAINT [FK_Role_AppResourceKey_LongLabel]
GO
ALTER TABLE [dbo].[Role]  WITH CHECK ADD  CONSTRAINT [FK_Role_AppResourceKey_ShortLabel] FOREIGN KEY([ShortLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[Role] CHECK CONSTRAINT [FK_Role_AppResourceKey_ShortLabel]
GO
ALTER TABLE [dbo].[Scenario]  WITH CHECK ADD  CONSTRAINT [FK_Scenario_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([ProjectId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Scenario] CHECK CONSTRAINT [FK_Scenario_Project]
GO
ALTER TABLE [dbo].[Scenario]  WITH CHECK ADD  CONSTRAINT [FK_Scenario_Scenario_Original] FOREIGN KEY([OriginalScenarioId])
REFERENCES [dbo].[Scenario] ([ScenarioId])
GO
ALTER TABLE [dbo].[Scenario] CHECK CONSTRAINT [FK_Scenario_Scenario_Original]
GO
ALTER TABLE [dbo].[Scenario]  WITH CHECK ADD  CONSTRAINT [FK_Scenario_ScenarioNature] FOREIGN KEY([ScenarioNatureCode])
REFERENCES [dbo].[ScenarioNature] ([ScenarioNatureCode])
GO
ALTER TABLE [dbo].[Scenario] CHECK CONSTRAINT [FK_Scenario_ScenarioNature]
GO
ALTER TABLE [dbo].[Scenario]  WITH CHECK ADD  CONSTRAINT [FK_Scenario_State] FOREIGN KEY([ScenarioStateCode])
REFERENCES [dbo].[ScenarioState] ([ScenarioStateCode])
GO
ALTER TABLE [dbo].[Scenario] CHECK CONSTRAINT [FK_Scenario_State]
GO
ALTER TABLE [dbo].[Scenario]  WITH CHECK ADD  CONSTRAINT [FK_Scenario_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Scenario] CHECK CONSTRAINT [FK_Scenario_User_CreatedBy]
GO
ALTER TABLE [dbo].[Scenario]  WITH CHECK ADD  CONSTRAINT [FK_Scenario_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Scenario] CHECK CONSTRAINT [FK_Scenario_User_ModifiedBy]
GO
ALTER TABLE [dbo].[ScenarioNature]  WITH CHECK ADD  CONSTRAINT [FK_ScenarioNature_AppResourceKey_LongLabel] FOREIGN KEY([LongLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[ScenarioNature] CHECK CONSTRAINT [FK_ScenarioNature_AppResourceKey_LongLabel]
GO
ALTER TABLE [dbo].[ScenarioNature]  WITH CHECK ADD  CONSTRAINT [FK_ScenarioNature_AppResourceKey_ShortLabel] FOREIGN KEY([ShortLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[ScenarioNature] CHECK CONSTRAINT [FK_ScenarioNature_AppResourceKey_ShortLabel]
GO
ALTER TABLE [dbo].[ScenarioState]  WITH CHECK ADD  CONSTRAINT [FK_State_AppResourceKey_LongLabel] FOREIGN KEY([LongLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[ScenarioState] CHECK CONSTRAINT [FK_State_AppResourceKey_LongLabel]
GO
ALTER TABLE [dbo].[ScenarioState]  WITH CHECK ADD  CONSTRAINT [FK_State_AppResourceKey_ShortLabel] FOREIGN KEY([ShortLabelResourceId])
REFERENCES [dbo].[AppResourceKey] ([ResourceId])
GO
ALTER TABLE [dbo].[ScenarioState] CHECK CONSTRAINT [FK_State_AppResourceKey_ShortLabel]
GO
ALTER TABLE [dbo].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_Skill_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Skill] CHECK CONSTRAINT [FK_Skill_User_CreatedBy]
GO
ALTER TABLE [dbo].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_Skill_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Skill] CHECK CONSTRAINT [FK_Skill_User_ModifiedBy]
GO
ALTER TABLE [dbo].[Solution]  WITH CHECK ADD  CONSTRAINT [FK_Solution_Scenario] FOREIGN KEY([ScenarioId])
REFERENCES [dbo].[Scenario] ([ScenarioId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Solution] CHECK CONSTRAINT [FK_Solution_Scenario]
GO
ALTER TABLE [dbo].[SurveyItem]  WITH CHECK ADD  CONSTRAINT [FK_SurveyItem_Survey] FOREIGN KEY([SurveyId])
REFERENCES [dbo].[Survey] ([Id])
GO
ALTER TABLE [dbo].[SurveyItem] CHECK CONSTRAINT [FK_SurveyItem_Survey]
GO
ALTER TABLE [dbo].[Training]  WITH CHECK ADD  CONSTRAINT [FK_TRAINING_PUBLICATION] FOREIGN KEY([PublicationId])
REFERENCES [dbo].[Publication] ([PublicationId])
GO
ALTER TABLE [dbo].[Training] CHECK CONSTRAINT [FK_TRAINING_PUBLICATION]
GO
ALTER TABLE [dbo].[Training]  WITH CHECK ADD  CONSTRAINT [FK_TRAINING_USER] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Training] CHECK CONSTRAINT [FK_TRAINING_USER]
GO
ALTER TABLE [dbo].[Training]  WITH CHECK ADD  CONSTRAINT [FK_Training_PreviousVersionTraining] FOREIGN KEY([PreviousVersionTrainingId])
REFERENCES [dbo].[Training] ([TrainingId])
GO
ALTER TABLE [dbo].[Training] CHECK CONSTRAINT [FK_Training_PreviousVersionTraining]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Language] FOREIGN KEY([DefaultLanguageCode])
REFERENCES [dbo].[Language] ([LanguageCode])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Language]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_User_CreatedBy]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_User_ModifiedBy]
GO
ALTER TABLE [dbo].[UserReadPublication]  WITH CHECK ADD  CONSTRAINT [FK_UserReadPublication_Publication] FOREIGN KEY([PublicationId])
REFERENCES [dbo].[Publication] ([PublicationId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserReadPublication] CHECK CONSTRAINT [FK_UserReadPublication_Publication]
GO
ALTER TABLE [dbo].[UserReadPublication]  WITH CHECK ADD  CONSTRAINT [FK_UserReadPublication_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserReadPublication] CHECK CONSTRAINT [FK_UserReadPublication_User]
GO
ALTER TABLE [dbo].[UserReadPublication]  WITH CHECK ADD  CONSTRAINT [FK_UserReadPublication_PreviousVersionPublication] FOREIGN KEY([PreviousVersionPublicationId])
REFERENCES [dbo].[Publication] ([PublicationId])
GO
ALTER TABLE [dbo].[UserReadPublication] CHECK CONSTRAINT [FK_UserReadPublication_PreviousVersionPublication]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([RoleCode])
REFERENCES [dbo].[Role] ([RoleCode])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO
ALTER TABLE [dbo].[UserRoleProcess]  WITH CHECK ADD  CONSTRAINT [FK_UserRoleProcess_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoleProcess] CHECK CONSTRAINT [FK_UserRoleProcess_Procedure]
GO
ALTER TABLE [dbo].[UserRoleProcess]  WITH CHECK ADD  CONSTRAINT [FK_UserRoleProcess_Role] FOREIGN KEY([RoleCode])
REFERENCES [dbo].[Role] ([RoleCode])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoleProcess] CHECK CONSTRAINT [FK_UserRoleProcess_Role]
GO
ALTER TABLE [dbo].[UserRoleProcess]  WITH CHECK ADD  CONSTRAINT [FK_UserRoleProcess_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoleProcess] CHECK CONSTRAINT [FK_UserRoleProcess_User]
GO
ALTER TABLE [dbo].[UserTeam]  WITH CHECK ADD  CONSTRAINT [FK_UserTeam_Team] FOREIGN KEY([TeamId])
REFERENCES [dbo].[Team] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserTeam] CHECK CONSTRAINT [FK_UserTeam_Team]
GO
ALTER TABLE [dbo].[UserTeam]  WITH CHECK ADD  CONSTRAINT [FK_UserTeam_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserTeam] CHECK CONSTRAINT [FK_UserTeam_User]
GO
ALTER TABLE [dbo].[ValidationTraining]  WITH CHECK ADD  CONSTRAINT [FK_VALIDATION_TRAINING_PUBLISHED_ACTION] FOREIGN KEY([PublishedActionId])
REFERENCES [dbo].[PublishedAction] ([PublishedActionId])
GO
ALTER TABLE [dbo].[ValidationTraining] CHECK CONSTRAINT [FK_VALIDATION_TRAINING_PUBLISHED_ACTION]
GO
ALTER TABLE [dbo].[ValidationTraining]  WITH CHECK ADD  CONSTRAINT [FK_VALIDATION_TRAINING_TRAINER] FOREIGN KEY([TrainerId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[ValidationTraining] CHECK CONSTRAINT [FK_VALIDATION_TRAINING_TRAINER]
GO
ALTER TABLE [dbo].[ValidationTraining]  WITH CHECK ADD  CONSTRAINT [FK_VALIDATION_TRAINING_TRAINING] FOREIGN KEY([TrainingId])
REFERENCES [dbo].[Training] ([TrainingId])
GO
ALTER TABLE [dbo].[ValidationTraining] CHECK CONSTRAINT [FK_VALIDATION_TRAINING_TRAINING]
GO
ALTER TABLE [dbo].[Video]  WITH CHECK ADD  CONSTRAINT [FK_Video_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
GO
ALTER TABLE [dbo].[Video]  WITH CHECK ADD  CONSTRAINT [FK_Video_Resource] FOREIGN KEY([DefaultResourceId])
REFERENCES [dbo].[RefResource] ([ResourceId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Video] CHECK CONSTRAINT [FK_Video_Resource]
GO
ALTER TABLE [dbo].[Video]  WITH CHECK ADD  CONSTRAINT [FK_Video_User_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Video] CHECK CONSTRAINT [FK_Video_User_CreatedBy]
GO
ALTER TABLE [dbo].[Video]  WITH CHECK ADD  CONSTRAINT [FK_Video_User_ModifiedBy] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Video] CHECK CONSTRAINT [FK_Video_User_ModifiedBy]
GO
ALTER TABLE [dbo].[VideoSync] ADD  DEFAULT ((0)) FOR [SyncValue]
GO
ALTER TABLE [dbo].[VideoSync]  WITH CHECK ADD  CONSTRAINT [FK_VideoSync_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[VideoSync] CHECK CONSTRAINT [FK_VideoSync_User]
GO
ALTER TABLE [dbo].[VideoSync]  WITH CHECK ADD  CONSTRAINT [FK_VideoSync_Procedure] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Procedure] ([ProcessId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[VideoSync] CHECK CONSTRAINT [FK_VideoSync_Procedure]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Représentente une action d''une activité du processus (activité,tache, action,sous-action)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Action', @level2type=N'COLUMN',@level2name=N'ActionId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Une action réduite' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ActionReduced'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Le coût' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Solution', @level2type=N'COLUMN',@level2name=N'Cost'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'La difficulté' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Solution', @level2type=N'COLUMN',@level2name=N'Difficulty'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'L''investissement' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Solution', @level2type=N'COLUMN',@level2name=N'Investment'
GO
