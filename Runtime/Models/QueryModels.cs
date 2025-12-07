using System;

namespace DIaaS.Models
{
    [Serializable]
    public class QueryRequest
    {
        public string query;
        public string type; // "sql" or "cypher"
        // params is Dict[str, Any]
    }

    [Serializable]
    public class QueryResponse
    {
        public string status;
        public int count;
        // data is List[Dict] or similar.
    }
}
