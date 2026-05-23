using System.Collections.Generic;
using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [Header("Selected Asset")]
    public PlaceableAsset selectedAsset;

    [Header("Scene References")]
    public Transform placedObjectsParent;

    private GameObject previewObject;

    private Vector3Int currentGridPosition;

    private Dictionary<Vector3Int, GameObject> placedObjects
        = new Dictionary<Vector3Int, GameObject>();

    void Start()
    {
        CreatePreviewObject();
    }

    void Update()
    {
        UpdatePreview();

        HandlePlacement();

        if (!Input.GetMouseButton(1))
        {
            HandleRemoval();
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

        Collider collider =
            previewObject.GetComponent<Collider>();

        if (collider != null)
        {
            Destroy(collider);
        }

        Renderer[] renderers =
            previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material previewMaterial =
                new Material(renderer.sharedMaterial);

            SetupMaterialTransparency(previewMaterial);

            Color color = previewMaterial.color;
            color.a = 0.5f;

            previewMaterial.color = color;

            renderer.material = previewMaterial;
        }
    }

    void SetupMaterialTransparency(Material material)
    {
        material.SetFloat("_Surface", 1);

        material.SetInt("_SrcBlend",
            (int)UnityEngine.Rendering.BlendMode.SrcAlpha);

        material.SetInt("_DstBlend",
            (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        material.SetInt("_ZWrite", 0);

        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

        material.renderQueue = 3000;
    }

    void UpdatePreview()
    {
        if (previewObject == null)
            return;

        Ray ray =
            Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3Int gridPosition =
                GetGridPosition(hit.point + Vector3.up * 0.5f);

            currentGridPosition = gridPosition;

            previewObject.transform.position =
                gridPosition;
        }
    }

    #endregion

    #region Placement

    void HandlePlacement()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (placedObjects.ContainsKey(currentGridPosition))
            return;

        GameObject spawnedObject =
            Instantiate(
                selectedAsset.prefab,
                currentGridPosition,
                Quaternion.identity,
                placedObjectsParent
            );

        placedObjects.Add(
            currentGridPosition,
            spawnedObject
        );
    }

    #endregion

    #region Removal

    void HandleRemoval()
    {
        if (!Input.GetMouseButtonDown(1))
            return;

        Ray ray =
            Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3Int gridPosition =
                GetGridPosition(
                    hit.collider.transform.position
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
            Mathf.FloorToInt(worldPosition.x + 0.5f),
            Mathf.FloorToInt(worldPosition.y),
            Mathf.FloorToInt(worldPosition.z + 0.5f)
        );
    }
}