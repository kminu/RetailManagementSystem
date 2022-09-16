using RMS.Library.Model;

namespace RMS.Library.Data
{
    public interface IInventoryData
    {
        Task<List<InventoryModel>> GetInventory();
        Task<int> SaveInventoryRecord(InventoryModel item);
    }
}