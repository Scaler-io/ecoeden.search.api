﻿namespace Ecoeden.Search.Api.Models.Contracts.Customer;

public class Customer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ContactDetails ContactDetails { get; set; }
    public bool Status { get; set; }
    public MetaData MetaData { get; set; }
}
