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
    public class EdgeCreate
    {
        public int from_node_id;
        public int to_node_id;
        public string type;
        // Properties is Dict[str, Any].
    }
}
