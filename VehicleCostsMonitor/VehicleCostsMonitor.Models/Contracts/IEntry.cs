using System;

namespace VehicleCostsMonitor.Models.Contracts
{
    public interface IEntry
    {
        int Id { get; }

        DateTime DateCreated { get; }
    }
}
