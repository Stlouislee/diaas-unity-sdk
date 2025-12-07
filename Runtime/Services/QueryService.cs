using System;
using DIaaS.Core;
using DIaaS.Models;

namespace DIaaS.Services
{
    public class QueryService
    {
        private const string Endpoint = "/sessions";

        public void ExecuteQuery(string sessionId, string query, string type, string paramsJson, Action<QueryResponse> onSuccess, Action<string> onError)
        {
            // paramsJson: {"p1": "v1"}
            string jsonBody = $"{{\"query\": \"{query}\", \"type\": \"{type}\", \"params\": {paramsJson}}}";
            
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<QueryResponse>($"{Endpoint}/{sessionId}/query", jsonBody, onSuccess, onError));
        }
    }
}
