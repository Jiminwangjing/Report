
GO
/****** Object:  StoredProcedure [dbo].[pos_uspGetNoneIssuedValidStockReceipts]    Script Date: 3/3/2023 8:36:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pos_uspGetNoneIssuedValidStockReceipts]
AS
BEGIN
	SELECT DISTINCT * FROM tbReceipt _r WHERE _r.ReceiptID 
	NOT IN (
		SELECT rd.ReceiptID FROM tbReceiptDetail rd
		INNER JOIN tbItemMasterData im on rd.ItemID = im.ID AND UPPER(im.Process) != 'STANDARD'
		INNER JOIN tbWarehouseSummary ws on ws.ItemID = im.ID 
			AND ws.WarehouseID = im.WarehouseID 
			AND (ws.InStock <= 0 OR (ws.InStock - ws.[Committed]) < rd.Qty)	
		GROUP BY rd.ReceiptID
	)
	AND  _r.SeriesDID NOT IN (SELECT i.SeriesDetailID FROM tbInventoryAudit i)
END;

