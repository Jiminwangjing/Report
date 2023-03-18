truncate table tbOrderDetail;
delete from tbOrder;
dbcc checkident(tbOrder, reseed, 0);
UPDATE tbWarehouseSummary set [Committed] = 0;