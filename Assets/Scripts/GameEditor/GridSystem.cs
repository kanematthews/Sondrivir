using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Chunk Settings")]
    public int chunkCountX = 0;

    public int chunkCountZ = 0;

    public int chunkSize = 16;

    [Header("Cell Settings")]
    public float cellSize = 1f;

    [Header("Visual Settings")]
    public Material lineMaterial;

    public float lineWidth = 0.03f;

    private GameObject placementSurface;

    private GameObject gridVisuals;

    private string chunkXInput = "";

    private string chunkZInput = "";

    public bool GridGenerated { get; private set; }

    public int WorldSizeX =>
        chunkCountX * chunkSize;

    public int WorldSizeZ =>
        chunkCountZ * chunkSize;

    void Start()
    {
        chunkXInput = "";
        chunkZInput = "";

        GridGenerated = false;
    }

    public bool IsInsideGrid(Vector3Int gridPosition)
    {
        if (!GridGenerated)
            return false;

        return
            gridPosition.x >= 0 &&
            gridPosition.z >= 0 &&
            gridPosition.x < WorldSizeX &&
            gridPosition.z < WorldSizeZ;
    }

    public void GenerateGrid()
    {
        ClearGrid();

        if (chunkCountX <= 0 ||
            chunkCountZ <= 0)
        {
            GridGenerated = false;
            return;
        }

        GridGenerated = true;

        GeneratePlacementSurface();

        GenerateGridLines();

        FocusCameraOnGrid();
    }

    void GeneratePlacementSurface()
    {
        placementSurface =
            GameObject.CreatePrimitive(
                PrimitiveType.Plane
            );

        placementSurface.name =
            "PlacementSurface";

        placementSurface.transform.SetParent(transform);

        placementSurface.transform.position =
            new Vector3(
                WorldSizeX * 0.5f,
                0f,
                WorldSizeZ * 0.5f
            );

        placementSurface.transform.localScale =
            new Vector3(
                WorldSizeX / 10f,
                1f,
                WorldSizeZ / 10f
            );

        MeshRenderer renderer =
            placementSurface.GetComponent<MeshRenderer>();

        renderer.enabled = false;
    }

    void GenerateGridLines()
    {
        gridVisuals =
            new GameObject("GridVisuals");

        gridVisuals.transform.SetParent(transform);

        for (int x = 0; x <= WorldSizeX; x++)
        {
            CreateLine(
                new Vector3(x, 0.01f, 0),
                new Vector3(x, 0.01f, WorldSizeZ)
            );
        }

        for (int z = 0; z <= WorldSizeZ; z++)
        {
            CreateLine(
                new Vector3(0, 0.01f, z),
                new Vector3(WorldSizeX, 0.01f, z)
            );
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObject =
            new GameObject("GridLine");

        lineObject.transform.SetParent(
            gridVisuals.transform
        );

        LineRenderer line =
            lineObject.AddComponent<LineRenderer>();

        line.material = lineMaterial;

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.positionCount = 2;

        line.useWorldSpace = true;

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        line.receiveShadows = false;
    }

    void FocusCameraOnGrid()
    {
        GameObject cameraRig =
            GameObject.Find("CameraRig");

        if (cameraRig == null)
            return;

        float centerX =
            WorldSizeX * 0.5f;

        float centerZ =
            WorldSizeZ * 0.5f;

        float largestDimension =
            Mathf.Max(WorldSizeX, WorldSizeZ);

        float distance =
            Mathf.Clamp(
                largestDimension * 0.8f,
                12f,
                100f
            );

        cameraRig.transform.position =
            new Vector3(
                centerX,
                0f,
                centerZ
            );

        Camera mainCam =
            Camera.main;

        if (mainCam != null)
        {
            mainCam.transform.localPosition =
                new Vector3(
                    0,
                    distance,
                    -distance
                );

            mainCam.transform.localRotation =
                Quaternion.Euler(
                    45f,
                    0f,
                    0f
                );
        }
    }

    void ClearGrid()
    {
        if (placementSurface != null)
        {
            Destroy(placementSurface);
        }

        if (gridVisuals != null)
        {
            Destroy(gridVisuals);
        }
    }

    void OnGUI()
    {
        GUI.Box(
            new Rect(10, 10, 240, 180),
            "Grid Controls"
        );

        GUI.Label(
            new Rect(20, 40, 100, 20),
            "Chunk X"
        );

        chunkXInput =
            GUI.TextField(
                new Rect(100, 40, 100, 20),
                chunkXInput
            );

        GUI.Label(
            new Rect(20, 70, 100, 20),
            "Chunk Z"
        );

        chunkZInput =
            GUI.TextField(
                new Rect(100, 70, 100, 20),
                chunkZInput
            );

        if (GUI.Button(
            new Rect(20, 110, 180, 30),
            "Generate Chunks"
        ))
        {
            if (int.TryParse(chunkXInput, out int x))
            {
                chunkCountX = Mathf.Max(0, x);
            }

            if (int.TryParse(chunkZInput, out int z))
            {
                chunkCountZ = Mathf.Max(0, z);
            }

            GenerateGrid();
        }

        GUI.Label(
            new Rect(20, 150, 200, 20),
            $"Grid: {WorldSizeX} x {WorldSizeZ}"
        );
    }
}