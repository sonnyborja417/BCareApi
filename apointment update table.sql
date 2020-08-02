USE [BCare]
GO

/****** Object:  Table [Trans].[Apointment]    Script Date: 23/02/2020 5:21:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Trans].[ApointmentUpdate](
	[PkApointmentUpdateId] [uniqueidentifier] NOT NULL,
	[FkApointmentId] [uniqueidentifier] NOT NULL,	
	[FkCreatorId] [uniqueidentifier] NOT NULL,	
	[CreatedDate] [datetime] NOT NULL,	
	[UpdateNote] [varchar](max) NULL,	
 CONSTRAINT [PK_ApointmentUpdateId] PRIMARY KEY CLUSTERED 
(
	[PkApointmentUpdateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [Trans].[ApointmentUpdate] ADD  CONSTRAINT [DF_ApointmentUpdate_PkApointmentUpdateId]  DEFAULT (newid()) FOR [PkApointmentUpdateId]
GO

ALTER TABLE [Trans].[ApointmentUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ApointmentUpdate_Apointment] FOREIGN KEY([FkApointmentId])
REFERENCES [Trans].[Apointment] ([PkApointmentId])
GO

ALTER TABLE [Trans].[ApointmentUpdate] CHECK CONSTRAINT [FK_ApointmentUpdate_Apointment]
GO

ALTER TABLE [Trans].[ApointmentUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ApointmentUpdate_Creator] FOREIGN KEY([FkCreatorId])
REFERENCES [Person].[Person] ([PkPersonId])
GO

ALTER TABLE [Trans].[ApointmentUpdate] CHECK CONSTRAINT [FK_ApointmentUpdate_Creator]
GO




