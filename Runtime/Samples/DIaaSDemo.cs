using System.Collections.Generic;
using UnityEngine;
using DIaaS;
using DIaaS.Models;
using DIaaS.Services;

namespace DIaaS.Samples 
{
    public class DIaaSDemo : MonoBehaviour
    {
        [SerializeField] private string sessionName = "UnityDemoSession";
        
        private string currentSessionId;
        private string tabularDatasetId;
        private string graphDatasetId;

        private int node1Id;
        private int node2Id;

        void Start()
        {
            if (DIaaSManager.Instance == null)
            {
                Debug.LogError("DIaaSManager instance not found. Please add DIaaSManager to the scene.");
                return;
            }
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

        [ContextMenu("2. Run Full Test (Requires API Key)")]
        public void RunTest()
        {
            Debug.Log("Starting DIaaS Test...");
            Debug.Log("Note: Make sure you have set a valid API Key in DIaaSConfig!");
            CreateSession();
        }

        void CreateSession()
        {
            DIaaSManager.Instance.Sessions.CreateSession(sessionName, "Session created by Unity SDK Demo",
                (session) => {
                    currentSessionId = session.id;
                    Debug.Log($"[Success] Session Created: {session.id}");
                    CreateTabularData();
                },
                (err) => Debug.LogError($"[Error] Create Session: {err}")
            );
        }

        void CreateTabularData()
        {
            var columns = new ColumnDefinition[] 
            {
                new ColumnDefinition { name = "id", type = "int" },
                new ColumnDefinition { name = "username", type = "text" },
                new ColumnDefinition { name = "score", type = "int" }
            };

            DIaaSManager.Instance.Tabular.CreateTabularDataset(currentSessionId, "PlayerScores", columns,
                (dataset) => {
                    tabularDatasetId = dataset.id;
                    Debug.Log($"[Success] Tabular Dataset Created: {dataset.id}");
                    InsertTabularData();
                },
                (err) => Debug.LogError($"[Error] Create Tabular: {err}")
            );
        }

        void InsertTabularData()
        {
            string jsonRows = "{\"rows\": [{\"id\": 1, \"username\": \"PlayerOne\", \"score\": 100}, {\"id\": 2, \"username\": \"PlayerTwo\", \"score\": 250}]}";

            DIaaSManager.Instance.Tabular.InsertRecords(currentSessionId, tabularDatasetId, jsonRows,
                (response) => {
                    Debug.Log($"[Success] Rows Inserted. Count: {response.count}");
                    QueryTabularData();
                },
                (err) => Debug.LogError($"[Error] Insert Rows: {err}")
            );
        }

        void QueryTabularData()
        {
            DIaaSManager.Instance.Tabular.QueryRecords(currentSessionId, tabularDatasetId, 10, 0,
                (response) => {
                    Debug.Log($"[Success] Query Executed. Count: {response.count}");
                    CreateGraphData();
                },
                (err) => Debug.LogError($"[Error] Query: {err}")
            );
        }

        void CreateGraphData()
        {
            DIaaSManager.Instance.Graph.CreateGraphDataset(currentSessionId, "GameLevelGraph",
                (dataset) => {
                    graphDatasetId = dataset.id;
                    Debug.Log($"[Success] Graph Dataset Created: {dataset.id}");
                    CreateNodes();
                },
                (err) => Debug.LogError($"[Error] Create Graph: {err}")
            );
        }

        void CreateNodes()
        {
            string props1 = "{\"name\": \"StartRoom\", \"type\": \"safe\"}";
            string props2 = "{\"name\": \"BossRoom\", \"type\": \"danger\"}";

            DIaaSManager.Instance.Graph.CreateNode(currentSessionId, graphDatasetId, "Room", props1,
                (node) => {
                    node1Id = node.id;
                    Debug.Log($"[Success] Node 1 Created: {node.id}");
                    
                    DIaaSManager.Instance.Graph.CreateNode(currentSessionId, graphDatasetId, "Room", props2,
                        (node2) => {
                            node2Id = node2.id;
                            Debug.Log($"[Success] Node 2 Created: {node2.id}");
                            CreateEdge();
                        },
                        (err) => Debug.LogError($"[Error] Create Node 2: {err}")
                    );
                },
                (err) => Debug.LogError($"[Error] Create Node 1: {err}")
            );
        }

        void CreateEdge()
        {
            string props = "{\"distance\": 10}";
            DIaaSManager.Instance.Graph.CreateEdge(currentSessionId, graphDatasetId, node1Id, node2Id, "CONNECTS_TO", props,
                (edge) => {
                    Debug.Log($"[Success] Edge Created: {edge.id}");
                    Debug.Log("Test Complete!");
                },
                (err) => Debug.LogError($"[Error] Create Edge: {err}")
            );
        }
    }
}
