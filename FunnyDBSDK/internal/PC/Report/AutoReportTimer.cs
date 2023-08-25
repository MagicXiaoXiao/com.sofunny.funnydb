using System.Collections;
using UnityEngine;
using Newtonsoft.Json;

namespace SoFunny.FunnyDB.PC
{
    internal class AutoReportTimer
    {
        private static readonly AutoReportTimer instance = new AutoReportTimer();

        private AutoReportTimer() { }

        internal static AutoReportTimer Instance => instance;

        internal static readonly int REAPEAT_RATE = 1000;
        internal static bool isFlushing = false;

        internal void Init()
        {
     
        }
    
 
        /// <summary>
        /// 发送失败直接回写，失败情况完善放在后面处理
        /// </summary>
        internal void DoCheckDataSource(bool isFlush = false)
        {
            if(!isFlush && isFlushing)
            {
                Logger.Log("flushing data, skip timing report");
                return;
            }
            var accessKeyHashtable = FunnyDBPCInstance.instance.AccessKeyHashTable;
            var accessKeySet = accessKeyHashtable.Keys;
            foreach (var channel in accessKeySet)
            {
                AccessInfo accessInfo = (AccessInfo)accessKeyHashtable[channel];
                Report(accessInfo, isFlush);
            }

            if(isFlushing)
            {
                isFlushing = false;
            }
        }


        internal void Report(AccessInfo accessInfo, bool isFlush)
        {
            if (!ReportSettings.CanSend())
            {
                return;
            }
            int savedCnt = DataSource.GetCountByAcKID(accessInfo.AccessKeyId);
            if (savedCnt == 0)
            {
                Logger.LogVerbose(accessInfo.AccessKeyId + " save cnt size is zero do not report");
                return;
            }
            var messageArr = DataSource.Read(accessInfo.AccessKeyId, ReportSettings.ReportSizeLimit);

            // 数据库中删除掉
            DataSource.Delete(accessInfo.AccessKeyId, ReportSettings.ReportSizeLimit);

            Hashtable sendObj = new Hashtable()
            {
                { Constants.KEY_MESSAGES, messageArr }
            };
            string sendData = JsonConvert.SerializeObject(sendObj);

            IngestSignature ingestSignature = new IngestSignature(accessInfo)
            {
                Nonce = new System.Random().Next() + "",
                Timestamp = FunnyDBPCInstance.instance.CalibratedTime.GetInMills() + "",
                Body = sendData
            };

            string sign = EncryptUtils.GetEncryptSign(accessInfo.AccessSecret, ingestSignature.GetToEncryptContent());
            ingestSignature.Sign = sign;
            ingestSignature.OriginEvents = messageArr;
            EventUpload.PostIngest(ingestSignature);
        }


    }
}

