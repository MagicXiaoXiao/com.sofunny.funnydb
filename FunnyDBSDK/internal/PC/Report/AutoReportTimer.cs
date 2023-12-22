#if UNITY_STANDALONE || UNITY_EDITOR
using Newtonsoft.Json;
using System;
using System.IO;

namespace SoFunny.FunnyDB.PC
{
    internal class AutoReportTimer
    {
        private static readonly AutoReportTimer instance = new AutoReportTimer();
        private StringWriter _stringWriter;
        private JsonTextWriter _jsonWriter;
        private bool _isFlushing = false;
        private AutoReportTimer()
        {
            _stringWriter = new StringWriter();
            _jsonWriter = new JsonTextWriter(_stringWriter);
        }

        internal static AutoReportTimer Instance => instance;

        internal void Init()
        {

        }

        /// <summary>
        /// 发送失败直接回写，失败情况完善放在后面处理
        /// </summary>
        internal void DoCheckDataSource(bool isFlush = false)
        {
            if (!ReportSettings.CanSend())
            {
                return;
            }
            if (_isFlushing)
            {
                Logger.Log("flushing data, skip timing report or flush");
                return;
            }
            _isFlushing = isFlush;
            var accessKeyHashtable = FunnyDBPCInstance.Instance.AccessKeyHashTable;
            var accessChannelSet = accessKeyHashtable.Keys;
            foreach (var channel in accessChannelSet)
            {
                AccessInfo accessInfo = (AccessInfo)accessKeyHashtable[channel];

                while (DataSource.GetCountByAcKID(accessInfo.AccessKeyId) != 0)
                {
                    Report(accessInfo);
                    if (!isFlush)
                    {
                        Logger.Log("not flush do once!");
                        break;
                    }
                }
            }
            _isFlushing = false;
        }


        private void Report(AccessInfo accessInfo)
        {
            var messageArr = DataSource.Read(accessInfo.AccessKeyId, ReportSettings.ReportSizeLimit);

            // 数据库中删除掉
            DataSource.Delete(accessInfo.AccessKeyId, messageArr.Count);

            try
            {
                _jsonWriter.WriteStartObject();

                _jsonWriter.WritePropertyName(Constants.KEY_MESSAGES);
                _jsonWriter.WriteStartArray();
                foreach (string json in messageArr)
                {
                    _jsonWriter.WriteRawValue(json);
                }
                _jsonWriter.WriteEndArray();
                _jsonWriter.WriteEndObject();

                string sendData = _stringWriter.ToString();
                _stringWriter.GetStringBuilder().Clear();

                Logger.Log("Auto Report: " + sendData);
                IngestSignature ingestSignature = new IngestSignature(accessInfo)
                {
                    Nonce = new Random().Next().ToString(),
                    Timestamp = FunnyDBPCInstance.Instance.CalibratedTime.GetInMills().ToString(),
                    Body = sendData
                };
                ingestSignature.OriginEvents = messageArr;
                EventUpload.PostIngest(ingestSignature);
            }
            catch (Exception)
            {
                _stringWriter = new StringWriter();
                _jsonWriter = new JsonTextWriter(_stringWriter);
            }
        }
    }
}
#endif
