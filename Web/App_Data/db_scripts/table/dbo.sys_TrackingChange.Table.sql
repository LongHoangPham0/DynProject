USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  Table [dbo].[sys_TrackingChange]    Script Date: 12/28/2022 10:42:31 PM ******/
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
