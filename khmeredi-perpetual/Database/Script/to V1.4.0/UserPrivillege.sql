
if not exists(select top 1 * from Funtion where Code='P026') 
	INSERT INTO Funtion(Name, [Type], Code)
	VALUES ('Delete Pending Void Item', 'POS', 'P026');
else
	print('Data already existed in table');

declare @FID int = 0;
select top 1 @FID=ID from Funtion where Code='P026'
--user 1
if not exists(select top 1 * from tbUserPrivillege where Code='P026' and UserID=1 and FunctionID=@FID) 
	INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
	VALUES (1, @FID, 1, 0,'P026');

--user 2
if not exists(select top 1 * from tbUserPrivillege where Code='P026' and UserID=2 and FunctionID=@FID) 
	INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
	VALUES (2, @FID, 1, 0,'P026');

--user 3
if not exists(select top 1 * from tbUserPrivillege where Code='P026' and UserID=3 and FunctionID=@FID) 
	INSERT INTO tbUserPrivillege(UserID, FunctionID, Used, [Delete], Code)
	VALUES (3, @FID, 1, 0,'P026')