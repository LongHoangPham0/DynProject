USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteItem]    Script Date: 12/28/2022 10:42:32 PM ******/
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
