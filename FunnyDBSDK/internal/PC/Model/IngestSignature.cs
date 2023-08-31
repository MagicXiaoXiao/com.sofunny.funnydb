#if UNITY_STANDALONE || UNITY_EDITOR
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

/// <summary>
/// Ingest Signature
/// </summary>
namespace SoFunny.FunnyDB.PC
{
    internal sealed class IngestSignature
    {

        private AccessInfo _accessInfo;
        private HttpMethod _method;
        private string _url;
        private string _nonce;
        private string _timestamp;
        private string _body;
        private string _getEndPoint;
        private string _sign;
        private List<string> _originEvents;
        private string _originEvent;


        internal AccessInfo AccessInfo
        {
            get { return _accessInfo; }
            set { _accessInfo = value; }
        }

        /// <summary>
        /// Http 请求方式 GET、POST
        /// </summary>
        internal HttpMethod Method
        {
            get { return _method; }
            set { _method = value; }
        }
        /// <summary>
        /// api/collect
        /// </summary>
        internal string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// 一个类型为 int32 的随机字符串
        /// </summary>
        internal string Nonce
        {
            get { return _nonce; }
            set { _nonce = value; }
        }
        internal string Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        internal string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        internal string Sign
        {
            get { return _sign; }
            set { _sign = value; }
        }

        internal string GetEndPoint
        {
            get
            {
                return string.IsNullOrEmpty(AccessInfo.EndPoint) ? "https://ingest.sg.xmfunny.com" : AccessInfo.EndPoint;
            }
        }

        internal List<string> OriginEvents
        {
            get { return _originEvents; }
            set { _originEvents = value; }
        }

        internal string OriginEvent
        {
            get { return _originEvent; }
            set { _originEvent = value; }
        }

        internal IngestSignature(AccessInfo info)
        {
            Method = HttpMethod.Post;
            Url = Constants.VALUE_API_URL;
            AccessInfo = info;
        }

        internal string GetToEncryptContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Method).Append(Url).Append(AccessInfo.AccessKeyId).Append(Nonce).Append(Timestamp).Append(Body);
            return sb.ToString();
        }

        internal string GetEventsStr()
        {
            if(_originEvent != null)
            {
                return _originEvent.ToString();
            } else if (_originEvent != null)
            {
                return _originEvents.ToString();
            }
            return null;
        }
    }
}
#endif