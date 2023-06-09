
/****** Object:  StoredProcedure [dbo].[pos_OrderDetailCommittedStock]    Script Date: 9/21/2021 4:50:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[pos_OrderDetailCommittedStock](@OrderID int)
AS
BEGIN
--Insert data to tmp table
insert into tmpOrderDetail(ItemID,Qty,UomID,Cost,UnitPrice,CurrencyID,Process,WarehouseID,BranchID,UserID,InvoiceNo,ExchageRate,[ExpireDate])
select ItemID,isnull(PrintQty,0),od.UomID,od.Cost,od.UnitPrice ,o.LocalCurrencyID,item.Process,o.WarehouseID,BranchID,UserOrderID,ReceiptNo ,ExchangeRate,'0001-01-01'
                                    from tbOrderDetail od 
                                    inner join tbOrder o on od.OrderID=o.OrderID
									inner join tbItemMasterData item on od.ItemID=item.ID
									where PrintQty !=0 and o.OrderID=@OrderID and item.Process!='Standard'
--Declare goble vaiable
Declare @LineID int 
Declare @ItemID int
Declare @Qty float
Declare @UomID int

While(select count(*) from tmpOrderDetail) > 0
Begin
	declare @Commited float
	declare @WarehouseID int
	select top 1 @LineID=ID,@ItemID=ItemID,@Qty=Qty,@UomID=UomID,@WarehouseID=WarehouseID,@LineID=ID from tmpOrderDetail
	select @Commited=SUM([Committed]) from tbWarehouseSummary where ItemID=@ItemID and WarehouseID=@WarehouseID
	  --uom factor
	declare @Factor float=0
	declare @GUomId int=0
	select @GUomId=GroupUomID from tbItemMasterData where ID=@ItemID
	select @Factor=isnull(Factor,0) from tbGroupDefindUoM where GroupUoMID=@GUomId and AltUOM=@UomID

	update tbWarehouseSummary
	set [Committed] = @Commited + @Qty*@Factor
	where ItemID=@ItemID and WarehouseID=@WarehouseID
	delete tmpOrderDetail where ID=@LineID

End

END




















