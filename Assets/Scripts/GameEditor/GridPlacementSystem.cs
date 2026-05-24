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

    // Which axis a resize handle controls.
    public enum ResizeHandle
    {
        PosX,   // +X arrow  (right face)
        NegX,   // -X arrow  (left face)
        PosZ,   // +Z arrow  (forward face)
        NegZ,   // -Z arrow  (back face)
        PosY,   // +Y corner (scale up uniform)
        NegY    // -Y corner (scale down uniform)
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

    private PlacedTile selectedTile;

    private Vector3Int selectedTilePosition;

    private bool isDraggingSelected;

    private GameObject movePreviewObject;

    private GameObject selectionOutline;

    // ── Resize gizmo state ──────────────────────
    // The tile the gizmo is currently attached to.
    private Vector3Int? resizeTargetTile = null;

    // Root GameObject that holds all handle arrows.
    private GameObject resizeGizmoRoot = null;

    // Which handle is being dragged right now.
    private ResizeHandle? activeHandle = null;

    // Mouse X position when drag started,
    // used to compute delta.
    private float dragStartMouseX;

    // Scale of the object when drag started.
    private Vector3 dragStartScale;

    // Original (first-seen) scale per object,
    // used as the "default" reference for the
    // snap-to-default-once logic.
    private Dictionary<GameObject, Vector3>
        defaultObjectScales =
        new Dictionary<GameObject, Vector3>();
    // ────────────────────────────────────────────

    void Update()
    {
        if (!gridSystem.GridGenerated)
        {
            HidePreview();
            return;
        }

        dragPlacementMode =
            Input.GetMouseButton(0);

        UpdatePreviewAsset();

        UpdatePreviewPosition();

        // Clear selection outline when not in
        // Select or Remove tool.
        if (
            currentTool != EditorTool.Select &&
            currentTool != EditorTool.Remove
        )
        {
            DestroySelectionOutline();
        }

        HandlePlacement();

        HandleFill();

        HandleSelection();

        HandleResize();

        HandleRemove();
    }

    // ── Preview ─────────────────────────────────

    void UpdatePreviewAsset()
    {
        if (selectedAsset == lastAsset)
            return;

        lastAsset = selectedAsset;

        CreatePreviewObject();
    }

    Quaternion GetPlacementRotation()
    {
        if (selectedAsset == null)
            return Quaternion.identity;

        Quaternion rotation =
            Quaternion.Euler(
                selectedAsset.placementRotation
            );

        if (selectedAsset.isQuad)
        {
            rotation *=
                Quaternion.Euler(90f, 0f, 0f);
        }

        return rotation;
    }

    void CreatePreviewObject()
    {
        if (previewObject != null)
            Destroy(previewObject);

        if (selectedAsset == null)
            return;

        previewObject =
            Instantiate(selectedAsset.prefab);

        previewObject.name = "PlacementPreview";

        previewObject.transform.rotation =
            GetPlacementRotation();

        SetTransparent(previewObject, 0.5f);

        DisableColliders(previewObject);
    }

    void UpdatePreviewPosition()
    {
        if (previewObject == null)
            return;

        if (
            currentTool == EditorTool.Remove ||
            currentTool == EditorTool.Select ||
            currentTool == EditorTool.Resize
        )
        {
            previewObject.SetActive(false);
            return;
        }

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
            )
            + selectedAsset.placementOffset;
    }

    // ── Placement ────────────────────────────────

    void HandlePlacement()
    {
        if (currentTool != EditorTool.Add)
            return;

        if (selectedAsset == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            TryPlace(false);
        }
        else if (Input.GetMouseButton(0))
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

        int topHeight = GetTopHeight(gridPos);

        int targetHeight;

        if (dragMode)
        {
            if (selectedAsset.placeAtGroundLevel)
                targetHeight = 0;
            else if (topHeight >= 0)
                targetHeight = topHeight;
            else
                targetHeight = 0;
        }
        else
        {
            if (selectedAsset.placeAtGroundLevel)
                targetHeight = 0;
            else
                targetHeight = topHeight + 1;
        }

        Vector3Int finalPos =
            new Vector3Int(
                gridPos.x,
                targetHeight,
                gridPos.z
            );

        if (finalPos == lastPlacedPosition)
            return;

        lastPlacedPosition = finalPos;

        if (PlacedTiles.ContainsKey(finalPos))
            return;

        GameObject obj =
            Instantiate(
                selectedAsset.prefab,
                new Vector3(
                    finalPos.x + 0.5f,
                    finalPos.y,
                    finalPos.z + 0.5f
                )
                + selectedAsset.placementOffset,
                GetPlacementRotation()
            );

        obj.name = selectedAsset.displayName;

        PlacedObjectData data =
            new PlacedObjectData();

        data.assetID = selectedAsset.assetID;
        data.x = finalPos.x;
        data.y = finalPos.y;
        data.z = finalPos.z;
        data.rotationY = 0f;

        PlacedTiles.Add(
            finalPos,
            new PlacedTile(obj, data)
        );
    }

    // ── Fill ─────────────────────────────────────

    void HandleFill()
    {
        if (currentTool != EditorTool.Fill)
            return;

        if (selectedAsset == null)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (!GetMouseGridPosition(
            out Vector3Int startPos))
            return;

        int targetHeight = GetTopHeight(startPos);

        Queue<Vector3Int> queue =
            new Queue<Vector3Int>();

        HashSet<Vector3Int> visited =
            new HashSet<Vector3Int>();

        queue.Enqueue(startPos);

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            if (visited.Contains(current))
                continue;

            visited.Add(current);

            if (!gridSystem.IsInsideGrid(current))
                continue;

            int currentHeight =
                GetTopHeight(current);

            if (currentHeight != targetHeight)
                continue;

            int placeHeight =
                selectedAsset.placeAtGroundLevel
                ? 0
                : currentHeight + 1;

            Vector3Int finalPos =
                new Vector3Int(
                    current.x,
                    placeHeight,
                    current.z
                );

            if (!PlacedTiles.ContainsKey(finalPos))
            {
                GameObject obj =
                    Instantiate(
                        selectedAsset.prefab,
                        new Vector3(
                            finalPos.x + 0.5f,
                            finalPos.y,
                            finalPos.z + 0.5f
                        )
                        + selectedAsset
                            .placementOffset,
                        GetPlacementRotation()
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
                data.rotationY = 0f;

                PlacedTiles.Add(
                    finalPos,
                    new PlacedTile(obj, data)
                );
            }

            queue.Enqueue(
                current + Vector3Int.right);
            queue.Enqueue(
                current + Vector3Int.left);
            queue.Enqueue(
                current + new Vector3Int(0, 0, 1));
            queue.Enqueue(
                current + new Vector3Int(0, 0, -1));
        }
    }

    // ── Selection / Move ─────────────────────────

    void HandleSelection()
    {
        if (currentTool != EditorTool.Select)
            return;

        if (!GetMouseGridPosition(
            out Vector3Int gridPos))
            return;

        Vector3Int? topTile =
            GetTopTile(gridPos);

        HighlightTarget(topTile, Color.cyan);

        if (Input.GetMouseButtonDown(0))
        {
            if (topTile == null)
                return;

            selectedTilePosition = topTile.Value;
            selectedTile =
                PlacedTiles[topTile.Value];

            CreateSelectionOutline(
                selectedTile.instance);

            CreateMovePreview();

            PlacedTiles.Remove(
                selectedTilePosition);

            selectedTile.instance.SetActive(false);

            isDraggingSelected = true;
        }

        if (isDraggingSelected &&
            movePreviewObject != null)
        {
            int topHeight = GetTopHeight(gridPos);
            int targetHeight =
                topHeight < 0 ? 0 : topHeight + 1;

            movePreviewObject.transform.position =
                new Vector3(
                    gridPos.x + 0.5f,
                    targetHeight,
                    gridPos.z + 0.5f
                );
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (
                selectedTile == null ||
                !isDraggingSelected
            )
                return;

            int topHeight = GetTopHeight(gridPos);
            int targetHeight =
                topHeight < 0 ? 0 : topHeight + 1;

            Vector3Int newPos =
                new Vector3Int(
                    gridPos.x,
                    targetHeight,
                    gridPos.z
                );

            selectedTile.instance.SetActive(true);

            selectedTile.instance
                .transform.position =
                new Vector3(
                    newPos.x + 0.5f,
                    newPos.y,
                    newPos.z + 0.5f
                );

            selectedTile.data.x = newPos.x;
            selectedTile.data.y = newPos.y;
            selectedTile.data.z = newPos.z;

            PlacedTiles.Add(newPos, selectedTile);

            isDraggingSelected = false;

            Destroy(movePreviewObject);

            CreateSelectionOutline(
                selectedTile.instance);
        }
    }

    void CreateMovePreview()
    {
        if (movePreviewObject != null)
            Destroy(movePreviewObject);

        if (selectedTile == null)
            return;

        movePreviewObject =
            Instantiate(selectedTile.instance);

        SetTransparent(movePreviewObject, 0.5f);
        DisableColliders(movePreviewObject);
    }

    // ── Remove ───────────────────────────────────

    void HandleRemove()
    {
        if (currentTool != EditorTool.Remove)
            return;

        if (!GetMouseGridPosition(
            out Vector3Int gridPos))
            return;

        Vector3Int? target = GetTopTile(gridPos);

        HighlightTarget(
            target,
            new Color(1f, 0.4f, 0.4f)
        );

        if (!Input.GetMouseButtonDown(0))
            return;

        if (target == null)
            return;

        PlacedTile tile =
            PlacedTiles[target.Value];

        Destroy(tile.instance);
        PlacedTiles.Remove(target.Value);
    }

    // ── Resize gizmo ─────────────────────────────
    //
    // How it works:
    //   1. Every frame in Resize mode we find the
    //      top tile under the cursor and, if it
    //      changed, rebuild the gizmo on that object.
    //   2. On MouseDown we check which handle (if any)
    //      was hit via a Raycast against the handle
    //      colliders. We record the starting mouse X
    //      and the object's starting scale.
    //   3. While dragging we compute horizontal mouse
    //      delta and apply it to the correct axis:
    //        PosX / NegX  → localScale.x
    //        PosZ / NegZ  → localScale.z
    //        PosY         → localScale.x + z (uniform up)
    //        NegY         → localScale.x + z (uniform down)
    //   4. On MouseUp we finish the drag.
    //   5. When the tool changes we destroy the gizmo.

    void HandleResize()
    {
        if (currentTool != EditorTool.Resize)
        {
            DestroyResizeGizmo();
            resizeTargetTile = null;
            activeHandle = null;
            return;
        }

        // ── Handle active drag ───────────────────
        if (activeHandle.HasValue)
        {
            ContinueResizeDrag();

            if (Input.GetMouseButtonUp(0))
            {
                activeHandle = null;
            }

            return; // don't re-pick while dragging
        }

        // ── Pick tile under cursor ───────────────
        if (!GetMouseGridPosition(
            out Vector3Int gridPos))
        {
            // Keep existing gizmo if cursor just
            // left the grid briefly.
            return;
        }

        Vector3Int? topTile =
            GetTopTile(gridPos);

        // Rebuild gizmo only when the hovered
        // tile changes.
        if (topTile != resizeTargetTile)
        {
            resizeTargetTile = topTile;

            if (topTile.HasValue)
            {
                PlacedTile tile =
                    PlacedTiles[topTile.Value];

                EnsureDefaultScale(tile.instance);
                BuildResizeGizmo(tile.instance);
            }
            else
            {
                DestroyResizeGizmo();
            }
        }

        HighlightTarget(topTile, Color.yellow);

        // ── Start drag on mouse down ─────────────
        if (Input.GetMouseButtonDown(0))
        {
            TryBeginResizeDrag();
        }
    }

    void TryBeginResizeDrag()
    {
        if (resizeGizmoRoot == null)
            return;

        Ray ray = sceneCamera.ScreenPointToRay(
            Input.mousePosition);

        // Check all handle children for a hit.
        foreach (Transform child
            in resizeGizmoRoot.transform)
        {
            Collider col =
                child.GetComponent<Collider>();

            if (col == null)
                continue;

            if (!col.Raycast(ray,
                out RaycastHit hit, 100f))
                continue;

            // Found the clicked handle.
            string n = child.name;

            if (!System.Enum.TryParse(
                n, out ResizeHandle h))
                continue;

            activeHandle = h;

            dragStartMouseX =
                Input.mousePosition.x;

            if (
                resizeTargetTile.HasValue &&
                PlacedTiles.ContainsKey(
                    resizeTargetTile.Value)
            )
            {
                dragStartScale =
                    PlacedTiles[
                        resizeTargetTile.Value
                    ].instance
                    .transform.localScale;
            }

            break;
        }
    }

    void ContinueResizeDrag()
    {
        if (!activeHandle.HasValue)
            return;

        if (!resizeTargetTile.HasValue)
            return;

        if (!PlacedTiles.ContainsKey(
            resizeTargetTile.Value))
            return;

        GameObject obj =
            PlacedTiles[
                resizeTargetTile.Value
            ].instance;

        // Pixels dragged → world units (1 unit
        // per 100 px feels natural at typical zoom).
        float delta =
            (Input.mousePosition.x -
             dragStartMouseX) / 100f;

        Vector3 scale = dragStartScale;

        float minSize = 0.1f;

        switch (activeHandle.Value)
        {
            case ResizeHandle.PosX:
            case ResizeHandle.NegX:
                scale.x =
                    Mathf.Max(
                        minSize,
                        dragStartScale.x + delta
                    );
                break;

            case ResizeHandle.PosZ:
            case ResizeHandle.NegZ:
                scale.z =
                    Mathf.Max(
                        minSize,
                        dragStartScale.z + delta
                    );
                break;

            case ResizeHandle.PosY:
                // Diagonal up → uniform scale up.
                float growUniform =
                    Mathf.Max(0f, delta);

                scale.x =
                    Mathf.Max(
                        minSize,
                        dragStartScale.x +
                        growUniform
                    );

                scale.z =
                    Mathf.Max(
                        minSize,
                        dragStartScale.z +
                        growUniform
                    );

                scale.y =
                    Mathf.Max(
                        minSize,
                        dragStartScale.y +
                        growUniform
                    );
                break;

            case ResizeHandle.NegY:
                // Diagonal down → uniform scale
                // down (delta is negative when
                // dragging left).
                float shrinkUniform =
                    Mathf.Min(0f, delta);

                scale.x =
                    Mathf.Max(
                        minSize,
                        dragStartScale.x +
                        shrinkUniform
                    );

                scale.z =
                    Mathf.Max(
                        minSize,
                        dragStartScale.z +
                        shrinkUniform
                    );

                scale.y =
                    Mathf.Max(
                        minSize,
                        dragStartScale.y +
                        shrinkUniform
                    );
                break;
        }

        obj.transform.localScale = scale;

        // Reposition gizmo to follow the
        // object's new bounds.
        BuildResizeGizmo(obj);
    }

    // Builds (or rebuilds) the gizmo arrows
    // around the given object.
    void BuildResizeGizmo(GameObject target)
    {
        DestroyResizeGizmo();

        resizeGizmoRoot =
            new GameObject("ResizeGizmo");

        Bounds bounds = GetObjectBounds(target);

        Vector3 c = bounds.center;
        Vector3 s = bounds.size;

        // Arrow colours matching Unity conventions:
        //   X → red   Z → blue   Y/uniform → white
        float arrowLen = 0.6f;
        float arrowRadius = 0.08f;

        // +X (right)
        CreateHandleArrow(
            ResizeHandle.PosX,
            c + new Vector3(s.x * 0.5f, 0f, 0f),
            Vector3.right,
            Color.red,
            arrowLen,
            arrowRadius
        );

        // -X (left)
        CreateHandleArrow(
            ResizeHandle.NegX,
            c + new Vector3(-s.x * 0.5f, 0f, 0f),
            Vector3.left,
            Color.red,
            arrowLen,
            arrowRadius
        );

        // +Z (forward)
        CreateHandleArrow(
            ResizeHandle.PosZ,
            c + new Vector3(0f, 0f, s.z * 0.5f),
            Vector3.forward,
            Color.blue,
            arrowLen,
            arrowRadius
        );

        // -Z (back)
        CreateHandleArrow(
            ResizeHandle.NegZ,
            c + new Vector3(0f, 0f, -s.z * 0.5f),
            Vector3.back,
            Color.blue,
            arrowLen,
            arrowRadius
        );

        // +Y diagonal corner (uniform grow) —
        // placed at top-right corner, angled up.
        CreateHandleArrow(
            ResizeHandle.PosY,
            c + new Vector3(
                s.x * 0.5f,
                s.y * 0.5f,
                s.z * 0.5f
            ),
            new Vector3(1f, 1f, 1f).normalized,
            Color.white,
            arrowLen,
            arrowRadius
        );

        // -Y diagonal corner (uniform shrink) —
        // placed at bottom-left corner, angled down.
        CreateHandleArrow(
            ResizeHandle.NegY,
            c + new Vector3(
                -s.x * 0.5f,
                -s.y * 0.5f,
                -s.z * 0.5f
            ),
            new Vector3(-1f, -1f, -1f).normalized,
            Color.white,
            arrowLen,
            arrowRadius
        );
    }

    // Creates one arrow handle: a thin cylinder
    // shaft + a cone tip, with a trigger collider
    // sized to be easy to click.
    void CreateHandleArrow(
        ResizeHandle handle,
        Vector3 origin,
        Vector3 direction,
        Color color,
        float length,
        float radius)
    {
        GameObject root =
            new GameObject(handle.ToString());

        root.transform.SetParent(
            resizeGizmoRoot.transform, false);

        root.transform.position = origin;

        // Rotation so the cylinder's local Y
        // points along 'direction'.
        root.transform.rotation =
            Quaternion.FromToRotation(
                Vector3.up, direction);

        // ── Shaft ────────────────────────────────
        GameObject shaft =
            GameObject.CreatePrimitive(
                PrimitiveType.Cylinder);

        shaft.name = "Shaft";

        Destroy(shaft.GetComponent<Collider>());

        shaft.transform.SetParent(
            root.transform, false);

        shaft.transform.localPosition =
            new Vector3(0f, length * 0.35f, 0f);

        shaft.transform.localScale =
            new Vector3(
                radius,
                length * 0.35f,
                radius
            );

        SetMaterialColor(shaft, color);

        // ── Cone tip (scaled cube) ────────────────
        GameObject tip =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube);

        tip.name = "Tip";

        Destroy(tip.GetComponent<Collider>());

        tip.transform.SetParent(
            root.transform, false);

        tip.transform.localPosition =
            new Vector3(0f, length * 0.75f, 0f);

        tip.transform.localScale =
            new Vector3(
                radius * 2.5f,
                radius * 2.5f,
                radius * 2.5f
            );

        SetMaterialColor(tip, color);

        // ── Click collider ───────────────────────
        // A single capsule covering shaft + tip,
        // slightly oversized so it's easy to hit.
        CapsuleCollider col =
            root.AddComponent<CapsuleCollider>();

        col.isTrigger = true;
        col.center =
            new Vector3(0f, length * 0.5f, 0f);

        col.height = length;
        col.radius = radius * 2f;
    }

    void DestroyResizeGizmo()
    {
        if (resizeGizmoRoot != null)
        {
            Destroy(resizeGizmoRoot);
            resizeGizmoRoot = null;
        }
    }

    // ── Helpers ──────────────────────────────────

    void EnsureDefaultScale(GameObject obj)
    {
        if (!defaultObjectScales.ContainsKey(obj))
        {
            defaultObjectScales.Add(
                obj,
                obj.transform.localScale
            );
        }
    }

    Bounds GetObjectBounds(GameObject target)
    {
        Bounds bounds =
            new Bounds(
                target.transform.position,
                Vector3.zero
            );

        Renderer[] renderers =
            target
            .GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        return bounds;
    }

    void SetTransparent(
        GameObject obj, float alpha)
    {
        Renderer[] renderers =
            obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] mats = renderer.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                Material m = new Material(mats[i]);
                Color c = m.color;
                c.a = alpha;
                m.color = c;
                mats[i] = m;
            }

            renderer.materials = mats;
        }
    }

    void DisableColliders(GameObject obj)
    {
        Collider[] cols =
            obj.GetComponentsInChildren<Collider>();

        foreach (Collider c in cols)
        {
            c.enabled = false;
        }
    }

    void SetMaterialColor(
        GameObject obj, Color color)
    {
        MeshRenderer mr =
            obj.GetComponent<MeshRenderer>();

        if (mr == null)
            return;

        Material mat =
            new Material(
                Shader.Find(
                    "Universal Render Pipeline/Unlit"
                )
            );

        mat.color = color;
        mr.material = mat;
    }

    int GetPlacementHeight(Vector3Int gridPos)
    {
        int topHeight = GetTopHeight(gridPos);

        if (Input.GetKey(KeyCode.LeftShift))
            return topHeight + 1;

        if (dragPlacementMode)
        {
            if (
                selectedAsset != null &&
                selectedAsset.placeAtGroundLevel
            )
                return 0;

            return topHeight >= 0 ? topHeight : 0;
        }

        return topHeight + 1;
    }

    void HighlightTarget(
        Vector3Int? target,
        Color highlightColor)
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

        if (currentTool == EditorTool.Select)
            return;

        Renderer[] targetRenderers =
            PlacedTiles[target.Value]
            .instance
            .GetComponentsInChildren<Renderer>();

        foreach (Renderer r in targetRenderers)
        {
            foreach (Material m in r.materials)
            {
                m.color = highlightColor;
            }
        }
    }

    void CreateSelectionOutline(
        GameObject target)
    {
        DestroySelectionOutline();

        if (target == null)
            return;

        Bounds bounds = GetObjectBounds(target);

        selectionOutline =
            new GameObject("SelectionOutline");

        Vector3 center = bounds.center;
        Vector3 size = bounds.size;
        float thickness = 0.03f;

        void Edge(Vector3 pos, Vector3 scale)
        {
            CreateOutlineEdge(
                pos, scale,
                selectionOutline.transform);
        }

        // Top edges
        Edge(center + new Vector3(0, size.y * .5f, size.z * .5f),
             new Vector3(size.x, thickness, thickness));
        Edge(center + new Vector3(0, size.y * .5f, -size.z * .5f),
             new Vector3(size.x, thickness, thickness));
        // Bottom edges
        Edge(center + new Vector3(0, -size.y * .5f, size.z * .5f),
             new Vector3(size.x, thickness, thickness));
        Edge(center + new Vector3(0, -size.y * .5f, -size.z * .5f),
             new Vector3(size.x, thickness, thickness));
        // Vertical edges
        Edge(center + new Vector3(-size.x * .5f, 0, size.z * .5f),
             new Vector3(thickness, size.y, thickness));
        Edge(center + new Vector3(-size.x * .5f, 0, -size.z * .5f),
             new Vector3(thickness, size.y, thickness));
        Edge(center + new Vector3(size.x * .5f, 0, size.z * .5f),
             new Vector3(thickness, size.y, thickness));
        Edge(center + new Vector3(size.x * .5f, 0, -size.z * .5f),
             new Vector3(thickness, size.y, thickness));
        // Z-direction edges
        Edge(center + new Vector3(-size.x * .5f, 0, size.z * .5f),
             new Vector3(thickness, thickness, size.z));
        Edge(center + new Vector3(size.x * .5f, 0, size.z * .5f),
             new Vector3(thickness, thickness, size.z));
        Edge(center + new Vector3(-size.x * .5f, 0, -size.z * .5f),
             new Vector3(thickness, thickness, size.z));
        Edge(center + new Vector3(size.x * .5f, 0, -size.z * .5f),
             new Vector3(thickness, thickness, size.z));
    }

    void CreateOutlineEdge(
        Vector3 position,
        Vector3 scale,
        Transform parent)
    {
        GameObject edge =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube);

        edge.name = "OutlineEdge";

        edge.transform.SetParent(parent);

        edge.transform.position = position;

        edge.transform.localScale = scale;

        Collider col =
            edge.GetComponent<Collider>();

        if (col != null)
            Destroy(col);

        SetMaterialColor(edge, Color.cyan);
    }

    void DestroySelectionOutline()
    {
        if (selectionOutline != null)
            Destroy(selectionOutline);
    }

    bool GetMouseGridPosition(
        out Vector3Int gridPos)
    {
        gridPos = Vector3Int.zero;

        Ray ray =
            sceneCamera.ScreenPointToRay(
                Input.mousePosition);

        Plane plane =
            new Plane(Vector3.up, Vector3.zero);

        if (!plane.Raycast(ray, out float enter))
            return false;

        Vector3 hitPoint = ray.GetPoint(enter);

        int x = Mathf.FloorToInt(hitPoint.x);
        int z = Mathf.FloorToInt(hitPoint.z);

        gridPos = new Vector3Int(x, 0, z);

        return gridSystem.IsInsideGrid(gridPos);
    }

    int GetTopHeight(Vector3Int gridPos)
    {
        int highest = -1;

        foreach (var pair in PlacedTiles)
        {
            Vector3Int pos = pair.Key;

            if (pos.x == gridPos.x &&
                pos.z == gridPos.z &&
                pos.y > highest)
            {
                highest = pos.y;
            }
        }

        return highest;
    }

    Vector3Int? GetTopTile(Vector3Int gridPos)
    {
        int highest = -1;
        Vector3Int? result = null;

        foreach (var pair in PlacedTiles)
        {
            Vector3Int pos = pair.Key;

            if (pos.x == gridPos.x &&
                pos.z == gridPos.z &&
                pos.y > highest)
            {
                highest = pos.y;
                result = pos;
            }
        }

        return result;
    }

    void HidePreview()
    {
        if (previewObject != null)
            previewObject.SetActive(false);
    }
}
