namespace VehicleCostsMonitor.Models
{
    public class FuelEntryExtraFuelConsumer
    {
        public int FuelEntryId { get; set; }

        public FuelEntry FuelEntry { get; set; }

        public int ExtraFuelConsumerId { get; set; }

        public ExtraFuelConsumer ExtraFuelConsumer { get; set; }
    }
}