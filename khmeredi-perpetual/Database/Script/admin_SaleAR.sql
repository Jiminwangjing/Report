USE [DatabaseSaleAdmin]
GO

/****** Object:  StoredProcedure [dbo].[admin_SaleAR]    Script Date: 8/25/2020 11:37:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE PROCEDURE [dbo].[admin_SaleAR](@OrderID int, @TransactType nvarchar(max))
AS
BEGIN
--Insert data to tmp table
insert into tmpOrderDetail(ItemID,Qty,UomID,Cost,UnitPrice,CurrencyID,Process,WarehouseID,BranchID,UserID,InvoiceNo,ExchageRate,[ExpireDate])
select ItemID,isnull(Qty,0),od.UomID,od.Cost*o.ExchangeRate,od.UnitPrice*o.ExchangeRate,o.LocalCurrencyID,item.Process,o.WarehouseID,BranchID,UserID,InvoiceNo,ExchangeRate,'0001-01-01'
                                    from tbSaleARDetail od 
                                    inner join tbSaleAR o on od.SARID=o.SARID
									inner join tbItemMasterData item on od.ItemID=item.ID
									where Qty!=0 and o.SARID=@OrderID
--Declare goble vaiable
Declare @LineID int 
Declare @ItemID int
Declare @Qty decimal(30,10)
Declare @UomID int
Declare @Cost decimal(30,10)
Declare @UnitPrice decimal(30,10)
Declare @CurrencyID int 
Declare @Process nvarchar(max)
Declare @WarehouseID int
Declare @BranchID int 
Declare @UserID int
Declare @InvoiceNo nvarchar(max)

While(select count(*) from tmpOrderDetail)>0
Begin
	declare @Instock decimal(30,10)
	declare @Check_Stock decimal(30,10)
	declare @CommitQty decimal(30,10)
	declare @Remain decimal(30,10)
	declare @IssusQty decimal(30,10)
	declare @Order_Qty decimal(30,10)
	declare @FIFOQty decimal(30,10)
	declare @FIFOCost decimal(30,10)
	declare @AvgCost decimal(30,10)
	declare @Tran_Value decimal(30,10)
	declare @CumulativeQty decimal(30,10)
	declare @CumulativeValue decimal(30,10)
	declare @ExpireDate date
	declare @StockMoveLineID int
	Declare @ExchangRate decimal(30,10)
	declare @WarehoseDetailLineID int
	

	select top 1 @LineID=ID,@ItemID=ItemID,@Qty=Qty,@UomID=UomID,@Cost=Cost,@UnitPrice=UnitPrice,@CurrencyID=CurrencyID,@Process=Process,@WarehouseID=WarehouseID,@BranchID=BranchID,@UserID=UserID,@InvoiceNo=InvoiceNo,@ExchangRate=ExchageRate from tmpOrderDetail
	IF(@Process!='Standard')
		Begin
		    INSERT INTO tbStockMoving(WarehoseDetailLineID,WarehouseID,UomID,UserID,SyetemDate,TimeIn,InStock,[Committed],Ordered,Available,CurrencyID,[ExpireDate],ItemID,Cost)
			SELECT ID,WarehouseID,UomID,UserID,SyetemDate,TimeIn,InStock,[Committed],Ordered,Available,CurrencyID,[ExpireDate],ItemID,Cost from tbWarehouseDetail 
			                                                                                                                            where ItemID=@ItemID --and UomID=@UomID
																																		order by [ExpireDate] ASC 
			
			
			--uom factor
					declare @Factor decimal(30,10)=0
					declare @GUomId int=0
					declare @InvUomID int=0
					select @GUomId=GroupUomID,@InvUomID=InventoryUoMID from tbItemMasterData where ID=@ItemID
					select @Factor=isnull(Factor,0) from tbGroupDefindUoM where GroupUoMID=@GUomId and AltUOM=@UomID

						
					set @Qty=@Qty*@Factor
					set @UomID=@InvUomID

					--update stock item master
						Declare @Order_master decimal(30,10) 
						Declare @Instock_master_po decimal(30,10)
						Select @Order_master=isnull(StockOnHand,0),@Instock_master_po=ISNULL(StockIn,0) from tbItemMasterData where ID=@ItemID
						UPDATE tbItemMasterData 
						SET 
							--StockCommit=@Order_master-@Qty*@Factor,
							StockIn=@Instock_master_po-@Qty
							where ID=@ItemID
	     
					--update stock in warehouse summary
					Declare @Order_Whs decimal(30,10)
					Declare @Instock_Whs_po decimal(30,10)
					Select @Order_Whs=isnull([Committed],0),@Instock_Whs_po=isnull(InStock,0) from tbWarehouseSummary where ItemID=@ItemID and WarehouseID=@WarehouseID
		
					UPDATE tbWarehouseSummary 
						SET [Committed]=@Order_Whs-@Qty,
							InStock=@Instock_Whs_po-@Qty
						where ItemID=@ItemID AND WarehouseID=@WarehouseID
				   --End
			while(select count(*) from tbStockMoving where InStock>0)>0
				begin
					declare @ItemID_SM int
					declare @UomID_SM int
					select top 1 @InStock=Instock,@FIFOCost=Cost,@ExpireDate=[ExpireDate],@StockMoveLineID=ID,@ItemID_SM=ItemID,@UomID_SM=UomID,@WarehoseDetailLineID=WarehoseDetailLineID from tbStockMoving
					
				
					set @Check_Stock=@Instock-@Qty

					if(@Check_Stock<0)
						begin

						    set @Remain=(@Instock-@Qty)*(-1)
							set @IssusQty = @Qty-@Remain
							if(@Remain<=0)
								begin
									set @Qty=0
								end
							else
								begin
									set @Qty=@Remain
								end
							
							if(@Process='FIFO')
								begin
									
									update tbWarehouseDetail
									set InStock=isnull(@Instock-@IssusQty,0)  where ID=@WarehoseDetailLineID --WarehouseID=@WarehouseID and ItemID=@ItemID_SM and UomID=@UomID_SM and Cost=@FIFOCost and [ExpireDate]=@ExpireDate
									update tbStockMoving
									set InStock=isnull(@Instock-@IssusQty,0) where ID=@StockMoveLineID
									delete tbStockMoving where ID=@StockMoveLineID

									IF @IssusQty>0
									begin
										 --insert to inventory audit report
										select @Tran_Value=ISNULL((@IssusQty*@FIFOCost/@Factor)*-1,0),@CumulativeQty=ISNULL(sum(Qty),0)+(@IssusQty*-1),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@IssusQty*@FIFOCost)*-1) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
										insert into tbInventoryAudit(WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate])
										values(@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@IssusQty*-1,@FIFOCost,@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,@ExpireDate)
										 --insert to revenues audit
										 
										select @Tran_Value=ISNULL((@IssusQty*@FIFOCost),0),@CumulativeQty=ISNULL(sum(Qty),0)+(@IssusQty*-1),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@IssusQty*@FIFOCost)*-1) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
										insert into tbRevenueItem(ReceiptID,WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate],OpenQty)
										values(0,@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@IssusQty,@FIFOCost,@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,@ExpireDate,@IssusQty)
									end
								end
						    else
								begin
									update tbWarehouseDetail
									set InStock=isnull(@Instock-@IssusQty,0)  where ID=@WarehoseDetailLineID --where WarehouseID=@WarehouseID and ItemID=@ItemID_SM and UomID=@UomID_SM and [ExpireDate]=@ExpireDate
									update tbStockMoving
									set InStock=isnull(@Instock-@IssusQty,0) where ID=@StockMoveLineID
									delete tbStockMoving where ID=@StockMoveLineID

									IF @IssusQty>0
									Begin
										 --insert to inventory audit report 
										select @FIFOCost=isnull(Cost,0) from tbPriceListDetail where ItemID=@ItemID_SM and UomID=@UomID_SM
										select @Tran_Value=ISNULL((@IssusQty*@FIFOCost)*-1,0),@CumulativeQty=ISNULL(sum(Qty),0)+(@IssusQty*-1),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@IssusQty*@FIFOCost)*-1) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
										set @AvgCost=nullif(@CumulativeValue,0)/nullif(@CumulativeQty,0)
										insert into tbInventoryAudit(WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate])
										values(@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@IssusQty*-1,@AvgCost,@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,@ExpireDate)
										 --insert to revenues audit
										
										select @FIFOCost=isnull(Cost,0) from tbPriceListDetail where ItemID=@ItemID_SM and UomID=@UomID_SM
										select @Tran_Value=ISNULL((@IssusQty*@FIFOCost),0),@CumulativeQty=ISNULL(sum(Qty),0)+(@IssusQty*-1),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@IssusQty*@FIFOCost)*-1) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
										set @AvgCost=nullif(@CumulativeValue,0)/nullif(@CumulativeQty,0)
										insert into tbRevenueItem(ReceiptID,WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate],OpenQty)
										values(0,@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@IssusQty,isnull(@AvgCost,0),@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,@ExpireDate,@IssusQty)
								
									End
								end

						end
					else
						begin
							set @FIFOQty=@Instock-@Qty
							set @IssusQty=@Instock-@FIFOQty
							
						    --insert to inventory audit report
							if(@Process='FIFO')
								begin
									update tbWarehouseDetail
									set InStock=isnull(@FIFOQty,0)   where ID=@WarehoseDetailLineID--where WarehouseID=@WarehouseID and ItemID=@ItemID_SM and UomID=@UomID_SM and Cost=@FIFOCost and [ExpireDate]=@ExpireDate
									update tbStockMoving
									set InStock=@FIFOQty where ID=@StockMoveLineID

									IF @IssusQty>0
									Begin
										--insert to inventory audit
										select @Tran_Value=ISNULL((@IssusQty*@FIFOCost)*-1,0),@CumulativeQty=ISNULL(sum(Qty),0)+(@IssusQty*-1),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@IssusQty*@FIFOCost)*-1) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
										insert into tbInventoryAudit(WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate])
										values(@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@IssusQty*-1,@FIFOCost,@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,@ExpireDate)
										--insert to revenues audit
										select @Tran_Value=ISNULL((@IssusQty*@FIFOCost),0),@CumulativeQty=ISNULL(sum(Qty),0)+(@IssusQty*-1),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@IssusQty*@FIFOCost)*-1) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
										insert into tbRevenueItem(ReceiptID,WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate],OpenQty)
										values(0,@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@IssusQty,@FIFOCost,@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,@ExpireDate,@IssusQty)
									End
								end
							else
								begin
								    update tbWarehouseDetail
									set InStock=isnull(@FIFOQty,0)   where ID=@WarehoseDetailLineID--where WarehouseID=@WarehouseID and ItemID=@ItemID_SM and UomID=@UomID_SM and [ExpireDate]=@ExpireDate
									update tbStockMoving
									set InStock=@FIFOQty where ID=@StockMoveLineID

									IF @IssusQty>0
									Begin
										select @FIFOCost=isnull(Cost,0) from tbPriceListDetail where ItemID=@ItemID_SM and UomID=@UomID_SM
										--insert into audit
										select @Tran_Value=ISNULL((@IssusQty*@FIFOCost)*-1,0),@CumulativeQty=ISNULL(sum(Qty),0)+(@IssusQty*-1),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@IssusQty*@FIFOCost)*-1) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
										set @AvgCost=nullif(@CumulativeValue,0)/nullif(@CumulativeQty,0)
										insert into tbInventoryAudit(WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate])
										values(@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@IssusQty*-1,isnull(@AvgCost,0),@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,@ExpireDate)
										--insert into revenues
										select @Tran_Value=ISNULL((@IssusQty*@FIFOCost),0),@CumulativeQty=ISNULL(sum(Qty),0)+(@IssusQty*-1),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@IssusQty*@FIFOCost)*-1) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
										set @AvgCost=nullif(@CumulativeValue,0)/nullif(@CumulativeQty,0)
										insert into tbRevenueItem(ReceiptID,WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate],OpenQty)
										values(0,@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@IssusQty,isnull(@AvgCost,0),@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,@ExpireDate,@IssusQty)
									
										--Transaction update cost in price list detail
										  declare @PriceListID int
										  INSERT INTO tpPriceList(PirceListID,currencyID)
										  SELECT pl.ID,pl.CurrencyID  from tbPriceList pl
  
										  While(select COUNT(*) from tpPriceList)>0
											begin 
				
												Select top 1 @PriceListID=PirceListID from tpPriceList
												--select @Rate_p1=ex.Rate from tbExchangeRate ex where ex.CurrencyID=@PriceCurrencyID 
												update tbPriceListDetail
												set Cost=isnull(@AvgCost,0)
												where ItemID=@ItemID and UomID=@UomID and PriceListID=@PriceListID
												Delete tpPriceList where PirceListID=@PriceListID
											end		
									End
								end
							
							delete tbStockMoving
							break;
						end
				end
				----update committed stock
				--declare @CommitIssuse float
				--select @CommitQty=max([Committed]) from tbWarehouseDetail where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
				--select @CommitIssuse=sum([Qty]) from tbOrderDetail where ItemID=@ItemID and UomID=@UomID and OrderID=@OrderID
				--update tbWarehouseDetail 
				--set [Committed]=@CommitQty-@CommitIssuse
				--where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID

				delete tmpOrderDetail where ID=@LineID
				
		End
	ELSE
		Begin
			----insert to inventory audit
			--select @Tran_Value=ISNULL((@Qty*@Cost)*-1,0),@CumulativeQty=ISNULL(sum(Qty),0)+(@Qty*-1),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@Qty*@Cost)*-1) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
			--insert into tbInventoryAudit(WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate])
			--values(@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,'SO',@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@Qty*-1,@Cost,@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,GETDATE())
			--delete tmpOrderDetail where ID=@LineID
			--insert to reveues audit
			select @Tran_Value=ISNULL((@Qty*@Cost),0),@CumulativeQty=ISNULL(sum(Qty),0)+(@Qty*-1),@CumulativeValue=ISNULL(sum(Trans_Valuse),0)+((@Qty*@Cost)*-1) from tbInventoryAudit where ItemID=@ItemID and UomID=@UomID and WarehouseID=@WarehouseID
			insert into tbRevenueItem(ReceiptID, WarehouseID,BranchID,UserID,ItemID,CurrencyID,UomID,InvoiceNo,Trans_Type,Process,SystemDate,TimeIn,Qty,Cost,Price,CumulativeQty,CumulativeValue,Trans_Valuse,[ExpireDate],OpenQty)
			values(0,@WarehouseID,@BranchID,@UserID,@ItemID,@CurrencyID,@UomID,@InvoiceNo,@TransactType,@Process,Getdate(),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),@Qty,@Cost,@UnitPrice,@CumulativeQty,@CumulativeValue,@Tran_Value,GETDATE(),@Qty)
			delete tmpOrderDetail where ID=@LineID
		End
	
End

END






















GO


