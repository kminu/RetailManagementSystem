using RMS.Library.Model;

namespace RMS.Library.Data
{
    public interface ISaleData
    {
        Task<List<SaleReportModel>> GetSaleReport();
        Task SaveSale(SaleModel saleInfo, string cashierId);
    }
}