USE [DatabaseSaleAdmin]
GO

/****** Object:  StoredProcedure [dbo].[admin_SaleCreditMemo]    Script Date: 8/25/2020 11:41:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[admin_SaleCreditMemo](@OrderID int, @TransactType nvarchar(max))
AS
BEGIN
--Insert data to tmp table
insert into tmpOrderDetail(ItemID,Qty,UomID,Cost,UnitPrice,CurrencyID,Process,WarehouseID,BranchID,UserID,InvoiceNo,ExchageRate,[ExpireDate])
select ItemID,isnull(Qty,0),od.UomID,od.Cost*o.ExchangeRate,od.UnitPrice*o.ExchangeRate,o.LocalCurrencyID,item.Process,o.WarehouseID,o.BranchID,UserID,InvoiceNo,ExchangeRate,'0001-01-01'
                                    from tbSaleCreditMemoDetail od
                                    inner join tbSaleCreditMemo o on od.SCMOID=o.SCMOID
									inner join tbItemMasterData item on od.ItemID=item.ID
									where o.SCMOID=@OrderID

--Declare goble vaiable
Declare @LineID int 
Declare @ItemID int
Declare @Qty float
Declare @UomID int
Declare @Cost float
Declare @UnitPrice float
Declare @CurrencyID int 
Declare @Process nvarchar(max)
Declare @WarehouseID int
Declare @BranchID int 
Declare @UserID int
Declare @InvoiceNo nvarchar(max)


While(select count(*) from tmpOrderDetail)>0
Begin
    declare @CumulativeQty float
	declare @Tran_Value float
	declare @ExchangRate float
	declare @ExpireDate date
	declare @CumulativeValue float
	select top 1 @LineID=ID,@ItemID=ItemID,@Qty=Qty,@UomID=UomID,@Cost=Cost,@UnitPrice=UnitPrice,@CurrencyID=CurrencyID,@Process=Process,@WarehouseID=WarehouseID,@BranchID=BranchID,@UserID=UserID,@InvoiceNo=InvoiceNo,@ExchangRate=ExchageRate,@ExpireDate=[ExpireDate] from tmpOrderDetail
	IF(@Process!='Standard')
		Begin
					--uom factor
					declare @Factor float=0
					declare @GUomId int=0
					declare @InvUomID int=0
					select @GUomId=GroupUomID,@InvUomID=InventoryUoMID from tbItemMasterData where ID=@ItemID
					select @Factor=isnull(Factor,0) from tbGroupDefindUoM where GroupUoMID=@GUomId and AltUOM=@UomID
						
					set @Qty=@Qty*@Factor
					set @UomID=@InvUomID

					--update stock item master
						Declare @Order_master float 
						Declare @Instock_master_po float
						Select @Order_master=isnull(StockOnHand,0),@Instock_master_po=ISNULL(StockIn,0) from tbItemMasterData where ID=@ItemID
						UPDATE tbItemMasterData 
						SET 
							StockIn=@Instock_master_po+@Qty
							where ID=@ItemID
	     
					--update stock in warehouse summary
					Declare @Order_Whs float
					Declare @Instock_Whs_po float
					Select @Order_Whs=isnull([Committed],0),@Instock_Whs_po=isnull(InStock,0) from tbWarehouseSummary where ItemID=@ItemID and WarehouseID=@WarehouseID
		
					UPDATE tbWarehouseSummary 
						SET InStock=@Instock_Whs_po+@Qty
						where ItemID=@ItemID AND WarehouseID=@WarehouseID

					--update stock in warehouse detail
					update tbWarehouseDetail
					set InStock=InStock+@Qty where ItemID=@ItemID and UomID=@UomID and Cost=@Cost and [ExpireDate]=@ExpireDate
					set @Cost = 0.208333333333333;
					 --insert to inventory audit report
					select @Tran_Value=ISNULL((@Qty*@Cost/@Factor),0),@CumulativeQty=ISNULL(sum(Qty),0)+(@Qty),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@Qty*@Cost)) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
					insert into tbInventoryAudit(WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate])
					values(@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@Qty,@Cost,@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,@ExpireDate)
					--insert to revenues audit				 
					select @Tran_Value=ISNULL((@Qty*@Cost),0),@CumulativeQty=ISNULL(sum(Qty),0)+(@Qty),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@Qty*@Cost)) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
					insert into tbRevenueItem(ReceiptID,WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate],OpenQty)
					values(@OrderID,@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@Qty,@Cost*-1,@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value*-1,@ExpireDate,0)
								
					delete tmpOrderDetail where ID=@LineID
				
		End
	 Else 
		Begin
			--insert to revenues audit
			select @Tran_Value=ISNULL((@Qty*@Cost),0),@CumulativeQty=ISNULL(sum(Qty),0)+(@Qty),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@Qty*@Cost)) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
			insert into tbRevenueItem(ReceiptID,WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate],OpenQty)
			values(@OrderID,@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@Qty,@Cost*-1,@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value*-1,@ExpireDate,0)
								
			delete tmpOrderDetail where ID=@LineID
		End

End

END






















GO


