USE [KWEBPOSV1.3.3.CardMember]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetItemSetPrice]    Script Date: 2/16/2022 9:21:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_GetItemSetPrice](
@PriceListID int=0,
@Group1 int=0,
@Group2 int=0,
@Group3 int=0,
@Process nvarchar(max)='Add')
AS
BEGIN
    Declare @SysCurrency nvarchar(max)
	select @SysCurrency=cur.[Description] from tbCompany cop inner join tbPriceList pl on cop.PriceListID=pl.ID
	                                                       inner join tbCurrency cur on cur.ID=pl.CurrencyID 
	IF(@Process='Add')
		Begin
			IF @PriceListID!=0 and @Group1=0 and @Group2=0 and @Group3=0
			Begin
				SELECT 
					  pld.ID,
					  item.Code,
					  item.KhmerName,
					  item.EnglishName,
					  uom.Name as Uom,
					  pld.Cost as Cost,
					  convert(float,0) as Makup,
					  pld.UnitPrice as Price,
					  cur.[Description] as Currency,
					  convert(float,pld.Discount) as Discount,
					  pld.TypeDis,
					  item.Process,
					  @SysCurrency as SysCurrency,
					  pld.Barcode as Barcode
				FROM tbPriceListDetail pld 
									   inner join tbItemMasterData item on pld.ItemID=item.ID
									   inner join tbCurrency cur on pld.CurrencyID=cur.ID
									   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
									   where item.[Delete]=0 and pld.PriceListID=@PriceListID and pld.UnitPrice=0
									   order by pld.ItemID
			End
		ELSE IF @PriceListID!=0 and @Group1!=0 and @Group2=0 and @Group3=0
			Begin 
				SELECT 
					 pld.ID,
					  item.Code,
					  item.KhmerName,
					  item.EnglishName,
					  uom.Name as Uom,
					  pld.Cost as Cost,
					  convert(float,0) as Makup,
					  pld.UnitPrice as Price,
					  cur.[Description] as Currency,
					  convert(float,pld.Discount) as Discount,
					  pld.TypeDis,
					  item.Process,
					  @SysCurrency as SysCurrency,
					   pld.Barcode as Barcode
				FROM tbPriceListDetail pld 
									   inner join tbItemMasterData item on pld.ItemID=item.ID
									   inner join tbCurrency cur on pld.CurrencyID=cur.ID
									   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
									   where item.[Delete]=0 and pld.PriceListID=@PriceListID and item.ItemGroup1ID=@Group1 and pld.UnitPrice=0
									   order by pld.ItemID
			End
		ELSE IF @PriceListID!=0 and @Group1!=0 and @Group2!=0 and @Group3=0
			Begin 
				SELECT 
					  pld.ID,
					  item.Code,
					  item.KhmerName,
					  item.EnglishName,
					  uom.Name as Uom,
					   convert(float,0) as Makup,
					  pld.Cost as Cost,
					  pld.UnitPrice as Price,
					  cur.[Description] as Currency,
					  convert(float,pld.Discount) as Discount,
					  pld.TypeDis,
					  item.Process,
					  @SysCurrency as SysCurrency,
					   pld.Barcode as Barcode
				FROM tbPriceListDetail pld 
									   inner join tbItemMasterData item on pld.ItemID=item.ID
									   inner join tbCurrency cur on pld.CurrencyID=cur.ID
									   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
									   where item.[Delete]=0 and pld.PriceListID=@PriceListID and item.ItemGroup1ID=@Group1 and item.ItemGroup2ID=@Group2 and pld.UnitPrice=0
									   order by pld.ItemID
			End
		ELSE
			Begin
				SELECT 
					  pld.ID,
					  item.Code,
					  item.KhmerName,
					  item.EnglishName,
					  uom.Name as Uom,
					  pld.Cost as Cost,
					   convert(float,0) as Makup,
					  pld.UnitPrice as Price,
					  cur.[Description] as Currency,
					  convert(float,pld.Discount) as Discount,
					  pld.TypeDis,
					  item.Process,
					  @SysCurrency as SysCurrency,
					   pld.Barcode as Barcode
				FROM tbPriceListDetail pld 
									   inner join tbItemMasterData item on pld.ItemID=item.ID
									   inner join tbCurrency cur on pld.CurrencyID=cur.ID
									   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
									   where item.[Delete]=0 and pld.PriceListID=@PriceListID and item.ItemGroup1ID=@Group1 and item.ItemGroup2ID=@Group2 and item.ItemGroup3ID=@Group3 and pld.UnitPrice=0
									   order by pld.ItemID
			End
		End
	Else
		Begin
			IF @PriceListID!=0 and @Group1=0 and @Group2=0 and @Group3=0
			Begin
				SELECT 
					  pld.ID,
					  item.Code,
					  item.KhmerName,
					  item.EnglishName,
					  uom.Name as Uom,
					  pld.Cost as Cost,
					  convert(float,0) as Makup,
					  pld.UnitPrice as Price,
					  cur.[Description] as Currency,
					  convert(float,pld.Discount) as Discount,
					  pld.TypeDis,
					  item.Process,
					  @SysCurrency as SysCurrency,
					  pld.Barcode as Barcode
				FROM tbPriceListDetail pld 
									   inner join tbItemMasterData item on pld.ItemID=item.ID
									   inner join tbCurrency cur on pld.CurrencyID=cur.ID
									   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
									   where item.[Delete]=0 and pld.PriceListID=@PriceListID and pld.UnitPrice>0
									   order by pld.ItemID
			End
		ELSE IF @PriceListID!=0 and @Group1!=0 and @Group2=0 and @Group3=0
			Begin 
				SELECT 
					 pld.ID,
					  item.Code,
					  item.KhmerName,
					  item.EnglishName,
					  uom.Name as Uom,
					  pld.Cost as Cost,
					  convert(float,0) as Makup,
					  pld.UnitPrice as Price,
					  cur.[Description] as Currency,
					  convert(float,pld.Discount) as Discount,
					  pld.TypeDis,
					  item.Process,
					  @SysCurrency as SysCurrency,
					   pld.Barcode as Barcode
				FROM tbPriceListDetail pld 
									   inner join tbItemMasterData item on pld.ItemID=item.ID
									   inner join tbCurrency cur on pld.CurrencyID=cur.ID
									   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
									   where item.[Delete]=0 and pld.PriceListID=@PriceListID and item.ItemGroup1ID=@Group1 and pld.UnitPrice>0
									   order by pld.ItemID
			End
		ELSE IF @PriceListID!=0 and @Group1!=0 and @Group2!=0 and @Group3=0
			Begin 
				SELECT 
					  pld.ID,
					  item.Code,
					  item.KhmerName,
					  item.EnglishName,
					  uom.Name as Uom,
					   convert(float,0) as Makup,
					  pld.Cost as Cost,
					  pld.UnitPrice as Price,
					  cur.[Description] as Currency,
					  convert(float,pld.Discount) as Discount,
					  pld.TypeDis,
					  item.Process,
					  @SysCurrency as SysCurrency,
					   pld.Barcode as Barcode
				FROM tbPriceListDetail pld 
									   inner join tbItemMasterData item on pld.ItemID=item.ID
									   inner join tbCurrency cur on pld.CurrencyID=cur.ID
									   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
									   where item.[Delete]=0 and pld.PriceListID=@PriceListID and item.ItemGroup1ID=@Group1 and item.ItemGroup2ID=@Group2 and pld.UnitPrice>0
									   order by pld.ItemID
			End
		ELSE
			Begin
				SELECT 
					  pld.ID,
					  item.Code,
					  item.KhmerName,
					  item.EnglishName,
					  uom.Name as Uom,
					  pld.Cost as Cost,
					   convert(float,0) as Makup,
					  pld.UnitPrice as Price,
					  cur.[Description] as Currency,
					  convert(float,pld.Discount) as Discount,
					  pld.TypeDis,
					  item.Process,
					  @SysCurrency as SysCurrency,
					   pld.Barcode as Barcode
				FROM tbPriceListDetail pld 
									   inner join tbItemMasterData item on pld.ItemID=item.ID
									   inner join tbCurrency cur on pld.CurrencyID=cur.ID
									   inner join tbUnitofMeasure uom on pld.UomID=uom.ID
									   where item.[Delete]=0 and pld.PriceListID=@PriceListID and item.ItemGroup1ID=@Group1 and item.ItemGroup2ID=@Group2 and item.ItemGroup3ID=@Group3 and pld.UnitPrice>0
									   order by pld.ItemID
			End
		End
END












