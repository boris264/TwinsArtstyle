using Newtonsoft.Json;

namespace TwinsArtstyle.Helpers
{
    public static class JsonHelper
    {
        public static string Serialize(object? value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
