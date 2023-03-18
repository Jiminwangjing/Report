update tbAccountBalance set tbAccountBalance.BPAcctID = 0, tbAccountBalance.Creator = 0;

update tbAccountBalance set tbAccountBalance.BPAcctID = (select top 1 BPAcctID from tbJournalEntryDetail
where ItemID = tbAccountBalance.GLAID);

update tbAccountBalance set tbAccountBalance.Creator = je.Creator from tbJournalEntry as je 
inner join tbJournalEntryDetail jed on je.ID = jed.JEID
where tbAccountBalance.BPAcctID = jed.BPAcctID and jed.BPAcctID > 0;

select * from tbAccountBalance
select * from tbJournalEntry
select * from tbJournalEntryDetail
