USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteItemType]    Script Date: 12/28/2022 10:42:32 PM ******/
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
