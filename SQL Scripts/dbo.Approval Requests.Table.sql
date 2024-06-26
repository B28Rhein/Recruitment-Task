USE [RecruitmentDB]
GO
/****** Object:  Table [dbo].[Approval Requests]    Script Date: 26.06.2024 12:21:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Approval Requests](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Approver] [int] NOT NULL,
	[Leave Request] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Comment] [text] NULL,
 CONSTRAINT [PK_Approval Requests] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Approval Requests]  WITH CHECK ADD  CONSTRAINT [FK_Approval Requests_Employees] FOREIGN KEY([Approver])
REFERENCES [dbo].[Employees] ([ID])
GO
ALTER TABLE [dbo].[Approval Requests] CHECK CONSTRAINT [FK_Approval Requests_Employees]
GO
ALTER TABLE [dbo].[Approval Requests]  WITH CHECK ADD  CONSTRAINT [FK_Approval Requests_Leave Requests] FOREIGN KEY([Leave Request])
REFERENCES [dbo].[Leave Requests] ([ID])
GO
ALTER TABLE [dbo].[Approval Requests] CHECK CONSTRAINT [FK_Approval Requests_Leave Requests]
GO
