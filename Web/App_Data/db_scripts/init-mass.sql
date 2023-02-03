Set NoCount ON;
GO
declare @ID int, @cID nvarchar(64), @OVID int, @cOVID nvarchar(64), @taskCnt int, @cnt int = 0

WhILE @cnt < 50000
BEGIN
	insert into [sys_Item] ([Type], [CreatedBy], [CreatedTime])
	values(1, 1, GETUTCDATE())
	SET @ID = @@IDENTITY

	SET @cID = cast(@ID as nvarchar)

	insert into [Case] ([ID], [CaseNumber], CaseDetail,[Status], [ContactEmail])
	values(@ID, 'C00'+@cID, 'This is description of the case number C00' + @cID, 'new', 'case@ab'+@cID+'.com')


	--insert overviewinfo
	insert into [sys_Item] ([Type], [CreatedBy], [CreatedTime])
	values(5, 1, GETUTCDATE())
	SET @OVID = @@IDENTITY

	SET @cOVID = cast(@OVID as nvarchar)
	insert into [OverviewInfo] ([ID], [Address], [Email], [Phone])
	values(@OVID, 'Address of overview info '+ @cOVID, 'ovi@xz'+@cOVID+'.net', '0234230'+ cast(floor(rand()*10000) as nvarchar))

	insert into [sys_ItemRelation](IDParent, IDChild)
	values(@ID, @OVID)

	SET @taskCnt = 0
	WhILE @taskCnt < 3
	BEGIN
		--insert tasks

		insert into [sys_Item] ([Type], [CreatedBy], [CreatedTime])
		values(2, 1, GETUTCDATE())
		SET @OVID = @@IDENTITY

		SET @cOVID = cast(@OVID as nvarchar)
		insert into [Task]([ID], [TaskName], [AssignedTo])
		values(@OVID, 'Task #'+ @cOVID + ' of case '+ @cID, case when cast(floor(rand()*10000) as int)%2=1 then 4 else 5 end)

		insert into [sys_ItemRelation](IDParent, IDChild)
		values(@ID, @OVID)

		SET @taskCnt += 1;
	END
	SET @cnt += 1;
END
GO
Set NoCount OFF;
GO