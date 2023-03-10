USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetRefItems]    Script Date: 12/28/2022 10:42:32 PM ******/
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
