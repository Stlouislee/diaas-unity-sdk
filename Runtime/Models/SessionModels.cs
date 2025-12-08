using System;
using System.Collections.Generic;

namespace DIaaS.Models
{
    [Serializable]
    public class SessionCreate
    {
        public string name;
        public string description;
    }

    /// <summary>
    /// Summary of a dataset (used in session listings)
    /// </summary>
    [Serializable]
    public class DatasetSummary
    {
        public string id;
        public string name;
    }

    [Serializable]
    public class SessionResponse
    {
        public string id;
        public string user_id;
        public string name;
        public string description;
        public string created_at;
        public string updated_at;
        // Note: tabular_datasets and graph_datasets are returned but Unity's JsonUtility
        // doesn't handle arrays in nested objects well. Use GetSessionRaw for full data.
    }

    /// <summary>
    /// Extended session response with dataset arrays (requires manual JSON parsing)
    /// </summary>
    [Serializable]
    public class SessionResponseFull
    {
        public string id;
        public string user_id;
        public string name;
        public string description;
        public string created_at;
        public string updated_at;
        public DatasetSummary[] tabular_datasets;
        public DatasetSummary[] graph_datasets;
    }
}
