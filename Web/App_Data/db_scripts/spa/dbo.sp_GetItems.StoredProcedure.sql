USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetItems]    Script Date: 12/28/2022 10:42:32 PM ******/
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
