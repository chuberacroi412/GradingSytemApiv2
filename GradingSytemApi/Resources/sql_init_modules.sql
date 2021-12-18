GO
BEGIN TRAN
BEGIN TRY
	SET IDENTITY_INSERT [dbo].[Modules] ON

	IF NOT EXISTS (SELECT * FROM [dbo].[Modules] WHERE [Id] = 1)
	BEGIN
		INSERT INTO Modules([Deleted], [Active], [ParentModuleId], [Name], [IsParent], [Id]) VALUES (0, 1, null, 'Account', 1, 1)
	END

	IF NOT EXISTS (SELECT * FROM [dbo].[Modules] WHERE [Id] = 2)
	BEGIN
		INSERT INTO Modules([Deleted], [Active], [ParentModuleId], [Name], [IsParent], [Id]) VALUES (0, 1, null, 'Workspace', 1, 2)
	END

	IF NOT EXISTS (SELECT * FROM [dbo].[Modules] WHERE [Id] = 3)
	BEGIN
		INSERT INTO Modules([Deleted], [Active], [ParentModuleId], [Name], [IsParent], [Id]) VALUES (0, 1, null, 'Advances', 1, 3)
	END

	IF NOT EXISTS (SELECT * FROM [dbo].[Modules] WHERE [Id] = 4)
	BEGIN
		INSERT INTO Modules([Deleted], [Active], [ParentModuleId], [Name], [IsParent], [Id]) VALUES (0, 1, 2, 'Course', 0, 4)
	END

	IF NOT EXISTS (SELECT * FROM [dbo].[Modules] WHERE [Id] = 5)
	BEGIN
		INSERT INTO Modules([Deleted], [Active], [ParentModuleId], [Name], [IsParent], [Id]) VALUES (0, 1, 3, 'Manage Role', 0, 5)
	END

	IF NOT EXISTS (SELECT * FROM [dbo].[Modules] WHERE [Id] = 6)
	BEGIN
		INSERT INTO Modules([Deleted], [Active], [ParentModuleId], [Name], [IsParent], [Id]) VALUES (0, 1, 3, 'Manage User', 0, 6)
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[Modules] WHERE [Id] = 7)
	BEGIN
		INSERT INTO Modules([Deleted], [Active], [ParentModuleId], [Name], [IsParent], [Id]) VALUES (0, 1, 3, 'Manage Course', 0, 7)
	END

	SET IDENTITY_INSERT [dbo].[Modules] OFF
	COMMIT
END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE() As ErrorMessage;
	ROLLBACK TRAN
END CATCH