using UnityEngine;

namespace DIaaS.Core
{
    [CreateAssetMenu(fileName = "DIaaSConfig", menuName = "DIaaS/Configuration")]
    public class DIaaSConfig : ScriptableObject
    {
        [Tooltip("The base URL of the DIaaS API (e.g., http://localhost:8000/api/v1)")]
        public string BaseUrl = "http://localhost:8000/api/v1";

        [Tooltip("The API Key for authentication")]
        public string ApiKey = "";

        [Tooltip("Timeout for requests in seconds")]
        public int Timeout = 30;
    }
}
