using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SoFunny.FunnyDB
{

    public static class JsonWriterUtils
    {
        public static string ConvertDictionaryToJson(Dictionary<string, object> dictionary)
        {
            StringWriter stringWriter = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter);
            try
            {
                WriteDictionary(jsonWriter, dictionary);
                return stringWriter.ToString();
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                jsonWriter.Close();
            }
        }

        private static void WriteDictionary(JsonTextWriter writer, Dictionary<string, object> dictionary)
        {
            writer.WriteStartObject();

            foreach (var kvp in dictionary)
            {
                writer.WritePropertyName(kvp.Key);

                if (kvp.Value is Dictionary<string, object> innerDictionary)
                {
                    WriteDictionary(writer, innerDictionary);
                }
                else if (kvp.Value is List<object> list)
                {
                    WriteList(writer, list);
                }
                else
                {
                    writer.WriteValue(kvp.Value);
                }
            }
            writer.WriteEndObject();
        }
        public static void WriteList(JsonTextWriter writer, List<object> list)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                if (item is Dictionary<string, object> innerDictionary)
                {
                    WriteDictionary(writer, innerDictionary);
                }
                else
                {
                    writer.WriteValue(item);
                }
            }

            writer.WriteEndArray();
        }

        internal static void Write(JsonWriter jsonWriter, string key, object value)
        {
            jsonWriter.WritePropertyName(key);
            jsonWriter.WriteValue(value);
        }
    }
}
