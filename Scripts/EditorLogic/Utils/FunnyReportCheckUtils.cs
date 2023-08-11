using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoFunny.FunnyDB
{
    /// <summary>
    /// 1、当前做了 Key 校验、类型校验,空值校验
    /// 2、暂时不做值的边界情况校验，随着规模的发展以及涉及的业务领域，逐步加强校验。
    /// </summary>
    public static class FunnyReportVerifyUtils
    {
        /// <summary>
        /// 检查事件名称
        /// </summary>
        /// <param name="eventName"></param>
        public static bool VerifyEventName(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogWarning("eventName null or empty please check!");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 检查上报的 CustomHashTable
        /// </summary>
        /// <param name="hashtable"></param>
        public static bool VerifyHashTableValue(this Hashtable hashtable)
        {

            if (hashtable == null)
            {
                Debug.LogWarning("custom table is null");
                return true;
            }

            bool ret = true;
            foreach (string key in hashtable.Keys)
            {
                if (!VerifyEventName(key))
                {
                    Debug.LogWarning("key is null,please check !");
                    //hashtable.Remove(key);
                    ret = false;
                }
                var curValue = hashtable[key];
                ret = VerifySingleValue(hashtable, key, curValue);
            }
            return ret;
        }

        private static bool VerifySingleValue(Hashtable originHashTable, string key, object curValue)
        {
            bool ret = true;

            if (!VerifySupportTypes(curValue))
            {
                Debug.LogWarning("value must be one of that: numeric、bool、string、list、hashtable,please check !");
                //originHashTable.Remove(key);
                ret = false;
            }
            return ret;
        }

        #region 类型判断

        private static bool VerifySupportTypes(object value)
        {
            return value is bool || value is string || value is Hashtable || VerifyDictionary(value) || VerifyList(value) || VerifyArray(value) || VerifyNumeric(value);
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
