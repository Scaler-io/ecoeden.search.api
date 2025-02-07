using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Models.Contracts.Stock;
using System.Globalization;

namespace Ecoeden.Search.Api.Mappers;

public static class StockMapper
{
    public static StockSearchSummary Map(ProductStock stock, ProductSearchSummary product, SupplierSearchSummary supplier)
    {
        return new StockSearchSummary
        {
            Id = stock.Id,
            Category = product.Category,
            Product = new()
            {
                Id = stock.ProductId,
                Name = product.Name,
            },
            Supplier = new()
            {
                Id = stock.SupplierId,
                Name = supplier.Name,
            },
            Quantity = stock.Quantity,
            CreatedOn = DateTime.ParseExact(stock.MetaData.CreatedAt, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
            UpdatedOn = DateTime.ParseExact(stock.MetaData.UpdatedAt, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
        };
    }
}
