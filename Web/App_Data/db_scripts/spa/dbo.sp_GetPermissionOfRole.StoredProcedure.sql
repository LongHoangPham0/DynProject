USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPermissionOfRole]    Script Date: 12/28/2022 10:42:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetPermissionOfRole]
	@RoleID int
AS
	SELECT P.*
	FROM [sys_Permission] P (NOLOCK)
	LEFT JOIN [sys_RolePermission] RP (NOLOCK) ON P.[ID] = RP.[PermissionID]
	WHERE RP.[RoleID] = @RoleID
GO
