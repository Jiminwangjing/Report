---- Update missing ReceiptIDs in table MultipaymentMeans
--UPDATE mp SET mp.ReceiptID = r.ReceiptID FROM tbReceipt r
--RIGHT JOIN tbPaymentMeans p ON p.ID = r.PaymentMeansID OR 1 = 1
--RIGHT JOIN MultiPaymentMean mp ON mp.PaymentMeanID = p.ID
--WHERE mp.ReceiptID NOT IN (SELECT r.ReceiptID FROM tbReceipt r)

-- Insert missing records in table MultipaymentMeans based on ReceiptID
INSERT INTO MultiPaymentMean (
	ReceiptID, PaymentMeanID, AltCurrencyID, AltCurrency, AltRate, PLCurrencyID, PLCurrency, 
	PLRate, Amount, OpenAmount, Total, SCRate, LCRate, ReturnStatus, [Type], Exceed
) 
SELECT r.ReceiptID, 
	CASE WHEN r.PaymentMeansID > 0 THEN r.PaymentMeansID 
	ELSE (SELECT TOP 1 ID from tbPaymentMeans) END, 
	r.SysCurrencyID, c.[Description], r.ExchangeRate, r.SysCurrencyID, c.[Description],
	r.ExchangeRate, r.GrandTotal, r.GrandTotal, r.GrandTotal, r.ExchangeRate, r.LocalSetRate, 0, 0, 0
FROM tbReceipt r JOIN tbCurrency c on c.ID = r.SysCurrencyID
WHERE r.ReceiptID NOT IN (SELECT mp.ReceiptID FROM MultiPaymentMean mp)