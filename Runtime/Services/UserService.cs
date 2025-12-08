using System;
using DIaaS.Core;
using DIaaS.Models;

namespace DIaaS.Services
{
    /// <summary>
    /// Service for user registration and API key management.
    /// Note: You must register to get an API key before using other endpoints.
    /// </summary>
    public class UserService
    {
        private const string Endpoint = "/users";

        /// <summary>
        /// Register a new user and get an API key.
        /// This endpoint does NOT require authentication.
        /// Save the returned api_key and configure it in DIaaSConfig.
        /// </summary>
        /// <param name="onSuccess">Callback with registration response containing user_id and api_key</param>
        /// <param name="onError">Error callback</param>
        public void Register(Action<UserRegistrationResponse> onSuccess, Action<string> onError)
        {
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<UserRegistrationResponse>($"{Endpoint}/register", "{}", onSuccess, onError));
        }
    }
}
