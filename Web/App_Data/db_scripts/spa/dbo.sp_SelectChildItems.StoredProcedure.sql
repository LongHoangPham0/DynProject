USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_SelectChildItems]    Script Date: 12/28/2022 10:42:32 PM ******/
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
