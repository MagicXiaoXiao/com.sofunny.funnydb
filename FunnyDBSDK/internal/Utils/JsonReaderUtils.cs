using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SoFunny.FunnyDB
{

    public static class JsonReaderUtils
    {
        private static StringReader _stringReader = null;

        public static Dictionary<string, object> Parse(string jsonString)
        {
            if (_stringReader == null)
            {
                _stringReader = new StringReader(jsonString);
            }
            else
            {
                _stringReader.Dispose();
                _stringReader = new StringReader(jsonString);
            }

            using (JsonTextReader reader = new JsonTextReader(_stringReader))
            {
                return ParseObject(reader);
            }
        }

        private static Dictionary<string, object> ParseObject(JsonTextReader reader)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            string currentPropertyName = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    if (!string.IsNullOrEmpty(currentPropertyName))
                    {
                        dictionary[currentPropertyName] = ParseObject(reader);
                    }
                }
                else if (reader.TokenType == JsonToken.StartArray)
                {
                    if (!string.IsNullOrEmpty(currentPropertyName))
                    {
                        dictionary[currentPropertyName] = ParseArray(reader);
                    }
                }
                else if (reader.TokenType == JsonToken.PropertyName)
                {
                    currentPropertyName = reader.Value?.ToString();
                }
                else if (reader.TokenType == JsonToken.Integer ||
                         reader.TokenType == JsonToken.Float ||
                         reader.TokenType == JsonToken.Boolean ||
                         reader.TokenType == JsonToken.String ||
                         reader.TokenType == JsonToken.Null)
                {
                    if (!string.IsNullOrEmpty(currentPropertyName))
                    {
                        dictionary[currentPropertyName] = reader.Value;
                    }
                }
                else if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }

            return dictionary;
        }

        private static List<object> ParseArray(JsonTextReader reader)
        {
            List<object> list = new List<object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    list.Add(ParseObject(reader));
                }
                else if (reader.TokenType == JsonToken.StartArray)
                {
                    list.Add(ParseArray(reader));
                }
                else if (reader.TokenType == JsonToken.Integer ||
                         reader.TokenType == JsonToken.Float ||
                         reader.TokenType == JsonToken.Boolean ||
                         reader.TokenType == JsonToken.String ||
                         reader.TokenType == JsonToken.Null)
                {
                    list.Add(reader.Value);
                }
                else if (reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }
            }

            return list;
        }

    }
}

