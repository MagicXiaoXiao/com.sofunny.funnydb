#if UNITY_STANDALONE || UNITY_EDITOR
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SoFunny.FunnyDB.PC
{
    /// <summary>
    /// 数据源存储信息的 CRUD，两级缓存：内存缓存与文件缓存（PlayerPfs 形式）
    /// 1、考虑性能问题，与移动端相比，获取数据只按自增 ID 进行读取，不按时间排序，如果一定要实现则要损失一些性能，与产品确认下。
    /// 2、文件缓存所有 AccessKeyID、minIndex、maxIndex，在后续的缓存管理中用的到。
    ///   2.1、初始化时将存储数据存储到内存中，如果有发现没添加的补充并会写到文件缓存。
    ///   2.2、什么时候删除？主动还是被动，存储量不大，给 1KB？，accessKeyId 不会频繁更换 !!!!!
    /// 3、数据读取存储策略
    ///     3.1、存储键值组成：accessKeyId + index
    ///     3.2、写策略：
    ///         a、正常：新数据文件缓存下标从 maxIndex 开始，写入完成后更新 maxIndex 值
    ///         b、回写：
    ///             1、简化方案：按新记录回写，放在新数据后面，发送机会放后面
    ///             2、最优方案：【发送失败回写 minIndex 下标的往前计算，以便优先得到发送机会】
    ///     3.2、读策略
    ///         a、从 minIndex 下标往后开始计算读 n 个
    ///         b、读到内存后删除文件缓存相应数据（有小概率的丢失风险，用户主动退出或崩溃）并更新 minIndex 值
    /// </summary>
    internal static partial class DataSource
    {
        /// <summary>
        /// 初始化方法，可以进行多次
        /// </summary>
        /// <param name="ackId"></param>
        internal static void Init(string ackId)
        {
            lock (dataLock)
            {
                if (!IsInit)
                {
                    string allAkIdJson = PlayerPfsUtils.Get<string>(KEY_ALL_ACCESS_KEYS);
                    if (!string.IsNullOrEmpty(allAkIdJson))
                    {
                        _curIndexDictionary = JsonConvert.DeserializeObject<Dictionary<string, MinMaxData>>(allAkIdJson);
                    }
                    IsInit = true;
                }
                if (!_curIndexDictionary.ContainsKey(ackId))
                {
                    _curIndexDictionary.Add(ackId, new MinMaxData());
                }
                SaveAccessKeyIdMappings();
                CheckData();
            }
        }

        /// <summary>
        /// 创建一条记录
        /// </summary>
        /// <param name="evenJson"></param>
        /// <param name="accessKeyID"></param>
        /// <returns></returns>
        internal static bool Create(string evenJson, string accessKeyID)
        {
            if (!IsInit)
            {
                return false;
            }
            MinMaxData data = _curIndexDictionary[accessKeyID];
            Logger.Log("Create start cnt: " + " data" + JsonConvert.SerializeObject(data));
            string saveKey = GetKey(accessKeyID, data.Max);
            PlayerPfsUtils.Save(saveKey, evenJson);
            data.Max += 1;
            if(data.Max == int.MaxValue)
            {

            }
            Logger.Log("Create end cnt: " + " data" + JsonConvert.SerializeObject(data));
            SaveAccessKeyIdMappings();
            
            return true;
        }

        internal static void Creates(List<string> message, AccessInfo accessInfo)
        {
            if (message == null)
            {
                return;
            }
            message.ForEach((msg) =>
            {
                Create(msg, accessInfo.AccessKeyId);
            });
        }

        /// <summary>
        /// 从 min 开始往后读取 count 个数据
        /// </summary>
        /// <param name="ackId">accessKeyID</param>
        /// <param name="count">读取条数</param>
        /// <returns></returns>
        internal static List<string> Read(string ackId, int count)
        {
            if (!IsInit)
            {
                return null;
            }
            List<string> pairs = new List<string>();
            MinMaxData data = _curIndexDictionary[ackId];
            int expectEndIndex = data.Min + count;
            //大于 Max 的时候可以考虑归零
            int realEndIndex = expectEndIndex > data.Max ? data.Max : expectEndIndex;
            for (int i = data.Min; i < realEndIndex; i++)
            {
                string key = GetKey(ackId, i);
                if (PlayerPfsUtils.Exist(key))
                {
                    string value = PlayerPfsUtils.Get<string>(key);
                    if (!string.IsNullOrEmpty(value))
                    {
                        pairs.Add(value);
                    }
                }

            }
            Logger.Log("Read: " + pairs.ToArray().ToString());
            return pairs;
        }
        /// <summary>
        /// 从 min 往后删除 count 个数据
        /// </summary>
        /// <param name="ackId"></param>
        /// <param name="count"></param>
        internal static void Delete(string ackId, int count)
        {
            if (!IsInit)
            {
                return;
            }
            MinMaxData data = _curIndexDictionary[ackId];
            int delCnt = 0;
            int expectEndIndex = data.Min + count;
            int realMinIndex = expectEndIndex >= data.Max ? data.Max : expectEndIndex;
            for (int i = data.Min; i < realMinIndex; i++)
            {
                string key = GetKey(ackId, i);
                PlayerPfsUtils.Delete(key);
                delCnt++;
            }
            data.Min = realMinIndex;
            Logger.Log("del cnt: " + delCnt + " data" + JsonConvert.SerializeObject(data));
            SaveAccessKeyIdMappings();
        }

        internal static int GetCountByAcKID(string ackId)
        {
            var data = _curIndexDictionary[ackId];
            return data.Max - data.Min;
        }
    }


    internal static partial class DataSource
    {
        private static readonly object dataLock = new object();
        private static readonly string KEY_ALL_ACCESS_KEYS = "Key_All_Access_Keys";
        /// <summary>
        /// 第一个是 accessKeyID，第二个为第一个对于事件的自增 ID 数值
        /// </summary>
        private static Dictionary<string, MinMaxData> _curIndexDictionary = new Dictionary<string, MinMaxData>();
        private static bool IsInit = false;
        private const int _DelWhenOverCnt = 100;
        private const int _MaxCacheCnt = 20000;

        private static void SaveAccessKeyIdMappings()
        {
            var mappingJsonStr = JsonConvert.SerializeObject(_curIndexDictionary);
            Logger.Log("SaveAccessKeyIdMappings: " + mappingJsonStr);
            PlayerPfsUtils.Save(KEY_ALL_ACCESS_KEYS, mappingJsonStr);
        }
        private static string GetKey(string ackId, int index)
        {
            return ackId + "_" + index;
        }

        private static void CheckData()
        {
            // 1、计算存储所有个数 TotalCnt
            // 2、判断是否超出最大个数,不超出结束，超出执行以下步骤
            // 3、算出最大个数的 keyId，MaxKeyID 与所有 keyID 的平均个数 avgCnt
            // 4、计算出最大值减去平均值 maxCntSubCnt = maxCnt - avgCnt，
            // 5、maxCntSubCnt >= _DelWhenOverCnt，减去最大个数 KeyID 一列数据即可，否则继续以下步骤
            // 6、减去 KeyId 最多列到平均值，还有多余值 delMaxLeftCnt = _DelWhenOverCnt - maxCntSubCnt
            // 7、各个 KeyId 需要减去 everyKeyIdShouldSubCnt = delMaxLeftCnt / KeyIdsCnt
            // 8、各个 KeyID 减去 everyKeyIdShouldSubCnt
            //int totalCnt = 0;
            //string maxKeyId = "";
            //int maxCnt = 0;
            //foreach (string key in _curIndexDictionary.Keys)
            //{
            //    MinMaxData curMinMax = _curIndexDictionary[key];
            //    int curCnt = curMinMax.Max - curMinMax.Min;
            //    totalCnt += curCnt;
            //    if (maxCnt < curCnt)
            //    {
            //        maxCnt = curCnt;
            //        maxKeyId = key;
            //    }
            //}
            //if (totalCnt < _MaxCacheCnt)
            //{
            //    return;
            //}

            //int avgCnt = totalCnt / _curIndexDictionary.Count;
            //int maxCntSubCnt = maxCnt - avgCnt;
            //// 数据最多的一行够减时，减去当前 KeyID 数据即可结束
            //if (maxCntSubCnt >= _DelWhenOverCnt)
            //{
            //    Delete(maxKeyId, _DelWhenOverCnt);
            //    return;
            //}

            //int delMaxLeftCnt = _DelWhenOverCnt - maxCntSubCnt;
            //int everyKeyIdShouldSubCnt = delMaxLeftCnt / _curIndexDictionary.Count;

            //foreach (string key in _curIndexDictionary.Keys)
            //{
            //    Delete(key, everyKeyIdShouldSubCnt);
            //}
        }
    }

    internal class MinMaxData
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

}
#endif