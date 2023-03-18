namespace KEDI.Core.Premise.Models.Sync
{
    public interface IEntityState
    {
        bool IsModified { get; }
        bool IsDuplicate { set; get; }  
        bool IsValid { set; get; }
    }
}
