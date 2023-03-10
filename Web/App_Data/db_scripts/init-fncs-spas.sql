/****** Object:  StoredProcedure [dbo].[sp_ViewReport]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_ViewReport]
GO
/****** Object:  StoredProcedure [dbo].[sp_SelectChildItems]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_SelectChildItems]
GO
/****** Object:  StoredProcedure [dbo].[sp_SearchItems]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_SearchItems]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetRefItems]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_GetRefItems]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPropertiesOfItem]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_GetPropertiesOfItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPermissionOfRole]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_GetPermissionOfRole]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetParentItems]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_GetParentItems]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetListComment]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_GetListComment]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetItems]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_GetItems]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetIndirectParents]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_GetIndirectParents]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetChildTypeItem]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_GetChildTypeItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAvailableCols]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_GetAvailableCols]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetApprovalList]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_GetApprovalList]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteRolePermission]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_DeleteRolePermission]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteRole]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_DeleteRole]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeletePermission]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_DeletePermission]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteItemTypeProperty]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_DeleteItemTypeProperty]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteItemType]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_DeleteItemType]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteItem]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_DeleteItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteChildItem]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP PROCEDURE [dbo].[sp_DeleteChildItem]
GO
/****** Object:  UserDefinedFunction [dbo].[fnc_GetParentTypeList]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP FUNCTION [dbo].[fnc_GetParentTypeList]
GO
/****** Object:  UserDefinedFunction [dbo].[fnc_GetParentList]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP FUNCTION [dbo].[fnc_GetParentList]
GO
/****** Object:  UserDefinedFunction [dbo].[fnc_GetDataSource]    Script Date: 12/28/2022 10:35:49 PM ******/
DROP FUNCTION [dbo].[fnc_GetDataSource]
GO
/****** Object:  UserDefinedFunction [dbo].[fnc_GetDataSource]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnc_GetDataSource](@v nvarchar(256))
RETURNS nvarchar(64)
AS
BEGIN
	IF @v = '' OR LEFT(@v,1)='[' return NULL;
	DECLARE @pos int =  CHARINDEX(':', @v);
	IF @pos=0 RETURN @v;
	RETURN LEFT(@v,@pos-1);
END
GO
/****** Object:  UserDefinedFunction [dbo].[fnc_GetParentList]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnc_GetParentList](@ItemID INT)
RETURNS @TB TABLE([ID] INT,[Level] INT)
AS
BEGIN
	DECLARE @Level INT = 0
	DECLARE @TB_LAST TABLE([ID] INT)
	DECLARE @TB_TMP TABLE([ID] INT)
	
	INSERT INTO @TB_LAST([ID]) VALUES(@ItemID)
	
	INSERT INTO @TB
	SELECT [ID],@Level
	FROM  @TB_LAST
	
	WHILE(EXISTS(SELECT * FROM @TB_LAST))
	BEGIN
	
		DELETE @TB_TMP
		INSERT INTO @TB_TMP
		SELECT [ID]
		FROM  @TB_LAST
		
		DELETE @TB_LAST
		SET @Level = @Level+1
		
		 
		INSERT INTO @TB_LAST 
		SELECT [IDParent]
		FROM [sys_ItemRelation] (NOLOCK)
		WHERE [IDChild] IN (SELECT [ID] FROM @TB_TMP)
		
		INSERT INTO @TB
		SELECT [ID],@Level
		FROM  @TB_LAST
		
	END
	RETURN;
END
GO
/****** Object:  UserDefinedFunction [dbo].[fnc_GetParentTypeList]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnc_GetParentTypeList](@TypeID INT)
RETURNS @TB TABLE([ID] INT,[Level] INT)
AS
BEGIN
	DECLARE @Level INT = 0
	DECLARE @TB_LAST TABLE([ID] INT)
	DECLARE @TB_TMP TABLE([ID] INT)
	
	INSERT INTO @TB_LAST([ID]) VALUES(@TypeID)
	
	INSERT INTO @TB
	SELECT [ID],@Level
	FROM  @TB_LAST
	
	WHILE(EXISTS(SELECT * FROM @TB_LAST))
	BEGIN
	
		DELETE @TB_TMP
		INSERT INTO @TB_TMP
		SELECT [ID]
		FROM  @TB_LAST
		
		DELETE @TB_LAST
		SET @Level = @Level+1
		
		 
		INSERT INTO @TB_LAST 
		SELECT [ParentTypeID]
		FROM [sys_ItemTypeRelation] (NOLOCK)
		WHERE [ChildTypeID] IN (SELECT [ID] FROM @TB_TMP)
			AND [ParentTypeID] NOT IN (SELECT [ID] FROM @TB)
		
		INSERT INTO @TB
		SELECT [ID],@Level
		FROM  @TB_LAST
		
	END
	RETURN;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteChildItem]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteChildItem]
	@ParentID int
AS
	DELETE FROM [sys_ItemTypeRelation] WHERE [ParentTypeID] = @ParentID
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteItem]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteItem]
	@ID int
AS

	

	SELECT b.ID, [Type], c.[Name] AS TableName INTO #tmp 
	FROM [sys_ItemRelation] a (NOLOCK)
	JOIN [sys_Item] b (NOLOCK) ON b.[ID] = a.[IDChild]
	JOIN [sys_ItemType] c (NOLOCK) ON c.[ID] = b.[Type] AND c.[TypeOfItem] = 1 -- child ony
	WHERE [IDParent] = @ID
	UNION
	SELECT a.[ID], [Type], c.[Name]
	FROM [sys_Item]  a (NOLOCK)
	JOIN [sys_ItemType] c (NOLOCK) ON c.[ID] = a.[Type]
	WHERE a.[ID] = @ID;

	DECLARE @sql NVARCHAR(MAX) = '';
	SELECT @sql = @sql + '
	DELETE FROM [' +[TableName]+'] WHERE [ID]=' + CAST([ID] AS NVARCHAR)+';
	DELETE FROM [sys_Item] WHERE [ID]=' + CAST([ID] AS NVARCHAR)+';'
	FROM #tmp;
	DELETE FROM [sys_ItemRelation] WHERE [IDParent] = @ID;
	EXEC(@sql);
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteItemType]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteItemType]
	@ItemTypeID int
AS

	DECLARE @Name nvarchar(128), @Query nvarchar(1000)
	SELECT @Name = [Name] FROM [sys_ItemType] (NOLOCK) WHERE [ID] = @ItemTypeID

	
	DELETE
	FROM [sys_ItemType]
	WHERE [ID] = @ItemTypeID

	SET @Query = N'IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'''+ @Name +''') AND type in (N''U'')) DROP TABLE [' + @Name +']'
	EXEC sp_executesql @Query
	
	DELETE 
	FROM [sys_Item]
	WHERE [Type] = @ItemTypeID

	DELETE
	FROM [sys_ItemTypeProperty]
	WHERE [ItemType] = @ItemTypeID

	DELETE 
	FROM [sys_ItemTypeRelation] 
	WHERE [ParentTypeID] = @ItemTypeID OR [ChildTypeID] = @ItemTypeID 
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteItemTypeProperty]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteItemTypeProperty]
	@ID int
AS
	DECLARE @ItemTypeID int, @TableName nvarchar(128), @ItemTypePropertyName nvarchar(128), @Query nvarchar(128)
	
	SELECT @ItemTypeID =  [ItemType]
	FROM [sys_ItemTypeProperty] (NOLOCK)
	WHERE [ID] = @ID
	
	SELECT @ItemTypePropertyName = [PropertyName] 
	FROM [sys_ItemTypeProperty] (NOLOCK)
	WHERE [ID] = @ID

	DELETE 
	FROM [sys_ItemTypeProperty]
	WHERE [ID] = @ID

	SELECT @TableName = [Name]
	FROM [sys_ItemType] (NOLOCK)
	WHERE [ID] = @ItemTypeID

	SET @Query = N'ALTER TABLE [' + @TableName + '] DROP COLUMN [' + @ItemTypePropertyName + ']'
	EXEC sp_executesql @Query
GO
/****** Object:  StoredProcedure [dbo].[sp_DeletePermission]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeletePermission]
	@ID int
AS
	DELETE 
	FROM [sys_Permission]
	WHERE [ID] = @ID

	DELETE
	FROM [sys_RolePermission]
	WHERE [PermissionID] = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteRole]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[sp_DeleteRole]
	@ID int
AS
	DELETE 
	FROM [sys_Role]
	WHERE [ID] = @ID

	DELETE 
	FROM [sys_RolePermission]
	WHERE [RoleID] = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteRolePermission]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteRolePermission]
	@ID int
AS
	DELETE
	FROM [sys_RolePermission]
	WHERE [RoleID] = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_GetApprovalList]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetApprovalList]
@PageIndex int ,
@PageSize int,
@Type int,
@QueryFilter nvarchar(max),
@ItemId int = 0,
@Print bit = 0
AS
BEGIN
DECLARE @Name nvarchar(128), @Query nvarchar(4000)
SELECT @Name = REPLACE([Name],' ','') FROM [sys_ItemType] (NOLOCK) WHERE [ID] = @Type
DECLARE @Join nvarchar(max) = ''
SELECT @Join = @Join + 'LEFT JOIN ['+[dbo].[fnc_GetDataSource]([DataSource])+'] ['+[PropertyName]+'] (NOLOCK) ON I.['+ [PropertyName]+ '] = ['+[PropertyName]+'].[ID] ' 
FROM [sys_ItemTypeProperty]
WHERE [ItemType] = @Type AND [dbo].[fnc_GetDataSource]([DataSource]) IS NOT NULL

SET @Join = @Join+'
		'
SELECT @Join = @Join + ' LEFT JOIN ['+[sys_ItemType].[Name]+'] ON [sys_ItemRelation].[IDChild] = ['+[sys_ItemType].[Name]+'].[ID]' 
FROM [sys_ItemTypeRelation] (NOLOCK)
JOIN [sys_ItemType] (NOLOCK) ON [sys_ItemType].[ID] = [sys_ItemTypeRelation].[ChildTypeID]
WHERE [ParentTypeID] = @Type
SET @QueryFilter = '(@ItemId = 0 OR I.[ID] = @ItemId) AND ' + @QueryFilter

 SET @Query = N'
 ;WITH [s] AS
 (  
    SELECT DISTINCT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY [ID]) AS [RowNum], * FROM (
		SELECT DISTINCT I.*, CA.[UserName] [CreatedBy], UA.[UserName] [UpdatedBy], [sys_Item].[State],[sys_Item].[CreatedTime],[sys_Item].[UpdatedTime]
		FROM ['+@Name+'] I (NOLOCK)

		LEFT JOIN [sys_Item] (NOLOCK) ON [sys_Item].[ID] = I.[ID]
		LEFT JOIN [sys_Account] CA (NOLOCK) ON [sys_Item].[CreatedBy] = CA.[ID]
		LEFT JOIN [sys_Account] UA (NOLOCK) ON [sys_Item].[UpdatedBy] = UA.[ID]
		LEFT JOIN [sys_ItemRelation] (NOLOCK) ON I.[ID] = [sys_ItemRelation].[IDParent]

		'+@Join+'
		WHERE ' + @QueryFilter +'
	) AS [DT]
 )
 SELECT * FROM [s] 
 WHERE [RowNum] BETWEEN 
    (@PageIndex - 1) * @PageSize + 1 AND @PageIndex * @PageSize
 ORDER BY [RowNum]
 '
 IF(@Print=1) PRINT @Query
 
 EXEC sp_executesql @Query, N'@PageSize int, @PageIndex int, @ItemId int', @PageSize, @PageIndex, @ItemId
 END

 /*
 sp_GetApprovalList
@PageIndex = 1 ,
@PageSize = 10,
@Type = 11,
@QueryFilter = '[Amount] > 3000',
@ItemId = 0,
@Print = 1
*/
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAvailableCols]    Script Date: 12/28/2022 10:35:49 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_GetChildTypeItem]    Script Date: 12/28/2022 10:35:49 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_GetIndirectParents]    Script Date: 12/28/2022 10:35:49 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_GetItems]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetItems]
@PageIndex int ,
@PageSize int,
@Type int,
@ParentId int = 0,
@Print bit = 0
AS
BEGIN
 DECLARE @Name nvarchar(128), @Query nvarchar(4000)
 SELECT @Name = REPLACE([Name],' ','') FROM [sys_ItemType] (NOLOCK) WHERE [ID] = @Type
 IF (@ParentId = 0)
 BEGIN
 SET @Query = N'
;WITH s AS
(  
	SELECT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY R.[UpdatedTime] DESC, R.[CreatedTime] DESC) AS [RowNum], I.*, A.[UserName] [CreatedBy]
	FROM [sys_Item] R (NOLOCK)
	JOIN ['+@Name+'] I (NOLOCK) ON R.[ID] = I.[ID]
	LEFT JOIN [sys_Account] A (NOLOCK) ON R.[CreatedBy] = A.[ID]
	WHERE  R.[Type] = @Type
  
)
'
END
ELSE
BEGIN
SET @Query = N'
;WITH s AS
(  
	SELECT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY R.[UpdatedTime] DESC, R.[CreatedTime] DESC) AS [RowNum], I.*, A.[UserName] [CreatedBy]
	FROM [sys_Item] R (NOLOCK)
	JOIN ['+@Name+'] I (NOLOCK) ON R.[ID] = I.[ID]
	LEFT JOIN [sys_Account] A (NOLOCK) ON R.[CreatedBy] = A.[ID]
	LEFT JOIN [sys_ItemRelation] (NOLOCK) IR ON R.[ID] = IR.[IDChild]
	WHERE  R.[Type] = @Type AND (IR.[IDParent] = @ParentId OR  @ParentId = 0)
  
)
'
END
SET @Query = @Query + '
SELECT * FROM s 
WHERE [RowNum] BETWEEN 
	(@PageIndex - 1) * @PageSize + 1 AND @PageIndex * @PageSize
ORDER BY [RowNum]
'
 IF(@Print=1) PRINT @Query

 EXEC sp_executesql @Query, N'@Type int, @ParentId int, @PageSize int, @PageIndex int', @Type, @ParentId, @PageSize, @PageIndex
END

/*
exec [sp_GetItems]
@PageIndex =1,
@PageSize = 10,
@Type=1,
@ParentId = 0
*/
GO
/****** Object:  StoredProcedure [dbo].[sp_GetListComment]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetListComment]
@PageIndex int ,
@PageSize int,
@ItemID int
AS

;WITH s AS
(  
	SELECT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY Cmt.[CreatedDate] DESC) AS [RowNum], Cmt.[ItemID], Acc.[Username], Cmt.[Content], Cmt.[CreatedDate]
	FROM [sys_Comment] Cmt (NOLOCK)
	LEFT JOIN [sys_Account] Acc (NOLOCK) ON Cmt.[AccountID] = Acc.[ID]
	WHERE Cmt.[ItemID] = @ItemID

)

SELECT * FROM s 
WHERE [RowNum] BETWEEN 
	(@PageIndex - 1) * @PageSize + 1 AND @PageIndex * @PageSize
GO
/****** Object:  StoredProcedure [dbo].[sp_GetParentItems]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetParentItems]
@PageIndex int ,
@PageSize int,
@Type int,
@ChildId int,
@Print bit = 0
AS
BEGIN
 DECLARE @Name nvarchar(128), @Query nvarchar(4000)
 SELECT @Name = [Name] FROM [sys_ItemType] (NOLOCK) WHERE [ID] = @Type
  SET @Query = N'
;WITH s AS
(  
	SELECT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY I.[ID]) AS [RowNum], A.*
	FROM [sys_Item] I (NOLOCK)
	JOIN ['+@Name+'] (NOLOCK) A ON I.[ID] = A.[ID]
	LEFT JOIN [sys_ItemRelation] (NOLOCK) IR ON I.[ID] = IR.[IDParent]
	WHERE  I.[Type] = @Type AND (IR.[IDChild] = @ChildId)
  
)

SELECT * FROM s 
WHERE [RowNum] BETWEEN 
	(@PageIndex - 1) * @PageSize + 1 AND @PageIndex * @PageSize
ORDER BY [RowNum]
'

 IF(@Print=1) PRINT @Query

 EXEC sp_executesql @Query, N'@Type int, @ChildId int, @PageSize int, @PageIndex int', @Type, @ChildId, @PageSize, @PageIndex
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPermissionOfRole]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetPermissionOfRole]
	@RoleID int
AS
	SELECT P.*
	FROM [sys_Permission] P (NOLOCK)
	LEFT JOIN [sys_RolePermission] RP (NOLOCK) ON P.[ID] = RP.[PermissionID]
	WHERE RP.[RoleID] = @RoleID
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPropertiesOfItem]    Script Date: 12/28/2022 10:35:49 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_GetRefItems]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetRefItems]
@PageIndex int ,
@PageSize int,
@Type int = 3,
@Filter nvarchar(4000),
@ItemId int,
@ParentId int = null,
@Print bit = 0
AS
BEGIN
DECLARE @Name NVARCHAR(128), @Query NVARCHAR(4000)
SELECT @Name = REPLACE([Name],' ','') FROM [sys_ItemType] (NOLOCK) WHERE [ID] = @Type

IF @ItemId > 0
	SELECT @ParentId = [IDParent] FROM [sys_ItemRelation] (NOLOCK)
	JOIN [sys_Item] (NOLOCK) ON  [sys_Item].[ID] = [sys_ItemRelation].IDParent 
	WHERE IDChild = @ItemId
IF @ParentId IS NULL SET @ParentId = 0
DECLARE @Join NVARCHAR(MAX) = ''
IF @ParentId > 0 AND EXISTS(
	SELECT 1 FROM [sys_Item] I (NOLOCK)
	JOIN [sys_ItemTypeRelation] IR (NOLOCK) ON IR.[ParentTypeID] = I.[Type]
	WHERE I.[ID] = @ParentId AND IR.[ChildTypeID] = @Type)
	SET @Join = 'JOIN [sys_ItemRelation] IR (NOLOCK) ON IR.[IDChild] = I.[ID] AND IR.[IDParent] = ' + CAST(@ParentId AS NVARCHAR)

 SET @Query = N'
 ;WITH [s] AS
 (  
    SELECT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY I.[ID]) AS [RowNum], I.*
    FROM ['+@Name+'] I (NOLOCK)
	'+@Join+'
    WHERE ' + @Filter +'
 )
 SELECT * FROM [s] 
 WHERE [RowNum] BETWEEN 
    (@PageIndex - 1) * @PageSize + 1 AND @PageIndex * @PageSize
 ORDER BY [RowNum]
 '
 IF(@Print=1) PRINT @Query

 EXEC sp_executesql @Query, N'@PageSize int, @PageIndex int', @PageSize, @PageIndex
 END
 /*
 [sp_GetRefItems]
@PageIndex = 1,
@PageSize = 10,
@Type = 3,
@Filter = '1=1',
@ItemId = 1068,
@ParentId = 0,
@Print  = 1
*/
GO
/****** Object:  StoredProcedure [dbo].[sp_SearchItems]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SearchItems]
@PageIndex int ,
@PageSize int,
@Type int,
@QueryFilter nvarchar(4000) = '1=1',
@Print bit = 0
AS
BEGIN
DECLARE @Name nvarchar(128), @Query nvarchar(4000)
SELECT @Name = REPLACE([Name],' ','') FROM [sys_ItemType] (NOLOCK) WHERE [ID] = @Type
DECLARE @Join nvarchar(max) = ''
SELECT @Join = @Join + 'LEFT JOIN ['+[dbo].[fnc_GetDataSource]([DataSource])+'] ['+[PropertyName]+'] (NOLOCK) ON I.['+ [PropertyName]+ '] = ['+[PropertyName]+'].[ID] ' 
FROM [sys_ItemTypeProperty]
WHERE [ItemType] = @Type AND [dbo].[fnc_GetDataSource]([DataSource]) IS NOT NULL


 SET @Query = N'
 ;WITH [s] AS
 (  
    SELECT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY R.[UpdatedTime] DESC, R.[CreatedTime] DESC) AS [RowNum], I.*, A.[UserName] [CreatedBy]
    FROM ['+@Name+'] I (NOLOCK)
	JOIN [sys_Item] R (NOLOCK) ON R.[ID] = I.[ID]
	LEFT JOIN [sys_Account] A (NOLOCK) ON R.[CreatedBy] = A.[ID]
    '+@Join+'
    WHERE ' + @QueryFilter +'
 )
 SELECT * FROM [s] 
 WHERE [RowNum] BETWEEN 
    (@PageIndex - 1) * @PageSize + 1 AND @PageIndex * @PageSize
 ORDER BY [RowNum]
 '
 IF(@Print=1) PRINT @Query

 EXEC sp_executesql @Query, N'@PageSize int, @PageIndex int', @PageSize, @PageIndex
 END

 /*
exec [sp_SearchItems]
@PageIndex = 1,
@PageSize = 10,
@Type = 8,
@QueryFilter = ' (R.[State] <> '' OR R.[State] IS NULL) ',
@Print = 0
*/
GO
/****** Object:  StoredProcedure [dbo].[sp_SelectChildItems]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
Get candidates to be selected as a child of @ParentId, 
@OtherParentId is optional, which means candidate must be already a child of @OtherParentId
*/
CREATE PROCEDURE [dbo].[sp_SelectChildItems]
@PageIndex int ,
@PageSize int,
@Type int,
@ParentId int,
@OtherParentId int = 0,
@Filter nvarchar(4000) = '1=1',
@Print bit = 0
AS
BEGIN
	DECLARE @Name nvarchar(128), @Query nvarchar(4000)
	SELECT @Name = REPLACE([Name],' ','') FROM [sys_ItemType] (NOLOCK) WHERE [ID] = @Type
	IF @Filter = '' OR @Filter IS NULL SET @Filter = '1=1'
	SET @Query = N'	
 ;WITH s AS
(  
SELECT DISTINCT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY i.[ID]) AS [RowNum], i.[ID], i.[Type]
FROM [sys_Item] i WITH(NOLOCK)
JOIN ['+@Name+'] A WITH (NOLOCK) ON i.[ID] = A.[ID]
WHERE i.[Type]=@Type AND i.[ID] NOT IN (SELECT IR2.[IDChild]
									FROM  [sys_ItemRelation] IR2 WITH(NOLOCK)
									WHERE IR2.[IDParent] = @ParentId)
									AND (@OtherParentId = 0 OR i.[ID] IN (SELECT IR3.[IDChild]
									FROM  [sys_ItemRelation] IR3 WITH(NOLOCK)
									WHERE IR3.[IDParent] = @OtherParentId))
	AND ' + @Filter +'

 )
SELECT * FROM s 
JOIN ['+@Name+'] A WITH (NOLOCK) ON s.[ID] = A.[ID]
 WHERE [RowNum] BETWEEN 
	(@PageIndex - 1) * @PageSize + 1 AND @PageIndex * @PageSize
 ORDER BY [RowNum];
	'
	IF(@Print=1) PRINT @Query
	EXEC sp_executesql @Query, N'@PageSize int, @PageIndex int, @Type int, @ParentId int, @OtherParentId int', @PageSize, @PageIndex, @Type, @ParentId, @OtherParentId
END
/*
[sp_SelectChildItems]
@PageIndex =1 ,
@PageSize =10,
@Type = 2,
@ParentId = 6,
@OtherParentId = 0,
@Print = 1
*/
GO
/****** Object:  StoredProcedure [dbo].[sp_ViewReport]    Script Date: 12/28/2022 10:35:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ViewReport]
@PageIndex int ,
@PageSize int,
@Type int,
@QueryFilter nvarchar(4000),
@OrderBy nvarchar(4000)='[ID]',
@Cols nvarchar(4000)='',
@Print bit = 0
AS
BEGIN
DECLARE @Name nvarchar(128), @Query nvarchar(4000)
SELECT @Name = REPLACE([Name],' ','') FROM [sys_ItemType] (NOLOCK) WHERE [ID] = @Type
DECLARE @Join nvarchar(max) = ''
SELECT @Join = @Join + 'LEFT JOIN ['+[dbo].[fnc_GetDataSource]([DataSource])+'] ['+[PropertyName]+'] (NOLOCK) ON I.['+ [PropertyName]+ '] = ['+[PropertyName]+'].[ID] ' 
FROM [sys_ItemTypeProperty]
WHERE [ItemType] = @Type AND [dbo].[fnc_GetDataSource]([DataSource]) IS NOT NULL

SET @Join = @Join+'
		'
SELECT @Join = @Join + ' LEFT JOIN ['+[sys_ItemType].[Name]+'] ON [sys_ItemRelation].[IDChild] = ['+[sys_ItemType].[Name]+'].[ID]' 
FROM [sys_ItemTypeRelation] (NOLOCK)
JOIN [sys_ItemType] (NOLOCK) ON [sys_ItemType].[ID] = [sys_ItemTypeRelation].[ChildTypeID]
WHERE [ParentTypeID] = @Type

IF @Cols=''
BEGIN
	SELECT @Cols = @Cols + 'I.['+[PropertyName]+'],' 
	FROM [sys_ItemTypeProperty]
	WHERE [ItemType] = @Type
	--SET @Cols = LEFT(@Cols, LEN(@Cols) - 1)
	SET @Cols = @Cols + '[sys_Item].[State],[sys_Item].[CreatedTime],[sys_Item].[UpdatedTime], CA.[UserName] [CreatedBy], UA.[UserName] [UpdatedBy]'

	SELECT @Cols = @Cols + ',['+T.[Name]+'].['+TP.PropertyName+'] ['+T.[Name]+'_'+TP.PropertyName+']' 
	--SELECT ['+T.[Name]+'].['+TP.PropertyName+'] ['+T.[Name]+'_'+TP.PropertyName+']'
	FROM [sys_ItemTypeRelation] TR (NOLOCK)
	JOIN [sys_ItemType] T (NOLOCK) ON T.[ID] = TR.[ChildTypeID]
	JOIN [sys_ItemTypeProperty] TP (NOLOCK) ON TP.[ItemType] = TR.[ChildTypeID]
	WHERE [ParentTypeID] = @Type
	ORDER BY T.[ID]
 
END 



 

 SET @Query = N'
 ;WITH [s] AS
 (  
    SELECT DISTINCT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY '+@OrderBy+') AS [RowNum], * FROM (
		SELECT DISTINCT I.[ID], '+@Cols+'
		FROM ['+@Name+'] I (NOLOCK)
		LEFT JOIN [sys_Item] (NOLOCK) ON [sys_Item].[ID] = I.[ID]
		LEFT JOIN [sys_Account] CA (NOLOCK) ON [sys_Item].[CreatedBy] = CA.[ID]
		LEFT JOIN [sys_Account] UA (NOLOCK) ON [sys_Item].[UpdatedBy] = UA.[ID]
		LEFT JOIN [sys_ItemRelation] (NOLOCK) ON I.[ID] = [sys_ItemRelation].[IDParent]
		'+@Join+'
		WHERE ' + @QueryFilter +'
	) AS [DT]
 )
 SELECT * FROM [s] 
 WHERE [RowNum] BETWEEN 
    (@PageIndex - 1) * @PageSize + 1 AND @PageIndex * @PageSize
 ORDER BY [RowNum]
 '
 IF(@Print=1) PRINT @Query
 
 EXEC sp_executesql @Query, N'@PageSize int, @PageIndex int', @PageSize, @PageIndex
 END
 /*
 [dbo].[sp_ViewReport]
@PageIndex = 1 ,
@PageSize = 1,
@Type = 1,
@QueryFilter = '1=1',
@Cols ='',
@Print = 1
*/
GO
