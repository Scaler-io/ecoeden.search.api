using System.Globalization;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Models.Contracts;

namespace Ecoeden.Search.Api.Mappers;

public class UserMapper
{
    public static IEnumerable<UserSearchSummary> Map(IEnumerable<User> users)
    {
        return users.Select(user => new UserSearchSummary
        {
            Id = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            IsDefaultAdmin = user.IsDefaultAdmin,
            IsActive = user.IsActive,
            UserName = user.UserName,
            UserRoles = user.UserRoles,
            Permissions = user.Permissions,
            LastLogin = user.LastLogin,
            CreatedOn = DateTime.ParseExact(user.MetaData.CreatedAt, "dd/MM/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture),
            UpdatedOn = DateTime.ParseExact(user.MetaData.UpdatedAt, "dd/MM/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture)
        });
    }
}
