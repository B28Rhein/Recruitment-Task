USE [RecruitmentDB]
GO
/****** Object:  Table [dbo].[Leave Requests]    Script Date: 26.06.2024 12:21:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Leave Requests](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Employee] [int] NOT NULL,
	[Absence Reason] [int] NOT NULL,
	[Start Date] [date] NOT NULL,
	[End Date] [date] NOT NULL,
	[Comment] [text] NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Leave Request] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Leave Requests]  WITH CHECK ADD  CONSTRAINT [FK_Leave Requests_Employees] FOREIGN KEY([Employee])
REFERENCES [dbo].[Employees] ([ID])
GO
ALTER TABLE [dbo].[Leave Requests] CHECK CONSTRAINT [FK_Leave Requests_Employees]
GO
