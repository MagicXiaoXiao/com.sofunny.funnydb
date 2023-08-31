#if UNITY_STANDALONE || UNITY_EDITOR
using Newtonsoft.Json;
using System.IO;

namespace SoFunny.FunnyDB.PC
{
    internal class AutoReportTimer
    {
        private static readonly AutoReportTimer instance = new AutoReportTimer();
        private StringWriter _stringWriter;
        private JsonTextWriter _jsonWriter;
        private bool _isFlushing = false;
        private AutoReportTimer() {
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
            if(_isFlushing)
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
                }
            }
            _isFlushing = false;
        }


        private void Report(AccessInfo accessInfo)
        {
            if (!ReportSettings.CanSend())
            {
                return;
            }
            var messageArr = DataSource.Read(accessInfo.AccessKeyId, ReportSettings.ReportSizeLimit);

            // 数据库中删除掉
            DataSource.Delete(accessInfo.AccessKeyId, messageArr.Count);


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
                Nonce = new System.Random().Next().ToString(),
                Timestamp = FunnyDBPCInstance.Instance.CalibratedTime.GetInMills().ToString(),
                Body = sendData
            };

            string sign = EncryptUtils.GetEncryptSign(accessInfo.AccessSecret, ingestSignature.GetToEncryptContent());
            ingestSignature.Sign = sign;
            ingestSignature.OriginEvents = messageArr;
            EventUpload.PostIngest(ingestSignature);
        }
    }
}
#endif
