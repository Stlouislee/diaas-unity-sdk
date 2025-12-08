using System;
using DIaaS.Core;
using DIaaS.Models;
using UnityEngine;

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
            Debug.Log("[DIaaS] Registering new user...");
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.PostRequest<UserRegistrationResponse>($"{Endpoint}/register", "{}", onSuccess, onError));
        }

        /// <summary>
        /// Test the API connection by checking the health endpoint.
        /// This does NOT require authentication.
        /// </summary>
        /// <param name="onSuccess">Callback with raw JSON response</param>
        /// <param name="onError">Error callback</param>
        public void HealthCheck(Action<string> onSuccess, Action<string> onError)
        {
            // Health endpoint is at the root, not under /api/v1
            string healthUrl = DIaaSClient.Instance.GetBaseUrlRoot() + "/health";
            Debug.Log($"[DIaaS] Health check: {healthUrl}");
            DIaaSClient.Instance.StartCoroutine(DIaaSClient.Instance.GetRequestRawUrl(healthUrl, onSuccess, onError));
        }
    }
}
