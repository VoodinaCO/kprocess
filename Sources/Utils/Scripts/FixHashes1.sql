UPDATE [dbo].[Video]
SET [Hash] = REPLACE([Hash],'-','')
FROM [dbo].[Video]
WHERE [Hash] IS NOT NULL;
GO
ALTER TABLE [dbo].[Video] ALTER COLUMN [Hash] NCHAR(32) NULL;
GO

IF EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_PublishedAction_PublishedFile')
	BEGIN
		ALTER TABLE dbo.PublishedAction DROP CONSTRAINT FK_PublishedAction_PublishedFile;
	END
GO
IF EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_PublishedAction_CutVideo')
	BEGIN
		ALTER TABLE dbo.PublishedAction DROP CONSTRAINT FK_PublishedAction_CutVideo;
	END
GO
IF EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_PublishedActionCategory_PublishedFile')
	BEGIN
		ALTER TABLE dbo.PublishedActionCategory DROP CONSTRAINT FK_PublishedActionCategory_PublishedFile;
	END
GO
IF EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_PublishedReferential_PublishedFile')
	BEGIN
		ALTER TABLE dbo.PublishedReferential DROP CONSTRAINT FK_PublishedReferential_PublishedFile;
	END
GO
IF EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_PublishedResource_PublishedFile')
	BEGIN
		ALTER TABLE dbo.PublishedResource DROP CONSTRAINT FK_PublishedResource_PublishedFile;
	END
GO

UPDATE [dbo].[PublishedFile] SET [Hash] = REPLACE([Hash],'-','') FROM [dbo].[PublishedFile];
GO
UPDATE [dbo].[CutVideo] SET [Hash] = REPLACE([Hash],'-','') FROM [dbo].[CutVideo];
GO
UPDATE [dbo].[CutVideo] SET [HashOriginalVideo] = REPLACE([HashOriginalVideo],'-','') FROM [dbo].[CutVideo];
GO

UPDATE [dbo].[PublishedAction] SET [ThumbnailHash] = REPLACE([ThumbnailHash],'-','') FROM [dbo].[PublishedAction] WHERE [ThumbnailHash] IS NOT NULL;
GO
UPDATE [dbo].[PublishedAction] SET [CutVideoHash] = REPLACE([CutVideoHash],'-','') FROM [dbo].[PublishedAction] WHERE [CutVideoHash] IS NOT NULL;
GO
UPDATE [dbo].[PublishedActionCategory] SET [FileHash] = REPLACE([FileHash],'-','') FROM [dbo].[PublishedActionCategory] WHERE [FileHash] IS NOT NULL;
GO
UPDATE [dbo].[PublishedReferential] SET [FileHash] = REPLACE([FileHash],'-','') FROM [dbo].[PublishedReferential] WHERE [FileHash] IS NOT NULL;
GO
UPDATE [dbo].[PublishedResource] SET [FileHash] = REPLACE([FileHash],'-','') FROM [dbo].[PublishedResource] WHERE [FileHash] IS NOT NULL;
GO