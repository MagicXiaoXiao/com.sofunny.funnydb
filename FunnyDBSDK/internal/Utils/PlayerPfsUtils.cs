using UnityEngine;
namespace SoFunny.FunnyDB
{
    internal static class PlayerPfsUtils
    {
        internal static void Save<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (typeof(T) == typeof(int))
            {
                PlayerPrefs.SetInt(key, (int)(object)value);
            }
            else if (typeof(T) == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (float)(object)value);
            }
            else if (typeof(T) == typeof(string))
            {
                PlayerPrefs.SetString(key, (string)(object)value);
            }
        }

        internal static T Get<T>(string key)
        {
            if (!string.IsNullOrEmpty(key) && PlayerPrefs.HasKey(key))
            {
                if (typeof(T) == typeof(int))
                {
                    return (T)(object)PlayerPrefs.GetInt(key);
                }
                else if (typeof(T) == typeof(float))
                {
                    return (T)(object)PlayerPrefs.GetFloat(key);
                }
                else if (typeof(T) == typeof(string))
                {
                    return (T)(object)PlayerPrefs.GetString(key);
                }
            }
            return default(T);
        }

        internal static void Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }

        internal static bool Exist(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            return PlayerPrefs.HasKey(key);
        }
    }
}

