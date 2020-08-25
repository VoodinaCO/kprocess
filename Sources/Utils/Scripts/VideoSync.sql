IF NOT EXISTS(SELECT * FROM sys.tables WHERE NAME = 'VideoSync')
	BEGIN
		CREATE TABLE [dbo].[VideoSync](
			[UserId] [int] NOT NULL,
			[ProcessId] [int] NOT NULL,
			[SyncValue] [bit] NOT NULL DEFAULT ((0)),
		 CONSTRAINT [PK_VideoSync] PRIMARY KEY CLUSTERED 
		(
			[UserId] ASC,
			[ProcessId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY];
		
		ALTER TABLE [dbo].[VideoSync]  WITH CHECK ADD  CONSTRAINT [FK_VideoSync_User] FOREIGN KEY([UserId])
		REFERENCES [dbo].[User] ([UserId])
		ON DELETE CASCADE;
		ALTER TABLE [dbo].[VideoSync] CHECK CONSTRAINT [FK_VideoSync_User];
		ALTER TABLE [dbo].[VideoSync]  WITH CHECK ADD  CONSTRAINT [FK_VideoSync_Procedure] FOREIGN KEY([ProcessId])
		REFERENCES [dbo].[Procedure] ([ProcessId])
		ON DELETE CASCADE;
		ALTER TABLE [dbo].[VideoSync] CHECK CONSTRAINT [FK_VideoSync_Procedure];
	END
GO