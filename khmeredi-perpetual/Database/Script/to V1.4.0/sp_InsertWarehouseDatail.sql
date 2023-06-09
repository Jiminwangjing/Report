USE [Aguatek_Solution101]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertWarehouseDatail]    Script Date: 3/11/2022 1:22:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_InsertWarehouseDatail]
@Process nvarchar(max)='A',
@ToWHID int=0,
@UserID int=0
AS
BEGIN
  Declare @CurrencyID int 
  select @CurrencyID=pl.CurrencyID from tbCompany cp inner join tbPriceList pl on cp.PriceListID=pl.ID
                                        inner join tbCurrency cur on pl.CurrencyID=cur.ID

  IF @Process='A'
	Begin
	     --Warehouse Detail
		 INSERT INTO tbWarehouseDetail(WarehouseID,UomID,UserID,SyetemDate,TimeIn,InStock,CurrencyID,[ExpireDate],Cost,ItemID)
		 SELECT  @ToWHID,gduom.AltUOM,@UserID,CONVERT(date,GETDATE()),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),0,@CurrencyID,CONVERT(date,GETDATE()),0,item.ID
		 FROM tbItemMasterData item inner join tbGroupDefindUoM gduom on item.GroupUomID=gduom.GroupUoMID
		 WHERE item.[Delete]=0 and item.ID not in (select ItemID from tbWarehouseDetail where WarehouseID=@ToWHID)
		
		 --Warehouse Summary
		 INSERT INTO tbWarehouseSummary(WarehouseID,UomID,UserID,SyetemDate,TimeIn,InStock,[Committed],Ordered,Available,CurrencyID,[ExpireDate],Cost,ItemID,Factor)
		 SELECT  @ToWHID,item.InventoryUoMID,@UserID,CONVERT(date,GETDATE()),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),0,0,0,0,@CurrencyID,CONVERT(date,GETDATE()),0,item.ID,0
		 FROM tbItemMasterData item 
		 WHERE item.Process!='Standard' and item.[Delete]=0 and item.ID not in (select ItemID from tbWarehouseSummary where WarehouseID=@ToWHID)
	End
  ELSE
	Begin
		--Warehouse Detail
		 INSERT INTO tbWarehouseDetail(WarehouseID,UomID,UserID,SyetemDate,TimeIn,InStock,CurrencyID,[ExpireDate],Cost,ItemID)
		 SELECT @ToWHID,gduom.AltUOM,@UserID,CONVERT(date,GETDATE()),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),0,1,CONVERT(date,GETDATE()),0,item.ID
		 FROM tpItemCopyToWHDetail tp 
		                            inner join tbItemMasterData item on item.ID=tp.ItemID
									inner join tbGroupDefindUoM gduom on item.GroupUomID=gduom.GroupUoMID
		  --WHERE tp.ItemID not in (select ItemID from tbWarehouseDetail where WarehouseID=@ToWHID)

		  --Warehouse Summary
		 INSERT INTO tbWarehouseSummary(WarehouseID,UomID,UserID,SyetemDate,TimeIn,InStock,[Committed],Ordered,Available,CurrencyID,[ExpireDate],Cost,ItemID,Factor)
		 SELECT @ToWHID,item.InventoryUoMID,@UserID,CONVERT(date,GETDATE()),RIGHT(CONVERT(VARCHAR, GETDATE(), 100),7),0,0,0,0,1,CONVERT(date,GETDATE()),0,item.ID,1
		 FROM tpItemCopyToWHDetail tp 
		                            inner join tbItemMasterData item on item.ID=tp.ItemID
									--inner join tbGroupDefindUoM gduom on item.GroupUomID=gduom.GroupUoMID
		  --WHERE tp.ItemID not in (select ItemID from tbWarehouseDetail where WarehouseID=@ToWHID)

		 delete tpItemCopyToWHDetail
		 delete tpItemCopyToWH
		 

	End
END











