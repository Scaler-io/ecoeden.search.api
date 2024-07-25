﻿namespace Ecoeden.Search.Api.Extensions;

public static class SearchIndexNameExtensions
{
    public static bool IsProductSearchIndex(this string value)
    {
        return value == "product-search-index";
    }
}