using System.Collections.Generic;
using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    public enum EditorTool
    {
        Add,
        Remove
    }

    [Header("References")]
    public Camera sceneCamera;

    public GridSystem gridSystem;

    [Header("Selected Asset")]
    public PlaceableAsset selectedAsset;

    [Header("Tools")]
    public EditorTool currentTool =
        EditorTool.Add;

    public Dictionary<Vector3Int, PlacedTile>
        PlacedTiles =
        new Dictionary<Vector3Int, PlacedTile>();

    private GameObject previewObject;

    private PlaceableAsset lastAsset;

    private Vector3Int lastPlacedPosition =
        Vector3Int.one * -9999;

    private bool dragPlacementMode;

    void Update()
    {
        if (!gridSystem.GridGenerated)
        {
            HidePreview();
            return;
        }

        UpdatePreviewAsset();

        UpdatePreviewPosition();

        HandlePlacement();

        HandleRemove();
    }

    void UpdatePreviewAsset()
    {
        if (selectedAsset == lastAsset)
            return;

        lastAsset = selectedAsset;

        CreatePreviewObject();
    }

    void CreatePreviewObject()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        if (selectedAsset == null)
            return;

        previewObject =
            Instantiate(
                selectedAsset.prefab
            );

        previewObject.name =
            "PlacementPreview";

        Renderer[] renderers =
            previewObject
            .GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] mats =
                renderer.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                Material previewMat =
                    new Material(mats[i]);

                Color color =
                    previewMat.color;

                color.a = 0.5f;

                previewMat.color = color;

                mats[i] = previewMat;
            }

            renderer.materials = mats;
        }

        Collider[] colliders =
            previewObject
            .GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    void UpdatePreviewPosition()
    {
        if (previewObject == null)
            return;

        if (!GetMouseGridPosition(
            out Vector3Int gridPos))
        {
            previewObject.SetActive(false);
            return;
        }

        previewObject.SetActive(true);

        int targetHeight =
            GetPlacementHeight(gridPos);

        previewObject.transform.position =
            new Vector3(
                gridPos.x + 0.5f,
                targetHeight,
                gridPos.z + 0.5f
            );
    }

    void HandlePlacement()
{
    if (currentTool != EditorTool.Add)
        return;

    if (selectedAsset == null)
        return;

    // SINGLE CLICK
    if (Input.GetMouseButtonDown(0))
    {
        TryPlace(false);
    }

    // HOLD DRAG
    if (Input.GetMouseButton(0))
    {
        TryPlace(true);
    }

    if (!Input.GetMouseButton(0))
    {
        lastPlacedPosition =
            Vector3Int.one * -9999;
    }
}

void TryPlace(bool dragMode)
{
    if (!GetMouseGridPosition(
        out Vector3Int gridPos))
        return;

    int topHeight =
        GetTopHeight(gridPos);

    int targetHeight;

    // DRAG MODE =
    // terrain painting
    if (dragMode)
    {
        targetHeight =
            Mathf.Max(0, topHeight);
    }
    else
    {
        // SINGLE CLICK =
        // stack upward
        targetHeight =
            topHeight + 1;
    }

    Vector3Int finalPos =
        new Vector3Int(
            gridPos.x,
            targetHeight,
            gridPos.z
        );

    if (finalPos == lastPlacedPosition)
        return;

    lastPlacedPosition =
        finalPos;

    if (PlacedTiles.ContainsKey(finalPos))
        return;

    GameObject obj =
        Instantiate(
            selectedAsset.prefab,
            new Vector3(
                finalPos.x + 0.5f,
                finalPos.y,
                finalPos.z + 0.5f
            ),
            Quaternion.identity
        );

    obj.name =
        selectedAsset.displayName;

    PlacedObjectData data =
        new PlacedObjectData();

    data.assetID =
        selectedAsset.assetID;

    data.x = finalPos.x;
    data.y = finalPos.y;
    data.z = finalPos.z;

    PlacedTiles.Add(
        finalPos,
        new PlacedTile(
            obj,
            data
        )
    );
}

    int GetPlacementHeight(
    Vector3Int gridPos)
{
    int topHeight =
        GetTopHeight(gridPos);

    // HOLD SHIFT =
    // force vertical stacking
    if (Input.GetKey(KeyCode.LeftShift))
    {
        return topHeight + 1;
    }

    // HOLDING MOUSE =
    // terrain painting mode
    if (dragPlacementMode)
    {
        // If tile exists,
        // stay at same height
        if (topHeight >= 0)
        {
            return topHeight;
        }

        // Otherwise place on ground
        return 0;
    }

    // SINGLE CLICK =
    // intentional stacking
    return topHeight + 1;
}

    void HandleRemove()
    {
        if (currentTool != EditorTool.Remove)
            return;

        if (!GetMouseGridPosition(
            out Vector3Int gridPos))
            return;

        Vector3Int? target =
            GetTopTile(gridPos);

        HighlightRemoveTarget(target);

        if (!Input.GetMouseButtonDown(0))
            return;

        if (target == null)
            return;

        PlacedTile tile =
            PlacedTiles[target.Value];

        Destroy(tile.instance);

        PlacedTiles.Remove(
            target.Value
        );
    }

    void HighlightRemoveTarget(
        Vector3Int? target)
    {
        foreach (var pair in PlacedTiles)
        {
            Renderer[] renderers =
                pair.Value.instance
                .GetComponentsInChildren<Renderer>();

            foreach (Renderer r in renderers)
            {
                foreach (Material m in r.materials)
                {
                    m.color = Color.white;
                }
            }
        }

        if (target == null)
            return;

        Renderer[] targetRenderers =
            PlacedTiles[target.Value]
            .instance
            .GetComponentsInChildren<Renderer>();

        foreach (Renderer r in targetRenderers)
        {
            foreach (Material m in r.materials)
            {
                m.color =
                    new Color(
                        1f,
                        0.5f,
                        0.5f
                    );
            }
        }
    }

    bool GetMouseGridPosition(
        out Vector3Int gridPos)
    {
        gridPos = Vector3Int.zero;

        Ray ray =
            sceneCamera.ScreenPointToRay(
                Input.mousePosition
            );

        Plane plane =
            new Plane(
                Vector3.up,
                Vector3.zero
            );

        if (!plane.Raycast(
            ray,
            out float enter))
        {
            return false;
        }

        Vector3 hitPoint =
            ray.GetPoint(enter);

        int x =
            Mathf.FloorToInt(hitPoint.x);

        int z =
            Mathf.FloorToInt(hitPoint.z);

        gridPos =
            new Vector3Int(
                x,
                0,
                z
            );

        return gridSystem
            .IsInsideGrid(gridPos);
    }

    int GetTopHeight(
        Vector3Int gridPos)
    {
        int highest = -1;

        foreach (var pair in PlacedTiles)
        {
            Vector3Int pos =
                pair.Key;

            if (pos.x == gridPos.x &&
                pos.z == gridPos.z)
            {
                if (pos.y > highest)
                {
                    highest = pos.y;
                }
            }
        }

        return highest;
    }

    Vector3Int? GetTopTile(
        Vector3Int gridPos)
    {
        int highest = -1;

        Vector3Int? result = null;

        foreach (var pair in PlacedTiles)
        {
            Vector3Int pos =
                pair.Key;

            if (pos.x == gridPos.x &&
                pos.z == gridPos.z)
            {
                if (pos.y > highest)
                {
                    highest = pos.y;
                    result = pos;
                }
            }
        }

        return result;
    }

    void HidePreview()
    {
        if (previewObject != null)
        {
            previewObject.SetActive(false);
        }
    }
}