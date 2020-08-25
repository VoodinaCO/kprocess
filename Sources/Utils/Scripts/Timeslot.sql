IF NOT EXISTS(SELECT * FROM sys.tables WHERE NAME = 'Timeslot')
    BEGIN
        CREATE TABLE [dbo].[Timeslot](
			[TimeslotId] [int] IDENTITY(1,1) NOT NULL,
			[Label] [nvarchar](50) NOT NULL,
			[Description] [nvarchar](4000) NULL,
			[StartTime] [time](7) NOT NULL,
			[EndTime] [time](7) NOT NULL,
			[Color] [varchar](10) NULL,
			[DisplayOrder] [int] NULL,
			[IsDeleted] [bit] NOT NULL,
		PRIMARY KEY CLUSTERED 
		(
			[TimeslotId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY];
    END
GO
