using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public static class RtsNetcodeRuntime
{
    public static NetworkManager EnsureNetworkManager()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null)
        {
            GameObject go = new GameObject("NetworkManager");
            Object.DontDestroyOnLoad(go);

            UnityTransport transport = go.AddComponent<UnityTransport>();
            networkManager = go.AddComponent<NetworkManager>();
            networkManager.NetworkConfig = new NetworkConfig
            {
                NetworkTransport = transport
            };
        }
        else if (networkManager.NetworkConfig == null)
        {
            UnityTransport transport = networkManager.GetComponent<UnityTransport>();
            if (transport == null)
            {
                transport = networkManager.gameObject.AddComponent<UnityTransport>();
            }

            networkManager.NetworkConfig = new NetworkConfig
            {
                NetworkTransport = transport
            };
        }
        else if (networkManager.NetworkConfig.NetworkTransport == null)
        {
            UnityTransport transport = networkManager.GetComponent<UnityTransport>();
            if (transport == null)
            {
                transport = networkManager.gameObject.AddComponent<UnityTransport>();
            }

            networkManager.NetworkConfig.NetworkTransport = transport;
        }

        return networkManager;
    }
}
