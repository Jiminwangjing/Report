USE [KWEBPOSV1.3.3.CardMember]
GO
SET IDENTITY_INSERT [dbo].[GeneralServiceSetups] ON 

INSERT [dbo].[GeneralServiceSetups] ([ID], [Code], [Name], [Active]) VALUES (1, N'DueDate', N'Due Date', 0)
INSERT [dbo].[GeneralServiceSetups] ([ID], [Code], [Name], [Active]) VALUES (2, N'Stock', N'Stock', 0)
INSERT [dbo].[GeneralServiceSetups] ([ID], [Code], [Name], [Active]) VALUES (3, N'ExItem', N'Expiration Item', 0)
SET IDENTITY_INSERT [dbo].[GeneralServiceSetups] OFF
