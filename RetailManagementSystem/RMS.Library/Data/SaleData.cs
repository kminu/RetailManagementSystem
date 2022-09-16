using Microsoft.Extensions.Configuration;
using RMS.Library.DataAccess;
using RMS.Library.Model;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace RMS.Library.Data;

public class SaleData : ISaleData
{
    private readonly IDataAccess _dataAccess;
    private readonly IProductData _product;

    public SaleData(IDataAccess dataAccess, IProductData product)
    {
        _dataAccess = dataAccess;
        _product = product;
    }
    public async Task SaveSale(SaleModel saleInfo, string cashierId)
    {
        // TODO: Make this SOLID/DRY/BETTER
        // Start filling in the sale detail models we will save to the database
        List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
        //ProductData products = new ProductData();

        // TODO: create tax rate endpoint in API

        string rateText = "0.08";

        bool IsValidTaxRate = Decimal.TryParse(rateText, out decimal taxRate);
        if (IsValidTaxRate == false)
        {
            throw new InvalidDataException("The tax rate is not set up properly");
        }


        foreach (var item in saleInfo.SaleDetails)
        {
            var detail = new SaleDetailDBModel()
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity

            };

            // Get the information about this product
            var productInfo = await _product.GetProductById(detail.ProductId);

            if (productInfo == null)
            {
                throw new Exception($"The product Id of {detail.ProductId} could not be found in the database");
            }

            detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

            if (productInfo.IsTaxable)
            {
                detail.Tax = (detail.PurchasePrice * taxRate);
            }

            details.Add(detail);
        }

        // create the sale model
        SaleDBModel sale = new SaleDBModel()
        {
            SubTotal = details.Sum(x => x.PurchasePrice),
            Tax = details.Sum(x => x.Tax),
            CashierId = cashierId
        };

        sale.Total = sale.SubTotal + sale.Tax;

        try
        {
            _dataAccess.StartTransaction("RMSDatabase");

            // Save the sale model
            _dataAccess.SaveDataInTransaction("dbo.spSale_Insert", sale);

            // Get the Id from the sale model
            var output = await _dataAccess.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate });
            sale.Id = output.FirstOrDefault();

            // Finish filling in the sale detail models
            foreach (var item in details)
            {
                item.SaleId = sale.Id;
                // save the sale detail models
                _dataAccess.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
            }
            _dataAccess.CommitTransaction();
        }
        catch
        {
            _dataAccess.RollbackTransaction();
        }

    }

    public async Task<List<SaleReportModel>> GetSaleReport()
    {
        var output = await _dataAccess.LoadData<SaleReportModel, dynamic>(
            "dbo.spSale_SaleReport",
            new { },
            "RMSDatabase");

        return output;
    }
}