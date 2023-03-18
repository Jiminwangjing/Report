
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='FR004') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Cash Flow For Treasury', 'Report', 'FR004');
   PRINT('inserting function name "Cash Flow For Treasury"')
END
ELSE
BEGIN
	PRINT('=> function name "Cash Flow For Treasury" already existed')
END

IF not EXISTS (SELECT * FROM Funtion WHERE Code ='FR005') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Transaction Journal', 'Report', 'FR005');
   PRINT('inserting function name "Transaction Journal"')
END
ELSE
BEGIN
	PRINT('=> function name "Transaction Journal" already existed')
END

IF not EXISTS (SELECT * FROM Funtion WHERE Code ='FR006') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Sale Gross Profit', 'Report', 'FR006'); 
   PRINT('inserting function name "Sale Gross Profit"')
END
ELSE
BEGIN
	PRINT('=> function name "Sale Gross Profit" already existed')
END

IF not EXISTS (SELECT * FROM Funtion WHERE Code ='FR007') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Gerneral Ledger', 'Report', 'FR007'); 
   PRINT('inserting function name "Gerneral Ledger"')
END
ELSE
BEGIN
	PRINT('=> function name "Gerneral Ledger" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='SR009') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Sale Group Customer', 'Report', 'SR009'); 
   PRINT('inserting function name "Sale Group Customer"')
END
ELSE
BEGIN
	PRINT('=> function name "Sale Group Customer" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='P027') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Count Member', 'POS', 'P027'); 
   PRINT('inserting function name "Count Member"')
END
ELSE
BEGIN
	PRINT('=> function name "Count Member" already existed')
END

IF not EXISTS (SELECT * FROM Funtion WHERE Code ='SR010') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Count Member', 'Report', 'SR010'); 
   PRINT('inserting function name "Count Member"')
END
ELSE
BEGIN
	PRINT('=> function name "Count Member" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='DB001') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('General Dashboard', 'Admin', 'DB001');
   PRINT('inserting function name "General Dashboard"')
END
ELSE
BEGIN
	PRINT('=> function name "General Dashboard" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='DB002') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('CRM Dashboard For Mananger', 'Admin', 'DB002');
   PRINT('inserting function name "CRM Dashboard For Mananger"')
END
ELSE
BEGIN
	PRINT('=> function name "Transaction Journal" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='DB003') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Service Dashboard', 'Admin', 'DB003'); 
   PRINT('inserting function name "Service Dashboard"')
END
ELSE
BEGIN
	PRINT('=> function name "Sale Gross Profit" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='ST001') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Freight Setup', 'Admin', 'ST001'); 
   PRINT('inserting function name "Freight Setup"')
END
ELSE
BEGIN
	PRINT('=> function name "Freight Setup" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='ST002') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Setting Dashboard Setup', 'Admin', 'ST002'); 
   PRINT('inserting function name "Setting Dashboard Setup"')
END
ELSE
BEGIN
	PRINT('=> function name "Setting Dashboard Setup" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='ST003') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('General Setting Setup', 'Admin', 'ST003'); 
   PRINT('inserting function name "General Setting Setup"')
END
ELSE
BEGIN
	PRINT('=> function name "General Setting Setup" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='ST004') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Service Setup', 'Admin', 'ST004'); 
   PRINT('inserting function name "Service Setup"')
END
ELSE
BEGIN
	PRINT('=> function name "Service Setup" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='ST005') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Remark Discount Setup', 'Admin', 'ST005'); 
   PRINT('inserting function name "Remark Discount Setup"')
END
ELSE
BEGIN
	PRINT('=> function name "Remark Discount Setup" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='CRM001') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Opportunity', 'Admin', 'CRM001'); 
   PRINT('inserting function name "Opportunity"')
END
ELSE
BEGIN
	PRINT('=> function name "Opportunity" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='CRM002') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Activity', 'Admin', 'CRM002'); 
   PRINT('inserting function name "Activity"')
END
ELSE
BEGIN
	PRINT('=> function name "Activity" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='CRM003') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Opportunity Reports', 'Admin', 'CRM003'); 
   PRINT('inserting function name "Opportunity Reports"')
END
ELSE
BEGIN
	PRINT('=> function name "Opportunity Reports" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='CRM004') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Won Opportunity Reports', 'Admin', 'CRM004'); 
   PRINT('inserting function name "Won Opportunity Reports"')
END
ELSE
BEGIN
	PRINT('=> function name "Won Opportunity Reports" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='CRM005') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Lost Opportunity Reports', 'Admin', 'CRM005'); 
   PRINT('inserting function name "Lost Opportunity Reports"')
END
ELSE
BEGIN
	PRINT('=> function name "Lost Opportunity Reports" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='SV001') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Service Call', 'Admin', 'SV001'); 
   PRINT('inserting function name "Service Call"')
END
ELSE
BEGIN
	PRINT('=> function name "Service Call" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='SV002') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Service Call Report', 'Admin', 'SV002'); 
   PRINT('inserting function name "Service Call Report"')
END
ELSE
BEGIN
	PRINT('=> function name "Service Call Report" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='SV003') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Equipment Card', 'Admin', 'SV003'); 
   PRINT('inserting function name "Equipment Card"')
END
ELSE
BEGIN
	PRINT('=> function name "Equipment Card" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='CR001') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Can Ring Setup List', 'Admin', 'CR001'); 
   PRINT('inserting function name "Can Ring Setup List"')
END
ELSE
BEGIN
	PRINT('=> function name "Can Ring Setup List" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='CR002') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Create Update Can Ring Setup', 'Admin', 'CR002'); 
   PRINT('inserting function name "Create Update Can Ring Setup"')
END
ELSE
BEGIN
	PRINT('=> function name "Create Update Can Ring Setup" already existed')
END
IF not EXISTS (SELECT * FROM Funtion WHERE Code ='P028') 
BEGIN
   INSERT INTO Funtion(Name, [Type], Code) VALUES ('Can Ring', 'POS', 'P028'); 
   PRINT('inserting function name "Can Ring"')
END
ELSE
BEGIN
	PRINT('=> function name "Can Ring" already existed')
	select ID, Code from Funtion where Code = 'P028'
END


DECLARE @RowCnt BIGINT = 0;
DECLARE @FR004 BIGINT = 1;
DECLARE @FR006 BIGINT = 1;
DECLARE @FR007 BIGINT = 1;
DECLARE @FR005 BIGINT = 1;
DECLARE @SR009 BIGINT = 1;
DECLARE @P027 BIGINT = 1;
DECLARE @SR010 BIGINT = 1;
DECLARE @DB001 BIGINT = 1;
DECLARE @DB002 BIGINT = 1;
DECLARE @DB003 BIGINT = 1;

DECLARE @ST001 BIGINT = 1;
DECLARE @ST002 BIGINT = 1;
DECLARE @ST003 BIGINT = 1;
DECLARE @ST004 BIGINT = 1;
DECLARE @ST005 BIGINT = 1;

DECLARE @CRM001 BIGINT = 1;
DECLARE @CRM002 BIGINT = 1;
DECLARE @CRM003 BIGINT = 1;
DECLARE @CRM004 BIGINT = 1;
DECLARE @CRM005 BIGINT = 1;

DECLARE @SV001 BIGINT = 1;
DECLARE @SV002 BIGINT = 1;
DECLARE @SV003 BIGINT = 1;

DECLARE @CR001 BIGINT = 1;
DECLARE @CR002 BIGINT = 1;

DECLARE @P028 BIGINT = 0;
SELECT @RowCnt = COUNT(0) FROM tbUserAccount --where [Delete]=0;
 
WHILE @FR004 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='FR004' and UserID=@FR004) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@FR004, 141, 0, 0,'FR004');
		PRINT(CONCAT('inserting ', @FR004))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@FR004, ' already existed'))
	END
	SET @FR004= @FR004+ 1 
END
WHILE @FR006 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='FR006' and UserID=@FR006) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@FR006, 143, 0, 0,'FR006');
		PRINT(CONCAT('inserting ', @FR006))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@FR006, ' already existed'))
	END
	SET @FR006= @FR006+ 1 
END
WHILE @FR007 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='FR007' and UserID=@FR007) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@FR007, 144, 0, 0,'FR007');
		PRINT(CONCAT('inserting ', @FR007))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@FR007, ' already existed'))
	END
	SET @FR007= @FR007+ 1 
END
WHILE @FR005 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='FR005' and UserID=@FR005) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@FR005, 142, 0, 0,'FR005');
		PRINT(CONCAT('inserting ', @FR005))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@FR005, ' already existed'))
	END
	SET @FR005 = @FR005 + 1 
END

WHILE @SR009 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='SR009' and UserID=@SR009) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@SR009, 145, 0, 0,'SR009');
		PRINT(CONCAT('inserting ', @FR005))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@SR009, ' already existed'))
	END
	SET @SR009 = @SR009 + 1 
END
WHILE @P027 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='P027' and UserID=@P027) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@P027, 146, 0, 0,'P027');
		PRINT(CONCAT('inserting ', @P027))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@P027, ' already existed'))
	END
	SET @P027= @P027+ 1 
END
WHILE @SR010 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='SR010' and UserID=@SR010) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@SR010, 147, 0, 0,'SR010');
		PRINT(CONCAT('inserting ', @SR010))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@SR010, ' already existed'))
	END
	SET @SR010= @SR010+ 1 
END
WHILE @DB001 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='DB001' and UserID=@DB001) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@DB001, 148, 0, 0,'DB001');
		PRINT(CONCAT('inserting ', @DB001))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@DB001, ' already existed'))
	END
	SET @DB001= @DB001+ 1 
END
WHILE @DB002 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='DB002' and UserID=@DB002) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@DB002, 149, 0, 0,'DB002');
		PRINT(CONCAT('inserting ', @DB002))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@DB002, ' already existed'))
	END
	SET @DB002= @DB002+ 1 
END
WHILE @DB003 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='DB003' and UserID=@DB003) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@DB003, 150, 0, 0,'DB003');
		PRINT(CONCAT('inserting ', @DB003))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@DB002, ' already existed'))
	END
	SET @DB003 = @DB003+ 1 
END
WHILE @ST001 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='ST001' and UserID=@ST001) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@ST001, 151, 0, 0,'ST001');
		PRINT(CONCAT('inserting ', @ST001))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@ST001, ' already existed'))
	END
	SET @ST001 = @ST001 + 1 
END
WHILE @ST002 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='ST002' and UserID=@ST002) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@ST002, 152, 0, 0,'ST002');
		PRINT(CONCAT('inserting ', @ST002))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@ST002, ' already existed'))
	END
	SET @ST002 = @ST002 + 1 
END
WHILE @ST003 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='ST003' and UserID=@ST003) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@ST003, 153, 0, 0,'ST003');
		PRINT(CONCAT('inserting ', @ST003))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@ST003, ' already existed'))
	END
	SET @ST003= @ST003+ 1 
END
WHILE @ST004 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='ST004' and UserID=@ST004) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@ST004, 154, 0, 0,'ST004');
		PRINT(CONCAT('inserting ', @ST004))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@ST004, ' already existed'))
	END
	SET @ST004= @ST004+ 1 
END
WHILE @ST005 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='ST005' and UserID=@ST005) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@ST005, 155, 0, 0,'ST005');
		PRINT(CONCAT('inserting ', @ST005))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@ST005, ' already existed'))
	END
	SET @ST005= @ST005+ 1 
END
WHILE @CRM001 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='CRM001' and UserID=@CRM001) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@CRM001, 156, 0, 0,'CRM001');
		PRINT(CONCAT('inserting ', @CRM001))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@CRM001, ' already existed'))
	END
	SET @CRM001= @CRM001+ 1 
END
WHILE @CRM002 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='CRM002' and UserID=@CRM002) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@CRM002, 157, 0, 0,'CRM002');
		PRINT(CONCAT('inserting ', @CRM002))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@CRM002, ' already existed'))
	END
	SET @CRM002 = @CRM002+ 1 
END
WHILE @CRM003 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='CRM003' and UserID=@CRM003) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@CRM003, 158, 0, 0,'CRM003');
		PRINT(CONCAT('inserting ', @CRM003))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@CRM003, ' already existed'))
	END
	SET @CRM003 = @CRM003 + 1 
END
WHILE @CRM004 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='CRM004' and UserID=@CRM004) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@CRM004, 159, 0, 0,'CRM004');
		PRINT(CONCAT('inserting ', @CRM004))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@CRM004, ' already existed'))
	END
	SET @CRM004 = @CRM004 + 1 
END
WHILE @CRM005 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='CRM005' and UserID=@CRM005) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@CRM005, 160, 0, 0,'CRM005');
		PRINT(CONCAT('inserting ', @CRM005))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@CRM005, ' already existed'))
	END
	SET @CRM005= @CRM005+ 1 
END
WHILE @SV001 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='SV001' and UserID=@SV001) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@SV001, 161, 0, 0,'SV001');
		PRINT(CONCAT('inserting ', @SV001))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@SV001, ' already existed'))
	END
	SET @SV001= @SV001+ 1 
END
WHILE @SV002 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='SV002' and UserID=@SV002) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@SV002, 162, 0, 0,'SV002');
		PRINT(CONCAT('inserting ', @SV002))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@SV002, ' already existed'))
	END
	SET @SV002= @SV002+ 1 
END
WHILE @SV003 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='SV003' and UserID=@SV003) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@SV003, 163, 0, 0,'SV003');
		PRINT(CONCAT('inserting ', @SV003))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@SV003, ' already existed'))
	END
	SET @SV003= @SV003+ 1 
END
WHILE @CR001 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='CR001' and UserID=@CR001) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@CR001, 164, 0, 0,'CR001');
		PRINT(CONCAT('inserting ', @CR001))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@CR001, ' already existed'))
	END
	SET @CR001= @CR001+ 1 
END
WHILE @CR002 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='CR002' and UserID=@CR002) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@CR002, 165, 0, 0,'CR002');
		PRINT(CONCAT('inserting ', @CR002))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@CR002, ' already existed'))
	END
	SET @CR002= @CR002 + 1 
END
WHILE @P028 <= @RowCnt
BEGIN
	
	IF not EXISTS (SELECT * FROM tbUserPrivillege WHERE Code ='P028' and UserID=@P028) 
	BEGIN
		INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
		VALUES (@P028, 166, 0, 0,'P028');
		PRINT(CONCAT('inserting ', @P028))
	END
	ELSE
	BEGIN
		PRINT(CONCAT(@P028, ' already existed'))
	END
	SET @P028= @P028 + 1 
END
select * from Funtion where Code='P011'
select * from tbUserPrivillege where Code='P028'