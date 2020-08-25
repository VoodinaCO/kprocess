EXEC AddColumnIfNotExists 'RefResource', 'Hash', '[NCHAR](32) NULL';
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_RefResource_CloudFile')
	BEGIN
		ALTER TABLE [dbo].[RefResource]  WITH CHECK ADD  CONSTRAINT [FK_RefResource_CloudFile] FOREIGN KEY([Hash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;
		ALTER TABLE [dbo].[RefResource] CHECK CONSTRAINT [FK_RefResource_CloudFile];
	END
GO



EXEC AddColumnIfNotExists 'Ref1', 'Hash', '[NCHAR](32) NULL';
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_Ref1_CloudFile')
	BEGIN
		ALTER TABLE [dbo].[Ref1]  WITH CHECK ADD  CONSTRAINT [FK_Ref1_CloudFile] FOREIGN KEY([Hash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;
		ALTER TABLE [dbo].[Ref1] CHECK CONSTRAINT [FK_Ref1_CloudFile];
	END
GO

EXEC AddColumnIfNotExists 'Ref2', 'Hash', '[NCHAR](32) NULL';
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_Ref2_CloudFile')
	BEGIN
		ALTER TABLE [dbo].[Ref2]  WITH CHECK ADD  CONSTRAINT [FK_Ref2_CloudFile] FOREIGN KEY([Hash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;
		ALTER TABLE [dbo].[Ref2] CHECK CONSTRAINT [FK_Ref2_CloudFile];
	END
GO

EXEC AddColumnIfNotExists 'Ref3', 'Hash', '[NCHAR](32) NULL';
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_Ref3_CloudFile')
	BEGIN
		ALTER TABLE [dbo].[Ref3]  WITH CHECK ADD  CONSTRAINT [FK_Ref3_CloudFile] FOREIGN KEY([Hash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;
		ALTER TABLE [dbo].[Ref3] CHECK CONSTRAINT [FK_Ref3_CloudFile];
	END
GO

EXEC AddColumnIfNotExists 'Ref4', 'Hash', '[NCHAR](32) NULL';
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_Ref4_CloudFile')
	BEGIN
		ALTER TABLE [dbo].[Ref4]  WITH CHECK ADD  CONSTRAINT [FK_Ref4_CloudFile] FOREIGN KEY([Hash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;
		ALTER TABLE [dbo].[Ref4] CHECK CONSTRAINT [FK_Ref4_CloudFile];
	END
GO

EXEC AddColumnIfNotExists 'Ref5', 'Hash', '[NCHAR](32) NULL';
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_Ref5_CloudFile')
	BEGIN
		ALTER TABLE [dbo].[Ref5]  WITH CHECK ADD  CONSTRAINT [FK_Ref5_CloudFile] FOREIGN KEY([Hash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;
		ALTER TABLE [dbo].[Ref5] CHECK CONSTRAINT [FK_Ref5_CloudFile];
	END
GO

EXEC AddColumnIfNotExists 'Ref6', 'Hash', '[NCHAR](32) NULL';
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_Ref6_CloudFile')
	BEGIN
		ALTER TABLE [dbo].[Ref6]  WITH CHECK ADD  CONSTRAINT [FK_Ref6_CloudFile] FOREIGN KEY([Hash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;
		ALTER TABLE [dbo].[Ref6] CHECK CONSTRAINT [FK_Ref6_CloudFile];
	END
GO

EXEC AddColumnIfNotExists 'Ref7', 'Hash', '[NCHAR](32) NULL';
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_Ref7_CloudFile')
	BEGIN
		ALTER TABLE [dbo].[Ref7]  WITH CHECK ADD  CONSTRAINT [FK_Ref7_CloudFile] FOREIGN KEY([Hash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;
		ALTER TABLE [dbo].[Ref7] CHECK CONSTRAINT [FK_Ref7_CloudFile];
	END
GO

EXEC AddColumnIfNotExists 'RefActionCategory', 'Hash', '[NCHAR](32) NULL';
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_RefActionCategory_CloudFile')
	BEGIN
		ALTER TABLE [dbo].[RefActionCategory]  WITH CHECK ADD  CONSTRAINT [FK_RefActionCategory_CloudFile] FOREIGN KEY([Hash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;
		ALTER TABLE [dbo].[RefActionCategory] CHECK CONSTRAINT [FK_RefActionCategory_CloudFile];
	END
GO

EXEC AddColumnIfNotExists 'Skill', 'Hash', '[NCHAR](32) NULL';
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_Skill_CloudFile')
	BEGIN
		ALTER TABLE [dbo].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_Skill_CloudFile] FOREIGN KEY([Hash])
		REFERENCES [dbo].[CloudFile] ([Hash])
		ON DELETE SET NULL;
		ALTER TABLE [dbo].[Skill] CHECK CONSTRAINT [FK_Skill_CloudFile];
	END
GO