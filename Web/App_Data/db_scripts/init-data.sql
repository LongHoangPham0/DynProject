SET IDENTITY_INSERT [dbo].[sys_Account] ON 
GO
INSERT [dbo].[sys_Account] ([ID], [Username], [Password], [RoleID]) VALUES (1, N'admin', N'21232f297a57a5a743894a0e4a801fc3', 1)
GO
SET IDENTITY_INSERT [dbo].[sys_Account] OFF
GO
SET IDENTITY_INSERT [dbo].[sys_Permission] ON 
GO
INSERT [dbo].[sys_Permission] ([ID], [Name], [Description]) VALUES (1, N'sys_manage_system', N'Manage system')
GO
INSERT [dbo].[sys_Permission] ([ID], [Name], [Description]) VALUES (2, N'sys_manage_items', N'Manage dynamic items')
GO
INSERT [dbo].[sys_Permission] ([ID], [Name], [Description]) VALUES (3, N'sys_manage_business', N'Manage business rules')
GO
INSERT [dbo].[sys_Permission] ([ID], [Name], [Description]) VALUES (4, N'sys_manage_approval', N'Manage approval rules')
GO
INSERT [dbo].[sys_Permission] ([ID], [Name], [Description]) VALUES (5, N'sys_manage_reporting', N'Manage reports')
GO
INSERT [dbo].[sys_Permission] ([ID], [Name], [Description]) VALUES (6, N'sys_access_reporting', N'Accessible to view reports')
GO
SET IDENTITY_INSERT [dbo].[sys_Permission] OFF
GO
SET IDENTITY_INSERT [dbo].[sys_Role] ON 
GO
INSERT [dbo].[sys_Role] ([ID], [Name], [Description], [RoleCode]) VALUES (1, N'Administrators', N'', N'admin')
GO
INSERT [dbo].[sys_Role] ([ID], [Name], [Description], [RoleCode]) VALUES (2, N'Users', N'', N'user')
GO
GO
SET IDENTITY_INSERT [dbo].[sys_Role] OFF
GO
INSERT [dbo].[sys_RolePermission] ([RoleID], [PermissionID]) VALUES (1, 1)
GO
INSERT [dbo].[sys_RolePermission] ([RoleID], [PermissionID]) VALUES (1, 2)
GO

