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

    [Serializable]
    public class SessionResponse
    {
        public string id;
        public string user_id;
        public string name;
        public string description;
        public string created_at;
        public string updated_at;
    }
}
