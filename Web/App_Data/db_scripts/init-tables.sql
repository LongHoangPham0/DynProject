ALTER TABLE [dbo].[sys_Report] DROP CONSTRAINT [DF__sys_Repor__Creat__52593CB8]
GO
ALTER TABLE [dbo].[sys_ItemType] DROP CONSTRAINT [DF__sys_ItemT__ListB__5165187F]
GO
ALTER TABLE [dbo].[sys_ItemType] DROP CONSTRAINT [DF__sys_ItemT__TypeO__5070F446]
GO
ALTER TABLE [dbo].[sys_ItemType] DROP CONSTRAINT [DF__sys_ItemT__Track__4F7CD00D]
GO
ALTER TABLE [dbo].[sys_ItemType] DROP CONSTRAINT [DF__sys_ItemT__Allow__4E88ABD4]
GO
ALTER TABLE [dbo].[sys_ItemType] DROP CONSTRAINT [DF__sys_ItemT__Appro__4D94879B]
GO
ALTER TABLE [dbo].[sys_ItemType] DROP CONSTRAINT [DF__sys_ItemT__Allow__4CA06362]
GO
ALTER TABLE [dbo].[sys_ItemType] DROP CONSTRAINT [DF__sys_ItemT__SortO__4BAC3F29]
GO
ALTER TABLE [dbo].[sys_ItemType] DROP CONSTRAINT [DF__sys_ItemT__Allow__4AB81AF0]
GO
ALTER TABLE [dbo].[sys_ItemApproval] DROP CONSTRAINT [DF__sys_ItemA__Creat__49C3F6B7]
GO
ALTER TABLE [dbo].[sys_Item] DROP CONSTRAINT [DF__sys_Item__Create__48CFD27E]
GO
ALTER TABLE [dbo].[sys_Comment] DROP CONSTRAINT [DF__sys_Comme__Creat__47DBAE45]
GO
ALTER TABLE [dbo].[sys_ApprovalRule] DROP CONSTRAINT [DF__sys_Appro__Creat__46E78A0C]
GO
/****** Object:  Table [dbo].[sys_TrackingChange]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_TrackingChange]
GO
/****** Object:  Table [dbo].[sys_RolePermission]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_RolePermission]
GO
/****** Object:  Table [dbo].[sys_Role]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_Role]
GO
/****** Object:  Table [dbo].[sys_Report]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_Report]
GO
/****** Object:  Table [dbo].[sys_Permission]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_Permission]
GO
/****** Object:  Table [dbo].[sys_ItemTypeRelation]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_ItemTypeRelation]
GO
/****** Object:  Table [dbo].[sys_ItemTypeProperty]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_ItemTypeProperty]
GO
/****** Object:  Table [dbo].[sys_ItemType]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_ItemType]
GO
/****** Object:  Table [dbo].[sys_ItemRelation]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_ItemRelation]
GO
/****** Object:  Table [dbo].[sys_ItemApproval]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_ItemApproval]
GO
/****** Object:  Table [dbo].[sys_Item]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_Item]
GO
/****** Object:  Table [dbo].[sys_Comment]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_Comment]
GO
/****** Object:  Table [dbo].[sys_Attachment]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_Attachment]
GO
/****** Object:  Table [dbo].[sys_ApprovalRule]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_ApprovalRule]
GO
/****** Object:  Table [dbo].[sys_Account]    Script Date: 12/28/2022 10:40:40 PM ******/
DROP TABLE [dbo].[sys_Account]
GO
/****** Object:  Table [dbo].[sys_Account]    Script Date: 12/28/2022 10:40:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_Account](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[RoleID] [int] NOT NULL,
	[LinkedIDs] [nvarchar](128) NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_ApprovalRule]    Script Date: 12/28/2022 10:40:40 PM ******/
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
/****** Object:  Table [dbo].[sys_Attachment]    Script Date: 12/28/2022 10:40:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_Attachment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[FileName] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_Comment]    Script Date: 12/28/2022 10:40:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_Comment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[AccountID] [int] NOT NULL,
	[EmployeeID] [int] NULL,
	[ItemID] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_Cmt] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_Item]    Script Date: 12/28/2022 10:40:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_Item](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[State] [nvarchar](128) NULL,
	[CreatedBy] [int] NULL,
	[CreatedTime] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedTime] [datetime] NULL,
 CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_ItemApproval]    Script Date: 12/28/2022 10:40:40 PM ******/
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
/****** Object:  Table [dbo].[sys_ItemRelation]    Script Date: 12/28/2022 10:40:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_ItemRelation](
	[IDChild] [int] NOT NULL,
	[IDParent] [int] NOT NULL,
	[Description] [nvarchar](64) NULL,
 CONSTRAINT [PK_ItemRelation] PRIMARY KEY CLUSTERED 
(
	[IDChild] ASC,
	[IDParent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_ItemType]    Script Date: 12/28/2022 10:40:40 PM ******/
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
/****** Object:  Table [dbo].[sys_ItemTypeProperty]    Script Date: 12/28/2022 10:40:40 PM ******/
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
/****** Object:  Table [dbo].[sys_ItemTypeRelation]    Script Date: 12/28/2022 10:40:40 PM ******/
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
/****** Object:  Table [dbo].[sys_Permission]    Script Date: 12/28/2022 10:40:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_Permission](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_Report]    Script Date: 12/28/2022 10:40:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_Report](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[DataSource] [int] NOT NULL,
	[Queries] [nvarchar](2048) NOT NULL,
	[DisplayColumns] [nvarchar](1024) NOT NULL,
	[Sorts] [nvarchar](1024) NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedTime] [datetime] NULL,
	[UpdatedTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_Role]    Script Date: 12/28/2022 10:40:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_Role](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[RoleCode] [nvarchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_RolePermission]    Script Date: 12/28/2022 10:40:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_RolePermission](
	[RoleID] [int] NOT NULL,
	[PermissionID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sys_TrackingChange]    Script Date: 12/28/2022 10:40:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_TrackingChange](
	[ItemId] [int] NOT NULL,
	[UpdatedBy] [nvarchar](128) NOT NULL,
	[UpdatedTime] [datetime] NOT NULL,
	[Field] [nvarchar](128) NOT NULL,
	[OldValue] [nvarchar](max) NOT NULL,
	[NewValue] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_TrackingChange] PRIMARY KEY CLUSTERED 
(
	[Field] ASC,
	[ItemId] ASC,
	[UpdatedBy] ASC,
	[UpdatedTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[sys_ApprovalRule] ADD  DEFAULT (getutcdate()) FOR [CreatedTime]
GO
ALTER TABLE [dbo].[sys_Comment] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[sys_Item] ADD  DEFAULT (getdate()) FOR [CreatedTime]
GO
ALTER TABLE [dbo].[sys_ItemApproval] ADD  DEFAULT (getutcdate()) FOR [CreatedTime]
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
ALTER TABLE [dbo].[sys_Report] ADD  DEFAULT (getdate()) FOR [CreatedTime]
GO
