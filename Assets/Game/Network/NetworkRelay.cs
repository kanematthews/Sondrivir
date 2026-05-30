using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkRelay : MonoBehaviour
{
    public static NetworkRelay instance;

    private bool isReady = false;

    // =====================================
    // SERVER CONFIG
    // Change serverIP to your public IP
    // when running as a client.
    // =====================================

    [Header("Server Config")]
    public string serverIP = "5.80.248.30";

    public ushort serverPort = 7777;

    public string gameWorldScene = "GameWorld";

    // =====================================
    // UI
    // =====================================

    [Header("UI")]
    public GameObject lobbyPanel;

    public TMP_Text statusText;

    public TMP_Text joinCodeDisplay;

    public TMP_InputField joinCodeInput;

    // =====================================
    // AWAKE
    // =====================================

    async void Awake()
    {
        instance = this;

        await InitializeServices();
    }

    // =====================================
    // INITIALIZE UGS (for auth only)
    // =====================================

    async Task InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService
                .Instance.IsSignedIn)
            {
                await AuthenticationService
                    .Instance
                    .SignInAnonymouslyAsync();
            }

            isReady = true;
            SetStatus("Ready.");
        }
        catch (Exception e)
        {
            // Auth failure is non-fatal
            // for direct IP connections
            Debug.LogWarning(
                "Auth warning: " + e.Message);

            isReady = true;
            SetStatus("Ready.");
        }
    }

    // =====================================
    // START SERVER (run on your PC)
    // =====================================

    public void StartDedicatedServer()
    {
        UnityTransport transport =
            NetworkManager.Singleton
                .GetComponent<UnityTransport>();

        // Listen on all interfaces
        transport.SetConnectionData(
            "0.0.0.0",
            serverPort);

        NetworkManager.Singleton.StartServer();

        SetStatus("Server running on port " +
            serverPort);

        Debug.Log("Dedicated server started.");

        // Load game world
        NetworkManager.Singleton
            .SceneManager
            .LoadScene(
                gameWorldScene,
                LoadSceneMode.Single);
    }

    // =====================================
    // HOST (server + local player)
    // Use this when testing locally
    // =====================================

    public void HostGame()
    {
        UnityTransport transport =
            NetworkManager.Singleton
                .GetComponent<UnityTransport>();

        transport.SetConnectionData(
            "0.0.0.0",
            serverPort);

        NetworkManager.Singleton.StartHost();

        SetStatus("Hosting on port " + serverPort);

        if (lobbyPanel != null)
            lobbyPanel.SetActive(false);

        NetworkManager.Singleton
            .SceneManager
            .LoadScene(
                gameWorldScene,
                LoadSceneMode.Single);
    }

    // =====================================
    // JOIN (connect to server IP)
    // =====================================

    public void JoinGame()
    {
        if (!isReady)
        {
            SetStatus("Still initializing, try again...");
            return;
        }

        SetStatus("Connecting to server...");

        try
        {
            UnityTransport transport =
                NetworkManager.Singleton
                    .GetComponent<UnityTransport>();

            transport.SetConnectionData(
                serverIP,
                serverPort);

            NetworkManager.Singleton.StartClient();

            SetStatus("Connecting...");

            if (lobbyPanel != null)
                lobbyPanel.SetActive(false);
        }
        catch (Exception e)
        {
            SetStatus("Failed: " + e.Message);
        }
    }

    // =====================================
    // ENTER WORLD
    // Just connects — server is always running
    // =====================================

    public void EnterWorld()
    {
        JoinGame();
    }

    // =====================================
    // STATUS
    // =====================================

    void SetStatus(string message)
    {
        Debug.Log("[Network] " + message);

        if (statusText != null)
            statusText.text = message;
    }
}
