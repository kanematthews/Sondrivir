using System.Collections.Generic;
using UnityEngine;

public class PlaceableAssetDatabase : MonoBehaviour
{
    public static PlaceableAssetDatabase Instance;

    [Header("Registered Assets")]
    public List<PlaceableAsset> assets = new();

    private Dictionary<string, PlaceableAsset> assetLookup =
        new Dictionary<string, PlaceableAsset>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildDatabase();
    }

    private void BuildDatabase()
    {
        assetLookup.Clear();

        foreach (var asset in assets)
        {
            if (asset == null)
                continue;

            if (string.IsNullOrEmpty(asset.assetID))
            {
                Debug.LogWarning("Found asset with empty ID.");
                continue;
            }

            if (assetLookup.ContainsKey(asset.assetID))
            {
                Debug.LogWarning(
                    $"Duplicate asset ID detected: {asset.assetID}"
                );

                continue;
            }

            assetLookup.Add(asset.assetID, asset);
        }

        Debug.Log(
            $"PlaceableAssetDatabase initialized with {assetLookup.Count} assets."
        );
    }

    public PlaceableAsset GetAsset(string assetID)
    {
        if (assetLookup.TryGetValue(assetID, out PlaceableAsset asset))
        {
            return asset;
        }

        Debug.LogWarning($"Missing asset ID: {assetID}");

        return null;
    }
}