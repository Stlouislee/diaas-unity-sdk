using System;
using DIaaS.Core;
using DIaaS.Models;

namespace DIaaS.Services
{
    public class SessionService
    {
        private const string Endpoint = "/sessions";

        public void CreateSession(string name, string description, Action<SessionResponse> onSuccess, Action<string> onError)
        {
            var body = new SessionCreate { name = name, description = description };
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<SessionResponse>(Endpoint + "/", body, onSuccess, onError));
        }

        public void ListSessions(Action<SessionResponse[]> onSuccess, Action<string> onError)
        {
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequest<SessionResponse[]>(Endpoint + "/", onSuccess, onError));
        }

        public void GetSession(string sessionId, Action<SessionResponse> onSuccess, Action<string> onError)
        {
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequest<SessionResponse>($"{Endpoint}/{sessionId}", onSuccess, onError));
        }

        public void DeleteSession(string sessionId, Action onSuccess, Action<string> onError)
        {
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.DeleteRequest($"{Endpoint}/{sessionId}", onSuccess, onError));
        }
    }
}
