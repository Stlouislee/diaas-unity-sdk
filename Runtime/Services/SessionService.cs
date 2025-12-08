using System;
using DIaaS.Core;
using DIaaS.Models;

namespace DIaaS.Services
{
    public class SessionService
    {
        private const string Endpoint = "/sessions";

        /// <summary>
        /// Create a new session for organizing data.
        /// </summary>
        /// <param name="name">Session name</param>
        /// <param name="description">Session description</param>
        /// <param name="onSuccess">Callback with the created session response</param>
        /// <param name="onError">Error callback</param>
        public void CreateSession(string name, string description, Action<SessionResponse> onSuccess, Action<string> onError)
        {
            string jsonBody = $"{{\"name\": \"{EscapeJsonString(name)}\", \"description\": \"{EscapeJsonString(description ?? "")}\"}}";
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<SessionResponse>(Endpoint + "/", jsonBody, onSuccess, onError));
        }

        /// <summary>
        /// List all sessions for the current user.
        /// </summary>
        /// <param name="onSuccess">Callback with array of session responses</param>
        /// <param name="onError">Error callback</param>
        public void ListSessions(Action<SessionResponse[]> onSuccess, Action<string> onError)
        {
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequest<SessionResponse[]>(Endpoint + "/", onSuccess, onError));
        }

        /// <summary>
        /// Get details of a specific session.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="onSuccess">Callback with session response</param>
        /// <param name="onError">Error callback</param>
        public void GetSession(string sessionId, Action<SessionResponse> onSuccess, Action<string> onError)
        {
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequest<SessionResponse>($"{Endpoint}/{sessionId}", onSuccess, onError));
        }

        /// <summary>
        /// Delete a session and all its associated data.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="onSuccess">Callback on successful deletion</param>
        /// <param name="onError">Error callback</param>
        public void DeleteSession(string sessionId, Action onSuccess, Action<string> onError)
        {
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.DeleteRequest($"{Endpoint}/{sessionId}", onSuccess, onError));
        }

        private string EscapeJsonString(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }
    }
}
