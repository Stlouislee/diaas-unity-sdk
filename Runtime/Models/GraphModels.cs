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
    }

    [Serializable]
    public class NodeCreate
    {
        public string label;
        // Properties is Dict[str, Any]. 
        // Unity JsonUtility limitation applies.
    }

    [Serializable]
    public class NodeResponse
    {
        public int id;
        // labels in Neo4j are list, JsonUtility might struggle if it's ["A", "B"]
        // properties is a dictionary
    }

    [Serializable]
    public class EdgeCreate
    {
        public int from_node_id;
        public int to_node_id;
        public string type;
        // Properties is Dict[str, Any].
    }

    [Serializable]
    public class EdgeResponse
    {
        public int id;
        public string type;
        public int start_node_id;
        public int end_node_id;
    }
}
