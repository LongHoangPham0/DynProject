USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeletePermission]    Script Date: 12/28/2022 10:42:32 PM ******/
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
