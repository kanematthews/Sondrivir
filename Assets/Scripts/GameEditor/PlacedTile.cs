using UnityEngine;

public class PlacedTile
{
    public GameObject instance;

    public PlacedObjectData data;

    public PlacedTile(
        GameObject instance,
        PlacedObjectData data)
    {
        this.instance = instance;
        this.data = data;
    }
}