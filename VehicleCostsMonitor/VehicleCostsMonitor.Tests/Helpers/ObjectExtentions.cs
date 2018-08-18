namespace VehicleCostsMonitor.Tests.Helpers
{
    using Newtonsoft.Json;

    internal static class ObjectExtentions
    {
        internal static TObject Clone<TObject>(this TObject obj)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var serializedObject = JsonConvert.SerializeObject(obj, jsonSettings);

            var deserialized = JsonConvert.DeserializeObject<TObject>(serializedObject);

            return deserialized;
        }
    }
}
