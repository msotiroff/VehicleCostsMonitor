namespace VehicleCostsMonitor.Services.Models.Entries.Interfaces
{
    using System;

    public interface IEntryModel
    {
        int Id { get; set; }

        DateTime DateCreated { get; set; }

        decimal Price { get; set; }

        string CurrencyCode { get; set; }
    }
}
