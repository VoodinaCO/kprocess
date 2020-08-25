IF NOT EXISTS(SELECT * FROM sys.tables WHERE NAME = 'NotificationTypeSetting')
    BEGIN
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
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
    END
GO

IF NOT EXISTS(SELECT * FROM sys.tables WHERE NAME = 'NotificationType')
    BEGIN
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
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

		ALTER TABLE [dbo].[NotificationType]  WITH CHECK ADD  CONSTRAINT [FK_NotificationType_NotificationTypeSetting] FOREIGN KEY([NotificationTypeSettingId]) REFERENCES [dbo].[NotificationTypeSetting] ([Id]);
		ALTER TABLE [dbo].[NotificationType] CHECK CONSTRAINT [FK_NotificationType_NotificationTypeSetting];
    END
GO

IF NOT EXISTS(SELECT * FROM sys.tables WHERE NAME = 'Notification')
    BEGIN
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
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

		ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_NotificationType] FOREIGN KEY([NotificationTypeId]) REFERENCES [dbo].[NotificationType] ([Id]);
		ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_NotificationType];
    END
GO

IF NOT EXISTS(SELECT * FROM sys.tables WHERE NAME = 'NotificationAttachment')
    BEGIN
        CREATE TABLE [dbo].[NotificationAttachment](
			[NotificationAttachmentId] [int] IDENTITY(1,1) NOT NULL,
			[NotificationId] [int] NOT NULL,
			[Name] [nvarchar](100) NULL,
			[Attachment] [varbinary](max) NULL
		PRIMARY KEY CLUSTERED 
		(
			[NotificationAttachmentId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

		ALTER TABLE [dbo].[NotificationAttachment]  WITH CHECK ADD  CONSTRAINT [FK_NotificationAttachment_Notification] FOREIGN KEY([NotificationId]) REFERENCES [dbo].[Notification] ([NotificationId]);
		ALTER TABLE [dbo].[NotificationAttachment] CHECK CONSTRAINT [FK_NotificationAttachment_Notification];
    END
GO