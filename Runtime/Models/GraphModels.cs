using System;
using System.Collections.Generic;

namespace DIaaS.Models
{
    [Serializable]
    public class GraphDatasetCreate
    {
        public string name;
    }

    [Serializable]
    public class GraphDatasetResponse
    {
        public string id;
        public string session_id;
        public string name;
        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class NodeCreate
    {
        public string label;
        // Properties should be passed as raw JSON string to the service methods
    }

    [Serializable]
    public class NodeResponse
    {
        public int id;
        public string label;
        // Properties are returned in raw JSON format
        // Use a JSON parser to access properties
    }

    [Serializable]
    public class EdgeCreate
    {
        public int from_node_id;
        public int to_node_id;
        public string type;
        // Properties should be passed as raw JSON string to the service methods
    }

    [Serializable]
    public class EdgeResponse
    {
        public int id;
        public string type;
        public int start_node_id;
        public int end_node_id;
        // Properties are returned in raw JSON format
    }
}
