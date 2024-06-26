USE [RecruitmentDB]
GO
/****** Object:  Table [dbo].[EmployeeProjectAssignments]    Script Date: 26.06.2024 12:21:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeProjectAssignments](
	[EmployeeID] [int] NOT NULL,
	[ProjectID] [int] NOT NULL,
 CONSTRAINT [PK_EmployeeProjectAssignments] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC,
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EmployeeProjectAssignments]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeProjectAssignments_Employees] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employees] ([ID])
GO
ALTER TABLE [dbo].[EmployeeProjectAssignments] CHECK CONSTRAINT [FK_EmployeeProjectAssignments_Employees]
GO
ALTER TABLE [dbo].[EmployeeProjectAssignments]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeProjectAssignments_Projects] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ID])
GO
ALTER TABLE [dbo].[EmployeeProjectAssignments] CHECK CONSTRAINT [FK_EmployeeProjectAssignments_Projects]
GO
