USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteItemTypeProperty]    Script Date: 12/28/2022 10:42:32 PM ******/
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
