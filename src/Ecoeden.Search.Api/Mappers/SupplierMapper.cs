using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Models.Contracts.Supplier;
using System.Globalization;

namespace Ecoeden.Search.Api.Mappers;

public class SupplierMapper
{
    public static IEnumerable<SupplierSearchSummary> Map(IEnumerable<Supplier> suppliers)
    {
        return suppliers.Select(supplier => new SupplierSearchSummary
        {
            Id = supplier.Id,
            Name  = supplier.Name,
            Email = supplier.ContactDetails.Email,
            Phone = supplier.ContactDetails.Phone,
            Address = supplier.ContactDetails.Address.GetAddressString(),
            CreatedOn = DateTime.ParseExact(supplier.MetaData.CreatedAt, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
            UpdatedOn = DateTime.ParseExact(supplier.MetaData.UpdatedAt, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
        });
    }
}
