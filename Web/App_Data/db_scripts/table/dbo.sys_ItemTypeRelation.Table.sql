USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  Table [dbo].[sys_ItemTypeRelation]    Script Date: 12/28/2022 10:42:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_ItemTypeRelation](
	[ParentTypeID] [int] NOT NULL,
	[ChildTypeID] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Alias] [nvarchar](512) NULL,
	[SortOrder] [int] NULL,
 CONSTRAINT [PK_ItemTypeRelation] PRIMARY KEY CLUSTERED 
(
	[ParentTypeID] ASC,
	[ChildTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
