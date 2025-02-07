namespace Ecoeden.Search.Api.Extensions;

public static class SearchIndexNameExtensions
{
    public static bool IsProductSearchIndex(this string value) => value == "product-search-index";

    public static bool IsUserSearchIndex(this string value) => value == "user-search-index";

    public static bool IsSupplierSearchIndex(this string value) => value == "supplier-search-index";

    public static bool IsCustomerSearchIndex(this string value) => value == "customer-search-index";
    public static bool IsUnitSearchIndex(this string value) => value == "unit-search-index";

    public static bool IsStockSearchIndex(this string value) => value == "stock-search-index";
}
