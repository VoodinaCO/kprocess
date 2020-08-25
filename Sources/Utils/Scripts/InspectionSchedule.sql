IF NOT EXISTS(SELECT * FROM sys.tables WHERE NAME = 'InspectionSchedule')
	BEGIN
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
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
		
		ALTER TABLE [dbo].[InspectionSchedule]  WITH CHECK ADD  CONSTRAINT [FK_InspectionSchedule_Procedure] FOREIGN KEY([ProcessId]) REFERENCES [dbo].[Procedure] ([ProcessId]);
		ALTER TABLE [dbo].[InspectionSchedule] CHECK CONSTRAINT [FK_InspectionSchedule_Procedure];
		ALTER TABLE [dbo].[InspectionSchedule]  WITH CHECK ADD  CONSTRAINT [FK_InspectionSchedule_Timeslot] FOREIGN KEY([TimeslotId]) REFERENCES [dbo].[Timeslot] ([TimeslotId]);
		ALTER TABLE [dbo].[InspectionSchedule] CHECK CONSTRAINT [FK_InspectionSchedule_Timeslot];
	END
GO