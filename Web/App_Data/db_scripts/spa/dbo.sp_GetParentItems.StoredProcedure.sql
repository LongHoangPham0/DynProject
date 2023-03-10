USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetParentItems]    Script Date: 12/28/2022 10:42:32 PM ******/
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
