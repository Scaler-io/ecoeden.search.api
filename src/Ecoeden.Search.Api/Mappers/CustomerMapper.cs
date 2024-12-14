using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Models.Contracts.Customer;
using System.Globalization;

namespace Ecoeden.Search.Api.Mappers;

public class CustomerMapper
{
    public static IEnumerable<CustomerSearchSummary> Map(IEnumerable<Customer> customers)
    {
        return customers.Select(customer => new CustomerSearchSummary
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.ContactDetails.Email,
            Phone = customer.ContactDetails.Phone,
            Status = customer.Status,
            Address = customer.ContactDetails.Address.GetAddressString(),
            CreatedOn = DateTime.ParseExact(customer.MetaData.CreatedAt, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
            UpdatedOn = DateTime.ParseExact(customer.MetaData.UpdatedAt, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
        });
    }
}
