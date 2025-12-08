using System;
using DIaaS.Core;
using DIaaS.Models;

namespace DIaaS.Services
{
    public class QueryService
    {
        private const string Endpoint = "/sessions";

        /// <summary>
        /// Execute a raw SQL or Cypher query within a session.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="query">The SQL or Cypher query string</param>
        /// <param name="type">Query type: "sql" or "cypher"</param>
        /// <param name="paramsJson">JSON object string for query parameters, e.g., {"name": "Alice"}. Pass "{}" if no params.</param>
        /// <param name="onSuccess">Callback with raw JSON response (contains status, data, count/rowcount)</param>
        /// <param name="onError">Error callback</param>
        public void ExecuteQuery(string sessionId, string query, string type, string paramsJson, Action<string> onSuccess, Action<string> onError)
        {
            // Escape the query string for JSON safety
            string escapedQuery = EscapeJsonString(query);
            string jsonBody = $"{{\"query\": \"{escapedQuery}\", \"type\": \"{type}\", \"params\": {paramsJson ?? "{}"}}}";

            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequestRawResponse($"{Endpoint}/{sessionId}/query", jsonBody, onSuccess, onError));
        }

        /// <summary>
        /// Execute a SQL query within a session.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="query">The SQL query string</param>
        /// <param name="paramsJson">JSON object string for query parameters. Pass "{}" if no params.</param>
        /// <param name="onSuccess">Callback with raw JSON response</param>
        /// <param name="onError">Error callback</param>
        public void ExecuteSql(string sessionId, string query, string paramsJson, Action<string> onSuccess, Action<string> onError)
        {
            ExecuteQuery(sessionId, query, "sql", paramsJson, onSuccess, onError);
        }

        /// <summary>
        /// Execute a Cypher query within a session.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="query">The Cypher query string</param>
        /// <param name="paramsJson">JSON object string for query parameters. Pass "{}" if no params.</param>
        /// <param name="onSuccess">Callback with raw JSON response</param>
        /// <param name="onError">Error callback</param>
        public void ExecuteCypher(string sessionId, string query, string paramsJson, Action<string> onSuccess, Action<string> onError)
        {
            ExecuteQuery(sessionId, query, "cypher", paramsJson, onSuccess, onError);
        }

        private string EscapeJsonString(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }
    }
}
