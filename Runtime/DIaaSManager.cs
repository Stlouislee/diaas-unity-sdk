using UnityEngine;
using DIaaS.Core;
using DIaaS.Services;

namespace DIaaS
{
    public class DIaaSManager : MonoBehaviour
    {
        public static DIaaSManager Instance { get; private set; }

        [SerializeField] private DIaaSConfig config;

        public SessionService Sessions { get; private set; }
        public TabularService Tabular { get; private set; }
        public GraphService Graph { get; private set; }
        public QueryService Query { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            // Ensure Client is ready
            if (DIaaSClient.Instance == null)
            {
                GameObject clientGO = new GameObject("DIaaSClient");
                clientGO.AddComponent<DIaaSClient>();
            }
            
            DIaaSClient.Instance.Initialize(config);

            Sessions = new SessionService();
            Tabular = new TabularService();
            Graph = new GraphService();
            Query = new QueryService();
        }
    }
}
