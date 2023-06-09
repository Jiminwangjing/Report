USE [KWEBPOSV1.3.3.CardMember]
GO
/****** Object:  StoredProcedure [dbo].[pos_GetItemForSale]    Script Date: 2/16/2022 8:51:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[pos_GetItemForSale]
@PricelistID int 
AS
BEGIN
DECLARE @VAT FLOAT=0
SELECT @VAT=ISNULL(Rate,0) FROM tbTax WHERE Type='Output' AND GETDATE()<=Effective AND [DELETE]=0

SELECT
	   pld.ID,
	   item.ID as ItemID,
	   item.Code,
	   item.ItemGroup1ID as Group1,
	   item.ItemGroup2ID as Group2,
	   item.ItemGroup3ID as Group3,
	   item.KhmerName,
	   item.EnglishName,
	   convert(float,0) as Qty,
	   convert(float,0) as PrintQty,
	   pld.Cost,
	   pld.UnitPrice,
	    case 
			when convert(date,GetDate()) >= max(convert(date,pro.StartDate)) and convert(date,GetDate()) <=max(convert(date,pro.StopDate)) and pro.Active=1 then isnull(max(pld.Discount),0)
			else 0 
	   end as DiscountRate,
	   convert(float,0) as DiscountValue,
	   pld.TypeDis,
	   @VAT as VAT,
	   curr.ID as CurrencyID,
	   curr.[Description] as Currency,
	   uom.ID as UomID,
	   uom.Name as UoM,
	   pld.Barcode,
	   item.Process,
	   max(item.[Image]) as [Image],
	   pld.PriceListID as PricListID,
	   convert(float,0) as InStock,
	   convert(float,0) as [Committed],
	   convert(float,0) as Ordered,
	   convert(float,0) as Available,
	   isnull(null,GETDATE()) as [ExpireDate],
	   item.GroupUomID as GroupUomID,
	   pt.Name as PrintTo,
	   item.Type as ItemType,
	   item.[Description],
	   item.Scale as IsScale,
	   item.TaxGroupSaleID as TaxGroupSaleID
	  from tbItemMasterData item 
					inner join tbPriceListDetail pld on pld.ItemID=item.ID
					inner join tbUnitofMeasure uom on pld.UomID=uom.ID
					inner join tbPriceList pr on pr.ID = @PricelistID
					inner join tbCurrency curr on pr.CurrencyID=curr.ID
					inner join tbPrinterName pt on item.PrintToID=pt.ID
					left join tbPromotion pro on pld.PromotionID=pro.ID
					WHERE item.[Delete]=0 and item.Sale=1 and pld.PriceListID=@PricelistID
			   group by
			   pld.ID,
			   pld.Barcode,
			   item.ID,
			    item.ItemGroup1ID,
				item.ItemGroup2ID,
				item.ItemGroup3ID,
			    pld.Cost,
				pld.UnitPrice,
				item.Code,
				pld.Discount,
				pld.TypeDis,
			   item.KhmerName,
			   item.EnglishName,
			   curr.ID,
			   curr.Description,
			   uom.ID,
			   uom.Name,
			   item.Barcode,
			   item.Process,
			   pld.PriceListID,
			   --wd.InStock,
			   --wd.Committed,
			   --wd.Ordered,
			   --wd.Available,
			   --wd.ExpireDate,
			   item.GroupUomID,
			   pro.Active,
			   pt.Name,
			   item.[Type],
			    item.[Description],
				item.Scale,
				item.TaxGroupSaleID
END