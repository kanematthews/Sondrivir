using UnityEngine;

[CreateAssetMenu(
    fileName = "PlaceableAsset",
    menuName = "GameEditor/Placeable Asset"
)]
public class PlaceableAsset
    : ScriptableObject
{
    public enum PlacementType
    {
        Tile,
        Prop,
        Structure
    }

    [Header("Info")]
    public string assetID;

    public string displayName;

    [TextArea]
    public string description;

    [Header("Category")]
    public string category;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Preview")]
    public Texture2D previewTexture;

    [Header("Placement")]
    public PlacementType placementType =
        PlacementType.Prop;

    public Vector3 placementRotation;

    public Vector3 placementOffset;

    public bool placeAtGroundLevel = true;

    [Header("Structure Footprint")]
    public Vector2Int footprintSize =
        Vector2Int.one;

    [Header("Surface")]
    public bool isQuad = false;

    [Header("Gameplay")]
    public bool blocksMovement = false;

    public bool isWater = false;
}