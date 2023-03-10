USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAvailableCols]    Script Date: 12/28/2022 10:42:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAvailableCols]
	@ItemTypeID int,
	@ViewMode tinyint = 2 --1: direct list, 2: extended direct list, 3: full extended list
AS
IF @ViewMode = 1
	SELECT '[I].['+[PropertyName]+']' [ColName], '['+[PropertyName]+']' [Alias], [LabelText] [Label], (1000 + [SortOrder]) [Order]  
	FROM  [sys_ItemTypeProperty]  (NOLOCK)
	WHERE [ItemType]= @ItemTypeID AND [dbo].[fnc_GetDataSource]([DataSource]) IS NULL

	UNION
	SELECT '['+TR.[PropertyName]+'].['+T.[DisplayProperty]+']' [ColName], '['+TR.[PropertyName]+'_'+T.[DisplayProperty]+']' [Alias], TR.[LabelText] [Label], (2000+ 100*TR.[SortOrder]) [Order]
	FROM  [sys_ItemTypeProperty] TR  (NOLOCK)
	JOIN [sys_ItemType] T (NOLOCK) ON T.[Name] = [dbo].[fnc_GetDataSource](TR.[DataSource])
	WHERE TR.[ItemType]= @ItemTypeID AND [dbo].[fnc_GetDataSource](TR.[DataSource]) IS NOT NULL

	ORDER BY [Order]


ELSE IF @ViewMode = 2 
	SELECT '[I].['+[PropertyName]+']' [ColName], '['+[PropertyName]+']' [Alias], [LabelText] [Label], (1000 + [SortOrder]) [Order]  
	FROM  [sys_ItemTypeProperty]  (NOLOCK)
	WHERE [ItemType]= @ItemTypeID AND [dbo].[fnc_GetDataSource]([DataSource]) IS NULL

	UNION
	SELECT '['+TR.[PropertyName]+'].['+T.[DisplayProperty]+']' [ColName], '['+TR.[PropertyName]+'_'+T.[DisplayProperty]+']' [Alias], TR.[LabelText] [Label], (2000+ 100*TR.[SortOrder]) [Order]
	FROM  [sys_ItemTypeProperty] TR  (NOLOCK)
	JOIN [sys_ItemType] T (NOLOCK) ON T.[Name] = [dbo].[fnc_GetDataSource](TR.[DataSource])
	WHERE TR.[ItemType]= @ItemTypeID AND [dbo].[fnc_GetDataSource](TR.[DataSource]) IS NOT NULL

	UNION
	SELECT '[sys_Item].[State]' [ColName], '[State]' [Alias], 'Approval State' [Label], 3000 [Order]
	UNION
	SELECT 'CA.[UserName]' [ColName], '[CreatedBy]' [Alias], 'Created By' [Label], 3001 [Order]
	UNION
	SELECT 'UA.[UserName]' [ColName], '[UpdatedBy]' [Alias], 'Updated By' [Label], 3002 [Order]
	UNION
	SELECT '[sys_Item].[CreatedTime]' [ColName], '[CreatedTime]' [Alias], 'Created Time' [Label], 3003 [Order]
	UNION
	SELECT '[sys_Item].[UpdatedTime]' [ColName], '[UpdatedTime]' [Alias], 'Updated Time' [Label], 3004 [Order]

	ORDER BY [Order]
ELSE IF @ViewMode = 3 
	SELECT '[I].['+[PropertyName]+']' [ColName], '['+[PropertyName]+']' [Alias], [LabelText] [Label], (1000 + [SortOrder]) [Order]  
	FROM  [sys_ItemTypeProperty]  (NOLOCK)
	WHERE [ItemType]= @ItemTypeID AND [dbo].[fnc_GetDataSource]([DataSource]) IS NULL

	UNION
	SELECT '['+T.[Name]+'].['+TP.PropertyName+']' [ColName], '['+T.[Name]+'_'+TP.PropertyName+']' [Alias], T.[Display] +'.'+ TP.[LabelText] [Label], (3000+ 100*TR.[SortOrder] + TP.[SortOrder]) [Order]
	FROM [sys_ItemTypeRelation] TR (NOLOCK)
	JOIN [sys_ItemType] T (NOLOCK) ON T.[ID] = TR.[ChildTypeID]
	JOIN [sys_ItemTypeProperty] TP (NOLOCK) ON TP.[ItemType] = T.[ID]
	WHERE [ParentTypeID] = @ItemTypeID

	UNION
	SELECT '['+TR.[PropertyName]+'].['+T.[DisplayProperty]+']' [ColName], '['+TR.[PropertyName]+'_'+T.[DisplayProperty]+']' [Alias], TR.[LabelText] [Label], (2000+ 100*TR.[SortOrder]) [Order]
	FROM  [sys_ItemTypeProperty] TR  (NOLOCK)
	JOIN [sys_ItemType] T (NOLOCK) ON T.[Name] = [dbo].[fnc_GetDataSource](TR.[DataSource])
	WHERE TR.[ItemType]= @ItemTypeID AND [dbo].[fnc_GetDataSource](TR.[DataSource]) IS NOT NULL

	UNION
	SELECT '[sys_Item].[State]' [ColName], '[State]' [Alias], 'Approval State' [Label], 4000 [Order]
	UNION
	SELECT 'CA.[UserName]' [ColName], '[CreatedBy]' [Alias], 'Created By' [Label], 4001 [Order]
	UNION
	SELECT 'UA.[UserName]' [ColName], '[UpdatedBy]' [Alias], 'Updated By' [Label], 4002 [Order]
	UNION
	SELECT '[sys_Item].[CreatedTime]' [ColName], '[CreatedTime]' [Alias], 'Created Time' [Label], 4003 [Order]
	UNION
	SELECT '[sys_Item].[UpdatedTime]' [ColName], '[UpdatedTime]' [Alias], 'Updated Time' [Label], 4004 [Order]


	ORDER BY [Order]
GO
