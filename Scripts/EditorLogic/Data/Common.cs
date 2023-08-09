#if (UNITY_EDITOR || UNITY_STANDALONE)

namespace SoFunny.FunnyDB {
    public static class Common {
        private static string accessKeyId;
        private static string accessKeySecret;
        private static string endpoint;
        public static string AccessKeyId {
            get { return accessKeyId; }
            set { accessKeyId = value; }
        }
        public static string AccessKeySecret {
            get { return accessKeySecret; }
            set { accessKeySecret = value; }
        }
        public static string Endpoint {
            get { return endpoint; }
            set { endpoint = value; }
        }

        public static void Init(string accessKeyId, string accessKeySecret, string endPoint) {
            AccessKeyId = accessKeyId;
            AccessKeySecret = accessKeySecret;
            if (string.IsNullOrEmpty(endPoint)) {
                // default
                Endpoint = "http://ingest.funnydb-stage.funnydata.net";
            }
            else {
                Endpoint = endPoint;
            }
        }
    }
}

#endif