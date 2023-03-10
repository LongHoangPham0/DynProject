USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  UserDefinedFunction [dbo].[fnc_GetDataSource]    Script Date: 12/28/2022 10:42:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnc_GetDataSource](@v nvarchar(256))
RETURNS nvarchar(64)
AS
BEGIN
	IF @v = '' OR LEFT(@v,1)='[' return NULL;
	DECLARE @pos int =  CHARINDEX(':', @v);
	IF @pos=0 RETURN @v;
	RETURN LEFT(@v,@pos-1);
END
GO
