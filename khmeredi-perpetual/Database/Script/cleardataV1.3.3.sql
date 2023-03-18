------------------------------------------------------Delete Admin -------------------------------------------
DELETE FROM FreightPurchaseDetial
DELETE FROM FreightPurchase
DELETE FROM tbPurchaseOrderDetail
DELETE FROM tbPurchaseOrder
DELETE FROM tbPurchaseAPDetail
DELETE FROM tbPurchase_AP
DELETE FROM tbPurchaseCreditMemoDetail
DELETE FROM tbPurchaseCreditMemo
DELETE FROM tbPurchaseQuotationDetail
DELETE FROM tbPurchaseQuotation
DELETE FROM tbPurchaseRequest
DELETE FROM tbPurchaseQuotationDetail
DELETE FROM tbPurchaseQuotation
DELETE FROM tbGoodsReciptPODatail
DELETE FROM tbGoodsReciptPO
DELETE FROM tbInventoryAudit
DELETE FROM tbOutgoingpaymnetDetail
DELETE FROM tbOutgoingpayment
DELETE FROM tbOutgoingPaymentVendor
DELETE FROM tbGoodReceitpDetail
DELETE FROM tbTarnsferDetail
DELETE FROM tbTransfer
DELETE FROM tbGoodIssuesDetail
DELETE FROM tbGoodIssues
DELETE FROM tbGoodsReceitp
DELETE FROM tbGoodReceitpDetail
DELETE FROM tbGoodReceiptReturn
DELETE FROM tbGoodsReceiptReturnDetail
------------------------------------------------------delete BOM -----------------------------------------------------
--DELETE FROM tbBOMDetail
--DELETE FROM tbBOMaterial
------------------------------------------------------detete sale----------------------------------------------------
DELETE FROM tbSaleQuoteDetail
DELETE FROM tbSaleQuote
DELETE FROM tbSaleOrderDetail
DELETE FROM tbSaleOrder
DELETE FROM tbSaleDeliveryDetail
DELETE FROM tbSaleDelivery
DELETE FROM tbSaleARDetail
DELETE FROM tbSaleAR
DELETE FROM tbSaleCreditMemoDetail
DELETE FROM SaleCreditMemos
DELETE FROM tbIncomingPaymentCustomer
DELETE FROM tbIncomingPaymentDetail
DELETE FROM tbIncomingPayment
DELETE FROM ARDownPaymentDetail
DELETE FROM ARDownPayment
DELETE FROM ReturnDeliveryDetail
DELETE FROM ReturnDelivery
DELETE FROM FreightSaleDetail
DELETE FROM FreightSale
DBCC CHECKIDENT (tbSaleQuoteDetail, RESEED, 0)
DBCC CHECKIDENT (tbSaleQuote, RESEED, 0)
DBCC CHECKIDENT (tbSaleOrder, RESEED, 0)
DBCC CHECKIDENT (tbSaleOrderDetail, RESEED, 0)
DBCC CHECKIDENT (tbSaleDeliveryDetail, RESEED, 0)
DBCC CHECKIDENT (tbSaleDelivery, RESEED, 0)
DBCC CHECKIDENT (tbSaleARDetail, RESEED, 0)
DBCC CHECKIDENT (tbSaleAR, RESEED, 0)
DBCC CHECKIDENT (tbSaleCreditMemoDetail, RESEED, 0)
DBCC CHECKIDENT (SaleCreditMemos, RESEED, 0)
DBCC CHECKIDENT (tbIncomingPaymentCustomer, RESEED, 0)
DBCC CHECKIDENT (tbIncomingPaymentDetail, RESEED, 0)
DBCC CHECKIDENT (tbIncomingPayment, RESEED, 0)
DBCC CHECKIDENT (ReturnDeliveryDetail, RESEED, 0)
DBCC CHECKIDENT (ReturnDelivery, RESEED, 0)
--------------------------------------------------------delete item-------------------------------------------------------
--DELETE FROM tbWarehouseDetail
--DELETE FROM tbWarehouseSummary
--DELETE FROM tbPriceListDetail
--DELETE FROM tbItemMasterData
--DELETE FROM ItemAccounting
--DELETE FROM tbPropertyDetails
--DBCC CHECKIDENT (tbWarehouseDetail, RESEED, 0)
--DBCC CHECKIDENT (tbWarehouseSummary, RESEED, 0)
--DBCC CHECKIDENT (tbPriceListDetail, RESEED, 0)
--DBCC CHECKIDENT (tbItemMasterData, RESEED, 0)
--DBCC CHECKIDENT (ItemAccounting, RESEED, 0)
----------------------------------------------------------delete financials -----------------------------------------------
DELETE FROM tbAccountBalance
DELETE FROM tbJournalEntryDetail
DELETE FROM tbJournalEntry
DELETE FROM tbSeriesDetail
UPDATE tbGLAccount SET Balance=0
UPDATE tbSeries SET NextNo=FirstNo
DBCC CHECKIDENT (tbAccountBalance, RESEED, 0)
DBCC CHECKIDENT (tbJournalEntryDetail, RESEED, 0)
DBCC CHECKIDENT (tbJournalEntry, RESEED, 0)
DBCC CHECKIDENT (tbSeriesDetail, RESEED, 0)
--------------------------------------------------------delete group-----------------------------------------------------
--DELETE FROM ItemGroup3
--DELETE FROM ItemGroup2
--DELETE FROM ItemGroup1
--DBCC CHECKIDENT (ItemGroup3, RESEED, 0)
--DBCC CHECKIDENT (ItemGroup2, RESEED, 0)
--DBCC CHECKIDENT (ItemGroup1, RESEED, 0)
----------------------------------------------------------delete uom-------------------------------------------------------
--DELETE FROM tbGroupDefindUoM
--DELETE FROM tbGroupUoM
--DELETE FROM tbUnitofMeasure
--DBCC CHECKIDENT (tbGroupUoM, RESEED, 0)
--DBCC CHECKIDENT (tbGroupDefindUoM, RESEED, 0)
--DBCC CHECKIDENT (tbUnitofMeasure, RESEED, 0)
-----------------------------------------------------------delete Pos----------------------------------------------------
DELETE FROM tbOrderDetail
DELETE FROM tbOrder
DELETE FROM tbOrder_Queue
DELETE FROM tbOrder_Receipt
DELETE FROM tbReceiptDetail
DELETE FROM tbReceipt
DELETE FROM tbCloseShift
DELETE FROM tbOpenShift
DELETE FROM tbRevenueItem
DELETE FROM ReceiptDetailMemoKvms
DELETE FROM ReceiptMemo
DELETE FROM VoidItemDetail
DELETE FROM VoidItem
DELETE FROM PendingVoidItemDetail
DELETE FROM PendingVoidItem
DELETE FROM tbVoidOrderDetail
DELETE FROM tbVoidOrder
DELETE FROM FreightReceipt
DBCC CHECKIDENT (tbOrderDetail, RESEED, 0)
DBCC CHECKIDENT (tbOrder, RESEED, 0)
DBCC CHECKIDENT (tbOrder_Queue, RESEED, 0)
DBCC CHECKIDENT (tbOrder_Receipt, RESEED, 0)
DBCC CHECKIDENT (tbReceiptDetail, RESEED, 0)
DBCC CHECKIDENT (tbReceipt, RESEED, 0)
DBCC CHECKIDENT (tbCloseShift, RESEED, 0)
DBCC CHECKIDENT (tbOpenShift, RESEED, 0)
DBCC CHECKIDENT (tbRevenueItem, RESEED, 0)
DBCC CHECKIDENT (ReceiptDetailMemoKvms, RESEED, 0)
DBCC CHECKIDENT (ReceiptMemo, RESEED, 0)
-------------------------------------------------------delete alert-----------------------------------------------------------
DELETE FROM StockAlerts
DELETE FROM DueDateAlerts
DELETE FROM CashOutAlerts
DELETE FROM ExpirationStockItems
DBCC CHECKIDENT (StockAlerts, RESEED,0)
DBCC CHECKIDENT (DueDateAlerts,RESEED,0)
DBCC CHECKIDENT (CashOutAlerts)
DBCC CHECKIDENT (ExpirationStockItems,RESEED,0)
-------------------------------------------------------update stock-----------------------------------------------------------
update tbTable set Time='00:00:00',Status='A'
update tbWarehouseDetail set InStock=0,ExpireDate='2019-09-09',Cost=0
update tbWarehouseSummary set  InStock=0,Ordered=0,[Committed]=0,ExpireDate='2019-09-09',Cost=0,CumulativeValue=0
update tbItemMasterData set StockIn=0,StockOnHand=0
update ItemAccounting set InStock=0,[Committed]=0,Ordered=0,Available=0,CumulativeValue=0
DELETE FROM tbWarehouseDetail where IsDeleted=1
DELETE FROM StockOut
DBCC CHECKIDENT (StockOut, RESEED,0)