using System;
using DIaaS.Core;
using DIaaS.Models;

namespace DIaaS.Services
{
    public class TabularService
    {
        private const string Endpoint = "/sessions";

        public void CreateTabularDataset(string sessionId, string name, ColumnDefinition[] columns, Action<TabularDatasetResponse> onSuccess, Action<string> onError)
        {
            string schemaJson = "{";
            for(int i=0; i<columns.Length; i++)
            {
                schemaJson += $"\"{columns[i].name}\": \"{columns[i].type}\"";
                if(i < columns.Length - 1) schemaJson += ",";
            }
            schemaJson += "}";
            
            string jsonBody = $"{{\"name\": \"{name}\", \"schema_def\": {schemaJson}}}";
            
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<TabularDatasetResponse>($"{Endpoint}/{sessionId}/datasets/tabular", jsonBody, onSuccess, onError));
        }

        public void InsertRecords(string sessionId, string datasetId, string jsonRows, Action<string> onSuccess, Action<string> onError)
        {
             // User provides the full JSON for rows: {"rows": [{"col": "val"}]}
             DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<object>($"{Endpoint}/{sessionId}/datasets/tabular/{datasetId}/records", jsonRows, (response) => onSuccess("Success"), onError));
        }

        public void QueryRecords(string sessionId, string datasetId, int limit, int offset, Action<QueryResponse> onSuccess, Action<string> onError)
        {
            string url = $"{Endpoint}/{sessionId}/datasets/tabular/{datasetId}/records?limit={limit}&offset={offset}";
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequest<QueryResponse>(url, onSuccess, onError));
        }
    }
}
