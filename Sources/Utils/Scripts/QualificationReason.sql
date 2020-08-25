SET ANSI_NULLS ON 
GO 
SET QUOTED_IDENTIFIER ON 
GO 
CREATE TABLE [dbo].[QualificationReason](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [int] NOT NULL,
	[Comment] [nvarchar](3000) NOT NULL,
	[IsEditable] [bit] NOT NULL DEFAULT((0)),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (1,N'Ne sait pas',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (2,N'Réponse incorrecte ou incomplète',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (3,N'Ne sait pas faire',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (4,N'Méthode incorrecte ou incomplète',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (5,N'EPI inadapté ou manquant',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (6,N'Matériel inadapté ou manquant',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (7,N'Autre',1)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (8,N'Non fait',0)
GO
IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE NAME = 'FK_QualificationStep_QualificationReason')
	BEGIN
		DELETE FROM [dbo].[QualificationStep];
		DELETE FROM [dbo].[Qualification];
		-- Creating new column [QualificationReasonId] in 'QualificationStep'
		EXEC AddColumnIfNotExists 'QualificationStep', 'QualificationReasonId', '[INT] NOT NULL';
		-- Creating foreign key on [QualificationReasonId] in table 'QualificationSteps'
		ALTER TABLE [dbo].[QualificationStep]
		ADD CONSTRAINT [FK_QualificationStep_QualificationReason]
		    FOREIGN KEY ([QualificationReasonId]) REFERENCES [dbo].[QualificationReason] ([Id])
		    ON DELETE CASCADE ON UPDATE NO ACTION;
		-- Creating non-clustered index for FOREIGN KEY 'FK_QualificationStep_QualificationReason'
		CREATE INDEX [IX_FK_QualificationStep_QualificationReason]
		ON [dbo].[QualificationStep] ([QualificationReasonId]);
	END
GO