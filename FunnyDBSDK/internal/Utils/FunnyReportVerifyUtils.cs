using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoFunny.FunnyDB
{
    /// <summary>
    /// 1、当前做了 Key 校验、类型校验,空值校验
    /// 2、暂时不做值的边界情况校验，随着规模的发展以及涉及的业务领域，逐步加强校验。
    /// </summary>
    internal static class FunnyReportVerifyUtils
    {
        /// <summary>
        /// 检查事件名称
        /// </summary>
        /// <param name="eventName"></param>
        internal static bool VerifyEventName(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogWarning($"{Logger.k_Tag} eventName null or empty please check!");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 检查上报的参数
        /// </summary>
        /// <param name="customProperties"></param>
        internal static bool VerifyDictionaryValue(this Dictionary<string, object> customProperties)
        {

            if (customProperties == null)
            {
                return true;
            }

            bool ret = true;
            foreach (string key in customProperties.Keys)
            {
                if (!VerifyEventName(key))
                {
                    Debug.LogWarning($"{Logger.k_Tag} key is null,please check !");
                    ret = false;
                }
                var curValue = customProperties[key];
                ret = VerifySingleValue(key, curValue);
            }
            return ret;
        }

        private static bool VerifySingleValue(string key, object curValue)
        {
            bool ret = true;

            if (!VerifySupportTypes(curValue))
            {
                Debug.LogWarning($"{Logger.k_Tag} key {key} value {curValue} did not match one of: numeric、bool、string、list、dictionary, please check !");
                ret = false;
            }
            if (VerifyArray(curValue))
            {
                return VerifyArrayValue(curValue);
            }

            if (VerifyDictionary(curValue))
            {
                return VerifyDictionaryValue((Dictionary<string, object>)curValue);
            }

            if (VerifyList(curValue))
            {
                return VerifyListValue(key, (List<object>)curValue);
            }
            return ret;
        }

        private static bool VerifyArrayValue(object curValue)
        {
            bool ret = false;
            Array array = curValue as Array;

            for (int i = 0; i < array.Length; i++)
            {
                ret = VerifySingleValue(i.ToString(), array.GetValue(i));
            }
            return ret;
        }

        private static bool VerifyListValue(string key, List<object> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var listItem = list[i];
                if (listItem == null)
                {
                    Debug.LogWarning($"{Logger.k_Tag} key {key} list index {i} is null, please check!");
                }
                VerifySingleValue(i.ToString(), listItem);
            }
            return false;
        }

        #region 类型判断

        private static bool VerifySupportTypes(object value)
        {
            return value is bool || value is string || VerifyDictionary(value) || VerifyList(value) || VerifyArray(value) || VerifyNumeric(value);
        }

        private static bool VerifyDictionary(object value)
        {
            if (value == null)
                return false;
            return (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>));
        }

        private static bool VerifyArray(object value)
        {
            return value != null && value is Array;
        }

        private static bool VerifyList(object value)
        {
            if (value == null)
                return false;
            var type = value.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        private static bool VerifyNumeric(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is float
                    || value is double
                    || value is long
                    || value is ulong
                    || value is decimal;
        }

        #endregion
    }
}
