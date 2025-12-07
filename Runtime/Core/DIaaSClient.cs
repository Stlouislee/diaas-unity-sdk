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
        }

        public IEnumerator GetRequest<T>(string endpoint, Action<T> onSuccess, Action<string> onError)
        {
            string url = $"{config.BaseUrl}{endpoint}";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                AttachHeaders(webRequest);
                yield return webRequest.SendWebRequest();

                HandleResponse(webRequest, onSuccess, onError);
            }
        }

        public IEnumerator PostRequest<T>(string endpoint, object body, Action<T> onSuccess, Action<string> onError)
        {
            string url = $"{config.BaseUrl}{endpoint}";
            string jsonBody = JsonUtility.ToJson(body);
            yield return PostRequestRaw(url, jsonBody, onSuccess, onError);
        }

        public IEnumerator PostRequestRaw<T>(string url, string jsonBody, Action<T> onSuccess, Action<string> onError)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                AttachHeaders(webRequest);

                yield return webRequest.SendWebRequest();

                HandleResponse(webRequest, onSuccess, onError);
            }
        }

        public IEnumerator PostRequest<T>(string endpoint, string jsonBody, Action<T> onSuccess, Action<string> onError)
        {
            string url = $"{config.BaseUrl}{endpoint}";
            yield return PostRequestRaw(url, jsonBody, onSuccess, onError);
        }

        public IEnumerator DeleteRequest(string endpoint, Action onSuccess, Action<string> onError)
        {
            string url = $"{config.BaseUrl}{endpoint}";
            using (UnityWebRequest webRequest = UnityWebRequest.Delete(url))
            {
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
                    onError?.Invoke("JSON Parse Error: " + e.Message);
                }
            }
            else
            {
                onError?.Invoke(request.error + ": " + request.downloadHandler.text);
            }
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T Items;
        }
    }
}
