USE [KWEBPOSV1.3.2_DB001-Default]
GO
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





















