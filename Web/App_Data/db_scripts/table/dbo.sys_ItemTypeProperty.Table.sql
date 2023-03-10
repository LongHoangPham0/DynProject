USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  Table [dbo].[sys_ItemTypeProperty]    Script Date: 12/28/2022 10:42:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_ItemTypeProperty](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PropertyName] [varchar](4000) NOT NULL,
	[DataType] [varchar](4000) NOT NULL,
	[ItemType] [int] NOT NULL,
	[LabelText] [nvarchar](4000) NOT NULL,
	[InputControl] [varchar](4000) NOT NULL,
	[Validation] [nvarchar](4000) NULL,
	[DataSource] [nvarchar](4000) NULL,
	[DisplayFormat] [nvarchar](4000) NULL,
	[SortOrder] [int] NOT NULL,
	[AllowSearch] [bit] NULL,
	[AllowShowGrid] [bit] NULL,
	[OnValueChanged] [nvarchar](4000) NULL,
	[ReadOnly] [bit] NULL,
 CONSTRAINT [PK_ObjectStructure] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
