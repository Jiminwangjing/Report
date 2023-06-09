USE [KWEBPOSV1.4.0_blank]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetItembyWarehouseFrom_GoodIssuse]    Script Date: 4/1/2022 5:32:28 PM ******/
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
		0 as GLID,
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


















