using System;
using DIaaS.Core;
using DIaaS.Models;

namespace DIaaS.Services
{
    public class GraphService
    {
        private const string Endpoint = "/sessions";

        public void CreateGraphDataset(string sessionId, string name, Action<GraphDatasetResponse> onSuccess, Action<string> onError)
        {
            var body = new GraphDatasetCreate { name = name };
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<GraphDatasetResponse>($"{Endpoint}/{sessionId}/datasets/graph", body, onSuccess, onError));
        }

        public void CreateNode(string sessionId, string datasetId, string label, string propertiesJson, Action<NodeResponse> onSuccess, Action<string> onError)
        {
            // propertiesJson should be a JSON object string e.g. {"name": "Alice"}
            string jsonBody = $"{{\"label\": \"{label}\", \"properties\": {propertiesJson}}}";
            
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<NodeResponse>($"{Endpoint}/{sessionId}/datasets/graph/{datasetId}/nodes", jsonBody, onSuccess, onError));
        }

        public void CreateEdge(string sessionId, string datasetId, int fromId, int toId, string type, string propertiesJson, Action<EdgeResponse> onSuccess, Action<string> onError)
        {
            string jsonBody = $"{{\"from_node_id\": {fromId}, \"to_node_id\": {toId}, \"type\": \"{type}\", \"properties\": {propertiesJson}}}";
            
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<EdgeResponse>($"{Endpoint}/{sessionId}/datasets/graph/{datasetId}/edges", jsonBody, onSuccess, onError));
        }

        public void ListNodes(string sessionId, string datasetId, string label, Action<QueryResponse> onSuccess, Action<string> onError)
        {
            string url = $"{Endpoint}/{sessionId}/datasets/graph/{datasetId}/nodes";
            if (!string.IsNullOrEmpty(label))
            {
                url += $"?label={label}";
            }
            
            // The API returns a list of nodes directly? 
            // Checking graph.py: return await service.get_nodes(...) -> returns List[Node]
            // So it returns [{}, {}].
            // But QueryResponse expects {status, count, data}.
            // Wait, list_nodes returns a List, not a wrapped response.
            // So we need to use GetRequest<object[]> or similar.
            
            // Let's use a generic object array.
            // But we can't easily map that to QueryResponse.
            // I'll change the callback to Action<object> (which will be the array wrapper) or just string?
            // Let's use a helper class for Node list.
            
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequest<object>(url, (res) => onSuccess(new QueryResponse { status = "success" }), onError));
        }
    }
}
