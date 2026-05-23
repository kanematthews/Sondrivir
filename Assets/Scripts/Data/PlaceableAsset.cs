using UnityEngine;

public enum AssetCategory
{
    Terrain,
    Buildings,
    Nature,
    Walls,
    Props,
    Misc
}

[CreateAssetMenu(menuName = "World Editor/Placeable Asset")]
public class PlaceableAsset : ScriptableObject
{
    [Header("Identification")]
    public string assetID;

    public string displayName;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Preview")]
    public Sprite previewImage;

    [Header("Category")]
    public AssetCategory category;

    [Header("Placement")]
    public bool stackable = true;

    public Vector2Int size = Vector2Int.one;

    public bool allowRotation = true;
}