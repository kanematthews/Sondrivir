using System.Collections.Generic;
using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    public enum EditorTool
    {
        Add,
        Remove,
        Fill,
        Select,
        Resize
    }

    public enum ResizeHandle
    {
        PosX,   // right  → scale X grow
        NegX,   // left   → scale X shrink
        PosZ,   // front  → scale Z grow
        NegZ,   // back   → scale Z shrink
        PosY,   // top    → scale Y grow
        NegY    // bottom → scale Y shrink
    }

    [Header("References")]
    public Camera sceneCamera;
    public GridSystem gridSystem;

    [Header("Selected Asset")]
    public PlaceableAsset selectedAsset;

    [Header("Tools")]
    public EditorTool currentTool = EditorTool.Add;

    // All placed tiles keyed by their origin grid cell.
    public Dictionary<Vector3Int, PlacedTile>
        PlacedTiles =
        new Dictionary<Vector3Int, PlacedTile>();

    // Every cell covered by a multi-tile structure,
    // mapped back to that structure's origin key so
    // overlap checks and removal both work correctly.
    private Dictionary<Vector3Int, Vector3Int>
        structureFootprintCells =
        new Dictionary<Vector3Int, Vector3Int>();

    // ── placement preview ────────────────────────
    private GameObject    previewObject;
    private PlaceableAsset lastAsset;

    // Sentinel: reset at the START of every new
    // discrete click so stacking always works.
    private Vector3Int lastPlacedPosition =
        Vector3Int.one * -9999;

    private bool dragPlacementMode;

    // ── selection / move ─────────────────────────
    private PlacedTile selectedTile;
    private Vector3Int selectedTilePosition;
    private bool       isDraggingSelected;
    private GameObject movePreviewObject;
    private GameObject selectionOutline;

    // ── resize gizmo ─────────────────────────────
    private Vector3Int?   resizeTargetTile = null;
    private GameObject    resizeGizmoRoot  = null;
    private ResizeHandle? activeHandle     = null;
    private float         dragStartMouseX;
    private float         dragStartMouseY;
    private Vector3       dragStartScale;
    private Dictionary<GameObject, Vector3>
        defaultObjectScales =
        new Dictionary<GameObject, Vector3>();

    // ════════════════════════════════════════════
    void Update()
    {
        if (!gridSystem.GridGenerated)
        {
            HidePreview();
            return;
        }

        dragPlacementMode = Input.GetMouseButton(0);

        UpdatePreviewAsset();
        UpdatePreviewPosition();

        if (currentTool != EditorTool.Select &&
            currentTool != EditorTool.Remove)
        {
            DestroySelectionOutline();
        }

        HandlePlacement();
        HandleFill();
        HandleSelection();
        HandleResize();
        HandleRemove();
    }

    // ════════════════════════════════════════════
    // PREVIEW
    // ════════════════════════════════════════════

    void UpdatePreviewAsset()
    {
        if (selectedAsset == lastAsset) return;
        lastAsset = selectedAsset;
        CreatePreviewObject();
    }

    Quaternion GetPlacementRotation()
    {
        if (selectedAsset == null)
            return Quaternion.identity;

        Quaternion r =
            Quaternion.Euler(selectedAsset.placementRotation);

        if (selectedAsset.isQuad)
            r *= Quaternion.Euler(90f, 0f, 0f);

        return r;
    }

    void CreatePreviewObject()
    {
        if (previewObject != null)
            Destroy(previewObject);

        if (selectedAsset == null) return;

        previewObject = Instantiate(selectedAsset.prefab);
        previewObject.name = "PlacementPreview";
        previewObject.transform.rotation =
            GetPlacementRotation();

        // Structures: force preview to footprint size
        // immediately so the ghost matches reality.
        if (IsStructure(selectedAsset))
            ApplyFootprintScale(previewObject, selectedAsset);

        MakeTransparent(previewObject, 0.5f);
        DisableColliders(previewObject);
    }

    void UpdatePreviewPosition()
    {
        if (previewObject == null) return;

        if (currentTool == EditorTool.Remove  ||
            currentTool == EditorTool.Select  ||
            currentTool == EditorTool.Resize)
        {
            previewObject.SetActive(false);
            return;
        }

        if (!GetMouseGridPosition(out Vector3Int gridPos))
        {
            previewObject.SetActive(false);
            return;
        }

        previewObject.SetActive(true);

        int h = GetStackHeight(gridPos, selectedAsset);

        previewObject.transform.position =
            GetSpawnPosition(gridPos, h, selectedAsset);
    }

    // ════════════════════════════════════════════
    // HEIGHT & POSITION HELPERS
    // ════════════════════════════════════════════

    // Returns the Y layer to place the NEXT object
    // at this column.
    // • placeAtGroundLevel = true  → always y=0
    //   UNLESS something is already there, in which
    //   case we stack on top (so tiles can be layered).
    // • placeAtGroundLevel = false → always stack.
    int GetStackHeight(
        Vector3Int gridPos, PlaceableAsset asset)
    {
        int top = GetTopHeight(gridPos);

        // Empty column → place at 0.
        if (top < 0) return 0;

        // Something already there → always stack.
        return top + 1;
    }

    // World-space spawn centre for an object placed
    // at originCell / height. Structures are offset
    // so the prefab is centred on its footprint.
    Vector3 GetSpawnPosition(
        Vector3Int     originCell,
        int            height,
        PlaceableAsset asset)
    {
        if (IsStructure(asset))
        {
            // Centre of the footprint in XZ.
            float cx = originCell.x +
                       asset.footprintSize.x * 0.5f;
            float cz = originCell.z +
                       asset.footprintSize.y * 0.5f;

            return new Vector3(cx, height, cz)
                   + asset.placementOffset;
        }

        return new Vector3(
                   originCell.x + 0.5f,
                   height,
                   originCell.z + 0.5f)
               + asset.placementOffset;
    }

    // ════════════════════════════════════════════
    // STRUCTURE FOOTPRINT
    // ════════════════════════════════════════════

    static bool IsStructure(PlaceableAsset asset) =>
        asset != null &&
        asset.placementType ==
        PlaceableAsset.PlacementType.Structure;

    // Scale the GameObject so its XZ renderer bounds
    // exactly fill footprintSize world-units.
    // Y is scaled proportionally to the larger axis
    // so tall buildings don't get squashed.
    void ApplyFootprintScale(
        GameObject obj, PlaceableAsset asset)
    {
        // Measure at unit scale to get natural size.
        Vector3 saved = obj.transform.localScale;
        obj.transform.localScale = Vector3.one;

        Bounds b = CalculateObjectBounds(obj);

        obj.transform.localScale = saved;

        if (b.size.x <= 0f || b.size.z <= 0f)
            return;

        float sx = asset.footprintSize.x / b.size.x;
        float sz = asset.footprintSize.y / b.size.z;
        float sy = Mathf.Max(sx, sz); // keep proportional

        obj.transform.localScale =
            new Vector3(sx, sy, sz);
    }

    bool CanPlaceStructure(
        Vector3Int origin, PlaceableAsset asset)
    {
        for (int x = 0; x < asset.footprintSize.x; x++)
        {
            for (int z = 0; z < asset.footprintSize.y; z++)
            {
                Vector3Int cell = new Vector3Int(
                    origin.x + x, origin.y, origin.z + z);

                if (structureFootprintCells.ContainsKey(cell))
                    return false;

                if (PlacedTiles.ContainsKey(cell))
                    return false;

                if (!gridSystem.IsInsideGrid(cell))
                    return false;
            }
        }
        return true;
    }

    void RegisterStructureFootprint(
        Vector3Int origin, PlaceableAsset asset)
    {
        for (int x = 0; x < asset.footprintSize.x; x++)
        {
            for (int z = 0; z < asset.footprintSize.y; z++)
            {
                Vector3Int cell = new Vector3Int(
                    origin.x + x, origin.y, origin.z + z);

                // Cell → origin so we can find the
                // PlacedTiles entry from any footprint cell.
                structureFootprintCells[cell] = origin;
            }
        }
    }

    void UnregisterStructureFootprint(Vector3Int origin)
    {
        List<Vector3Int> toRemove = new List<Vector3Int>();

        foreach (var pair in structureFootprintCells)
        {
            if (pair.Value == origin)
                toRemove.Add(pair.Key);
        }

        foreach (Vector3Int key in toRemove)
            structureFootprintCells.Remove(key);
    }

    // ════════════════════════════════════════════
    // ADD TOOL
    // Click  = stack ON TOP of whatever is there.
    // Drag   = paint flat across the surface.
    // ════════════════════════════════════════════

    void HandlePlacement()
    {
        if (currentTool != EditorTool.Add) return;
        if (selectedAsset == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            // Reset sentinel on every new discrete
            // click so the same column can be clicked
            // repeatedly to build a stack.
            lastPlacedPosition = Vector3Int.one * -9999;
            TryPlace(false);
        }
        else if (Input.GetMouseButton(0))
        {
            TryPlace(true);
        }

        if (!Input.GetMouseButton(0))
            lastPlacedPosition = Vector3Int.one * -9999;
    }

    void TryPlace(bool dragMode)
    {
        if (!GetMouseGridPosition(out Vector3Int gridPos))
            return;

        int top = GetTopHeight(gridPos);
        int targetHeight;

        if (dragMode)
        {
            // Drag/paint: stay at the current surface,
            // don't stack.
            targetHeight = Mathf.Max(0, top);
        }
        else
        {
            // Click: stack — place one layer above
            // whatever is already here.
            // top == -1 → empty column → y = 0.
            targetHeight = top + 1;
        }

        Vector3Int finalPos = new Vector3Int(
            gridPos.x, targetHeight, gridPos.z);

        if (finalPos == lastPlacedPosition) return;
        lastPlacedPosition = finalPos;

        if (IsStructure(selectedAsset))
        {
            if (!CanPlaceStructure(finalPos, selectedAsset))
                return;
        }
        else
        {
            if (PlacedTiles.ContainsKey(finalPos)) return;
        }

        Vector3 spawnPos =
            GetSpawnPosition(finalPos, finalPos.y, selectedAsset);

        GameObject obj = Instantiate(
            selectedAsset.prefab,
            spawnPos,
            GetPlacementRotation());

        obj.name = selectedAsset.displayName;

        // Structures are always scaled to their
        // footprint so they physically fill the tiles.
        if (IsStructure(selectedAsset))
            ApplyFootprintScale(obj, selectedAsset);

        PlacedObjectData data = new PlacedObjectData
        {
            assetID   = selectedAsset.assetID,
            x         = finalPos.x,
            y         = finalPos.y,
            z         = finalPos.z,
            rotationY = 0f
        };

        PlacedTiles.Add(finalPos, new PlacedTile(obj, data));

        if (IsStructure(selectedAsset))
            RegisterStructureFootprint(finalPos, selectedAsset);
    }

    // ════════════════════════════════════════════
    // FILL TOOL
    // ════════════════════════════════════════════

    void HandleFill()
    {
        if (currentTool != EditorTool.Fill) return;
        if (selectedAsset == null) return;
        if (!Input.GetMouseButtonDown(0)) return;

        if (!GetMouseGridPosition(out Vector3Int startPos))
            return;

        int targetSurface = GetTopHeight(startPos);

        Queue<Vector3Int>   queue   = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        queue.Enqueue(startPos);

        while (queue.Count > 0)
        {
            Vector3Int cur = queue.Dequeue();
            if (visited.Contains(cur)) continue;
            visited.Add(cur);

            if (!gridSystem.IsInsideGrid(cur)) continue;
            if (GetTopHeight(cur) != targetSurface) continue;

            int placeY = targetSurface + 1;
            Vector3Int finalPos =
                new Vector3Int(cur.x, placeY, cur.z);

            if (!PlacedTiles.ContainsKey(finalPos))
            {
                Vector3 spawnPos = GetSpawnPosition(
                    finalPos, placeY, selectedAsset);

                GameObject obj = Instantiate(
                    selectedAsset.prefab,
                    spawnPos,
                    GetPlacementRotation());

                obj.name = selectedAsset.displayName;

                if (IsStructure(selectedAsset))
                    ApplyFootprintScale(obj, selectedAsset);

                PlacedTiles.Add(finalPos,
                    new PlacedTile(obj,
                        new PlacedObjectData
                        {
                            assetID   = selectedAsset.assetID,
                            x         = finalPos.x,
                            y         = finalPos.y,
                            z         = finalPos.z,
                            rotationY = 0f
                        }));
            }

            queue.Enqueue(cur + Vector3Int.right);
            queue.Enqueue(cur + Vector3Int.left);
            queue.Enqueue(cur + new Vector3Int(0,0, 1));
            queue.Enqueue(cur + new Vector3Int(0,0,-1));
        }
    }

    // ════════════════════════════════════════════
    // SELECT / MOVE TOOL
    // Click → lifts tile, ghost follows cursor.
    // Release → drops at new position.
    // ════════════════════════════════════════════

    void HandleSelection()
    {
        if (currentTool != EditorTool.Select) return;

        if (!GetMouseGridPosition(out Vector3Int gridPos))
        {
            if (isDraggingSelected)
                UpdateMovePreviewPosition(gridPos);
            return;
        }

        if (!isDraggingSelected)
        {
            Vector3Int? topTile = GetTopTile(gridPos);
            HighlightTarget(topTile, Color.cyan);

            if (Input.GetMouseButtonDown(0) &&
                topTile != null)
            {
                BeginSelectDrag(topTile.Value);
            }
            return;
        }

        UpdateMovePreviewPosition(gridPos);

        if (Input.GetMouseButtonUp(0))
            FinishSelectDrag(gridPos);
    }

    void BeginSelectDrag(Vector3Int tilePos)
    {
        selectedTilePosition = tilePos;
        selectedTile = PlacedTiles[tilePos];

        PlacedTiles.Remove(tilePos);

        // Also free up the structure footprint cells
        // so the drag doesn't block its own landing.
        if (IsStructure(GetAssetForTile(selectedTile)))
            UnregisterStructureFootprint(tilePos);

        selectedTile.instance.SetActive(false);

        CreateMovePreview();
        CreateSelectionOutline(movePreviewObject);

        isDraggingSelected = true;
    }

    void UpdateMovePreviewPosition(Vector3Int gridPos)
    {
        if (movePreviewObject == null) return;

        int top    = GetTopHeight(gridPos);
        int height = top < 0 ? 0 : top + 1;

        movePreviewObject.transform.position =
            new Vector3(gridPos.x + 0.5f, height,
                        gridPos.z + 0.5f);

        if (selectionOutline != null)
            RebuildOutlineOnTarget(movePreviewObject);
    }

    void FinishSelectDrag(Vector3Int gridPos)
    {
        if (selectedTile == null) return;

        int top    = GetTopHeight(gridPos);
        int height = top < 0 ? 0 : top + 1;

        Vector3Int newPos =
            new Vector3Int(gridPos.x, height, gridPos.z);

        selectedTile.instance.SetActive(true);
        selectedTile.instance.transform.position =
            new Vector3(newPos.x + 0.5f, newPos.y,
                        newPos.z + 0.5f);

        selectedTile.data.x = newPos.x;
        selectedTile.data.y = newPos.y;
        selectedTile.data.z = newPos.z;

        PlacedTiles.Add(newPos, selectedTile);

        // Re-register footprint at new position.
        PlaceableAsset asset =
            GetAssetForTile(selectedTile);
        if (IsStructure(asset))
            RegisterStructureFootprint(newPos, asset);

        isDraggingSelected = false;
        selectedTile = null;

        Destroy(movePreviewObject);
        DestroySelectionOutline();
    }

    void CreateMovePreview()
    {
        if (movePreviewObject != null)
            Destroy(movePreviewObject);

        if (selectedTile == null) return;

        movePreviewObject =
            Instantiate(selectedTile.instance);
        movePreviewObject.SetActive(true);

        MakeTransparent(movePreviewObject, 0.5f);
        DisableColliders(movePreviewObject);
    }

    // ════════════════════════════════════════════
    // REMOVE TOOL
    // ════════════════════════════════════════════

    void HandleRemove()
    {
        if (currentTool != EditorTool.Remove) return;

        if (!GetMouseGridPosition(out Vector3Int gridPos))
            return;

        // Check both normal tiles and structure cells.
        Vector3Int? target = GetTopTile(gridPos);

        // If no direct tile, check if this cell is
        // part of a structure footprint.
        if (target == null &&
            structureFootprintCells.ContainsKey(
                new Vector3Int(gridPos.x, 0, gridPos.z)))
        {
            Vector3Int origin =
                structureFootprintCells[
                    new Vector3Int(gridPos.x, 0, gridPos.z)];
            target = origin;
        }

        HighlightTarget(target, new Color(1f, 0.4f, 0.4f));

        if (!Input.GetMouseButtonDown(0)) return;
        if (target == null) return;

        if (!PlacedTiles.ContainsKey(target.Value)) return;

        PlacedTile tile = PlacedTiles[target.Value];

        UnregisterStructureFootprint(target.Value);
        Destroy(tile.instance);
        PlacedTiles.Remove(target.Value);
    }

    // ════════════════════════════════════════════
    // RESIZE TOOL  (world-space gizmo)
    // Click tile → lock gizmo.
    // Drag handle → scale that axis.
    // Click empty → deselect.
    // ════════════════════════════════════════════

    void HandleResize()
    {
        if (currentTool != EditorTool.Resize)
        {
            DestroyResizeGizmo();
            resizeTargetTile = null;
            activeHandle     = null;
            return;
        }

        if (activeHandle.HasValue)
        {
            ContinueResizeDrag();

            if (Input.GetMouseButtonUp(0))
                activeHandle = null;

            return;
        }

        if (!GetMouseGridPosition(out Vector3Int gridPos))
            return;

        Vector3Int? hovered = GetTopTile(gridPos);

        if (hovered.HasValue &&
            hovered.Value != resizeTargetTile)
        {
            HighlightSingleTile(hovered.Value, Color.yellow);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (resizeTargetTile.HasValue &&
                TryBeginResizeDrag())
                return;

            if (hovered.HasValue)
            {
                resizeTargetTile = hovered;
                PlacedTile tile =
                    PlacedTiles[hovered.Value];
                EnsureDefaultScale(tile.instance);
                BuildResizeGizmo(tile.instance);
            }
            else
            {
                resizeTargetTile = null;
                DestroyResizeGizmo();
            }
        }
    }

    bool TryBeginResizeDrag()
    {
        if (resizeGizmoRoot == null) return false;

        Ray ray = sceneCamera.ScreenPointToRay(
            Input.mousePosition);

        foreach (Transform child in
            resizeGizmoRoot.transform)
        {
            Collider col =
                child.GetComponent<Collider>();
            if (col == null) continue;

            if (!col.Raycast(ray, out RaycastHit _, 200f))
                continue;

            if (!System.Enum.TryParse(
                child.name, out ResizeHandle h))
                continue;

            activeHandle    = h;
            dragStartMouseX = Input.mousePosition.x;
            dragStartMouseY = Input.mousePosition.y;

            if (resizeTargetTile.HasValue &&
                PlacedTiles.ContainsKey(
                    resizeTargetTile.Value))
            {
                dragStartScale =
                    PlacedTiles[resizeTargetTile.Value]
                    .instance.transform.localScale;
            }

            return true;
        }

        return false;
    }

    void ContinueResizeDrag()
    {
        if (!activeHandle.HasValue)     return;
        if (!resizeTargetTile.HasValue) return;
        if (!PlacedTiles.ContainsKey(
            resizeTargetTile.Value))    return;

        GameObject obj =
            PlacedTiles[resizeTargetTile.Value].instance;

        float dH =
            (Input.mousePosition.x - dragStartMouseX) / 80f;
        float dV =
            (Input.mousePosition.y - dragStartMouseY) / 80f;

        const float MIN = 0.1f;
        Vector3 s = dragStartScale;

        switch (activeHandle.Value)
        {
            case ResizeHandle.PosX:
                s.x = Mathf.Max(MIN, s.x + dH); break;
            case ResizeHandle.NegX:
                s.x = Mathf.Max(MIN, s.x - dH); break;
            case ResizeHandle.PosZ:
                s.z = Mathf.Max(MIN, s.z + dH); break;
            case ResizeHandle.NegZ:
                s.z = Mathf.Max(MIN, s.z - dH); break;
            case ResizeHandle.PosY:
                s.y = Mathf.Max(MIN, s.y + dV); break;
            case ResizeHandle.NegY:
                s.y = Mathf.Max(MIN, s.y - dV); break;
        }

        obj.transform.localScale = s;
        BuildResizeGizmo(obj);
    }

    void BuildResizeGizmo(GameObject target)
    {
        DestroyResizeGizmo();
        resizeGizmoRoot = new GameObject("ResizeGizmo");

        Bounds  b  = GetObjectBounds(target);
        Vector3 c  = b.center;
        Vector3 sz = b.size;
        float   L  = 0.55f, R = 0.07f;

        MakeArrow(ResizeHandle.PosX,
            c + new Vector3( sz.x*.5f, 0, 0),
            Vector3.right,   Color.red,   L, R);
        MakeArrow(ResizeHandle.NegX,
            c + new Vector3(-sz.x*.5f, 0, 0),
            Vector3.left,    Color.red,   L, R);
        MakeArrow(ResizeHandle.PosZ,
            c + new Vector3(0, 0,  sz.z*.5f),
            Vector3.forward, Color.blue,  L, R);
        MakeArrow(ResizeHandle.NegZ,
            c + new Vector3(0, 0, -sz.z*.5f),
            Vector3.back,    Color.blue,  L, R);
        MakeArrow(ResizeHandle.PosY,
            c + new Vector3(0,  sz.y*.5f, 0),
            Vector3.up,      Color.green, L, R);
        MakeArrow(ResizeHandle.NegY,
            c + new Vector3(0, -sz.y*.5f, 0),
            Vector3.down,    Color.green, L, R);
    }

    void MakeArrow(
        ResizeHandle handle, Vector3 origin,
        Vector3 dir, Color color,
        float length, float radius)
    {
        GameObject root =
            new GameObject(handle.ToString());
        root.transform.SetParent(
            resizeGizmoRoot.transform, false);
        root.transform.position = origin;
        root.transform.rotation =
            Quaternion.FromToRotation(Vector3.up, dir);

        GameObject shaft =
            GameObject.CreatePrimitive(
                PrimitiveType.Cylinder);
        shaft.name = "Shaft";
        Destroy(shaft.GetComponent<Collider>());
        shaft.transform.SetParent(root.transform, false);
        shaft.transform.localPosition =
            new Vector3(0f, length * 0.33f, 0f);
        shaft.transform.localScale =
            new Vector3(radius, length * 0.33f, radius);
        SetMaterialColor(shaft, color);

        GameObject tip =
            GameObject.CreatePrimitive(PrimitiveType.Cube);
        tip.name = "Tip";
        Destroy(tip.GetComponent<Collider>());
        tip.transform.SetParent(root.transform, false);
        tip.transform.localPosition =
            new Vector3(0f, length * 0.75f, 0f);
        tip.transform.localRotation =
            Quaternion.Euler(45f, 45f, 0f);
        tip.transform.localScale =
            new Vector3(radius*2f, radius*2f, radius*2f);
        SetMaterialColor(tip, color);

        CapsuleCollider col =
            root.AddComponent<CapsuleCollider>();
        col.isTrigger = true;
        col.center    = new Vector3(0f, length*.5f, 0f);
        col.height    = length * 1.1f;
        col.radius    = radius * 2.5f;
    }

    void DestroyResizeGizmo()
    {
        if (resizeGizmoRoot != null)
        {
            Destroy(resizeGizmoRoot);
            resizeGizmoRoot = null;
        }
    }

    // ════════════════════════════════════════════
    // SELECTION OUTLINE
    // ════════════════════════════════════════════

    void CreateSelectionOutline(GameObject target)
    {
        DestroySelectionOutline();
        if (target == null) return;

        selectionOutline =
            new GameObject("SelectionOutline");
        RebuildOutlineEdges(
            target, selectionOutline.transform);
    }

    void RebuildOutlineOnTarget(GameObject target)
    {
        if (selectionOutline == null) return;

        foreach (Transform child in
            selectionOutline.transform)
            Destroy(child.gameObject);

        RebuildOutlineEdges(
            target, selectionOutline.transform);
    }

    void RebuildOutlineEdges(
        GameObject target, Transform root)
    {
        Bounds  b = GetObjectBounds(target);
        Vector3 c = b.center;
        Vector3 s = b.size;
        float   t = 0.03f;

        void E(Vector3 p, Vector3 sc) =>
            CreateOutlineEdge(p, sc, root);

        // Top ring
        E(c+new Vector3(0,        s.y*.5f, s.z*.5f),  new Vector3(s.x,t,t));
        E(c+new Vector3(0,        s.y*.5f,-s.z*.5f),  new Vector3(s.x,t,t));
        E(c+new Vector3(-s.x*.5f, s.y*.5f,0),         new Vector3(t,t,s.z));
        E(c+new Vector3( s.x*.5f, s.y*.5f,0),         new Vector3(t,t,s.z));
        // Bottom ring
        E(c+new Vector3(0,        -s.y*.5f, s.z*.5f), new Vector3(s.x,t,t));
        E(c+new Vector3(0,        -s.y*.5f,-s.z*.5f), new Vector3(s.x,t,t));
        E(c+new Vector3(-s.x*.5f, -s.y*.5f,0),        new Vector3(t,t,s.z));
        E(c+new Vector3( s.x*.5f, -s.y*.5f,0),        new Vector3(t,t,s.z));
        // Vertical pillars
        E(c+new Vector3(-s.x*.5f,0, s.z*.5f), new Vector3(t,s.y,t));
        E(c+new Vector3( s.x*.5f,0, s.z*.5f), new Vector3(t,s.y,t));
        E(c+new Vector3(-s.x*.5f,0,-s.z*.5f), new Vector3(t,s.y,t));
        E(c+new Vector3( s.x*.5f,0,-s.z*.5f), new Vector3(t,s.y,t));
    }

    void CreateOutlineEdge(
        Vector3 pos, Vector3 scale, Transform parent)
    {
        GameObject edge =
            GameObject.CreatePrimitive(PrimitiveType.Cube);
        edge.name = "OutlineEdge";
        edge.transform.SetParent(parent);
        edge.transform.position   = pos;
        edge.transform.localScale = scale;

        Collider col = edge.GetComponent<Collider>();
        if (col != null) Destroy(col);

        Material mat = new Material(
            Shader.Find("Universal Render Pipeline/Unlit"));
        mat.color = Color.cyan;
        edge.GetComponent<MeshRenderer>().material = mat;
    }

    void DestroySelectionOutline()
    {
        if (selectionOutline != null)
            Destroy(selectionOutline);
    }

    // ════════════════════════════════════════════
    // HELPERS
    // ════════════════════════════════════════════

    bool GetMouseGridPosition(out Vector3Int gridPos)
    {
        gridPos = Vector3Int.zero;

        Ray   ray   = sceneCamera.ScreenPointToRay(
            Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (!plane.Raycast(ray, out float enter))
            return false;

        Vector3 hit = ray.GetPoint(enter);

        gridPos = new Vector3Int(
            Mathf.FloorToInt(hit.x),
            0,
            Mathf.FloorToInt(hit.z));

        return gridSystem.IsInsideGrid(gridPos);
    }

    // Highest Y layer placed at this XZ column.
    // Returns -1 if the column is empty.
    int GetTopHeight(Vector3Int gridPos)
    {
        int highest = -1;

        foreach (var pair in PlacedTiles)
        {
            Vector3Int p = pair.Key;
            if (p.x == gridPos.x &&
                p.z == gridPos.z &&
                p.y > highest)
                highest = p.y;
        }

        return highest;
    }

    Vector3Int? GetTopTile(Vector3Int gridPos)
    {
        int highest = -1;
        Vector3Int? result = null;

        foreach (var pair in PlacedTiles)
        {
            Vector3Int p = pair.Key;
            if (p.x == gridPos.x &&
                p.z == gridPos.z &&
                p.y > highest)
            {
                highest = p.y;
                result  = p;
            }
        }

        return result;
    }

    int GetPlacementHeight(Vector3Int gridPos)
    {
        int top = GetTopHeight(gridPos);

        if (Input.GetKey(KeyCode.LeftShift))
            return top + 1;

        if (dragPlacementMode)
        {
            if (selectedAsset != null &&
                selectedAsset.placeAtGroundLevel)
                return 0;

            return top >= 0 ? top : 0;
        }

        return top + 1;
    }

    Bounds GetObjectBounds(GameObject target)
    {
        Bounds b = new Bounds(
            target.transform.position, Vector3.zero);

        foreach (Renderer r in
            target.GetComponentsInChildren<Renderer>())
            b.Encapsulate(r.bounds);

        return b;
    }

    Bounds CalculateObjectBounds(GameObject obj)
    {
        Renderer[] renderers =
            obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(
                obj.transform.position, Vector3.one);

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);

        return b;
    }

    void EnsureDefaultScale(GameObject obj)
    {
        if (!defaultObjectScales.ContainsKey(obj))
            defaultObjectScales.Add(
                obj, obj.transform.localScale);
    }

    // Look up which PlaceableAsset a PlacedTile uses
    // (needed when re-registering during drag/drop).
    PlaceableAsset GetAssetForTile(PlacedTile tile)
    {
        // We only need this for structure detection,
        // so a null return is safe for non-structures.
        if (tile == null) return null;

        foreach (var asset in
            FindObjectsOfType<PlaceableAsset>())
        {
            if (asset.assetID == tile.data.assetID)
                return asset;
        }

        return null;
    }

    void HighlightTarget(
        Vector3Int? target, Color color)
    {
        foreach (var pair in PlacedTiles)
        {
            foreach (Renderer r in
                pair.Value.instance
                .GetComponentsInChildren<Renderer>())
            {
                foreach (Material m in r.materials)
                    m.color = Color.white;
            }
        }

        if (target == null) return;
        if (currentTool == EditorTool.Select) return;

        HighlightSingleTile(target.Value, color);
    }

    void HighlightSingleTile(
        Vector3Int pos, Color color)
    {
        if (!PlacedTiles.ContainsKey(pos)) return;

        foreach (Renderer r in
            PlacedTiles[pos].instance
            .GetComponentsInChildren<Renderer>())
        {
            foreach (Material m in r.materials)
                m.color = color;
        }
    }

    void MakeTransparent(GameObject obj, float alpha)
    {
        foreach (Renderer r in
            obj.GetComponentsInChildren<Renderer>())
        {
            Material[] mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                Material m = new Material(mats[i]);
                Color    c = m.color;
                c.a = alpha;
                m.color = c;
                mats[i] = m;
            }
            r.materials = mats;
        }
    }

    void DisableColliders(GameObject obj)
    {
        foreach (Collider c in
            obj.GetComponentsInChildren<Collider>())
            c.enabled = false;
    }

    void SetMaterialColor(
        GameObject obj, Color color)
    {
        MeshRenderer mr =
            obj.GetComponent<MeshRenderer>();
        if (mr == null) return;

        Material mat = new Material(
            Shader.Find(
                "Universal Render Pipeline/Unlit"));
        mat.color   = color;
        mr.material = mat;
    }

    void HidePreview()
    {
        if (previewObject != null)
            previewObject.SetActive(false);
    }
}