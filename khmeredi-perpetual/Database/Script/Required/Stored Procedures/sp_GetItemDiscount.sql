
GO
/****** Object:  StoredProcedure [dbo].[sp_GetItemDiscount]    Script Date: 3/3/2023 3:46:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_GetItemDiscount](
@PriceListID int=0,
@Group1 int=0,
@Group2 int=0,
@Group3 int=0,
@PromotionId int=0
)
AS
BEGIN
	IF @PriceListID!=0 and @Group1=0 and @Group2=0 and @Group3=0
		Begin
			SELECT DISTINCT
				  pld.ID,
				  item.Code,
				  item.KhmerName,
				  item.EnglishName,
				  uom.Name as Uom,
				  pld.UnitPrice as Price,
				  cur.[Description] as Currency,
				  convert(float, ISNULL(prod.Discount, 0)) as Discount,
				  pld.TypeDis
			FROM tbPriceListDetail pld 
								   inner join tbItemMasterData item on pld.ItemID=item.ID
								   inner join tbCurrency cur on pld.CurrencyID=cur.ID
								   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
								   left join tbPromotionDetail prod on prod.PromotionID = @PromotionId
								   where item.[Delete]=0 and pld.PriceListID=@PriceListID
								   order by item.Code
		End
	ELSE IF @PriceListID!=0 and @Group1!=0 and @Group2=0 and @Group3=0
		Begin 
			SELECT DISTINCT
				  pld.ID,
				  item.Code,
				  item.KhmerName,
				  item.EnglishName,
				  uom.Name as Uom,
				  pld.UnitPrice as Price,
				  cur.[Description] as Currency,
				  convert(float,ISNULL(prod.Discount, 0)) as Discount,
				  pld.TypeDis
			FROM tbPriceListDetail pld 
								   inner join tbItemMasterData item on pld.ItemID=item.ID
								   inner join tbCurrency cur on pld.CurrencyID=cur.ID
								   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
								   left join tbPromotionDetail prod on prod.PromotionID = @PromotionId
								   where item.[Delete]=0 and pld.PriceListID=@PriceListID and item.ItemGroup1ID=@Group1
								   order by item.Code
		End
	ELSE IF @PriceListID!=0 and @Group1!=0 and @Group2!=0 and @Group3=0
		Begin 
			SELECT DISTINCT
				  pld.ID,
				  item.Code,
				  item.KhmerName,
				  item.EnglishName,
				  uom.Name as Uom,
				  pld.UnitPrice as Price,
				  cur.[Description] as Currency,
				  convert(float, ISNULL(prod.Discount, 0)) as Discount,
				  pld.TypeDis
			FROM tbPriceListDetail pld 
								   inner join tbItemMasterData item on pld.ItemID=item.ID
								   inner join tbCurrency cur on pld.CurrencyID=cur.ID
								   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
								   left join tbPromotionDetail prod on prod.PromotionID = @PromotionId 
								   where item.[Delete]=0 and pld.PriceListID=@PriceListID and item.ItemGroup1ID=@Group1 and item.ItemGroup2ID=@Group2
								   order by item.Code
		End
	ELSE
		Begin
			SELECT DISTINCT
				  pld.ID,
				  item.Code,
				  item.KhmerName,
				  item.EnglishName,
				  uom.Name as Uom,
				  pld.UnitPrice as Price,
				  cur.[Description] as Currency,
				   convert(float, ISNULL(prod.Discount, 0)) as Discount,
				  pld.TypeDis
			FROM tbPriceListDetail pld 
								   inner join tbItemMasterData item on pld.ItemID=item.ID
								   inner join tbCurrency cur on pld.CurrencyID=cur.ID
								   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
								   left join tbPromotionDetail prod on prod.PromotionID = @PromotionId
								   where item.[Delete]=0 and pld.PriceListID=@PriceListID and item.ItemGroup1ID=@Group1 and item.ItemGroup2ID=@Group2 and item.ItemGroup3ID=@Group3
								   order by item.Code
		End
END













