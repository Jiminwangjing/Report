UPDATE tbWarehouseSummary set [Committed] = 0;
UPDATE tbWarehouseSummary set [Committed] = cstock from (select od.ItemID, sum (od.Qty * gu.Factor) as cstock from tbOrderDetail od 
	INNER JOIN tbGroupDefindUoM gu on od.GroupUomID = od.GroupUomID and od.UomID = gu.AltUOM
	INNER JOIN tbWarehouseSummary ws on ws.ItemID = od.ItemID
	INNEr JOIN tbItemMasterData im on ws.ItemID = im.ID and Process != 'Standard' group by od.ItemID) _ws
	where _ws.ItemID = tbWarehouseSummary.ItemID