# Installation Guide

This guide covers all methods of installing the DIaaS Unity SDK into your Unity project.

## Prerequisites

- **Unity 2020.3 LTS** or later
- **A running DIaaS instance** - See [DIaaS Setup](https://github.com/Stlouislee/DIaaS)

## Installation Methods

### Method 1: Unity Package Manager (Recommended)

The easiest way to install the SDK is through Unity's Package Manager using a Git URL.

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click the **+** button in the top-left corner
4. Select **Add package from git URL...**
5. Enter the following URL:
   ```
   https://github.com/Stlouislee/diaas-unity-sdk.git
   ```
6. Click **Add**

Unity will download and import the package automatically.

#### Updating the Package

To update to the latest version:
1. Open **Window > Package Manager**
2. Find **DIaaS Unity SDK** in the list
3. Click the **Update** button (if available)

Or remove and re-add the package using the Git URL.

### Method 2: Manual Installation

If you prefer to have the source code directly in your project:

1. **Download the SDK**
   - Go to [Releases](https://github.com/Stlouislee/diaas-unity-sdk/releases)
   - Download the latest `.unitypackage` or source ZIP

2. **Import into Unity**
   - If using `.unitypackage`: Double-click to import
   - If using ZIP: Extract to `Assets/DIaaS/` in your project

### Method 3: Git Submodule

For projects using Git, you can add the SDK as a submodule:

```bash
cd YourUnityProject
git submodule add https://github.com/Stlouislee/diaas-unity-sdk.git Assets/DIaaS
```

## Project Setup

### Step 1: Create Configuration File

Create a configuration file to store your DIaaS settings.

**Option A: Resources Folder (Recommended for most projects)**

1. Create folder: `Assets/Resources/` (if it doesn't exist)
2. Create file: `Assets/Resources/diaas_config.json`

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

**Option B: StreamingAssets (For runtime-readable config)**

1. Create folder: `Assets/StreamingAssets/`
2. Create file: `Assets/StreamingAssets/diaas_config.json`

This option allows the config file to be read (but not modified) at runtime on all platforms.

**Option C: No Config File (Programmatic)**

You can also configure the SDK entirely in code:

```csharp
var config = new DIaaSConfig
{
    serverUrl = "http://your-server:8000",
    apiKey = "your-api-key",
    timeout = 30
};
var client = new DIaaSClient(config);
```

### Step 2: Create a Manager Script

Create a script to manage the DIaaS connection:

```csharp
using UnityEngine;
using DIaaS;

public class DIaaSManager : MonoBehaviour
{
    public static DIaaSManager Instance { get; private set; }
    public DIaaSClient Client { get; private set; }
    public bool IsReady { get; private set; }

    [SerializeField] private ConfigSource configSource = ConfigSource.Resources;
    [SerializeField] private string configFileName = "diaas_config";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeClient();
    }

    private void InitializeClient()
    {
        Client = new DIaaSClient(configSource, configFileName);
        StartCoroutine(Client.Initialize(
            onSuccess: () =>
            {
                IsReady = true;
                Debug.Log("DIaaS is ready!");
            },
            onError: (error) =>
            {
                Debug.LogError($"DIaaS initialization failed: {error}");
            }
        ));
    }
}
```

### Step 3: Add to Scene

1. Create an empty GameObject in your first scene
2. Name it `DIaaSManager`
3. Attach the `DIaaSManager` script
4. Configure the settings in the Inspector

## Platform-Specific Notes

### Windows / Mac / Linux

No special configuration required. The SDK works out of the box.

### WebGL

- Ensure your DIaaS server has **CORS** enabled
- Use `https://` URLs if your game is hosted on HTTPS

### Mobile (iOS / Android)

- For local development, use your computer's IP address instead of `localhost`
- Example: `http://192.168.1.100:8000`
- Ensure your device is on the same network as the DIaaS server

### Console Platforms

- Contact your platform representative for network requirements
- May require additional certificates for HTTPS

## Verifying Installation

To verify the SDK is working correctly:

1. Create a test script:

```csharp
using UnityEngine;
using DIaaS;

public class DIaaSTest : MonoBehaviour
{
    void Start()
    {
        var client = new DIaaSClient();
        StartCoroutine(client.HealthCheck(
            onSuccess: (response) => Debug.Log($"DIaaS Status: {response.status}"),
            onError: (error) => Debug.LogError($"Connection failed: {error}")
        ));
    }
}
```

2. Attach to a GameObject and run the scene
3. Check the Console for the health check response

## Troubleshooting

### "Config file not found" Warning

- Ensure the config file is in the correct location
- For Resources: `Assets/Resources/diaas_config.json` (no `.json` extension in code)
- For StreamingAssets: `Assets/StreamingAssets/diaas_config.json`

### "Connection refused" Error

- Verify DIaaS server is running
- Check the `serverUrl` in your config
- Ensure firewall isn't blocking the connection

### "401 Unauthorized" Error

- Your API key may be invalid or expired
- Try setting `autoRegister: true` to get a new key
- Or manually register and set the `apiKey` in config

### CORS Errors (WebGL)

- Configure your DIaaS server to allow requests from your game's origin
- Add appropriate CORS headers to the server

## Next Steps

- Check out the [README](README.md) for usage examples
- Explore the [Samples](Samples~/) for complete example scripts
- Review the API reference for all available methods
