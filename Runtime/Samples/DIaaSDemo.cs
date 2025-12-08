using System.Collections.Generic;
using UnityEngine;
using DIaaS;
using DIaaS.Models;
using DIaaS.Services;

namespace DIaaS.Samples 
{
    public class DIaaSDemo : MonoBehaviour
    {
        void Start()
        {
            if (DIaaSManager.Instance == null)
            {
                Debug.LogError("DIaaSManager instance not found. Please add DIaaSManager to the scene.");
                return;
            }
        }

        [ContextMenu("0. Test Connection (Health Check)")]
        public void HealthCheck()
        {
            Debug.Log("Testing connection to DIaaS server...");
            DIaaSManager.Instance.Users.HealthCheck(
                (response) => {
                    Debug.Log($"[Success] Server is healthy!");
                    Debug.Log($"  Response: {response}");
                },
                (err) => Debug.LogError($"[Error] Health check failed: {err}")
            );
        }

        [ContextMenu("1. Register New User (Get API Key)")]
        public void RegisterUser()
        {
            Debug.Log("Registering new user to get API key...");
            DIaaSManager.Instance.Users.Register(
                (response) => {
                    Debug.Log($"[Success] User Registered!");
                    Debug.Log($"  User ID: {response.user_id}");
                    Debug.Log($"  API Key: {response.api_key}");
                    Debug.Log($"  IMPORTANT: Copy this API Key to your DIaaSConfig asset!");
                },
                (err) => Debug.LogError($"[Error] Registration failed: {err}")
            );
        }

        [ContextMenu("2. List All Sessions and Data (Read Only)")]
        public void ListAllData()
        {
            Debug.Log("=== Fetching All Sessions and Data ===");
            Debug.Log("Note: Make sure you have set a valid API Key in DIaaSConfig!");
            
            DIaaSManager.Instance.Sessions.ListSessionsRaw(
                (json) => {
                    Debug.Log("[Success] Sessions Retrieved:");
                    Debug.Log(json);
                    
                    // Parse sessions and fetch details for each
                    ParseAndFetchSessionDetails(json);
                },
                (err) => Debug.LogError($"[Error] List Sessions: {err}")
            );
        }

        void ParseAndFetchSessionDetails(string sessionsJson)
        {
            // Simple JSON array parsing (Unity's JsonUtility needs wrapper)
            // The response is an array like: [{...}, {...}]
            
            // Try to parse using a wrapper approach
            string wrappedJson = "{\"sessions\":" + sessionsJson + "}";
            SessionsWrapper wrapper = JsonUtility.FromJson<SessionsWrapper>(wrappedJson);
            
            if (wrapper == null || wrapper.sessions == null || wrapper.sessions.Length == 0)
            {
                Debug.Log("No sessions found for this user.");
                return;
            }

            Debug.Log($"\n=== Found {wrapper.sessions.Length} Session(s) ===\n");

            foreach (var session in wrapper.sessions)
            {
                Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Debug.Log($"SESSION: {session.name}");
                Debug.Log($"  ID: {session.id}");
                Debug.Log($"  Description: {session.description}");
                Debug.Log($"  Created: {session.created_at}");
                
                // Fetch full session details to get datasets
                FetchSessionDetails(session.id, session.name);
            }
        }

        void FetchSessionDetails(string sessionId, string sessionName)
        {
            DIaaSManager.Instance.Sessions.GetSessionRaw(sessionId,
                (json) => {
                    Debug.Log($"\n[Session: {sessionName}] Full Details:");
                    Debug.Log(json);
                    
                    // Parse to get dataset info
                    SessionDetailWrapper detail = JsonUtility.FromJson<SessionDetailWrapper>("{\"session\":" + json + "}");
                    
                    if (detail?.session != null)
                    {
                        // Check for tabular datasets
                        if (detail.session.tabular_datasets != null && detail.session.tabular_datasets.Length > 0)
                        {
                            Debug.Log($"\n  📊 Tabular Datasets ({detail.session.tabular_datasets.Length}):");
                            foreach (var ds in detail.session.tabular_datasets)
                            {
                                Debug.Log($"    - {ds.name} (ID: {ds.id})");
                                FetchTabularData(sessionId, ds.id, ds.name);
                            }
                        }
                        else
                        {
                            Debug.Log($"  📊 No Tabular Datasets");
                        }

                        // Check for graph datasets
                        if (detail.session.graph_datasets != null && detail.session.graph_datasets.Length > 0)
                        {
                            Debug.Log($"\n  🔗 Graph Datasets ({detail.session.graph_datasets.Length}):");
                            foreach (var ds in detail.session.graph_datasets)
                            {
                                Debug.Log($"    - {ds.name} (ID: {ds.id})");
                                FetchGraphData(sessionId, ds.id, ds.name);
                            }
                        }
                        else
                        {
                            Debug.Log($"  🔗 No Graph Datasets");
                        }
                    }
                },
                (err) => Debug.LogError($"[Error] Get Session {sessionId}: {err}")
            );
        }

        void FetchTabularData(string sessionId, string datasetId, string datasetName)
        {
            DIaaSManager.Instance.Tabular.QueryRecordsRaw(sessionId, datasetId, 100, 0,
                (json) => {
                    Debug.Log($"\n      📄 Tabular Data for '{datasetName}':");
                    Debug.Log($"      {json}");
                },
                (err) => Debug.LogError($"      [Error] Query Tabular {datasetName}: {err}")
            );
        }

        void FetchGraphData(string sessionId, string datasetId, string datasetName)
        {
            DIaaSManager.Instance.Graph.ListNodes(sessionId, datasetId, null, 100,
                (json) => {
                    Debug.Log($"\n      🔵 Graph Nodes for '{datasetName}':");
                    Debug.Log($"      {json}");
                },
                (err) => Debug.LogError($"      [Error] Query Graph {datasetName}: {err}")
            );
        }

        // Helper classes for JSON parsing
        [System.Serializable]
        private class SessionsWrapper
        {
            public SessionResponse[] sessions;
        }

        [System.Serializable]
        private class SessionDetailWrapper
        {
            public SessionResponseFull session;
        }
    }
}
