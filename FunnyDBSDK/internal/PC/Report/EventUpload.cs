using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <summary>
/// Upload To Server
/// </summary>
namespace SoFunny.FunnyDB.PC
{
    internal sealed class EventUpload
    {

        #region 高级 API 实现
        private static SemaphoreSlim semaphore = new SemaphoreSlim(2); // 控制并发数为 2
        private static HttpClient httpClient = new HttpClient();
        private static Queue<IngestSignature> requestQueue = new Queue<IngestSignature>();

        internal static void PostIngest(IngestSignature ingestSignature)
        {
            Logger.LogVerbose("enqueue: " + ingestSignature.Timestamp);
            bool startProcessing;
            lock (requestQueue)
            {
                startProcessing = requestQueue.Count == 0; // 判断队列是否为空，决定是否启动处理任务
                requestQueue.Enqueue(ingestSignature);
            }

            if (startProcessing)
            {
                _ = ProcessRequestsAsync();
            }
        }

        private static async Task ProcessRequestsAsync()
        {
            while (true)
            {
                IngestSignature ingestSignature;
                lock (requestQueue)
                {
                    if (requestQueue.Count == 0)
                    {
                        Logger.LogVerbose("queue size zero,quit,wait data active");
                        break;
                    }

                    ingestSignature = requestQueue.Dequeue();
                }
                Logger.LogVerbose("try get lock：" + ingestSignature.Timestamp);
                await semaphore.WaitAsync();
                Logger.LogVerbose("get lock start：" + ingestSignature.Timestamp);
                try
                {
                    await SendRequestAsync(ingestSignature);
                }
                finally
                {
                    semaphore.Release();
                    Logger.LogVerbose("release lock: " + ingestSignature.Timestamp);
                }
            }
        }

        private static async Task SendRequestAsync(IngestSignature ingestSignature)
        {
            Logger.Log("sending: " + ingestSignature.Timestamp + JsonConvert.SerializeObject(ingestSignature.OriginEvents));
            try
            {
                var url = string.Format("{0}{1}", ingestSignature.GetEndPoint, ingestSignature.Url);
                HttpRequestMessage request = new HttpRequestMessage();

                request.Headers.Add("X-Timestamp", ingestSignature.Timestamp);
                request.Headers.Add("X-Nonce", ingestSignature.Nonce);
                request.Headers.Add("X-AccessKeyID", ingestSignature.AccessInfo.AccessKeyId);
                request.Headers.Add("X-Signature", ingestSignature.Sign);

                request.Content = new StringContent(ingestSignature.Body, Encoding.UTF8);

                request.Method = ingestSignature.Method;
                request.RequestUri = new Uri(url);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                CancellationTokenSource timeOutToken = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                var response = await httpClient.SendAsync(request, timeOutToken.Token);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Logger.Log($"上报成功 {responseBody} {ingestSignature.Timestamp}");
                }
                else if (((int)response.StatusCode) >= 500 && ((int)response.StatusCode) <= 599)
                {
                    Logger.Log($"上报失败！StatusCode: {(int)response.StatusCode} Response: {responseBody}");
                    FunnyDBPCInstance.instance.StartCoroutinWithAciton(() =>
                    {
                        Logger.LogVerbose("restore messages: " + JsonConvert.SerializeObject(ingestSignature.OriginEvents, Formatting.Indented));
                        DataSource.SaveAgain(ingestSignature.OriginEvents, ingestSignature.AccessInfo);
                    });
                }
                else
                {
                    Logger.Log($"上报失败！StatusCode: {(int)response.StatusCode} Response: {responseBody} 数据直接丢弃！");
                }
            }
            catch (Exception e)
            {
                Logger.LogError("PostIngest exception: " + e.Message);
                FunnyDBPCInstance.instance.StartCoroutinWithAciton(() =>
                {
                    Logger.LogVerbose("restore messages: " + JsonConvert.SerializeObject(ingestSignature.OriginEvents, Formatting.Indented));
                    DataSource.SaveAgain(ingestSignature.OriginEvents, ingestSignature.AccessInfo);
                });
            }
        }
        #endregion

        #region 旧 API 实现
        //private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(Constants.MAX_CONCURRENCY);

        //internal static void PostIngestOld(IngestSignature ingestSignature)
        //{

        //    ThreadPool.QueueUserWorkItem(state =>
        //    {
        //        try
        //        {
        //            Semaphore.Wait();

        //            //Logger.Log("send_now" + ingestSignature.Body);
        //            //Thread.Sleep(1500);
        //            //Logger.Log("send_end" + ingestSignature.Body);
        //            #region 发起上报请求
        //            var url = string.Format("{0}{1}", ingestSignature.GetEndPoint, ingestSignature.Url);
        //            Logger.Log("PostIngest url: " + url);
        //            var request = WebRequest.Create(url) as HttpWebRequest;
        //            var sendData = Encoding.UTF8.GetBytes(ingestSignature.Body);
        //            request.Method = ingestSignature.Method.Method;
        //            request.ContentType = "application/json";
        //            request.Headers.Add("X-Timestamp", ingestSignature.Timestamp);
        //            request.Headers.Add("X-Nonce", ingestSignature.Nonce);
        //            request.Headers.Add("X-AccessKeyID", ingestSignature.AccessInfo.AccessKeyId);
        //            request.Headers.Add("X-Signature", ingestSignature.Sign);
        //            request.ContentLength = sendData.Length;
        //            request.Timeout = 3000;

        //            var stream = request.GetRequestStream();
        //            stream.Write(sendData, 0, sendData.Length);
        //            stream.Close();
        //            var res = request.GetResponse() as HttpWebResponse;
        //            if (res.StatusCode == HttpStatusCode.OK)
        //            {
        //                var myStreamReader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
        //                var retString = myStreamReader.ReadToEnd();
        //                Logger.Log("now responseStr: " + retString);
        //                Logger.Log("send success");
        //            }
        //            else
        //            {
        //                Logger.LogError("send fail: " + res.StatusCode);
        //            }
        //            res.Close();
        //            #endregion
        //        }
        //        catch (Exception e)
        //        {
        //            Logger.LogError("PostIngest exception: " + e.Message);
        //        }
        //        finally
        //        {
        //            Semaphore.Release();
        //        }
        //    });
        //}
        #endregion
    }
}