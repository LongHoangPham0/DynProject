USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetChildTypeItem]    Script Date: 12/28/2022 10:42:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetChildTypeItem]
	@ParentID int
AS
	SELECT *
	FROM [sys_ItemTypeRelation]
	WHERE [ParentTypeID] = @ParentID
	ORDER BY [SortOrder]
GO
