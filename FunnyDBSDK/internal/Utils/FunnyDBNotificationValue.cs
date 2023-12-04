using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SoFunny.FunnyDB
{
    internal class FunnyDBNotificationValue
    {
        public static FunnyDBNotificationValue Empty = new FunnyDBNotificationValue();

        public static FunnyDBNotificationValue Create(string value)
        {
            return new FunnyDBNotificationValue(value);
        }

        private readonly string optional;
        private readonly JObject jsonData;

        private FunnyDBNotificationValue()
        {
            optional = "";
        }

        private FunnyDBNotificationValue(string value)
        {
            optional = value;
            jsonData = JObject.Parse(value);
        }

        internal bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(optional);
            }
        }

        internal string RawValue => optional;

        internal bool TryGet<T>(out T target)
        {
            target = default;

            if (string.IsNullOrEmpty(optional))
            {
                return false;
            }
            try
            {
                target = JsonConvert.DeserializeObject<T>(optional);
                return true;
            }
            catch (JsonException ex)
            {
                Logger.LogError("FunnyDBNotificationValue Deserialize error. " + ex.Message);
                return false;
            }

        }

        internal T TryGetValue<T>(string propertyName)
        {
            if (jsonData.TryGetValue(propertyName, out JToken token))
            {
                return token.Value<T>();
            }

            return default;
        }

    }
}

