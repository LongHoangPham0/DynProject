USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  Table [dbo].[sys_ItemType]    Script Date: 12/28/2022 10:42:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_ItemType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](4000) NOT NULL,
	[ParentItems] [varchar](4000) NULL,
	[DisplayProperty] [varchar](4000) NULL,
	[Display] [nvarchar](4000) NULL,
	[AllowComment] [bit] NULL,
	[SortOrder] [int] NULL,
	[Permission] [nvarchar](4000) NULL,
	[ModPermission] [nvarchar](4000) NULL,
	[AllowReport] [bit] NULL,
	[ApprovalProcess] [bit] NULL,
	[AllowFileAttachment] [bit] NULL,
	[TrackingChange] [bit] NULL,
	[TypeOfItem] [int] NULL,
	[ListByCreator] [bit] NULL,
 CONSTRAINT [PK_ItemType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[sys_ItemType] ADD  DEFAULT ((0)) FOR [AllowComment]
GO
ALTER TABLE [dbo].[sys_ItemType] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[sys_ItemType] ADD  DEFAULT ((0)) FOR [AllowReport]
GO
ALTER TABLE [dbo].[sys_ItemType] ADD  DEFAULT ((0)) FOR [ApprovalProcess]
GO
ALTER TABLE [dbo].[sys_ItemType] ADD  DEFAULT ((0)) FOR [AllowFileAttachment]
GO
ALTER TABLE [dbo].[sys_ItemType] ADD  DEFAULT ((0)) FOR [TrackingChange]
GO
ALTER TABLE [dbo].[sys_ItemType] ADD  DEFAULT ((0)) FOR [TypeOfItem]
GO
ALTER TABLE [dbo].[sys_ItemType] ADD  DEFAULT ((0)) FOR [ListByCreator]
GO
