USE [KWEBPOSV1.3.2_DB002-Default]
GO
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
