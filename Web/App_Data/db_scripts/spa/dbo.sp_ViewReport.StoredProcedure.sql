USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_ViewReport]    Script Date: 12/28/2022 10:42:32 PM ******/
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
