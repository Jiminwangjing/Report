--V 1.3.3
--Update document type
IF NOT EXISTS(select 1 from tbDocumentType where Code = 'RE')
INSERT INTO tbDocumentType(Code, [Name]) VALUES('RE', 'Return Delivery');

IF NOT EXISTS(select 1 from tbDocumentType where Code = 'RD')
INSERT INTO tbDocumentType(Code, [Name]) VALUES('RD', 'Redeem Point');

IF NOT EXISTS(select 1 from tbDocumentType where Code = 'US')
INSERT INTO tbDocumentType(Code, [Name]) VALUES('US', 'Use Service');

