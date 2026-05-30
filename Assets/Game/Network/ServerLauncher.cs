using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class ServerLauncher : MonoBehaviour
{
    [Header("Settings")]
    public ushort port = 7777;

    public string gameWorldScene = "GameWorld";

    void Awake()
    {
        if (!IsServer()) return;

        Debug.Log("[Server] Starting dedicated server...");

        UnityTransport transport =
            NetworkManager.Singleton
                .GetComponent<UnityTransport>();

        transport.SetConnectionData("0.0.0.0", port);

        // StartHost avoids the NGO ILPP bug
        // on dedicated server builds.
        // NetworkManager is configured to not
        // spawn a player for the host connection.
        NetworkManager.Singleton.StartHost();

        Debug.Log("[Server] Listening on port " + port);

        NetworkManager.Singleton
            .SceneManager
            .LoadScene(
                gameWorldScene,
                LoadSceneMode.Single);
    }

    // =====================================
    // IS SERVER
    // Returns true if:
    // 1. Dedicated server build
    // 2. -server command line arg passed
    // 3. server.txt file exists next to exe
    // =====================================

    bool IsServer()
    {
#if UNITY_SERVER
        return true;
#else
        // Command line arg
        foreach (string arg in
            System.Environment.GetCommandLineArgs())
        {
            if (arg.ToLower() == "-server")
                return true;
        }

        // Config file — place server.txt next
        // to the exe to run as server
        string serverFile =
            Path.Combine(
                Application.dataPath,
                "..", "server.txt");

        if (File.Exists(serverFile))
        {
            Debug.Log(
                "[Server] server.txt found " +
                "— starting as server.");

            return true;
        }

        return false;
#endif
    }
}
