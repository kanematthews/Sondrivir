using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuntimeEditorUI : MonoBehaviour
{
    [Header("References")]
    public GridSystem gridSystem;

    public GridPlacementSystem placementSystem;

    public WorldExporter worldExporter;

    [Header("Chunk Controls")]
    public TMP_InputField chunkXInput;

    public TMP_InputField chunkZInput;

    public Button generateButton;

    [Header("Tool Buttons")]
    public Button addToolButton;

    public Button removeToolButton;

    [Header("Export")]
    public TMP_InputField exportNameInput;

    public Button exportButton;

    [Header("Palette")]
    public RectTransform palettePanel;

    public Button collapseButton;

    public Transform categoryContainer;

    public GameObject categoryPrefab;

    public GameObject assetButtonPrefab;

    [Header("Assets")]
    public List<PlaceableAsset> assets =
        new List<PlaceableAsset>();

    private bool paletteExpanded = true;

    void Start()
    {
        SetupButtons();

        BuildPalette();
    }

    void SetupButtons()
    {
        generateButton.onClick.RemoveAllListeners();

        generateButton.onClick.AddListener(
            GenerateGrid
        );

        exportButton.onClick.RemoveAllListeners();

        exportButton.onClick.AddListener(
            ExportWorld
        );

        addToolButton.onClick.RemoveAllListeners();

        addToolButton.onClick.AddListener(() =>
        {
            placementSystem.currentTool =
                GridPlacementSystem.EditorTool.Add;
        });

        removeToolButton.onClick.RemoveAllListeners();

        removeToolButton.onClick.AddListener(() =>
        {
            placementSystem.currentTool =
                GridPlacementSystem.EditorTool.Remove;
        });

        collapseButton.onClick.RemoveAllListeners();

        collapseButton.onClick.AddListener(
            TogglePalette
        );
    }

    void GenerateGrid()
    {
        int x = 0;
        int z = 0;

        int.TryParse(
            chunkXInput.text,
            out x
        );

        int.TryParse(
            chunkZInput.text,
            out z
        );

        gridSystem.chunkCountX =
            Mathf.Max(0, x);

        gridSystem.chunkCountZ =
            Mathf.Max(0, z);

        gridSystem.GenerateGrid();
    }

    void ExportWorld()
    {
        worldExporter.exportFileName =
            exportNameInput.text;

        worldExporter.ExportWorld();
    }

    void TogglePalette()
    {
        paletteExpanded =
            !paletteExpanded;

        palettePanel.sizeDelta =
            paletteExpanded
            ? new Vector2(320, 0)
            : new Vector2(40, 0);
    }

    void BuildPalette()
    {
        foreach (Transform child
            in categoryContainer)
        {
            Destroy(child.gameObject);
        }

        Dictionary<
            string,
            List<PlaceableAsset>
        > categories =
            new Dictionary<
                string,
                List<PlaceableAsset>
            >();

        foreach (PlaceableAsset asset
            in assets)
        {
            if (!categories.ContainsKey(
                asset.category))
            {
                categories.Add(
                    asset.category,
                    new List<PlaceableAsset>()
                );
            }

            categories[
                asset.category
            ].Add(asset);
        }

        foreach (var category
            in categories)
        {
            CreateCategory(
                category.Key,
                category.Value
            );
        }
    }

    void CreateCategory(
        string categoryName,
        List<PlaceableAsset> categoryAssets)
    {
        GameObject categoryObject =
            Instantiate(
                categoryPrefab,
                categoryContainer
            );

        CategoryUI categoryUI =
            categoryObject
            .GetComponent<CategoryUI>();

        categoryUI.Setup(
            categoryName
        );

        foreach (PlaceableAsset asset
            in categoryAssets)
        {
            CreateAssetButton(
                asset,
                categoryUI.contentRoot
            );
        }
    }

    void CreateAssetButton(
        PlaceableAsset asset,
        Transform parent)
    {
        GameObject buttonObject =
            Instantiate(
                assetButtonPrefab,
                parent
            );

        AssetButtonUI buttonUI =
            buttonObject
            .GetComponent<AssetButtonUI>();

        buttonUI.Setup(
            asset,
            placementSystem
        );
    }

    public void RefreshPalette()
    {
        BuildPalette();
    }
}