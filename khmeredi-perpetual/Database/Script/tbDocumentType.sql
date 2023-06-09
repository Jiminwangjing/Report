USE [KWEBPOSV1.3.3.CardMember]
GO
SET IDENTITY_INSERT [dbo].[tbDocumentType] ON 

INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (1, N'JE', N'Journal Entry')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (2, N'PO', N'Purchase Order')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (3, N'PD', N'Goods Receipt PO')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (4, N'PU', N'Purchase AP')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (5, N'PC', N'A/P Credit Memo')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (6, N'SQ', N'Sale Quotation')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (7, N'SO', N'Sale Order')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (8, N'DN', N'Sale Delivery')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (9, N'IN', N'Sale A/R')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (10, N'CN', N'Sale Credit Memo')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (11, N'PS', N'Outgoing Payment')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (12, N'RC', N'Incoming Payment')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (13, N'GR', N'Goods Receipt')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (14, N'GI', N'Goods Issue')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (15, N'ST', N'Transfer')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (16, N'SP', N'Sale POS')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (17, N'RP', N'Return POS')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (18, N'PQ', N'POS Quotation')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (19, N'RE', N'Return Delivery')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (20, N'US', N'Use Service')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (21, N'CM', N'Card Member')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (22, N'SC', N'Service Call')
INSERT [dbo].[tbDocumentType] ([ID], [Code], [Name]) VALUES (23, N'CD', N'A/R Down Payment')
SET IDENTITY_INSERT [dbo].[tbDocumentType] OFF
