using System;

namespace DIaaS.Models
{
    [Serializable]
    public class QueryRequest
    {
        public string query;
        public string type; // "sql" or "cypher"
        // params is Dict[str, Any] - must be serialized manually
    }

    [Serializable]
    public class QueryResponse
    {
        public string status;
        public int count;
        public int rowcount; // For non-SELECT SQL queries
        // data is returned as raw JSON string since Unity's JsonUtility can't handle dynamic dictionaries
        // Use dataRaw and parse manually with your preferred JSON library
        public string dataRaw;
    }

    /// <summary>
    /// Response wrapper for tabular record queries
    /// </summary>
    [Serializable]
    public class TabularQueryResponse
    {
        public int count;
        // data contains the rows - stored as raw JSON for manual parsing
        public string dataRaw;
    }

    /// <summary>
    /// Response wrapper for insert operations
    /// </summary>
    [Serializable]
    public class InsertResponse
    {
        public string status;
        public int count;
    }
}
