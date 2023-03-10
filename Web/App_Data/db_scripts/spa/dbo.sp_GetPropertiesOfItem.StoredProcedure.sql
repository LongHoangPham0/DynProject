USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPropertiesOfItem]    Script Date: 12/28/2022 10:42:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetPropertiesOfItem]
	@ItemType int
AS
	SELECT *
	FROM [sys_ItemTypeProperty] (NOLOCK)
	WHERE [ItemType] = @ItemType
	ORDER BY [SortOrder]
GO
