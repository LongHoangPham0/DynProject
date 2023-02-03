USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  UserDefinedFunction [dbo].[fnc_GetParentTypeList]    Script Date: 12/28/2022 10:42:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnc_GetParentTypeList](@TypeID INT)
RETURNS @TB TABLE([ID] INT,[Level] INT)
AS
BEGIN
	DECLARE @Level INT = 0
	DECLARE @TB_LAST TABLE([ID] INT)
	DECLARE @TB_TMP TABLE([ID] INT)
	
	INSERT INTO @TB_LAST([ID]) VALUES(@TypeID)
	
	INSERT INTO @TB
	SELECT [ID],@Level
	FROM  @TB_LAST
	
	WHILE(EXISTS(SELECT * FROM @TB_LAST))
	BEGIN
	
		DELETE @TB_TMP
		INSERT INTO @TB_TMP
		SELECT [ID]
		FROM  @TB_LAST
		
		DELETE @TB_LAST
		SET @Level = @Level+1
		
		 
		INSERT INTO @TB_LAST 
		SELECT [ParentTypeID]
		FROM [sys_ItemTypeRelation] (NOLOCK)
		WHERE [ChildTypeID] IN (SELECT [ID] FROM @TB_TMP)
			AND [ParentTypeID] NOT IN (SELECT [ID] FROM @TB)
		
		INSERT INTO @TB
		SELECT [ID],@Level
		FROM  @TB_LAST
		
	END
	RETURN;
END
GO
