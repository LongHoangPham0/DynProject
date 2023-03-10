USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_SearchItems]    Script Date: 12/28/2022 10:42:32 PM ******/
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
