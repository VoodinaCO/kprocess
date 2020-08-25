ALTER TABLE [dbo].[CutVideo] ALTER COLUMN [HashOriginalVideo] NCHAR(32) NOT NULL;
GO

ALTER TABLE [dbo].[PublishedAction] ALTER COLUMN [ThumbnailHash] NCHAR(32) NULL;
GO
ALTER TABLE [dbo].[PublishedAction] ALTER COLUMN [CutVideoHash] NCHAR(32) NULL;
GO
ALTER TABLE [dbo].[PublishedActionCategory] ALTER COLUMN [FileHash] NCHAR(32) NULL;
GO
ALTER TABLE [dbo].[PublishedReferential] ALTER COLUMN [FileHash] NCHAR(32) NULL;
GO
ALTER TABLE [dbo].[PublishedResource] ALTER COLUMN [FileHash] NCHAR(32) NULL;
GO

ALTER TABLE dbo.PublishedAction ADD CONSTRAINT FK_PublishedAction_PublishedFile
	FOREIGN KEY(ThumbnailHash) REFERENCES dbo.PublishedFile(Hash)
	ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE dbo.PublishedAction ADD CONSTRAINT FK_PublishedAction_CutVideo
	FOREIGN KEY(CutVideoHash) REFERENCES dbo.CutVideo(Hash)
	ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE dbo.PublishedActionCategory ADD CONSTRAINT FK_PublishedActionCategory_PublishedFile
	FOREIGN KEY(FileHash) REFERENCES dbo.PublishedFile(Hash)
	ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE dbo.PublishedReferential ADD CONSTRAINT FK_PublishedReferential_PublishedFile
	FOREIGN KEY(FileHash) REFERENCES dbo.PublishedFile(Hash)
	ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE dbo.PublishedResource ADD CONSTRAINT FK_PublishedResource_PublishedFile
	FOREIGN KEY(FileHash) REFERENCES dbo.PublishedFile(Hash)
	ON DELETE NO ACTION ON UPDATE NO ACTION;
GO