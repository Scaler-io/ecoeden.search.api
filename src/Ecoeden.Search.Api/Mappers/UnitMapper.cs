using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Models.Contracts.Unit;
using System.Globalization;

namespace Ecoeden.Search.Api.Mappers;

public class UnitMapper
{
    public static IEnumerable<UnitSearchSummary> Map(IEnumerable<Unit> units)
    {
        return units.Select(unit => new UnitSearchSummary
        {
            Id = unit.Id,
            Name = unit.Name,
            Status = unit.Status,
            CreatedOn = DateTime.ParseExact(unit.MetaData.CreatedAt, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
            UpdatedOn = DateTime.ParseExact(unit.MetaData.UpdatedAt, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)
        });
    }
}
