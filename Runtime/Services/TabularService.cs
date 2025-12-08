using System;
using DIaaS.Core;
using DIaaS.Models;

namespace DIaaS.Services
{
    public class TabularService
    {
        private const string Endpoint = "/sessions";

        /// <summary>
        /// Create a new tabular dataset with the specified schema.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="name">Name of the dataset</param>
        /// <param name="columns">Array of column definitions (name and type)</param>
        /// <param name="onSuccess">Callback with the created dataset response</param>
        /// <param name="onError">Error callback</param>
        public void CreateTabularDataset(string sessionId, string name, ColumnDefinition[] columns, Action<TabularDatasetResponse> onSuccess, Action<string> onError)
        {
            string schemaJson = "{";
            for (int i = 0; i < columns.Length; i++)
            {
                // Escape column name and type to handle special characters
                schemaJson += $"\"{EscapeJsonString(columns[i].name)}\": \"{EscapeJsonString(columns[i].type)}\"";
                if (i < columns.Length - 1) schemaJson += ",";
            }
            schemaJson += "}";

            string jsonBody = $"{{\"name\": \"{EscapeJsonString(name)}\", \"schema_def\": {schemaJson}}}";

            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<TabularDatasetResponse>($"{Endpoint}/{sessionId}/datasets/tabular", jsonBody, onSuccess, onError));
        }

        /// <summary>
        /// Insert records into a tabular dataset.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="datasetId">The dataset ID</param>
        /// <param name="jsonRows">JSON string in format: {"rows": [{"col": "val"}, ...]}</param>
        /// <param name="onSuccess">Callback with insert response containing status and count</param>
        /// <param name="onError">Error callback</param>
        public void InsertRecords(string sessionId, string datasetId, string jsonRows, Action<InsertResponse> onSuccess, Action<string> onError)
        {
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<InsertResponse>($"{Endpoint}/{sessionId}/datasets/tabular/{datasetId}/records", jsonRows, onSuccess, onError));
        }

        /// <summary>
        /// Query records from a tabular dataset with pagination.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="datasetId">The dataset ID</param>
        /// <param name="limit">Maximum number of records to return</param>
        /// <param name="offset">Number of records to skip</param>
        /// <param name="onSuccess">Callback with query response containing data and count</param>
        /// <param name="onError">Error callback</param>
        public void QueryRecords(string sessionId, string datasetId, int limit, int offset, Action<TabularQueryResponse> onSuccess, Action<string> onError)
        {
            string url = $"{Endpoint}/{sessionId}/datasets/tabular/{datasetId}/records?limit={limit}&offset={offset}";
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequest<TabularQueryResponse>(url, onSuccess, onError));
        }

        /// <summary>
        /// Query records with optional sorting and column selection.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="datasetId">The dataset ID</param>
        /// <param name="limit">Maximum number of records to return</param>
        /// <param name="offset">Number of records to skip</param>
        /// <param name="sort">Optional sort column (prefix with - for descending)</param>
        /// <param name="selectColumns">Optional comma-separated list of columns to return</param>
        /// <param name="onSuccess">Callback with query response</param>
        /// <param name="onError">Error callback</param>
        public void QueryRecordsAdvanced(string sessionId, string datasetId, int limit, int offset, string sort, string selectColumns, Action<TabularQueryResponse> onSuccess, Action<string> onError)
        {
            string url = $"{Endpoint}/{sessionId}/datasets/tabular/{datasetId}/records?limit={limit}&offset={offset}";
            if (!string.IsNullOrEmpty(sort))
            {
                url += $"&sort={UnityEngine.Networking.UnityWebRequest.EscapeURL(sort)}";
            }
            if (!string.IsNullOrEmpty(selectColumns))
            {
                url += $"&select={UnityEngine.Networking.UnityWebRequest.EscapeURL(selectColumns)}";
            }
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequest<TabularQueryResponse>(url, onSuccess, onError));
        }

        private string EscapeJsonString(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }
    }
}
