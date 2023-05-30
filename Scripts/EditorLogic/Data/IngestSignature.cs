#if (UNITY_EDITOR || UNITY_STANDALONE)

using System.Text;

/// <summary>
/// Ingest Signature
/// </summary>
namespace SoFunny.FunnyDB {
    public sealed class IngestSignature {
        // POST  GET
        private string method;
        // /api/collect
        private string url;
        private string accessKeyId;
        // 一个类型为 int32 的随机字符串
        private string nonce;
        private string timestamp;
        // request content
        private string body;

        public string Method {
            get { return method; }
            set { method = value; }
        }
        public string Url {
            get { return url; }
            set { url = value; }
        }
        public string AccessKeyId {
            get { return accessKeyId; }
            set { accessKeyId = value; }
        }
        public string Nonce {
            get { return nonce; }
            set { nonce = value; }
        }
        public string Timestamp {
            get { return timestamp; }
            set { timestamp = value; }
        }
        public string Body {
            get { return body; }
            set { body = value; }
        }
        public IngestSignature(string akId) {
            Method = Constants.VALUE_METHOD;
            Url = Constants.VALUE_API_URL;
            AccessKeyId = akId;
        }
        
        public string getToEncryptContent() {
            StringBuilder sb = new StringBuilder();
            sb.Append(method).Append(url).Append(accessKeyId).Append(nonce).Append(timestamp).Append(body);
            return sb.ToString();
        }
    }
}

#endif