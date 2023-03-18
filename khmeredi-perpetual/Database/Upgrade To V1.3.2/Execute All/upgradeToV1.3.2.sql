--V 1.3.3
--Update document type
IF NOT EXISTS(select 1 from tbDocumentType where Code = 'RE')
INSERT INTO tbDocumentType(Code, [Name]) VALUES('RE', 'Return Delivery');

IF NOT EXISTS(select 1 from tbDocumentType where Code = 'RPS')
INSERT INTO tbDocumentType(Code, [Name]) VALUES('RPS', 'Redeem Point');

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

/****** Object:  StoredProcedure [dbo].[sp_GetItembyWarehouseFrom_GoodIssuse]    Script Date: 8/19/2021 5:49:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_GetItembyWarehouseFrom_GoodIssuse]
@WarehouseID int
AS
BEGIN
   select 
        max(wd.ID) as  GoodIssuesDetailID,
	    max(wd.ID) as  GoodIssuesID,
		max(wd.ID) as LineID,
	    item.ID as  ItemID,
		max(cur.ID) as CurrencyID,
		uom.ID as UomID,
	    max(item.Code) as Code,
		max(item.KhmerName) as KhmerName,
		max(item.EnglishName) as EnglishName,
	    sum(wd.InStock) as Quantity,	
	    max(wd.Cost) as Cost,	
		max(cur.[Description]) as Currency,
		max(uom.Name) as UomName,
		max(item.Barcode) as BarCode,
		max(wd.[ExpireDate]) as [ExpireDate],
		null as [Check],
		max(item.ManageExpire) as ManageExpire
        from
        tbWarehouseDetail wd
		JOIN tbItemMasterData item on wd.ItemID=item.ID
		JOIN tbUnitofMeasure uom on item.InventoryUoMID=uom.ID
		--JOIN tbGroupDefindUoM gUoM on gUoM.ID=item.GroupUomID and gUoM.AltUOM=uom.ID
		JOIN tbCurrency cur on wd.CurrencyID=cur.ID
		where wd.WarehouseID=@WarehouseID and item.[Delete]=0 and item.Process!='Standard' and wd.InStock>0 and item.Inventory=1 and item.Purchase=1
		group by  uom.ID,
		         item.ID
		        
END

/****** Object:  StoredProcedure [dbo].[sp_GetItembyWarehouseFrom_GoodsReceipt]    Script Date: 8/19/2021 5:49:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_GetItembyWarehouseFrom_GoodsReceipt]
@WarehouseID int
AS
BEGIN
   select 
        max(wd.ID) as  GoodReceitpDetailID,
	    max(wd.ID) as  GoodsReceiptID,
		max(wd.ID) as LineID,
	    item.ID as  ItemID,
		max(cur.ID) as CurrencyID,
		uom.ID as UomID,
	    max(item.Code) as Code,
		max(item.KhmerName) as KhmerName,
		max(item.EnglishName) as EnglishName,
	    sum(wd.InStock) as Quantity,	
	    max(wd.Cost) as Cost,	
		max(cur.[Description]) as Currency,
		max(uom.Name) as UomName,
		max(item.Barcode) as BarCode,
	    max(wd.[ExpireDate]) as [ExpireDate],
		null as [Check],
		max(item.ManageExpire) as ManageExpire
        from
        tbWarehouseDetail wd
		JOIN tbItemMasterData item on wd.ItemID=item.ID
		--JOIN tbGroupDefindUoM gUoM on gUoM.ID=item.GroupUomID
		JOIN tbUnitofMeasure uom on item.InventoryUoMID = uom.ID
		JOIN tbCurrency cur on wd.CurrencyID=cur.ID
		where wd.WarehouseID=@WarehouseID and wd.InStock > 0 and item.[Delete]=0 and item.Process!='Standard' 
		group by uom.ID,
		         item.ID
		         
END

/****** Object:  StoredProcedure [dbo].[sp_GetListItemMasterDataGoodReceiptPO]    Script Date: 8/19/2021 11:12:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER  PROC [dbo].[sp_GetListItemMasterDataGoodReceiptPO]
@WarehouseID int
AS 
BEGIN 
 
  Declare @ExchangeRate float 
  Declare @LocalCurrecy nvarchar(50)
  Declare @LocalCurrencyID int
  Declare @SetRate float
  SELECT @ExchangeRate= ex.Rate,@LocalCurrecy=cr.[Description],@LocalCurrencyID=cr.ID,@SetRate=ex.SetRate FROM tbCompany cp join tbCurrency cr on cp.SystemCurrencyID = cr.ID
							 join tbExchangeRate ex on cr.ID = ex.CurrencyID
																				  										                    
  SELECT 
   max(wh.ID) as ID,
   0 as PurchaseDetailAPID,
   max(wh.ID) as LineID,
   item.ID as ItemID,
   max(item.Code) as Code,
   max(item.KhmerName) as KhmerName,
   max(item.EnglishName) as EnglishName,
   sum(wh.InStock/isnull(nullif(wh.Factor,0),1)) as Qty,
   round(max(wh.Cost*wh.Factor),3) as PurchasPrice, 
   CONVERT(float,0) as DiscountRate,
   CONVERT(float,0) as DiscountValue,
   'Percent' as TypeDis,
   round(max(wh.Cost*wh.Factor)​​,3) as Total,
   max(@LocalCurrecy) as SysCurrency,
   max(uom.Name) as UomName,
   max(item.ManageExpire) as ManageExpire,
   null as [ExpireDate],
   CONVERT(float,0) as AlertStock,
   round(max(wh.Cost),3) as Total_Sys,
   max(@ExchangeRate) as ExchangeRate,
   max(cur.ID) as SysCurrencyID,   
   max(@LocalCurrencyID) as LocalCurrencyID,
   max(@LocalCurrecy) as LocalCurrency,   
   uom.ID as UomID, 
   max(item.GroupUomID) as GroupUomID ,
   max(item.Barcode) as Barcode,
   Convert(float,1) as OpenQty ,
   max(@SetRate) as SetRate
   FROM tbWarehouseDetail wh 
         join tbItemMasterData item on 
		 wh.ItemID=item.ID 
		 join tbUnitofMeasure uom on
		 wh.UomID=uom.ID
		 join tbCurrency cur on
		 wh.CurrencyID=cur.ID
		 where item.[Delete]=0 and wh.WarehouseID=@WarehouseID and item.Process!='Standard' and item.Inventory=1 and item.Purchase=1 and item.PurchaseUomID=uom.ID
        group by uom.ID,
		         item.ID
END 

/****** Object:  StoredProcedure [dbo].[sp_GetListItemMasterDataPurchaseAP]    Script Date: 8/19/2021 11:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER  PROC [dbo].[sp_GetListItemMasterDataPurchaseAP]
@WarehouseID int
AS 
BEGIN 
 
  Declare @ExchangeRate float 
  Declare @LocalCurrecy nvarchar(50)
  Declare @LocalCurrencyID int
  Declare @SetRate float
  SELECT @ExchangeRate= ex.Rate,@LocalCurrecy=cr.[Description],@LocalCurrencyID=cr.ID,@SetRate=ex.SetRate FROM tbCompany cp join tbCurrency cr on cp.SystemCurrencyID = cr.ID
							 join tbExchangeRate ex on cr.ID = ex.CurrencyID
																				  										                    
  SELECT 
   max(wh.ID) as ID,
   0 as PurchaseDetailAPID,
   max(wh.ID) as LineID,
   item.ID as ItemID,
   max(item.Code) as Code,
   max(item.KhmerName) as KhmerName,
   max(item.EnglishName) as EnglishName,
   sum(wh.InStock/isnull(nullif(wh.Factor,0),1)) as Qty,
   round(max(wh.Cost*wh.Factor),3) as PurchasPrice, 
   CONVERT(float,0) as DiscountRate,
   CONVERT(float,0) as DiscountValue,
   'Percent' as TypeDis,
   round(max(wh.Cost*wh.Factor),3)​​ as Total,
   max(@LocalCurrecy) as SysCurrency,
   max(uom.Name) as UomName,
   max(item.ManageExpire) as ManageExpire,
   null as [ExpireDate],
   CONVERT(float,0) as AlertStock,
   round(max(wh.Cost),3) as Total_Sys,
   max(@ExchangeRate) as ExchangeRate,
   max(cur.ID) as SysCurrencyID,   
   max(@LocalCurrencyID) as LocalCurrencyID,
   max(@LocalCurrecy) as LocalCurrency,   
   uom.ID as UomID, 
   max(item.GroupUomID) as GroupUomID ,
   max(item.Barcode) as Barcode,
   Convert(float,1) as OpenQty,
   max(@SetRate) as SetRate
   FROM tbWarehouseDetail wh 
         join tbItemMasterData item on 
		 wh.ItemID=item.ID 
		 join tbUnitofMeasure uom on
		 wh.UomID=uom.ID
		 join tbCurrency cur on
		 wh.CurrencyID=cur.ID
		 where item.[Delete]=0 and wh.WarehouseID=@WarehouseID and item.Process!='Standard' and item.Inventory=1 and item.Purchase=1 and item.PurchaseUomID=uom.ID
        group by uom.ID,
		         item.ID
		order by KhmerName
				 
				  
END 

/****** Object:  StoredProcedure [dbo].[sp_GetListItemMasterDataPurchaseCreditMemo]    Script Date: 8/19/2021 11:13:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_GetListItemMasterDataPurchaseCreditMemo]
@WarehouseID int
AS
BEGIN
 Declare @ExchangeRate float 
  Declare @LocalCurrecy nvarchar(50)
  Declare @LocalCurrencyID int
  Declare @SetRate float
  SELECT @ExchangeRate= ex.Rate,@LocalCurrecy=cr.[Description],@LocalCurrencyID=cr.ID,@SetRate=ex.SetRate FROM tbCompany cp join tbCurrency cr on cp.SystemCurrencyID = cr.ID
							 join tbExchangeRate ex on cr.ID = ex.CurrencyID
																				  										                    
  SELECT 
   max(wh.ID) as ID,
   0 as PurchaseMemoDetailID,
   max(wh.ID) as LineID,
   item.ID as ItemID,
   max(item.Code) as Code,
   max(item.KhmerName) as KhmerName,
   max(item.EnglishName) as EnglishName,
   sum(wh.InStock/isnull(nullif(wh.Factor,0),1)) as Qty,
   round(max(wh.Cost*wh.Factor),3) as PurchasPrice, 
   CONVERT(float,0) as DiscountRate,
   CONVERT(float,0) as DiscountValue,
   'Percent' as TypeDis,
   round(max(wh.Cost*wh.Factor),3)​​ as Total,
   max(@LocalCurrecy) as SysCurrency,
   max(uom.Name) as UomName,
   max(item.ManageExpire) as ManageExpire,
   null as [ExpireDate],
   CONVERT(float,0) as AlertStock,
   round(max(wh.Cost),3) as Total_Sys,
   max(@ExchangeRate) as ExchangeRate,
   max(cur.ID) as SysCurrencyID,   
   max(@LocalCurrencyID) as LocalCurrencyID,
   max(@LocalCurrecy) as LocalCurrency,   
   uom.ID as UomID, 
   max(item.GroupUomID) as GroupUomID ,
   max(item.Barcode) as Barcode,
   max(@SetRate) as SetRate
   FROM tbWarehouseDetail wh 
         join tbItemMasterData item on 
		 wh.ItemID=item.ID 
		 join tbUnitofMeasure uom on
		 wh.UomID=uom.ID
		 join tbCurrency cur on
		 wh.CurrencyID=cur.ID
		 where item.[Delete]=0 and wh.WarehouseID=@WarehouseID and item.Process!='Standard' and item.Inventory=1 and item.Purchase=1 and item.PurchaseUomID=uom.ID
        group by uom.ID,
		         item.ID
		          
END

/****** Object:  StoredProcedure [dbo].[sp_GetListItemMasterDataPurchaseOrder]    Script Date: 8/19/2021 11:13:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER  PROC [dbo].[sp_GetListItemMasterDataPurchaseOrder]
@WarehouseID int
AS 
BEGIN 
 
  Declare @ExchangeRate float 
  Declare @LocalCurrecy nvarchar(50)
  Declare @LocalCurrencyID int
  Declare @SetRate float

  --SELECT @ExchangeRate= ex.Rate,@LocalCurrecy=cr.[Description],@LocalCurrencyID=cr.ID from tbCompany cp join tbPriceList pl on cp.PriceListID=pl.ID
  --                                                join tbExchangeRate ex on pl.CurrencyID=ex.CurrencyID
		--										  join tbCurrency cr on cr.ID=ex.CurrencyID
  SELECT @ExchangeRate= ex.Rate,@LocalCurrecy=cr.[Description],@LocalCurrencyID=cr.ID,@SetRate=ex.SetRate FROM tbCompany cp join tbCurrency cr on cp.SystemCurrencyID = cr.ID
							 join tbExchangeRate ex on cr.ID = ex.CurrencyID
  SELECT 
   max(wh.ID) as ID,
   0 as PurchaseOrderDetailID,
   max(wh.ID) as LineID,
   item.ID as ItemID,
   max(item.Code) as Code,
   max(item.KhmerName) as KhmerName,
   max(item.EnglishName) as EnglishName,
   sum(wh.Ordered) as Qty,
   round(max(wh.Cost),3) as PurchasPrice, 
   CONVERT(float,0) as DiscountRate,
   CONVERT(float,0) as DiscountValue,
   'Percent' as TypeDis,
   ROUND(max(wh.Cost)​​,3) as Total,
   max(@LocalCurrecy) as SysCurrency,
   max(uom.Name) as UomName,
   max(item.ManageExpire) as ManageExpire,
   null as [ExpireDate],
   CONVERT(float,0) as AlertStock,
   round(max(wh.Cost),3) as Total_Sys,
   max(@ExchangeRate) as ExchangeRate,
   max(cur.ID) as SysCurrencyID,   
   max(@LocalCurrencyID) as LocalCurrencyID,
   max(@LocalCurrecy) as LocalCurrency,   
   uom.ID as UomID, 
   max(item.GroupUomID) as GroupUomID ,
   max(item.Barcode) as Barcode,
   Convert(float,1) as OpenQty,
   convert(bit,0) as Choosed,
   max(@SetRate) as SetRate 
   FROM tbWarehouseDetail wh 
         join tbItemMasterData item on 
		 wh.ItemID=item.ID 
		 join tbUnitofMeasure uom on
		 wh.UomID=uom.ID
		 join tbCurrency cur on
		 wh.CurrencyID=cur.ID
		
		 where item.[Delete]=0 and wh.WarehouseID=@WarehouseID and item.Process!='Standard' and item.Inventory=1 and item.Purchase=1 and item.PurchaseUomID=uom.ID
        group by uom.ID,
		         item.ID
		         
				  
END 

/****** Object:  StoredProcedure [dbo].[sp_GetPurchaseAP_form_PurchaseOrder]    Script Date: 8/31/2021 8:41:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_GetPurchaseAP_form_PurchaseOrder]
@PurchaseOrderID int,
@Number nvarchar(max)
AS
BEGIN
	SELECT   od.LineID as ID,
	         po.WarehouseID as WarehouseID,
			 w.Name as Warehouse,
			 po.VendorID as VendorID,
			 bn.Name as Vendor,
			 po.Reff_No as Reff_No,
			 po.PurCurrencyID as LocalCurrencyID,
			 po.SysCurrencyID as SystemCurrencyID,
			 cs.[Description] as SystemCurrency,
			 c.[Description] as LocalCurrency,
			 od.UomID as UomID,
			 s.PreFix as PreFix,
			 uom.Name as UomName,
			 po.UserID as UserID,
			 emp.Name as [User],
			 po.InvoiceNo as Invoice,
			 po.Number as Number,
			 po.PostingDate as PostingDate,
			 po.DocumentDate as DocumentDate,
			 po.DeliveryDate as DeliveryDate,
			 po.Sub_Total as Sub_Total,
			 po.Sub_Total_sys as Sub_Total_Sys,
			 po.DiscountRate as DiscountRate,
			 po.DiscountValues as DiscountValues,
			 po.TypeDis as TypeDis,
			 po.TaxRate as TaxRate,
			 po.TaxValues as TaxValues,
			 po.Down_Payment as DownPayment,
			 CONVERT(float,0) as AppliedAmount,
			 po.Balance_Due as Balance_Due,
			 po.Balance_Due_Sys as Balance_Due_Sys,
			 po.Additional_Expense as AdditionalExpense,
			 po.Return_Amount as ReturnAmount,
			 po.Additional_Note as AdditionalNode,
			 po.PurRate as ExchangRate,
			 po.Remark as Remark,
			 po.[Status] as [Status],
			 od.ItemID as ItemID,
			 od.PurchaseOrderDetailID as OrderID,
			 od.LineID as APID,
			 od.LineID as LineID,
			 item.Code as Code,
			 item.KhmerName as KhmerName,
			 item.EnglishName as EnglishName,
			 od.Qty as Qty,
			 od.OpenQty as OpenQty,
			 od.PurchasPrice as PurchasePrice,
			 od.DiscountRate as Discount_Rate,
			 od.DiscountValue as Discount_Values,
			 od.TypeDis as Type_Dis,
			 od.Total as Total,
			 od.Total_Sys as Total_Sys,
			 item.ManageExpire as ManageExpire,
			 od.[ExpireDate] as [ExpireDate],
			 od.AlertStock as AlertStock,
			 item.Barcode as Barcode,
			 item.GroupUomID as GroupUomID,
			 (1/po.PurRate) as SetRate,
			 po.LocalSetRate as LocalSetRate,
			 po.PurchaseOrderID as BaseOnID
	         FROM tbPurchaseOrderDetail od 
	         JOIN  tbPurchaseOrder po on od.PurchaseOrderID=po.PurchaseOrderID
			 JOIN tbBusinessPartner bn on po.VendorID=bn.ID
			 JOIN tbWarhouse w on po.WarehouseID=w.ID
			 JOIN tbCurrency c on po.PurCurrencyID=c.ID
			 JOIN tbCurrency cs on po.SysCurrencyID=cs.ID
			 JOIN tbUserAccount u on po.UserID=u.ID
			 JOIN tbEmployee emp on u.EmployeeID=emp.ID
			 JOIN tbItemMasterData item on od.ItemID=item.ID
			 JOIN tbUnitofMeasure uom on od.UomID=uom.ID
			 JOIN tbSeries s on po.SeriesID = s.ID
	where po.Number=@Number and po.PurchaseOrderID=@PurchaseOrderID and od.[Delete]=0
			 
END

