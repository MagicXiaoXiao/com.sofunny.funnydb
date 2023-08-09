#if (UNITY_EDITOR || UNITY_STANDALONE)

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

/// <summary>
/// Upload To Server
/// </summary>
namespace SoFunny.FunnyDB {
    public sealed class EventUpload {
        public static void PostIngest(IngestSignature ingestSignature, string data, string sign) {
            var url = string.Format("{0}{1}", Common.Endpoint, ingestSignature.Url);
            ThreadPool.QueueUserWorkItem((state => {
                try {
                    var request = WebRequest.Create(url) as HttpWebRequest;
                    var sendData = Encoding.UTF8.GetBytes(data);
                    request.Method = ingestSignature.Method;
                    request.ContentType = "application/json";
                    request.Headers.Add("X-Timestamp", ingestSignature.Timestamp);
                    request.Headers.Add("X-Nonce", ingestSignature.Nonce);
                    request.Headers.Add("X-AccessKeyID", ingestSignature.AccessKeyId);
                    request.Headers.Add("X-Signature", sign);
                    request.ContentLength = sendData.Length;
                    request.Timeout = 3000;
                    
                    var stream = request.GetRequestStream();
                    stream.Write(sendData, 0, sendData.Length);
                    stream.Close();
                    var res = request.GetResponse() as HttpWebResponse;
                    if (res.StatusCode == HttpStatusCode.OK) {
                        var myStreamReader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                        var retString = myStreamReader.ReadToEnd();
                        Logger.Log("now responseStr: " + retString);
                        Logger.Log("send success");
                    } else {
                        Logger.LogError("send fail: " + res.StatusCode);
                    }
                    res.Close();
                } catch (Exception e) {
                    Logger.LogError("PostIngest exception: " + e.Message);
                }
            }));
        }
    }
}

#endif