USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetIndirectParents]    Script Date: 12/28/2022 10:42:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetIndirectParents]
@TypeID int, 
@ParentID int
AS
BEGIN

DECLARE @ParentTypeID int
SELECT @ParentTypeID = [Type] FROM [sys_Item] (NOLOCK)
WHERE [ID] = @ParentID




SELECT a.[IDChild] AS [ID] FROM [sys_ItemRelation] (NOLOCK) a
JOIN [sys_Item] (NOLOCK) b ON b.[ID] = a.[IDChild]
WHERE [IDParent] IN (SELECT [ID] FROM fnc_GetParentList(@ParentID))
AND b.[Type] IN (
	SELECT [ID] FROM fnc_GetParentTypeList(@TypeID) WHERE [ID] <> @ParentTypeID AND [ID] <> @TypeID
)
END
GO
