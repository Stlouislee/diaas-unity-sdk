using System;
using DIaaS.Core;
using DIaaS.Models;

namespace DIaaS.Services
{
    public class GraphService
    {
        private const string Endpoint = "/sessions";

        /// <summary>
        /// Create a new graph dataset.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="name">Name of the graph dataset</param>
        /// <param name="onSuccess">Callback with the created dataset response</param>
        /// <param name="onError">Error callback</param>
        public void CreateGraphDataset(string sessionId, string name, Action<GraphDatasetResponse> onSuccess, Action<string> onError)
        {
            string jsonBody = $"{{\"name\": \"{EscapeJsonString(name)}\"}}";
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<GraphDatasetResponse>($"{Endpoint}/{sessionId}/datasets/graph", jsonBody, onSuccess, onError));
        }

        /// <summary>
        /// Create a node in the graph dataset.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="datasetId">The dataset ID</param>
        /// <param name="label">Node label (e.g., "Person", "Product")</param>
        /// <param name="propertiesJson">JSON object string for node properties, e.g., {"name": "Alice", "age": 30}</param>
        /// <param name="onSuccess">Callback with the created node response</param>
        /// <param name="onError">Error callback</param>
        public void CreateNode(string sessionId, string datasetId, string label, string propertiesJson, Action<NodeResponse> onSuccess, Action<string> onError)
        {
            string jsonBody = $"{{\"label\": \"{EscapeJsonString(label)}\", \"properties\": {propertiesJson ?? "{}"}}}";
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<NodeResponse>($"{Endpoint}/{sessionId}/datasets/graph/{datasetId}/nodes", jsonBody, onSuccess, onError));
        }

        /// <summary>
        /// Create an edge (relationship) between two nodes.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="datasetId">The dataset ID</param>
        /// <param name="fromId">Source node ID</param>
        /// <param name="toId">Target node ID</param>
        /// <param name="type">Relationship type (e.g., "KNOWS", "WORKS_FOR")</param>
        /// <param name="propertiesJson">JSON object string for edge properties</param>
        /// <param name="onSuccess">Callback with the created edge response</param>
        /// <param name="onError">Error callback</param>
        public void CreateEdge(string sessionId, string datasetId, int fromId, int toId, string type, string propertiesJson, Action<EdgeResponse> onSuccess, Action<string> onError)
        {
            string jsonBody = $"{{\"from_node_id\": {fromId}, \"to_node_id\": {toId}, \"type\": \"{EscapeJsonString(type)}\", \"properties\": {propertiesJson ?? "{}"}}}";
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<EdgeResponse>($"{Endpoint}/{sessionId}/datasets/graph/{datasetId}/edges", jsonBody, onSuccess, onError));
        }

        /// <summary>
        /// List nodes in a graph dataset. Returns raw JSON since node structure is dynamic.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="datasetId">The dataset ID</param>
        /// <param name="label">Optional label filter</param>
        /// <param name="limit">Maximum number of nodes to return</param>
        /// <param name="onSuccess">Callback with raw JSON array of nodes</param>
        /// <param name="onError">Error callback</param>
        public void ListNodes(string sessionId, string datasetId, string label, int limit, Action<string> onSuccess, Action<string> onError)
        {
            string url = $"{Endpoint}/{sessionId}/datasets/graph/{datasetId}/nodes?limit={limit}";
            if (!string.IsNullOrEmpty(label))
            {
                url += $"&label={UnityEngine.Networking.UnityWebRequest.EscapeURL(label)}";
            }
            // API returns a list of nodes directly, so we get raw JSON
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequestRaw(url, onSuccess, onError));
        }

        /// <summary>
        /// Get neighbors of a specific node.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="datasetId">The dataset ID</param>
        /// <param name="nodeId">The node ID to get neighbors for</param>
        /// <param name="onSuccess">Callback with raw JSON of neighbor nodes</param>
        /// <param name="onError">Error callback</param>
        public void GetNeighbors(string sessionId, string datasetId, int nodeId, Action<string> onSuccess, Action<string> onError)
        {
            string url = $"{Endpoint}/{sessionId}/datasets/graph/{datasetId}/nodes/{nodeId}/neighbors";
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequestRaw(url, onSuccess, onError));
        }

        /// <summary>
        /// Find the shortest path between two nodes.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="datasetId">The dataset ID</param>
        /// <param name="fromId">Source node ID</param>
        /// <param name="toId">Target node ID</param>
        /// <param name="onSuccess">Callback with raw JSON of the path</param>
        /// <param name="onError">Error callback</param>
        public void ShortestPath(string sessionId, string datasetId, int fromId, int toId, Action<string> onSuccess, Action<string> onError)
        {
            string url = $"{Endpoint}/{sessionId}/datasets/graph/{datasetId}/algorithms/shortest_path?from_id={fromId}&to_id={toId}";
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequestRaw(url, onSuccess, onError));
        }

        private string EscapeJsonString(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }
    }
}
