USE [KWEBPOSV1.3.3.CardMember]
GO
SET IDENTITY_INSERT [dbo].[SaleGLADeter] ON 

INSERT [dbo].[SaleGLADeter] ([ID], [CusID], [AccountMemberCardID]) VALUES (1, 1, 1)
SET IDENTITY_INSERT [dbo].[SaleGLADeter] OFF
SET IDENTITY_INSERT [dbo].[SaleGLAccountDetermination] ON 

INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (1, N'Domestic Accounts Receivable', 0, 0, N'DAR', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (2, N'Foreign Accounts Receivable', 0, 0, N'FAR', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (3, N'Checks Received', 0, 0, N'CR', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (4, N'Cash on Hand', 0, 0, N'COH', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (6, N'Overpayment A/R Account', 0, 0, N'OARA', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (7, N'Underpayment A/R Account', 0, 0, N'UARA', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (8, N'Down Payment Clearing Account', 0, 17, N'DPCA', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (9, N'Realized Exchange Diff. Gain', 0, 0, N'REDG', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (10, N'Realized Exchange Diff. Loss', 0, 0, N'REDL', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (11, N'Cash Discount', 0, 0, N'CD', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (12, N'Revenue Account', 0, 0, N'RA', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (13, N'Revenue Account - Foreign', 0, 0, N'RAF', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (14, N'Revenue Account - EU', 0, 0, N'RAEU', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (15, N'Sales Credit Account', 0, 0, N'SCA', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (16, N'Sales Credit Account - Foreign', 0, 0, N'SCAF', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (17, N'Sales Credit Account - EU', 0, 0, N'SCAEU', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (18, N'Down Payment Interim Account', 0, 0, N'DPIA', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (19, N'Dunning Interest', 0, 0, N'DI', 1)
INSERT [dbo].[SaleGLAccountDetermination] ([ID], [TypeOfAccount], [CusID], [GLID], [Code], [SaleGLDeterminationMasterID]) VALUES (20, N'Dunning Fee', 0, 0, N'DF', 1)
SET IDENTITY_INSERT [dbo].[SaleGLAccountDetermination] OFF
