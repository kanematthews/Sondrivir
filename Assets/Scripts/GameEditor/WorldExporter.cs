using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldExporter : MonoBehaviour
{
    [Header("References")]
    public GridPlacementSystem placementSystem;

    public GridSystem gridSystem;

    [Header("Export Settings")]
    public string exportFileName =
        "GeneratedWorld.json";

    [Header("World Placement")]
    [Tooltip("Where this region should exist in the REAL game world.")]
    public int exportChunkOffsetX = 0;

    public int exportChunkOffsetZ = 0;

    public void ExportWorld()
    {
        if (placementSystem == null)
        {
            Debug.LogError(
                "Placement System reference missing!"
            );

            return;
        }

        if (gridSystem == null)
        {
            Debug.LogError(
                "Grid System reference missing!"
            );

            return;
        }

        WorldData worldData =
            new WorldData();

        worldData.chunkSize =
            gridSystem.chunkSize;

        worldData.chunkCountX =
            gridSystem.chunkCountX;

        worldData.chunkCountZ =
            gridSystem.chunkCountZ;

        Dictionary<Vector2Int, ChunkData>
            chunkLookup =
            new Dictionary<Vector2Int, ChunkData>();

        foreach (var pair in placementSystem.PlacedTiles)
        {
            Vector3Int position =
                pair.Key;

            PlacedTile tile =
                pair.Value;

            // Save rotation
            tile.data.rotationY =
                tile.instance.transform.eulerAngles.y;

            // LOCAL chunk coordinates
            int localChunkX =
                Mathf.FloorToInt(
                    (float)position.x /
                    gridSystem.chunkSize
                );

            int localChunkZ =
                Mathf.FloorToInt(
                    (float)position.z /
                    gridSystem.chunkSize
                );

            // FINAL WORLD chunk coordinates
            int worldChunkX =
                localChunkX + exportChunkOffsetX;

            int worldChunkZ =
                localChunkZ + exportChunkOffsetZ;

            Vector2Int chunkKey =
                new Vector2Int(
                    worldChunkX,
                    worldChunkZ
                );

            if (!chunkLookup.ContainsKey(chunkKey))
            {
                ChunkData chunk =
                    new ChunkData();

                chunk.chunkX =
                    worldChunkX;

                chunk.chunkZ =
                    worldChunkZ;

                chunkLookup.Add(
                    chunkKey,
                    chunk
                );

                worldData.chunks.Add(
                    chunk
                );
            }

            // CREATE A COPY
            // Never modify editor data directly
            PlacedObjectData objectData =
                new PlacedObjectData();

            objectData.assetID =
                tile.data.assetID;

            objectData.rotationY =
                tile.data.rotationY;

            objectData.y =
                tile.data.y;

            // Convert LOCAL editor positions
            // into FINAL WORLD positions
            objectData.x =
                tile.data.x +
                (exportChunkOffsetX * gridSystem.chunkSize);

            objectData.z =
                tile.data.z +
                (exportChunkOffsetZ * gridSystem.chunkSize);

            chunkLookup[chunkKey]
                .objects
                .Add(objectData);
        }

        string json =
            JsonUtility.ToJson(
                worldData,
                true
            );

        string directory =
            Path.Combine(
                Application.dataPath,
                "GeneratedMaps"
            );

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(
                directory
            );
        }

        string fullPath =
            Path.Combine(
                directory,
                exportFileName
            );

        File.WriteAllText(
            fullPath,
            json
        );

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log(
            $"World exported successfully:\n{fullPath}"
        );

        Debug.Log(
            $"Export Offset Applied:\nChunk X: {exportChunkOffsetX}\nChunk Z: {exportChunkOffsetZ}"
        );
    }

    void OnGUI()
    {
        GUI.Box(
            new Rect(10, 350, 250, 190),
            "Export"
        );

        GUI.Label(
            new Rect(20, 380, 100, 20),
            "File Name"
        );

        exportFileName =
            GUI.TextField(
                new Rect(20, 405, 200, 20),
                exportFileName
            );

        GUI.Label(
            new Rect(20, 435, 120, 20),
            "Chunk Offset X"
        );

        string offsetXString =
            GUI.TextField(
                new Rect(140, 435, 60, 20),
                exportChunkOffsetX.ToString()
            );

        int.TryParse(
            offsetXString,
            out exportChunkOffsetX
        );

        GUI.Label(
            new Rect(20, 465, 120, 20),
            "Chunk Offset Z"
        );

        string offsetZString =
            GUI.TextField(
                new Rect(140, 465, 60, 20),
                exportChunkOffsetZ.ToString()
            );

        int.TryParse(
            offsetZString,
            out exportChunkOffsetZ
        );

        if (GUI.Button(
            new Rect(20, 500, 200, 25),
            "Export World"
        ))
        {
            ExportWorld();
        }
    }
}