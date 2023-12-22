#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Upload To Server
/// </summary>
namespace SoFunny.FunnyDB.PC
{
    internal sealed class EventUpload
    {
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
                catch(Exception e)
                {
                    Logger.LogError(e.ToString());
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
            Logger.Log("sending: " + ingestSignature.Timestamp + " content: " + ingestSignature.GetEventsStr());
            try
            {
                var url = string.Format("{0}{1}", ingestSignature.GetEndPoint, ingestSignature.Url);
                var compressData = GzipUtils.Compress(Encoding.UTF8.GetBytes(ingestSignature.Body));
                
                string sign = EncryptUtils.GetEncryptSign(ingestSignature.AccessInfo.AccessSecret,
                    GetToEncryptContent(
                        ingestSignature.Method.ToString(),
                        ingestSignature.Url,
                        ingestSignature.AccessInfo._accessKeyId,
                        ingestSignature.Nonce,
                        ingestSignature.Timestamp,
                        compressData));
                ingestSignature.Sign = sign;

                HttpRequestMessage request = new HttpRequestMessage();
                request.Headers.Add("X-Timestamp", ingestSignature.Timestamp);
                request.Headers.Add("X-Nonce", ingestSignature.Nonce);
                request.Headers.Add("X-AccessKeyID", ingestSignature.AccessInfo.AccessKeyId);
                request.Headers.Add("X-Signature", ingestSignature.Sign);
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                ByteArrayContent content = new ByteArrayContent(compressData);
                content.Headers.Add("Content-Encoding", "gzip");
                request.Content = content;
                request.Method = ingestSignature.Method;
                request.RequestUri = new Uri(url);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                CancellationTokenSource timeOutToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.CONNECTION_TIMEOUT));
                var response = await httpClient.SendAsync(request, timeOutToken.Token);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Logger.Log($"上报成功 {responseBody} {ingestSignature.Timestamp}");
                }
                else if (((int)response.StatusCode) >= 500 && ((int)response.StatusCode) <= 599)
                {
                    Logger.LogVerbose("restore messages: " + ingestSignature.GetEventsStr());
                    Logger.Log($"上报失败！StatusCode: {(int)response.StatusCode} Response: {responseBody}");
                    ProccessSendFailedEvents(ingestSignature);
                }
                else
                {
                    Logger.LogVerbose("restore messages: " + ingestSignature.GetEventsStr());
                    Logger.Log($"上报失败！StatusCode: {(int)response.StatusCode} Response: {responseBody} 数据直接丢弃！");
                }
            }
            catch (Exception e)
            {
                Logger.LogError("PostIngest exception: " + e.Message);
                ProccessSendFailedEvents(ingestSignature);
            }
        }

        private static void ProccessSendFailedEvents(IngestSignature ingestSignature)
        {
            if(ingestSignature.OriginEvent != null)
            {
                DataSource.Create(ingestSignature.OriginEvent, ingestSignature.AccessInfo.AccessKeyId);
            }
            else if (ingestSignature.OriginEvents != null)
            {
                DataSource.Creates(ingestSignature.OriginEvents, ingestSignature.AccessInfo);
            }
        }
        private static byte[] GetToEncryptContent(string method, string url, string key, string nonce, string timestamp, byte[] compressBody)
        {
            byte[] all = Encoding.UTF8.GetBytes(method);

            all = GzipUtils.AddAll(all, Encoding.UTF8.GetBytes(url));
            all = GzipUtils.AddAll(all, Encoding.UTF8.GetBytes(key));
            all = GzipUtils.AddAll(all, Encoding.UTF8.GetBytes(nonce));
            all = GzipUtils.AddAll(all, Encoding.UTF8.GetBytes(timestamp));

            if (compressBody != null)
            {
                all = GzipUtils.AddAll(all, compressBody);
            }

            return all;
        }
    }
}
#endif