using System.Collections.Generic;

[System.Serializable]
public class WorldData
{
    public int chunkSize;

    public int chunkCountX;

    public int chunkCountZ;

    public List<ChunkData> chunks =
        new List<ChunkData>();
}