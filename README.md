# DIaaS Unity SDK

A Unity SDK for interacting with [DIaaS (Data Infrastructure as a Service)](https://github.com/Stlouislee/DIaaS) - a comprehensive data infrastructure service that allows you to manage **Tabular** (PostgreSQL) and **Graph** (Neo4j) datasets within isolated sessions.

[![Unity](https://img.shields.io/badge/Unity-2020.3%2B-black.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

## Features

- **Session Management** - Create isolated workspaces for your data
- **Tabular Data** - Dynamic table creation, schema management, and SQL-like querying
- **Graph Data** - Node/Edge management and graph algorithms (e.g., Shortest Path)
- **Export** - Download sessions as ZIP/CSV/JSON
- **Configurable** - JSON-based configuration with multiple loading options
- **Async Operations** - Coroutine-based API for non-blocking requests
- **Retry Logic** - Built-in retry mechanism for failed requests
- **Logging** - Configurable logging levels for debugging

## Requirements

- Unity 2020.3 LTS or later
- A running DIaaS instance ([Setup Instructions](https://github.com/Stlouislee/DIaaS))

## Installation

### Option 1: Unity Package Manager (Git URL)

1. Open Unity and go to **Window > Package Manager**
2. Click the **+** button and select **Add package from git URL**
3. Enter: `https://github.com/Stlouislee/diaas-unity-sdk.git`
4. Click **Add**

### Option 2: Manual Installation

1. Download or clone this repository
2. Copy the contents into your Unity project's `Assets/DIaaS/` folder

## Quick Start

### 1. Create a Configuration File

Create `Assets/Resources/diaas_config.json`:

```json
{
    "serverUrl": "http://localhost:8000",
    "apiKey": "",
    "autoRegister": true,
    "timeout": 30,
    "retryAttempts": 3,
    "retryDelaySeconds": 1.0,
    "logging": {
        "enabled": true,
        "logLevel": "Info"
    },
    "session": {
        "autoCreate": true,
        "defaultName": "Unity Game Session",
        "defaultDescription": "Auto-created from Unity"
    }
}
```

### 2. Initialize the Client

```csharp
using UnityEngine;
using DIaaS;

public class GameManager : MonoBehaviour
{
    private DIaaSClient _client;

    void Start()
    {
        // Load config from Resources folder
        _client = new DIaaSClient(ConfigSource.Resources, "diaas_config");
        
        // Initialize (auto-registers and creates session based on config)
        StartCoroutine(_client.Initialize(
            onSuccess: () => Debug.Log("DIaaS Ready!"),
            onError: (err) => Debug.LogError(err)
        ));
    }
}
```

### 3. Store Player Data

```csharp
using System.Collections.Generic;
using UnityEngine;
using DIaaS;

public class PlayerDataManager : MonoBehaviour
{
    private DIaaSClient _client;
    private string _scoresDatasetId;

    public void SetupPlayerScores()
    {
        var schema = new Dictionary<string, string>
        {
            { "player_name", "VARCHAR" },
            { "score", "INTEGER" },
            { "level", "INTEGER" }
        };

        StartCoroutine(_client.CreateTabularDataset(
            _client.CurrentSessionId,
            "player_scores",
            schema,
            onSuccess: (dataset) => _scoresDatasetId = dataset.id,
            onError: Debug.LogError
        ));
    }

    public void SaveScore(string playerName, int score, int level)
    {
        var rows = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                { "player_name", playerName },
                { "score", score },
                { "level", level }
            }
        };

        StartCoroutine(_client.InsertRecords(
            _client.CurrentSessionId,
            _scoresDatasetId,
            rows,
            onSuccess: (r) => Debug.Log("Score saved!"),
            onError: Debug.LogError
        ));
    }
}
```

### 4. Create a Social Graph

```csharp
public void CreatePlayerNetwork()
{
    // Create graph dataset
    StartCoroutine(_client.CreateGraphDataset(
        _client.CurrentSessionId,
        "player_network",
        onSuccess: (dataset) => 
        {
            // Add a player node
            var props = new Dictionary<string, object>
            {
                { "name", "Alice" },
                { "guild", "Warriors" }
            };
            
            StartCoroutine(_client.CreateNode(
                _client.CurrentSessionId,
                dataset.id,
                "Player",
                props,
                onSuccess: (node) => Debug.Log($"Created node: {node.id}"),
                onError: Debug.LogError
            ));
        },
        onError: Debug.LogError
    ));
}
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `serverUrl` | string | `http://localhost:8000` | DIaaS server URL |
| `apiKey` | string | `""` | Pre-configured API key (leave empty to auto-register) |
| `autoRegister` | bool | `true` | Automatically register if no API key provided |
| `timeout` | int | `30` | Request timeout in seconds |
| `retryAttempts` | int | `3` | Number of retry attempts for failed requests |
| `retryDelaySeconds` | float | `1.0` | Delay between retry attempts |
| `logging.enabled` | bool | `true` | Enable/disable logging |
| `logging.logLevel` | string | `Info` | Log level: Debug, Info, Warning, Error |
| `session.autoCreate` | bool | `false` | Auto-create session on initialization |
| `session.defaultName` | string | `Unity Session` | Default session name |
| `session.defaultDescription` | string | `Auto-created session from Unity` | Default session description |

## Config Loading Options

```csharp
// From Resources folder (compiled into build)
var client = new DIaaSClient(ConfigSource.Resources, "diaas_config");

// From StreamingAssets (copied as-is, readable at runtime)
var client = new DIaaSClient(ConfigSource.StreamingAssets, "diaas_config.json");

// From PersistentData (runtime writable, user-modifiable)
var client = new DIaaSClient(ConfigSource.PersistentData, "diaas_config.json");

// Programmatic config
var config = new DIaaSConfig { serverUrl = "http://myserver:8000" };
var client = new DIaaSClient(config);
```

## API Reference

### Authentication
| Method | Description |
|--------|-------------|
| `Register()` | Register a new user and get an API key |

### Sessions
| Method | Description |
|--------|-------------|
| `CreateSession()` | Create a new session |
| `GetSessions()` | List all sessions |
| `GetSession()` | Get session details |
| `DeleteSession()` | Delete a session |
| `ExportSession()` | Export session data as ZIP |

### Tabular Data
| Method | Description |
|--------|-------------|
| `CreateTabularDataset()` | Create a new table with schema |
| `GetTabularDatasets()` | List all tabular datasets |
| `InsertRecords()` | Insert rows into a table |
| `GetRecords()` | Query records with optional filters |
| `UpdateRecord()` | Update a record |
| `DeleteRecord()` | Delete a record |

### Graph Data
| Method | Description |
|--------|-------------|
| `CreateGraphDataset()` | Create a new graph dataset |
| `CreateNode()` | Add a node to the graph |
| `GetNodes()` | Get nodes (optionally filtered by label) |
| `UpdateNode()` | Update node properties |
| `DeleteNode()` | Delete a node |
| `CreateEdge()` | Create a relationship between nodes |
| `GetEdges()` | Get all edges |
| `DeleteEdge()` | Delete an edge |
| `ShortestPath()` | Find shortest path between nodes |

## Filtering Records

Use URL-style filters when querying tabular data:

```csharp
// Exact match
StartCoroutine(_client.GetRecords(sessionId, datasetId, "player_name=Alice", ...));

// Greater than
StartCoroutine(_client.GetRecords(sessionId, datasetId, "score=gt:1000", ...));

// Less than
StartCoroutine(_client.GetRecords(sessionId, datasetId, "level=lt:5", ...));

// Multiple filters
StartCoroutine(_client.GetRecords(sessionId, datasetId, "score=gt:1000&level=lt:10", ...));
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Related

- [DIaaS Server](https://github.com/Stlouislee/DIaaS) - The backend service this SDK connects to
