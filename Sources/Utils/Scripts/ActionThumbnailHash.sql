IF NOT EXISTS(SELECT * FROM sys.tables WHERE NAME = 'CloudFile')
	BEGIN
		CREATE TABLE [dbo].[CloudFile](
			[Hash] [nchar](32) NOT NULL,
			[Extension] [nvarchar](20) NULL,
		PRIMARY KEY CLUSTERED 
		(
			[Hash] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY];
	END
GO

EXEC AddColumnIfNotExists 'Action', 'ThumbnailHash', '[NCHAR](32) NULL';
GO

IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_Action_CloudFile')
	BEGIN	
		ALTER TABLE [dbo].[Action]  WITH CHECK ADD  CONSTRAINT [FK_Action_CloudFile] FOREIGN KEY([ThumbnailHash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;		
		ALTER TABLE [dbo].[Action] CHECK CONSTRAINT [FK_Action_CloudFile];
	END
GO