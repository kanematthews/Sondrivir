using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuntimeEditorUI : MonoBehaviour
{
    [Header("References")]
    public GridPlacementSystem placementSystem;

    public WorldExporter worldExporter;

    [Header("Assets")]
    public List<PlaceableAsset> assets =
        new List<PlaceableAsset>();

    [Header("UI")]
    public float panelWidth = 360f;

    public float panelPadding = 10f;

    public float categorySpacing = 20f;

    public float buttonSpacing = 8f;

    public Vector2 buttonSize =
        new Vector2(150, 80);

    private RectTransform panelRect;

    private bool paletteExpanded =
        true;

    private Dictionary<string, bool>
        categoryExpanded =
        new Dictionary<string, bool>();

    void Start()
    {
        CreateUI();
    }

    void CreateUI()
    {
        Canvas canvas =
            FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObj =
                new GameObject(
                    "RuntimeCanvas",
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster)
                );

            canvas =
                canvasObj.GetComponent<
                    Canvas>();

            canvas.renderMode =
                RenderMode
                    .ScreenSpaceOverlay;

            CanvasScaler scaler =
                canvasObj.GetComponent<
                    CanvasScaler>();

            scaler.uiScaleMode =
                CanvasScaler
                    .ScaleMode
                    .ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(
                    1920,
                    1080
                );

            scaler.matchWidthOrHeight =
                0.5f;
        }

        CreatePalette(
            canvas.transform
        );
    }

    void CreatePalette(
        Transform parent)
    {
        GameObject panel =
            CreateUIObject(
                "PalettePanel",
                parent
            );

        panelRect =
            panel.GetComponent<
                RectTransform>();

        panelRect.anchorMin =
            new Vector2(1, 0);

        panelRect.anchorMax =
            new Vector2(1, 1);

        panelRect.pivot =
            new Vector2(1, 1);

        panelRect.sizeDelta =
            new Vector2(
                panelWidth,
                0
            );

        panelRect.anchoredPosition =
            new Vector2(
                -10,
                0
            );

        Image bg =
            panel.AddComponent<
                Image>();

        bg.color =
            new Color(
                0.08f,
                0.08f,
                0.08f,
                0.95f
            );

        CreateCollapseButton(
            panel.transform
        );

        float currentY =
            -50f;

        BuildPalette(
            panel.transform,
            ref currentY
        );
    }

    void BuildPalette(
        Transform parent,
        ref float currentY)
    {
        Dictionary<
            string,
            List<PlaceableAsset>
        > categories =
            new Dictionary<
                string,
                List<PlaceableAsset>
            >();

        foreach (
            PlaceableAsset asset
            in assets
        )
        {
            if (
                !categories
                .ContainsKey(
                    asset.category
                )
            )
            {
                categories.Add(
                    asset.category,
                    new List<
                        PlaceableAsset>()
                );

                if (
                    !categoryExpanded
                    .ContainsKey(
                        asset.category
                    )
                )
                {
                    categoryExpanded.Add(
                        asset.category,
                        true
                    );
                }
            }

            categories[
                asset.category
            ].Add(asset);
        }

        foreach (
            var category
            in categories
        )
        {
            CreateCategoryHeader(
                parent,
                category.Key,
                ref currentY
            );

            if (
                !categoryExpanded[
                    category.Key
                ]
            )
            {
                currentY -=
                    categorySpacing;

                continue;
            }

            int column = 0;

            int row = 0;

            foreach (
                PlaceableAsset asset
                in category.Value
            )
            {
                CreateAssetButton(
                    parent,
                    asset,
                    column,
                    row,
                    currentY
                );

                column++;

                if (column > 1)
                {
                    column = 0;
                    row++;
                }
            }

            currentY -=
                ((row + 1)
                * (buttonSize.y
                + buttonSpacing));

            currentY -=
                categorySpacing;
        }
    }

    void CreateCollapseButton(
        Transform parent)
    {
        GameObject buttonObj =
            CreateUIObject(
                "CollapseButton",
                parent
            );

        RectTransform rect =
            buttonObj.GetComponent<
                RectTransform>();

        rect.anchorMin =
            new Vector2(0, 1);

        rect.anchorMax =
            new Vector2(0, 1);

        rect.pivot =
            new Vector2(0, 1);

        rect.sizeDelta =
            new Vector2(30, 30);

        rect.anchoredPosition =
            new Vector2(5, -5);

        Image bg =
            buttonObj.AddComponent<
                Image>();

        bg.color =
            new Color(
                0.2f,
                0.2f,
                0.2f,
                1f
            );

        Button button =
            buttonObj.AddComponent<
                Button>();

        TMP_Text text =
            CreateText(
                buttonObj.transform,
                ">",
                22
            );

        text.alignment =
            TextAlignmentOptions
                .Center;

        RectTransform textRect =
            text.GetComponent<
                RectTransform>();

        textRect.anchorMin =
            Vector2.zero;

        textRect.anchorMax =
            Vector2.one;

        textRect.offsetMin =
            Vector2.zero;

        textRect.offsetMax =
            Vector2.zero;

        button.onClick
            .AddListener(
                TogglePalette
            );
    }

    void CreateCategoryHeader(
        Transform parent,
        string categoryName,
        ref float currentY)
    {
        GameObject header =
            CreateUIObject(
                categoryName + "_Header",
                parent
            );

        RectTransform rect =
            header.GetComponent<
                RectTransform>();

        rect.anchorMin =
            new Vector2(0, 1);

        rect.anchorMax =
            new Vector2(1, 1);

        rect.pivot =
            new Vector2(
                0.5f,
                1
            );

        rect.sizeDelta =
            new Vector2(
                -20,
                40
            );

        rect.anchoredPosition =
            new Vector2(
                0,
                currentY
            );

        Image bg =
            header.AddComponent<
                Image>();

        bg.color =
            new Color(
                0.16f,
                0.16f,
                0.16f,
                1f
            );

        TMP_Text text =
            CreateText(
                header.transform,
                categoryName,
                24
            );

        text.alignment =
            TextAlignmentOptions
                .MidlineLeft;

        RectTransform textRect =
            text.GetComponent<
                RectTransform>();

        textRect.anchorMin =
            Vector2.zero;

        textRect.anchorMax =
            Vector2.one;

        textRect.offsetMin =
            new Vector2(
                10,
                0
            );

        textRect.offsetMax =
            Vector2.zero;

        currentY -= 50f;
    }

    void CreateAssetButton(
        Transform parent,
        PlaceableAsset asset,
        int column,
        int row,
        float startY)
    {
        GameObject buttonObj =
            CreateUIObject(
                asset.displayName,
                parent
            );

        RectTransform rect =
            buttonObj.GetComponent<
                RectTransform>();

        rect.anchorMin =
            new Vector2(0, 1);

        rect.anchorMax =
            new Vector2(0, 1);

        rect.pivot =
            new Vector2(0, 1);

        float x =
            panelPadding +
            (column
            * (buttonSize.x
            + buttonSpacing));

        float y =
            startY -
            (row
            * (buttonSize.y
            + buttonSpacing));

        rect.anchoredPosition =
            new Vector2(
                x,
                y
            );

        rect.sizeDelta =
            buttonSize;

        Image bg =
            buttonObj.AddComponent<
                Image>();

        bg.color =
            new Color(
                0.2f,
                0.2f,
                0.2f,
                1f
            );

        Button button =
            buttonObj.AddComponent<
                Button>();

        GameObject preview =
            CreateUIObject(
                "Preview",
                buttonObj.transform
            );

        RectTransform previewRect =
            preview.GetComponent<
                RectTransform>();

        previewRect.anchorMin =
            new Vector2(0, 1);

        previewRect.anchorMax =
            new Vector2(0, 1);

        previewRect.pivot =
            new Vector2(0, 1);

        previewRect.sizeDelta =
            new Vector2(64, 64);

        previewRect.anchoredPosition =
            new Vector2(8, -8);

        RawImage image =
            preview.AddComponent<
                RawImage>();

        image.texture =
            asset.previewTexture != null
            ? asset.previewTexture
            : Texture2D.grayTexture;

        image.color =
            Color.white;

        TMP_Text text =
            CreateText(
                buttonObj.transform,
                asset.displayName,
                18
            );

        text.alignment =
            TextAlignmentOptions
                .Bottom;

        RectTransform textRect =
            text.GetComponent<
                RectTransform>();

        textRect.anchorMin =
            new Vector2(0, 0);

        textRect.anchorMax =
            new Vector2(1, 0);

        textRect.pivot =
            new Vector2(
                0.5f,
                0
            );

        textRect.sizeDelta =
            new Vector2(
                -10,
                22
            );

        textRect.anchoredPosition =
            new Vector2(0, 4);

        button.onClick
            .AddListener(() =>
        {
            placementSystem.selectedAsset =
                asset;
        });
    }

    TMP_Text CreateText(
        Transform parent,
        string value,
        int size)
    {
        GameObject obj =
            CreateUIObject(
                "Text",
                parent
            );

        TMP_Text text =
            obj.AddComponent<
                TextMeshProUGUI>();

        text.text =
            value;

        text.fontSize =
            size;

        text.color =
            Color.white;

        text.enableWordWrapping =
            false;

        text.overflowMode =
            TextOverflowModes
                .Ellipsis;

        return text;
    }

    GameObject CreateUIObject(
        string name,
        Transform parent)
    {
        GameObject obj =
            new GameObject(
                name,
                typeof(RectTransform)
            );

        obj.transform
            .SetParent(
                parent,
                false
            );

        return obj;
    }

    void TogglePalette()
    {
        paletteExpanded =
            !paletteExpanded;

        panelRect.sizeDelta =
            paletteExpanded
            ? new Vector2(
                panelWidth,
                0
            )
            : new Vector2(
                40,
                0
            );
    }
}