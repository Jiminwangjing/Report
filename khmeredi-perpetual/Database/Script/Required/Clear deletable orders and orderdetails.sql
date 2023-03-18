
DELETE FROM tbOrderDetail WHERE OrderID IN (SELECT o.OrderID FROM tbOrder o WHERE o.[Delete] = 1)
DELETE FROM tbOrder WHERE [Delete] = 1;


