USE [F:\DATA\SOURCE\DOTNET\DYNPROJECT\WEB\APP_DATA\TINHMOC.MDF]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetListComment]    Script Date: 12/28/2022 10:42:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetListComment]
@PageIndex int ,
@PageSize int,
@ItemID int
AS

;WITH s AS
(  
	SELECT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY Cmt.[CreatedDate] DESC) AS [RowNum], Cmt.[ItemID], Acc.[Username], Cmt.[Content], Cmt.[CreatedDate]
	FROM [sys_Comment] Cmt (NOLOCK)
	LEFT JOIN [sys_Account] Acc (NOLOCK) ON Cmt.[AccountID] = Acc.[ID]
	WHERE Cmt.[ItemID] = @ItemID

)

SELECT * FROM s 
WHERE [RowNum] BETWEEN 
	(@PageIndex - 1) * @PageSize + 1 AND @PageIndex * @PageSize
GO
