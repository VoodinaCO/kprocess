ALTER TABLE [dbo].[Video] ALTER COLUMN [FilePath] NVARCHAR(255);
GO
EXEC AddColumnIfNotExists 'Video', 'ProcessId', '[INT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Video', 'CameraName', '[NVARCHAR](50) NULL';
GO
EXEC AddColumnIfNotExists 'Video', 'ResourceView', '[INT] NULL';
GO
EXEC AddColumnIfNotExists 'Video', 'OriginalHash', '[NCHAR](32) NULL';
GO
EXEC AddColumnIfNotExists 'Video', 'Hash', '[NCHAR](32) NULL';
GO
EXEC AddColumnIfNotExists 'Video', 'OnServer', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Video', 'Extension', '[NVARCHAR](20) NULL';
GO
EXEC AddColumnIfNotExists 'Video', 'Sync', '[BIT] NOT NULL DEFAULT((0))';
GO

IF EXISTS(SELECT * FROM sys.tables t INNER JOIN sys.columns c ON t.[object_id] = c.[object_id] WHERE t.[name] = 'Video' AND c.[name] = 'ProjectId')
	BEGIN	
		UPDATE [dbo].[Video]
			SET ProcessId = p.ProcessId
			FROM [dbo].[Video] v
			INNER JOIN [dbo].[Project] p ON p.ProjectId = v.ProjectId;
		ALTER TABLE [dbo].[Video] DROP CONSTRAINT [FK_Video_Project];
		ALTER TABLE [dbo].[Video] DROP COLUMN [ProjectId];
		ALTER TABLE [dbo].[Video]  WITH CHECK ADD CONSTRAINT [FK_Video_Procedure] FOREIGN KEY([ProcessId]) 
			REFERENCES [dbo].[Procedure] ([ProcessId]);
	END
GO

IF EXISTS(SELECT * FROM sys.tables t INNER JOIN sys.columns c ON t.[object_id] = c.[object_id] WHERE t.[name] = 'Video' AND c.[name] = 'Name')
	BEGIN	
		UPDATE [dbo].[Video]
			SET CameraName = v.Name
			FROM [dbo].[Video] v;
		ALTER TABLE [dbo].[Video] DROP COLUMN [Name];
	END
GO

IF EXISTS(SELECT * FROM sys.tables t INNER JOIN sys.columns c ON t.[object_id] = c.[object_id] WHERE t.[name] = 'Video' AND c.[name] = 'Description')
	BEGIN	
		ALTER TABLE [dbo].[Video] DROP COLUMN [Description];
	END
GO

IF EXISTS(SELECT * FROM sys.tables WHERE [name] = 'VideoNature')
	BEGIN
		ALTER TABLE [dbo].[Video] DROP CONSTRAINT [FK_Video_Nature];
		ALTER TABLE [dbo].[VideoNature] DROP CONSTRAINT [FK_VideoNature_AppResourceKey_LongLabel];
		ALTER TABLE [dbo].[VideoNature] DROP CONSTRAINT [FK_VideoNature_AppResourceKey_ShortLabel];
		ALTER TABLE [dbo].[Video] DROP COLUMN [VideoNatureCode];
		DROP TABLE [dbo].[VideoNature];
	END
GO