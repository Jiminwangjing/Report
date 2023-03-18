
delete from tbOrderDetail;
dbcc checkident(tbOrderDetail, reseed, 0);
delete from tbOrder;
dbcc checkident(tbOrder, reseed, 0);

update tbTable set [Time] = '00:00:00', [Status] = 'A', 
	StartDateTime = '0001-01-01 00:00:00.0000000 +00:00',
	EndDateTime = '0001-01-01 00:00:00.0000000 +00:00';