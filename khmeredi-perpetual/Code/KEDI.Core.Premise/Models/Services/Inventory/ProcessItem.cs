namespace CKBS.Models.Services.Inventory
{
    public enum ProcessItem
    {
        None = 0,
        FIFO = 1,
        FEFO = 2,
        Standard = 3,
        Average = 4,
        SEBA = 5, //(Serial/Batch)
    }
}
