namespace KEDI.Core.Premise.Models.Services.Inventory
{
    public interface ISelectListItem
    {
        public int ID { set; get; }
        public string Name { set; get; }
        public bool Delete { set; get; }
    }
}
