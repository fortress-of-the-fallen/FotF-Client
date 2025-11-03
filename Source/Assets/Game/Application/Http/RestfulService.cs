// Unity implementation of RestfulService using UnityWebRequest instead of HttpClient
// Mirrors the original API and behavior (timeouts, headers, cancellation, JSON/multipart)

#if UNITY_2019_1_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Game.Application.Interface;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Game.Application.Base;

namespace Game.Application.Http
{
    public class RestfulService : Singleton<RestfulService>, IRestfulService
    {
        public async Task<(bool, HttpStatusCode)> Delete(
            string url,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false)
        {
            using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbDELETE))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                ApplyHeaders(request, headers);
                request.timeout = NormalizeTimeout(timeout);

                var status = await SendAsync(request, token);

                if (status.isTimeoutOrCanceled)
                    return (false, HttpStatusCode.RequestTimeout);

                if (status.isSuccess || status.code == HttpStatusCode.BadRequest)
                {
                    return (status.isSuccess, status.code);
                }

                return (false, status.code);
            }
        }

        public async Task<(TResult, HttpStatusCode)> Get<TResult>(
            string url,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class
        {
            using (var request = UnityWebRequest.Get(url))
            {
                ApplyHeaders(request, headers);
                request.timeout = NormalizeTimeout(timeout);

                var status = await SendAsync(request, token);

                if (status.isTimeoutOrCanceled)
                    return (null, HttpStatusCode.RequestTimeout);

                if (status.isSuccess || status.code == HttpStatusCode.BadRequest)
                {
                    TResult result = default;
                    if (typeof(TResult) == typeof(string))
                    {
                        result = (TResult)(object)(status.text ?? string.Empty);
                    }
                    else
                    {
                        result = JsonConvert.DeserializeObject<TResult>(status.text ?? string.Empty);
                    }

                    return (result, status.code);
                }

                return (null, status.code);
            }
        }

        public async Task<(TResult, HttpStatusCode)> Patch<TResult>(
            string url,
            object rawData,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class
        {
            var json = JsonConvert.SerializeObject(rawData);
            var body = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(url, "PATCH"))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                ApplyHeaders(request, headers);
                request.timeout = NormalizeTimeout(timeout);

                var status = await SendAsync(request, token);

                if (status.isTimeoutOrCanceled)
                    return (null, HttpStatusCode.RequestTimeout);

                if (status.isSuccess || status.code == HttpStatusCode.BadRequest)
                {
                    TResult result;
                    if (typeof(TResult) == typeof(string))
                    {
                        result = (TResult)(object)(status.text ?? string.Empty);
                    }
                    else
                    {
                        result = JsonConvert.DeserializeObject<TResult>(status.text ?? string.Empty);
                    }

                    return (result, status.code);
                }

                return (null, status.code);
            }
        }

        public async Task<(TResult, HttpStatusCode)> Post<TResult>(
            string url,
            object rawData,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class
        {
            var json = JsonConvert.SerializeObject(rawData);
            var body = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                ApplyHeaders(request, headers);
                request.timeout = NormalizeTimeout(timeout);

                var status = await SendAsync(request, token);

                if (status.isTimeoutOrCanceled)
                    return (null, HttpStatusCode.RequestTimeout);

                if (status.isSuccess || status.code == HttpStatusCode.BadRequest)
                {
                    TResult result;
                    if (typeof(TResult) == typeof(string))
                    {
                        result = (TResult)(object)(status.text ?? string.Empty);
                    }
                    else
                    {
                        result = JsonConvert.DeserializeObject<TResult>(status.text ?? string.Empty);
                    }

                    return (result, status.code);
                }

                return (null, status.code);
            }
        }

        public async Task<(TResult, HttpStatusCode)> Post<TResult>(
            string url,
            Dictionary<string, object> formFields = null,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class
        {
            var disposableStreams = new List<Stream>();
            try
            {
                var form = new WWWForm();

                if (formFields != null && formFields.Count > 0)
                {
                    foreach (var kv in formFields)
                    {
                        if (kv.Value is FileStream fs)
                        {
                            disposableStreams.Add(fs);
                            var fileName = Path.GetFileName(fs.Name);
                            using (var ms = new MemoryStream())
                            {
                                await fs.CopyToAsync(ms, 81920, token);
                                var bytes = ms.ToArray();
                                form.AddBinaryData(kv.Key, bytes, fileName, "application/octet-stream");
                            }
                        }
                        else
                        {
                            form.AddField(kv.Key, kv.Value?.ToString() ?? string.Empty);
                        }
                    }
                }

                using (var request = UnityWebRequest.Post(url, form))
                {
                    ApplyHeaders(request, headers);
                    request.timeout = NormalizeTimeout(timeout);

                    var status = await SendAsync(request, token);

                    if (status.isTimeoutOrCanceled)
                        return (null, HttpStatusCode.RequestTimeout);

                    if (status.isSuccess || status.code == HttpStatusCode.BadRequest)
                    {
                        TResult result = default;
                        if (typeof(TResult) == typeof(string))
                        {
                            result = (TResult)(object)(status.text ?? string.Empty);
                        }
                        else
                        {
                            result = JsonConvert.DeserializeObject<TResult>(status.text ?? string.Empty);
                        }

                        return (result, status.code);
                    }

                    return (null, status.code);
                }
            }
            finally
            {
                foreach (var s in disposableStreams)
                {
                    try { s.Dispose(); } catch { /* ignore */ }
                }
            }
        }

        public async Task<(TResult, HttpStatusCode)> Put<TResult>(
            string url,
            object rawData,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class
        {
            var json = JsonConvert.SerializeObject(rawData);
            var body = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                ApplyHeaders(request, headers);
                request.timeout = NormalizeTimeout(timeout);

                var status = await SendAsync(request, token);

                if (status.isTimeoutOrCanceled)
                    return (null, HttpStatusCode.RequestTimeout);

                if (status.isSuccess || status.code == HttpStatusCode.BadRequest)
                {
                    TResult result;
                    if (typeof(TResult) == typeof(string))
                    {
                        result = (TResult)(object)(status.text ?? string.Empty);
                    }
                    else
                    {
                        result = JsonConvert.DeserializeObject<TResult>(status.text ?? string.Empty);
                    }

                    return (result, status.code);
                }

                return (null, status.code);
            }
        }

        public async Task<(TResult, HttpStatusCode)> Put<TResult>(
            string url,
            Dictionary<string, object> formFields = null,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class
        {
            var disposableStreams = new List<Stream>();
            try
            {
                var sections = new List<IMultipartFormSection>();

                if (formFields != null && formFields.Count > 0)
                {
                    foreach (var kv in formFields)
                    {
                        if (kv.Value is FileStream fs)
                        {
                            disposableStreams.Add(fs);
                            var fileName = Path.GetFileName(fs.Name);
                            using (var ms = new MemoryStream())
                            {
                                await fs.CopyToAsync(ms, 81920, token);
                                var bytes = ms.ToArray();
                                sections.Add(new MultipartFormFileSection(kv.Key, bytes, fileName, "application/octet-stream"));
                            }
                        }
                        else
                        {
                            sections.Add(new MultipartFormDataSection(kv.Key, kv.Value?.ToString() ?? string.Empty));
                        }
                    }
                }

                // Create a POST with multipart content, then change method to PUT to keep body/headers intact
                using (var request = UnityWebRequest.Post(url, sections))
                {
                    request.method = UnityWebRequest.kHttpVerbPUT;
                    ApplyHeaders(request, headers);
                    request.timeout = NormalizeTimeout(timeout);

                    var status = await SendAsync(request, token);

                    if (status.isTimeoutOrCanceled)
                        return (null, HttpStatusCode.RequestTimeout);

                    if (status.isSuccess || status.code == HttpStatusCode.BadRequest)
                    {
                        TResult result = default;
                        if (typeof(TResult) == typeof(string))
                        {
                            result = (TResult)(object)(status.text ?? string.Empty);
                        }
                        else
                        {
                            result = JsonConvert.DeserializeObject<TResult>(status.text ?? string.Empty);
                        }

                        return (result, status.code);
                    }

                    return (null, status.code);
                }
            }
            finally
            {
                foreach (var s in disposableStreams)
                {
                    try { s.Dispose(); } catch { /* ignore */ }
                }
            }
        }

        private static void ApplyHeaders(UnityWebRequest request, Dictionary<string, string> headers)
        {
            if (headers == null) return;
            foreach (var h in headers)
            {
                // Allow overriding existing headers (like Content-Type)
                request.SetRequestHeader(h.Key, h.Value);
            }
        }

        private static int NormalizeTimeout(int seconds)
        {
            // UnityWebRequest.timeout: 0 = no timeout. Use provided value if > 0
            return seconds > 0 ? seconds : 0;
        }

        private struct SendStatus
        {
            public string text;
            public HttpStatusCode code;
            public bool isSuccess;
            public bool isTimeoutOrCanceled;
        }

        private static async Task<SendStatus> SendAsync(UnityWebRequest request, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();
            var operation = request.SendWebRequest();

            // Complete when Unity finishes the request
            operation.completed += _ =>
            {
                try { tcs.TrySetResult(true); } catch { }
            };

            // Abort on cancellation
            using (token.Register(() =>
                   {
                       try
                       {
                           if (!request.isDone)
                               request.Abort();
                       }
                       catch { /* ignore */ }
                   }))
            {
                await tcs.Task;
            }

            // Map results
            var responseText = request.downloadHandler != null ? request.downloadHandler.text : null;
            var code = request.responseCode > 0 ? (HttpStatusCode)request.responseCode : HttpStatusCode.RequestTimeout;

            bool canceledOrTimeout = false;

#if UNITY_2020_2_OR_NEWER
            // Unity modern API
            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    return new SendStatus { text = responseText, code = code, isSuccess = true, isTimeoutOrCanceled = false };
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    canceledOrTimeout = (code == HttpStatusCode.RequestTimeout) || token.IsCancellationRequested;
                    break;
            }
#else
            // Legacy API: check isNetworkError/isHttpError
            if (!request.isNetworkError && !request.isHttpError)
            {
                return new SendStatus { text = responseText, code = code, isSuccess = true, isTimeoutOrCanceled = false };
            }
            canceledOrTimeout = (code == HttpStatusCode.RequestTimeout) || token.IsCancellationRequested;
#endif

            if (canceledOrTimeout)
            {
                return new SendStatus { text = responseText, code = HttpStatusCode.RequestTimeout, isSuccess = false, isTimeoutOrCanceled = true };
            }

            return new SendStatus { text = responseText, code = code, isSuccess = false, isTimeoutOrCanceled = false };
        }
    }
}
#endif
