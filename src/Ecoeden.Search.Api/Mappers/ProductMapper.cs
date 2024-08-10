using System.Globalization;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Models.Contracts;

namespace Ecoeden.Search.Api.Mappers;

public static class ProductMapper
{
    public static IEnumerable<ProductSearchSummary> Map(IEnumerable<Product> products)
    {
        return products.Select(product => new ProductSearchSummary
        {
            Category = product.Category,
            CreatedOn = DateTime.ParseExact(product.MetaData.CreatedAt, "dd/MM/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture),
            LastUpdatedOn = DateTime.ParseExact(product.MetaData.UpdatedAt, "dd/MM/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture),
            Id = product.Id,
            ImageFile = product.ImageFile,
            Name = product.Name,
            Slug = product.Slug,
        });
    }
}
