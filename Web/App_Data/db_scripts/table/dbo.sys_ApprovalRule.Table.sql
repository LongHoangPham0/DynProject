USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  Table [dbo].[sys_ApprovalRule]    Script Date: 12/28/2022 10:42:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_ApprovalRule](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Permissions] [nvarchar](64) NOT NULL,
	[AppliedItemType] [int] NOT NULL,
	[QueryState] [nvarchar](64) NOT NULL,
	[NextState] [nvarchar](64) NOT NULL,
	[Queries] [nvarchar](max) NULL,
	[Actions] [nvarchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedTime] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[sys_ApprovalRule] ADD  DEFAULT (getutcdate()) FOR [CreatedTime]
GO
