namespace VehicleCostsMonitor.Models.Contracts
{
    using System;

    public interface IEntry
    {
        int Id { get; }

        DateTime DateCreated { get; }
    }
}
