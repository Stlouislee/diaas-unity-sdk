using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DIaaS.Core
{
    public class DIaaSClient : MonoBehaviour
    {
        public static DIaaSClient Instance { get; private set; }

        [SerializeField] private DIaaSConfig config;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(DIaaSConfig configuration)
        {
            config = configuration;
            config.BaseUrl = NormalizeBaseUrl(config.BaseUrl);
            Debug.Log($"[DIaaS] Initialized with BaseUrl: {config.BaseUrl}");
        }

        /// <summary>
        /// Get the root URL (without /api/v1) for health checks
        /// </summary>
        public string GetBaseUrlRoot()
        {
            if (string.IsNullOrEmpty(config.BaseUrl)) return "";
            int apiIndex = config.BaseUrl.IndexOf("/api/");
            if (apiIndex > 0)
            {
                return config.BaseUrl.Substring(0, apiIndex);
            }
            return config.BaseUrl;
        }

        private string NormalizeBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl)) return baseUrl;

            const string requiredPath = "/api/v1";
            string trimmed = baseUrl.Trim().TrimEnd('/');

            if (!trimmed.Contains(requiredPath))
            {
                string normalized = trimmed + requiredPath;
                Debug.LogWarning($"DIaaS BaseUrl missing '{requiredPath}', normalized to: {normalized}");
                return normalized;
            }

            return trimmed;
        }

        public IEnumerator GetRequest<T>(string endpoint, Action<T> onSuccess, Action<string> onError)
        {
            string url = CombineUrl(config.BaseUrl, endpoint);
            Debug.Log($"[DIaaS] GET {url}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.timeout = config.Timeout;
                AttachHeaders(webRequest);
                yield return webRequest.SendWebRequest();

                HandleResponse(webRequest, onSuccess, onError);
            }
        }

        /// <summary>
        /// GET request that returns raw JSON string (useful for dynamic/complex responses)
        /// </summary>
        public IEnumerator GetRequestRaw(string endpoint, Action<string> onSuccess, Action<string> onError)
        {
            string url = CombineUrl(config.BaseUrl, endpoint);
            yield return GetRequestRawUrl(url, onSuccess, onError);
        }

        /// <summary>
        /// GET request to a direct URL (not combined with base URL)
        /// </summary>
        public IEnumerator GetRequestRawUrl(string url, Action<string> onSuccess, Action<string> onError)
        {
            Debug.Log($"[DIaaS] GET (raw) {url}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.timeout = config.Timeout;
                AttachHeaders(webRequest);
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    onSuccess?.Invoke(webRequest.downloadHandler.text);
                }
                else
                {
                    onError?.Invoke(webRequest.error + ": " + webRequest.downloadHandler.text);
                }
            }
        }

        public IEnumerator PostRequest<T>(string endpoint, object body, Action<T> onSuccess, Action<string> onError)
        {
            string url = CombineUrl(config.BaseUrl, endpoint);
            string jsonBody = JsonUtility.ToJson(body);
            yield return PostRequestInternal(url, jsonBody, onSuccess, onError);
        }

        private IEnumerator PostRequestInternal<T>(string url, string jsonBody, Action<T> onSuccess, Action<string> onError)
        {
            Debug.Log($"[DIaaS] POST {url}");
            Debug.Log($"[DIaaS] Body: {jsonBody}");
            Debug.Log($"[DIaaS] API Key: {(string.IsNullOrEmpty(config.ApiKey) ? "NOT SET" : "***" + config.ApiKey.Substring(config.ApiKey.Length - 4))}");
            using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.timeout = config.Timeout;
                AttachHeaders(webRequest);

                yield return webRequest.SendWebRequest();

                HandleResponse(webRequest, onSuccess, onError);
            }
        }

        public IEnumerator PostRequest<T>(string endpoint, string jsonBody, Action<T> onSuccess, Action<string> onError)
        {
            string url = CombineUrl(config.BaseUrl, endpoint);
            yield return PostRequestInternal(url, jsonBody, onSuccess, onError);
        }

        /// <summary>
        /// POST request that returns raw JSON string (useful for dynamic/complex responses)
        /// </summary>
        public IEnumerator PostRequestRawResponse(string endpoint, string jsonBody, Action<string> onSuccess, Action<string> onError)
        {
            string url = CombineUrl(config.BaseUrl, endpoint);
            using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.timeout = config.Timeout;
                AttachHeaders(webRequest);

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    onSuccess?.Invoke(webRequest.downloadHandler.text);
                }
                else
                {
                    onError?.Invoke(webRequest.error + ": " + webRequest.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteRequest(string endpoint, Action onSuccess, Action<string> onError)
        {
            string url = CombineUrl(config.BaseUrl, endpoint);
            using (UnityWebRequest webRequest = UnityWebRequest.Delete(url))
            {
                webRequest.timeout = config.Timeout;
                AttachHeaders(webRequest);
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    onSuccess?.Invoke();
                }
                else
                {
                    onError?.Invoke(webRequest.error + ": " + webRequest.downloadHandler.text);
                }
            }
        }

        private string CombineUrl(string baseUri, string relativeUri)
        {
            if (string.IsNullOrEmpty(baseUri)) return relativeUri;
            if (string.IsNullOrEmpty(relativeUri)) return baseUri;

            baseUri = baseUri.Trim();
            relativeUri = relativeUri.Trim();

            if (baseUri.EndsWith("/"))
            {
                baseUri = baseUri.Substring(0, baseUri.Length - 1);
            }
            if (relativeUri.StartsWith("/"))
            {
                relativeUri = relativeUri.Substring(1);
            }

            return $"{baseUri}/{relativeUri}";
        }

        private void AttachHeaders(UnityWebRequest request)
        {
            request.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                request.SetRequestHeader("X-API-Key", config.ApiKey);
            }
        }

        private void HandleResponse<T>(UnityWebRequest request, Action<T> onSuccess, Action<string> onError)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string json = request.downloadHandler.text;
                    
                    if (typeof(T).IsArray)
                    {
                        if (json.TrimStart().StartsWith("["))
                        {
                            json = "{ \"Items\": " + json + "}";
                            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
                            onSuccess?.Invoke(wrapper.Items);
                        }
                        else
                        {
                             T responseData = JsonUtility.FromJson<T>(json);
                             onSuccess?.Invoke(responseData);
                        }
                    }
                    else
                    {
                        T responseData = JsonUtility.FromJson<T>(json);
                        onSuccess?.Invoke(responseData);
                    }
                }
                catch (Exception e)
                {
                    onError?.Invoke($"JSON Parse Error: {e.Message}\\nResponse: {request.downloadHandler.text}");
                }
            }
            else
            {
                // Provide detailed error information for debugging
                string errorDetail = $"HTTP {request.responseCode}";
                if (!string.IsNullOrEmpty(request.error))
                {
                    errorDetail += $" - {request.error}";
                }
                
                string responseBody = request.downloadHandler?.text ?? "";
                if (!string.IsNullOrEmpty(responseBody))
                {
                    errorDetail += $"\\nResponse: {responseBody}";
                }
                
                // Add hint for common errors
                if (request.responseCode == 401 || request.responseCode == 403)
                {
                    errorDetail += "\\n[Hint] Authentication error - check your API Key in DIaaSConfig";
                }
                else if (request.responseCode == 404)
                {
                    errorDetail += "\\n[Hint] Endpoint not found - check your BaseUrl in DIaaSConfig";
                }
                else if (request.responseCode == 500)
                {
                    errorDetail += "\\n[Hint] Server error - this may be caused by missing/invalid API Key or malformed request";
                }
                
                onError?.Invoke(errorDetail);
            }
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T Items;
        }
    }
}
