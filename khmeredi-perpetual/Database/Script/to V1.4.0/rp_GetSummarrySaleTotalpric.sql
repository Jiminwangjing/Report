USE [KWEBPOSV1.3.3.CardMember]
GO
/****** Object:  StoredProcedure [dbo].[rp_GetSummarrySaleTotalpric]    Script Date: 2/19/2022 11:51:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER proc [dbo].[rp_GetSummarrySaleTotalpric](@DateFrom date='1900-01-01',@DateTo date='1900-01-01', @BranchID int=0,@UserID int=0, @TimeFrom time='00:00',@TimeTo time='00:00', @plid int = 0)
as 
begin
    declare @Count int=0
	declare @SoldAmount float
	declare @DisItem float=0
	declare @DisTotal float=0
	declare @TaxValue float=0;
	declare @GrandTotal float=0
	declare @GrandTotalSys float=0
	declare @DTFrom datetime
	declare @DTTo datetime

	IF @DateFrom!='1900-01-01' and @DateTo !='1900-01-01' and @BranchID=0 and @UserID=0 and @TimeFrom='00:00' and @TimeTo='00:00'
		Begin
			--detail
			select @SoldAmount=sum(rd.Qty*rd.UnitPrice),@DisItem=sum(rd.DiscountValue) 
			from tbReceipt r inner join tbReceiptDetail rd on r.ReceiptID=rd.ReceiptID 
			where r.DateOut>=@DateFrom and r.DateOut<=@DateTo  and r.PriceListID=@plid
			--summarry
			select @Count=count(*), @GrandTotal=sum(r.GrandTotal*r.PLRate*r.LocalSetRate),@GrandTotalSys=sum(r.GrandTotal_Sys),@TaxValue=sum(TaxValue),@DisTotal=sum(DiscountValue*PLRate) 
			from tbReceipt r where r.DateOut>=@DateFrom and r.DateOut<=@DateTo and r.PriceListID=@plid
			--profit
			--select @TotalCost=sum(rv.Qty*rv.Cost) from tbRevenueItem rv where rv.SystemDate>=@DateFrom and rv.SystemDate<=@DateTo
			--group by rv.ItemID,rv.Cost
		End
	Else if @DateFrom!='1900-01-01' and @DateTo !='1900-01-01' and @BranchID!=0 and @UserID=0 and @TimeFrom='00:00' and @TimeTo='00:00'
		Begin
			--detail
			select @SoldAmount=sum(rd.Qty*rd.UnitPrice),@DisItem=sum(rd.DiscountValue) 
			from tbReceipt r inner join tbReceiptDetail rd on r.ReceiptID=rd.ReceiptID 
			where r.DateOut>=@DateFrom and r.DateOut<=@DateTo and r.BranchID=@BranchID  and r.PriceListID=@plid
			--summarry
			select @Count=count(*), @GrandTotal=sum(r.GrandTotal*r.PLRate*r.LocalSetRate),@GrandTotalSys=sum(r.GrandTotal_Sys),@TaxValue=sum(TaxValue),@DisTotal=sum(DiscountValue*PLRate) 
			from tbReceipt r where r.DateOut>=@DateFrom and r.DateOut<=@DateTo and r.BranchID=@BranchID and r.PriceListID=@plid
			--profit
			--select @TotalCost=sum(rv.Qty*rv.Cost) from tbRevenueItem rv where rv.SystemDate>=@DateFrom and rv.SystemDate<=@DateTo and rv.BranchID=@BranchID
			--group by rv.ItemID,rv.Cost
		End
	Else if @DateFrom!='1900-01-01' and @DateTo !='1900-01-01' and @BranchID!=0 and @UserID!=0 and @TimeFrom='00:00' and @TimeTo='00:00'
		Begin
			--detail
			select @SoldAmount=sum(rd.Qty*rd.UnitPrice),@DisItem=sum(rd.DiscountValue) 
			from tbReceipt r inner join tbReceiptDetail rd on r.ReceiptID=rd.ReceiptID 
			where r.DateOut>=@DateFrom and r.DateOut<=@DateTo and r.BranchID=@BranchID and r.UserOrderID=@UserID and r.PriceListID=@plid
			--summarry
			select @Count=count(*),  @GrandTotal=sum(r.GrandTotal*r.PLRate*r.LocalSetRate),@GrandTotalSys=sum(r.GrandTotal_Sys),@TaxValue=sum(TaxValue),@DisTotal=sum(DiscountValue*PLRate) 
			from tbReceipt r where r.DateOut>=@DateFrom and r.DateOut<=@DateTo and r.BranchID=@BranchID and r.UserOrderID=@UserID and r.PriceListID=@plid
			--profit
			--select @TotalCost=sum(rv.Qty*rv.Cost) from tbRevenueItem rv where rv.SystemDate>=@DateFrom and rv.SystemDate<=@DateTo and rv.BranchID=@BranchID and rv.UserID=@UserID
			--group by rv.ItemID,rv.Cost
		End
	Else if @DateFrom!='1900-01-01' and @DateTo !='1900-01-01' and @BranchID=0 and @UserID=0 and @TimeFrom!='00:00' and @TimeTo!='00:00'
		Begin

			SET @DTFrom = CONVERT(datetime, CONCAT(CONVERT(varchar, @DateFrom,23), ' ' ,CONVERT(varchar, @TimeFrom,8)),20)
			SET @DTTo = CONVERT(datetime, CONCAT(CONVERT(varchar, @DateTo,23), ' ' ,CONVERT(varchar, @TimeTo,8)),20)
			
			--detail
			select @SoldAmount=sum(rd.Qty*rd.UnitPrice),@DisItem=sum(rd.DiscountValue) 
			from tbReceipt r inner join tbReceiptDetail rd on r.ReceiptID=rd.ReceiptID
			where CONVERT(datetime, CONCAT(CONVERT(varchar,r.DateOut,23), ' ',CONVERT(varchar, r.[TimeOut],8)),20) Between @DTFrom and @DTTo and r.PriceListID=@plid
			--summarry
			select @Count=count(*),  @GrandTotal=sum(r.GrandTotal*r.PLRate*r.LocalSetRate),@GrandTotalSys=sum(r.GrandTotal_Sys),@TaxValue=sum(TaxValue),@DisTotal=sum(DiscountValue*PLRate) 
			from tbReceipt r where CONVERT(datetime, CONCAT(CONVERT(varchar,r.DateOut,23), ' ',CONVERT(varchar, r.[TimeOut],8)),20) Between @DTFrom and @DTTo and r.PriceListID=@plid
			--profit
			--select @TotalCost=sum(rv.Qty*rv.Cost) from tbRevenueItem rv where CONVERT(datetime, CONCAT(CONVERT(varchar, rv.SystemDate,23), ' ',CONVERT(varchar, rv.TimeIn,8)),20) Between @DTFrom and @DTTo
			--group by rv.ItemID,rv.Cost
		End
	Else if @DateFrom!='1900-01-01' and @DateTo !='1900-01-01' and @BranchID!=0 and @UserID=0 and @TimeFrom!='00:00' and @TimeTo!='00:00'
		Begin

			SET @DTFrom = CONVERT(datetime, CONCAT(CONVERT(varchar, @DateFrom,23), ' ' ,CONVERT(varchar, @TimeFrom,8)),20)
			SET @DTTo = CONVERT(datetime, CONCAT(CONVERT(varchar, @DateTo,23), ' ' ,CONVERT(varchar, @TimeTo,8)),20)

			--detail
			select @SoldAmount=sum(rd.Qty*rd.UnitPrice),@DisItem=sum(rd.DiscountValue) 
			from tbReceipt r inner join tbReceiptDetail rd on r.ReceiptID=rd.ReceiptID
			where CONVERT(datetime, CONCAT(CONVERT(varchar,r.DateOut,23), ' ',CONVERT(varchar, r.[TimeOut],8)),20) Between @DTFrom and @DTTo and r.BranchID=@BranchId and r.PriceListID=@plid
			--summarry
			select @Count=count(*),  @GrandTotal=sum(r.GrandTotal*r.PLRate*r.LocalSetRate),@GrandTotalSys=sum(r.GrandTotal_Sys),@TaxValue=sum(TaxValue),@DisTotal=sum(DiscountValue*PLRate) 
			from tbReceipt r where CONVERT(datetime, CONCAT(CONVERT(varchar,r.DateOut,23), ' ',CONVERT(varchar, r.[TimeOut],8)),20) Between @DTFrom and @DTTo and r.BranchID=@BranchId and r.PriceListID=@plid
			--profit
			--select @TotalCost=sum(rv.Qty*rv.Cost) from tbRevenueItem rv where CONVERT(datetime, CONCAT(CONVERT(varchar, rv.SystemDate,23), ' ',CONVERT(varchar, rv.TimeIn,8)),20) Between @DTFrom and @DTTo and rv.BranchID=@BranchID
			--group by rv.ItemID,rv.Cost
		End
	Else if @DateFrom!='1900-01-01' and @DateTo !='1900-01-01' and @BranchID!=0 and @UserID!=0 and @TimeFrom!='00:00' and @TimeTo!='00:00'
		Begin

			SET @DTFrom = CONVERT(datetime, CONCAT(CONVERT(varchar, @DateFrom,23), ' ' ,CONVERT(varchar, @TimeFrom,8)),20)
			SET @DTTo = CONVERT(datetime, CONCAT(CONVERT(varchar, @DateTo,23), ' ' ,CONVERT(varchar, @TimeTo,8)),20)
			
			--detail
			select @SoldAmount=sum(rd.Qty*rd.UnitPrice),@DisItem=sum(rd.DiscountValue) 
			from tbReceipt r inner join tbReceiptDetail rd on r.ReceiptID=rd.ReceiptID
			where CONVERT(datetime, CONCAT(CONVERT(varchar,r.DateOut,23), ' ',CONVERT(varchar, r.[TimeOut],8)),20) Between @DTFrom and @DTTo and r.BranchID=@BranchId and r.UserOrderID=@UserId and r.PriceListID=@plid
			--summarry
			select @Count=count(*),  @GrandTotal=sum(r.GrandTotal*r.PLRate*r.LocalSetRate),@GrandTotalSys=sum(r.GrandTotal_Sys),@TaxValue=sum(TaxValue),@DisTotal=sum(DiscountValue*PLRate) 
			from tbReceipt r where CONVERT(datetime, CONCAT(CONVERT(varchar,r.DateOut,23), ' ',CONVERT(varchar, r.[TimeOut],8)),20) Between @DTFrom and @DTTo and r.BranchID=@BranchId and r.UserOrderID=@UserId and r.PriceListID=@plid
			--profit
			--select @TotalCost=sum(rv.Qty*rv.Cost) from tbRevenueItem rv where CONVERT(datetime, CONCAT(CONVERT(varchar, rv.SystemDate,23), ' ',CONVERT(varchar, rv.TimeIn,8)),20) Between @DTFrom and @DTTo and rv.BranchID=@BranchID and rv.UserID=@UserID
			--group by rv.ItemID,rv.Cost
		End
	select 
		   1 as ID,
		   @Count as CountReceipt,
		   @SoldAmount as SoldAmount,
	       @DisItem as DiscountItem,
		   @DisTotal as DiscountTotal,
		   @TaxValue as TaxValue,
		   @GrandTotal as GrandTotal,
		   @GrandTotalSys as GrandTotalSys
		   --@TotalCost as TotalCost,
		   --@GrandTotal-@TotalCost as TotalProfit
end


