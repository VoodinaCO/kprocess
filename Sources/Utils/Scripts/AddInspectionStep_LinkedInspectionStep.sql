ALTER TABLE [dbo].[InspectionStep] ADD LinkedInspectionId INT NULL
GO
ALTER TABLE [dbo].[InspectionStep]  WITH CHECK ADD  CONSTRAINT [FK_InspectionStep_LinkedInspection] FOREIGN KEY([LinkedInspectionId])
REFERENCES [dbo].[Inspection] ([InspectionId])
GO
ALTER TABLE [dbo].[InspectionStep] CHECK CONSTRAINT [FK_InspectionStep_LinkedInspection]
GO