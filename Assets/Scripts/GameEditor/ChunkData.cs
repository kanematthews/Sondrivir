using System.Collections.Generic;

[System.Serializable]
public class ChunkData
{
    public int chunkX;

    public int chunkZ;

    public List<PlacedObjectData> objects =
        new List<PlacedObjectData>();
}