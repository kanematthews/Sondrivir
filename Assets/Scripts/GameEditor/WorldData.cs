using System;
using System.Collections.Generic;

[Serializable]
public class WorldData
{
    public int chunkSize;

    public int chunkCountX;

    public int chunkCountZ;

    public List<ChunkData> chunks =
        new List<ChunkData>();
}