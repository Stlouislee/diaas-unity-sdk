using System;

namespace DIaaS.Models
{
    /// <summary>
    /// Response from user registration
    /// </summary>
    [Serializable]
    public class UserRegistrationResponse
    {
        public string user_id;
        public string api_key;
        public string created_at;
    }
}
