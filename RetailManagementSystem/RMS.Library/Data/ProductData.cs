using RMS.Library.DataAccess;
using RMS.Library.Model;

namespace RMS.Library.Data;

public class ProductData : IProductData
{
    private readonly IDataAccess _dataAccess;

    public ProductData(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public Task<List<ProductModel>> GetProducts()
    {
        var output = _dataAccess.LoadData<ProductModel, dynamic>(
            "dbo.spProduct_GetAll",
            new { },
            "RMSDatabase");

        return output;
    }

    public async Task<ProductModel?> GetProductById(int productId)
    {
        var output = await _dataAccess.LoadData<ProductModel, dynamic>
            ("dbo.spProduct_GetById",
                new { Id = productId },
"RMSDatabase");

        return output.FirstOrDefault();
    }

}