using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.AI.Navigation;

public class WorldChunkLoader : MonoBehaviour
{
    [Header("World Folder")]
    [Tooltip("Relative to Assets/")]
    public string chunksFolder = "Game/World/Chunks";

    private Dictionary<string, GameObject> loadedChunks =
        new Dictionary<string, GameObject>();

    private NavMeshSurface navMeshSurface;

    private void Start()
    {
        // GET NAVMESH SURFACE
        navMeshSurface =
            GetComponent<NavMeshSurface>();

        // LOAD WORLD
        LoadAllChunkFiles();

        // BUILD NAVMESH AFTER WORLD LOADS
        BuildNavigation();
    }

    private void BuildNavigation()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();

            Debug.Log("NavMesh Built");
        }
    }

    private void LoadAllChunkFiles()
    {
        string fullPath =
            Path.Combine(
                Application.dataPath,
                chunksFolder
            );

        if (!Directory.Exists(fullPath))
        {
            Debug.LogError(
                $"Chunk folder not found:\n{fullPath}"
            );

            return;
        }

        string[] jsonFiles =
            Directory.GetFiles(fullPath, "*.json");

        Debug.Log(
            $"Found {jsonFiles.Length} chunk files."
        );

        foreach (string file in jsonFiles)
        {
            LoadChunkFile(file);
        }
    }

    private void LoadChunkFile(string filePath)
    {
        string json =
            File.ReadAllText(filePath);

        WorldData worldData =
            JsonUtility.FromJson<WorldData>(json);

        if (worldData == null)
        {
            Debug.LogWarning(
                $"Failed to parse file:\n{filePath}"
            );

            return;
        }

        if (worldData.chunks == null)
        {
            Debug.LogWarning(
                $"No chunks found in:\n{filePath}"
            );

            return;
        }

        Debug.Log(
            $"Loading {worldData.chunks.Count} chunks from {Path.GetFileName(filePath)}"
        );

        foreach (ChunkData chunkData in worldData.chunks)
        {
            SpawnChunk(chunkData);
        }
    }

    private void SpawnChunk(ChunkData chunkData)
    {
        string chunkKey =
            $"{chunkData.chunkX}_{chunkData.chunkZ}";

        if (loadedChunks.ContainsKey(chunkKey))
        {
            Debug.LogWarning(
                $"Chunk already loaded: {chunkKey}"
            );

            return;
        }

        GameObject chunkRoot =
            new GameObject($"Chunk_{chunkKey}");

        foreach (PlacedObjectData obj in chunkData.objects)
        {
            SpawnObject(
                obj,
                chunkRoot.transform
            );
        }

        loadedChunks.Add(chunkKey, chunkRoot);

        Debug.Log($"Spawned chunk: {chunkKey}");
    }

    private void SpawnObject(
        PlacedObjectData obj,
        Transform parent)
    {
        PlaceableAsset asset =
            PlaceableAssetDatabase.Instance.GetAsset(obj.assetID);

        if (asset == null)
        {
            Debug.LogWarning(
                $"Missing asset ID: {obj.assetID}"
            );

            return;
        }

        if (asset.prefab == null)
        {
            Debug.LogWarning(
                $"Asset has no prefab: {obj.assetID}"
            );

            return;
        }

        Vector3 worldPosition =
            new Vector3(
                obj.x,
                obj.y,
                obj.z
            );

        Quaternion objectRotation =
            Quaternion.Euler(
                0f,
                obj.rotationY,
                0f
            );

        Quaternion placementRotation =
            Quaternion.identity;

        // ROTATE QUADS FLAT
        if (asset.isQuad)
        {
            placementRotation =
                Quaternion.Euler(90f, 0f, 0f);
        }

        Quaternion finalRotation =
            objectRotation * placementRotation;

        GameObject spawned =
            Instantiate(
                asset.prefab,
                worldPosition,
                finalRotation,
                parent
            );

        spawned.name = obj.assetID;
    }
}