using System;
using System.Collections.Generic;

[Serializable]
public class ChunkData
{
    public int chunkX;

    public int chunkZ;

    public List<PlacedObjectData> objects =
        new List<PlacedObjectData>();
}