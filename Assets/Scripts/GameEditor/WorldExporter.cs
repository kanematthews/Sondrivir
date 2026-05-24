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

            // FUTURE ROTATION SUPPORT
            tile.data.rotationY =
                tile.instance.transform.eulerAngles.y;

            int chunkX =
                Mathf.FloorToInt(
                    (float)position.x /
                    gridSystem.chunkSize
                );

            int chunkZ =
                Mathf.FloorToInt(
                    (float)position.z /
                    gridSystem.chunkSize
                );

            Vector2Int chunkKey =
                new Vector2Int(
                    chunkX,
                    chunkZ
                );

            if (!chunkLookup.ContainsKey(chunkKey))
            {
                ChunkData chunk =
                    new ChunkData();

                chunk.chunkX =
                    chunkX;

                chunk.chunkZ =
                    chunkZ;

                chunkLookup.Add(
                    chunkKey,
                    chunk
                );

                worldData.chunks.Add(
                    chunk
                );
            }

            chunkLookup[chunkKey]
                .objects
                .Add(tile.data);
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
    }

    void OnGUI()
    {
        GUI.Box(
            new Rect(10, 350, 220, 120),
            "Export"
        );

        GUI.Label(
            new Rect(20, 380, 100, 20),
            "File Name"
        );

        exportFileName =
            GUI.TextField(
                new Rect(20, 405, 180, 20),
                exportFileName
            );

        if (GUI.Button(
            new Rect(20, 435, 180, 25),
            "Export World"
        ))
        {
            ExportWorld();
        }
    }
}