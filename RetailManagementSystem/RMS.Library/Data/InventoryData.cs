using RMS.Library.DataAccess;
using RMS.Library.Model;

namespace RMS.Library.Data;

public class InventoryData : IInventoryData
{
    private readonly IDataAccess _dataAccess;

    public InventoryData(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public Task<List<InventoryModel>> GetInventory()
    {
        return _dataAccess.LoadData<InventoryModel, dynamic>(
            "dbo.spInventory_GetAll",
                new { },
            "RMSDatabase");
    }

    public Task<int> SaveInventoryRecord(InventoryModel item)
    {
        return _dataAccess.SaveData("dbo.spInventory_Insert", item, "RMSDatabase");
}
}

