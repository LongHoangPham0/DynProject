USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  Table [dbo].[sys_ItemApproval]    Script Date: 12/28/2022 10:42:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_ItemApproval](
	[ItemId] [int] NOT NULL,
	[Actor] [int] NOT NULL,
	[State] [nvarchar](64) NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
	[Comment] [nvarchar](2048) NULL,
 CONSTRAINT [PK_ItemApproval] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC,
	[Actor] ASC,
	[State] ASC,
	[CreatedTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[sys_ItemApproval] ADD  DEFAULT (getutcdate()) FOR [CreatedTime]
GO
