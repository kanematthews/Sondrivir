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
    public PlaceableAsset selectedAsset;

    public Transform placedObjectsParent;

    public GridSystem gridSystem;

    [Header("Tool Settings")]
    public EditorTool currentTool =
        EditorTool.Add;

    public bool enableDragPlacement = true;

    private GameObject previewObject;

    private GameObject highlightedObject;

    private Material originalMaterial;

    private Vector3Int currentGridPosition;

    private Dictionary<Vector3Int, GameObject> placedObjects
        = new Dictionary<Vector3Int, GameObject>();

    private bool isDragging;

    private int dragLayerY;

    void Start()
    {
        CreatePreviewObject();
    }

    void Update()
    {
        if (!gridSystem.GridGenerated)
        {
            if (previewObject != null)
            {
                previewObject.SetActive(false);
            }

            return;
        }

        HandleToolUI();

        HandleDragState();

        if (currentTool == EditorTool.Add)
        {
            UpdatePreview();

            HandlePlacement();

            if (previewObject != null)
            {
                previewObject.SetActive(true);
            }

            ClearHighlight();
        }
        else
        {
            if (previewObject != null)
            {
                previewObject.SetActive(false);
            }

            HandleRemovePreview();

            HandleRemoval();
        }
    }

    void HandleToolUI()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentTool = EditorTool.Add;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentTool = EditorTool.Remove;
        }
    }

    void HandleDragState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;

            dragLayerY =
                currentGridPosition.y;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    #region Preview

    void CreatePreviewObject()
    {
        if (selectedAsset == null)
            return;

        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject =
            Instantiate(selectedAsset.prefab);

        previewObject.name =
            "PlacementPreview";

        Collider[] colliders =
            previewObject.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            Destroy(col);
        }

        Renderer[] renderers =
            previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material previewMaterial =
                new Material(renderer.sharedMaterial);

            SetupTransparentMaterial(previewMaterial);

            Color color = previewMaterial.color;

            color.a = 0.5f;

            previewMaterial.color = color;

            renderer.material = previewMaterial;
        }
    }

    void SetupTransparentMaterial(Material material)
    {
        material.SetFloat("_Surface", 1);

        material.SetInt(
            "_SrcBlend",
            (int)UnityEngine.Rendering.BlendMode.SrcAlpha
        );

        material.SetInt(
            "_DstBlend",
            (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha
        );

        material.SetInt("_ZWrite", 0);

        material.EnableKeyword(
            "_SURFACE_TYPE_TRANSPARENT"
        );

        material.renderQueue = 3000;
    }

    void UpdatePreview()
    {
        if (previewObject == null)
        {
            CreatePreviewObject();
            return;
        }

        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition
            );

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3Int gridPosition;

            if (hit.collider.gameObject.name ==
                "PlacementSurface")
            {
                gridPosition =
                    GetGridPosition(hit.point);
            }
            else
            {
                Vector3 hitPoint =
                    hit.collider.transform.position;

                Vector3 normal =
                    hit.normal;

                if (normal == Vector3.up &&
                    !isDragging)
                {
                    hitPoint += Vector3.up;
                }

                gridPosition =
                    GetGridPosition(hitPoint);
            }

            if (isDragging)
            {
                gridPosition.y =
                    dragLayerY;
            }

            if (!gridSystem.IsInsideGrid(gridPosition))
            {
                previewObject.SetActive(false);
                return;
            }

            previewObject.SetActive(true);

            currentGridPosition =
                gridPosition;

            previewObject.transform.position =
                GetWorldPosition(gridPosition);
        }
    }

    #endregion

    #region Placement

    void HandlePlacement()
    {
        if (enableDragPlacement)
        {
            if (!Input.GetMouseButton(0))
                return;
        }
        else
        {
            if (!Input.GetMouseButtonDown(0))
                return;
        }

        if (placedObjects.ContainsKey(
            currentGridPosition))
        {
            return;
        }

        GameObject spawnedObject =
            Instantiate(
                selectedAsset.prefab,
                GetWorldPosition(currentGridPosition),
                Quaternion.identity,
                placedObjectsParent
            );

        placedObjects.Add(
            currentGridPosition,
            spawnedObject
        );
    }

    #endregion

    #region Remove Tool

    void HandleRemovePreview()
    {
        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition
            );

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject =
                hit.collider.gameObject;

            if (hitObject.name ==
                "PlacementSurface")
            {
                ClearHighlight();
                return;
            }

            if (highlightedObject != hitObject)
            {
                ClearHighlight();

                highlightedObject = hitObject;

                Renderer renderer =
                    highlightedObject.GetComponent<Renderer>();

                if (renderer != null)
                {
                    originalMaterial =
                        renderer.material;

                    Material highlightMat =
                        new Material(renderer.material);

                    highlightMat.color =
                        Color.red;

                    renderer.material =
                        highlightMat;
                }
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    void ClearHighlight()
    {
        if (highlightedObject != null)
        {
            Renderer renderer =
                highlightedObject.GetComponent<Renderer>();

            if (renderer != null &&
                originalMaterial != null)
            {
                renderer.material =
                    originalMaterial;
            }

            highlightedObject = null;
        }
    }

    void HandleRemoval()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition
            );

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject =
                hit.collider.gameObject;

            Vector3Int gridPosition =
                GetGridPosition(
                    hitObject.transform.position
                );

            if (placedObjects.TryGetValue(
                gridPosition,
                out GameObject placedObject))
            {
                Destroy(placedObject);

                placedObjects.Remove(gridPosition);
            }
        }
    }

    #endregion

    Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        return new Vector3Int(
            Mathf.FloorToInt(worldPosition.x),
            Mathf.RoundToInt(worldPosition.y),
            Mathf.FloorToInt(worldPosition.z)
        );
    }

    Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        return new Vector3(
            gridPosition.x + 0.5f,
            gridPosition.y,
            gridPosition.z + 0.5f
        );
    }

    void OnGUI()
    {
        GUI.Box(
            new Rect(10, 210, 180, 100),
            "Tools"
        );

        GUI.color =
            currentTool == EditorTool.Add
            ? Color.green
            : Color.white;

        if (GUI.Button(
            new Rect(20, 240, 140, 25),
            "Add Tool"
        ))
        {
            currentTool = EditorTool.Add;
        }

        GUI.color =
            currentTool == EditorTool.Remove
            ? Color.red
            : Color.white;

        if (GUI.Button(
            new Rect(20, 270, 140, 25),
            "Remove Tool"
        ))
        {
            currentTool = EditorTool.Remove;
        }

        GUI.color = Color.white;
    }
}