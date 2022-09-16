using RMS.Library.Model;

namespace RMS.Library.Data
{
    public interface IProductData
    {
        Task<List<ProductModel>> GetProducts();
        Task<ProductModel?> GetProductById(int productId);
    }
}