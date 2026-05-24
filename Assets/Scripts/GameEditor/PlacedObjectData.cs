[System.Serializable]
public class PlacedObjectData
{
    public string assetID;

    public int x;
    public int y;
    public int z;

    // Future-proofed for
    // walls/furniture/etc
    public float rotationY;
}