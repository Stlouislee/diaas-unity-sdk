using System;
using System.Collections.Generic;

namespace DIaaS.Models
{
    [Serializable]
    public class TabularDatasetCreate
    {
        public string name;
        // Schema definition is complex (Dict[str, str]). 
        // JsonUtility doesn't support Dictionaries. 
        // We might need a custom serializer or a list of key-value pairs for Unity.
        // For this SDK, we will pass it as a raw JSON string or a list of objects if we can change the API, 
        // but since we are building an SDK for an existing API, we have to match it.
        // The API expects {"col_name": "type"}.
        // We will use a workaround class to serialize to this format manually or use a helper.
        // For simplicity in this prototype, we'll assume the user passes a serializable object 
        // that matches the structure, or we use a custom wrapper.
        
        // Workaround: We will not define schema_def here directly if it's dynamic.
        // Instead, we might need to use a specific class that mimics the structure.
    }

    // Helper for Schema Definition since JsonUtility hates Dictionaries
    [Serializable]
    public class ColumnDefinition
    {
        public string name;
        public string type;
    }

    [Serializable]
    public class TabularDatasetResponse
    {
        public string id;
        public string session_id;
        public string name;
        public string table_name; // The actual PostgreSQL table name
        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class RowInsert
    {
        // The API expects "rows": [{"col": "val"}, ...].
        // Again, dynamic dictionaries are hard with JsonUtility.
        // We will likely need to accept a raw JSON string or use a more robust JSON library.
        // For this SDK, we'll assume the user provides a wrapper that serializes correctly.
    }
}
