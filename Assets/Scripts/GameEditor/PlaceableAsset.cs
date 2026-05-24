using UnityEngine;

[CreateAssetMenu(
    fileName = "PlaceableAsset",
    menuName = "GameEditor/Placeable Asset"
)]
public class PlaceableAsset
    : ScriptableObject
{
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
    public Texture2D previewImage;
}