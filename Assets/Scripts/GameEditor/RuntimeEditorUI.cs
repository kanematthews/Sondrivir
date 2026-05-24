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

    [Header("Assets")]
    public List<PlaceableAsset> assets =
        new List<PlaceableAsset>();

    private RectTransform palettePanel;

    private bool paletteExpanded = true;

    void Start()
    {
        CreateUI();
    }

    void CreateUI()
    {
        Canvas canvas =
            CreateCanvas();

        CreatePalette(
            canvas.transform
        );
    }

    Canvas CreateCanvas()
    {
        GameObject canvasObj =
            new GameObject(
                "RuntimeCanvas",
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );

        Canvas canvas =
            canvasObj.GetComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

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

        return canvas;
    }

    void CreatePalette(
        Transform parent)
    {
        GameObject panel =
            CreateUIObject(
                "PalettePanel",
                parent
            );

        palettePanel =
            panel.GetComponent<
                RectTransform>();

        Image panelImage =
            panel.AddComponent<Image>();

        panelImage.color =
            new Color(
                0.12f,
                0.12f,
                0.12f,
                0.96f
            );

        RectTransform panelRect =
            palettePanel;

        panelRect.anchorMin =
            new Vector2(1, 0);

        panelRect.anchorMax =
            new Vector2(1, 1);

        panelRect.pivot =
            new Vector2(1, 1);

        panelRect.sizeDelta =
            new Vector2(
                340,
                0
            );

        panelRect.anchoredPosition =
            new Vector2(
                -10,
                0
            );

        Button collapseButton =
            CreateButton(
                panel.transform,
                ">",
                new Vector2(
                    30,
                    30
                ),
                new Vector2(
                    0,
                    1
                ),
                new Vector2(
                    0,
                    1
                ),
                new Vector2(
                    5,
                    -5
                )
            );

        collapseButton
            .onClick
            .AddListener(
                TogglePalette
            );

        GameObject scrollObj =
            CreateUIObject(
                "ScrollView",
                panel.transform
            );

        RectTransform scrollRect =
            scrollObj.GetComponent<
                RectTransform>();

        scrollRect.anchorMin =
            new Vector2(0, 0);

        scrollRect.anchorMax =
            new Vector2(1, 1);

        scrollRect.offsetMin =
            new Vector2(
                10,
                10
            );

        scrollRect.offsetMax =
            new Vector2(
                -10,
                -40
            );

        ScrollRect sr =
            scrollObj.AddComponent<
                ScrollRect>();

        sr.horizontal = false;

        Image scrollBg =
            scrollObj.AddComponent<
                Image>();

        scrollBg.color =
            new Color(
                0.15f,
                0.15f,
                0.15f,
                0.95f
            );

        GameObject viewport =
            CreateUIObject(
                "Viewport",
                scrollObj.transform
            );

        RectTransform viewportRect =
            viewport.GetComponent<
                RectTransform>();

        viewportRect.anchorMin =
            Vector2.zero;

        viewportRect.anchorMax =
            Vector2.one;

        viewportRect.offsetMin =
            Vector2.zero;

        viewportRect.offsetMax =
            Vector2.zero;

        Image viewportImage =
            viewport.AddComponent<
                Image>();

        viewportImage.color =
            new Color(
                1,
                1,
                1,
                0.001f
            );

        Mask mask =
            viewport.AddComponent<
                Mask>();

        mask.showMaskGraphic =
            false;

        GameObject content =
            CreateUIObject(
                "Content",
                viewport.transform
            );

        RectTransform contentRect =
            content.GetComponent<
                RectTransform>();

        contentRect.anchorMin =
            new Vector2(0, 1);

        contentRect.anchorMax =
            new Vector2(1, 1);

        contentRect.pivot =
            new Vector2(
                0.5f,
                1
            );

        contentRect.anchoredPosition =
            Vector2.zero;

        VerticalLayoutGroup layout =
            content.AddComponent<
                VerticalLayoutGroup>();

        layout.spacing = 8;

        layout.padding =
            new RectOffset(
                5,
                5,
                5,
                5
            );

        layout.childAlignment =
            TextAnchor.UpperCenter;

        layout.childControlHeight =
            true;

        layout.childControlWidth =
            true;

        layout.childForceExpandHeight =
            false;

        layout.childForceExpandWidth =
            true;

        ContentSizeFitter fitter =
            content.AddComponent<
                ContentSizeFitter>();

        fitter.verticalFit =
            ContentSizeFitter
                .FitMode
                .PreferredSize;

        sr.viewport =
            viewportRect;

        sr.content =
            contentRect;

        BuildCategories(
            content.transform
        );
    }

    void BuildCategories(
        Transform parent)
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
            CreateCategory(
                parent,
                category.Key,
                category.Value
            );
        }
    }

    void CreateCategory(
        Transform parent,
        string categoryName,
        List<PlaceableAsset>
            categoryAssets)
    {
        GameObject category =
            CreateUIObject(
                categoryName,
                parent
            );

        LayoutElement layout =
            category.AddComponent<
                LayoutElement>();

        layout.preferredHeight =
            40 +
            (categoryAssets.Count * 90);

        VerticalLayoutGroup group =
            category.AddComponent<
                VerticalLayoutGroup>();

        group.spacing = 5;

        group.childControlHeight =
            true;

        group.childControlWidth =
            true;

        group.childForceExpandHeight =
            false;

        group.childForceExpandWidth =
            true;

        bool expanded = true;

        Button header =
            CreateButton(
                category.transform,
                "▼ " + categoryName,
                new Vector2(
                    0,
                    35
                ),
                new Vector2(
                    0,
                    1
                ),
                new Vector2(
                    1,
                    1
                ),
                Vector2.zero
            );

        GameObject content =
            CreateUIObject(
                "Content",
                category.transform
            );

        VerticalLayoutGroup
            contentGroup =
            content.AddComponent<
                VerticalLayoutGroup>();

        contentGroup.spacing =
            5;

        contentGroup
            .childControlHeight =
            true;

        contentGroup
            .childControlWidth =
            true;

        contentGroup
            .childForceExpandHeight =
            false;

        contentGroup
            .childForceExpandWidth =
            true;

        ContentSizeFitter
            contentFit =
            content.AddComponent<
                ContentSizeFitter>();

        contentFit.verticalFit =
            ContentSizeFitter
                .FitMode
                .PreferredSize;

        header.onClick
            .AddListener(() =>
        {
            expanded =
                !expanded;

            content.SetActive(
                expanded
            );

            TMP_Text txt =
                header
                .GetComponentInChildren<
                    TMP_Text>();

            txt.text =
                (expanded
                    ? "▼ "
                    : "▶ ")
                + categoryName;
        });

        foreach (
            PlaceableAsset asset
            in categoryAssets
        )
        {
            CreateAssetButton(
                content.transform,
                asset
            );
        }
    }

    void CreateAssetButton(
        Transform parent,
        PlaceableAsset asset)
    {
        GameObject buttonObj =
            CreateUIObject(
                asset.displayName,
                parent
            );

        RectTransform rect =
            buttonObj.GetComponent<
                RectTransform>();

        rect.sizeDelta =
            new Vector2(
                300,
                80
            );

        LayoutElement layout =
            buttonObj.AddComponent<
                LayoutElement>();

        layout.preferredHeight =
            80;

        layout.minHeight =
            80;

        Image bg =
            buttonObj.AddComponent<
                Image>();

        bg.color =
            new Color(
                0.18f,
                0.18f,
                0.18f,
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
            new Vector2(
                0,
                0.5f
            );

        previewRect.anchorMax =
            new Vector2(
                0,
                0.5f
            );

        previewRect.pivot =
            new Vector2(
                0,
                0.5f
            );

        previewRect.anchoredPosition =
            new Vector2(
                8,
                0
            );

        previewRect.sizeDelta =
            new Vector2(
                64,
                64
            );

        RawImage rawImage =
            preview.AddComponent<
                RawImage>();

        rawImage.color =
            Color.white;

        if (
            asset.previewTexture
            != null
        )
        {
            rawImage.texture =
                asset.previewTexture;
        }
        else
        {
            rawImage.texture =
                Texture2D
                    .grayTexture;
        }

        GameObject nameObj =
            CreateUIObject(
                "NameText",
                buttonObj.transform
            );

        RectTransform nameRect =
            nameObj.GetComponent<
                RectTransform>();

        nameRect.anchorMin =
            new Vector2(
                0,
                0.5f
            );

        nameRect.anchorMax =
            new Vector2(
                0,
                0.5f
            );

        nameRect.pivot =
            new Vector2(
                0,
                0.5f
            );

        nameRect.anchoredPosition =
            new Vector2(
                85,
                12
            );

        nameRect.sizeDelta =
            new Vector2(
                180,
                30
            );

        TMP_Text nameText =
            nameObj.AddComponent<
                TextMeshProUGUI>();

        nameText.text =
            asset.displayName;

        nameText.fontSize =
            22;

        nameText.color =
            Color.white;

        GameObject categoryObj =
            CreateUIObject(
                "CategoryText",
                buttonObj.transform
            );

        RectTransform
            categoryRect =
            categoryObj
            .GetComponent<
                RectTransform>();

        categoryRect.anchorMin =
            new Vector2(
                0,
                0.5f
            );

        categoryRect.anchorMax =
            new Vector2(
                0,
                0.5f
            );

        categoryRect.pivot =
            new Vector2(
                0,
                0.5f
            );

        categoryRect
            .anchoredPosition =
            new Vector2(
                85,
                -15
            );

        categoryRect.sizeDelta =
            new Vector2(
                180,
                25
            );

        TMP_Text categoryText =
            categoryObj
            .AddComponent<
                TextMeshProUGUI>();

        categoryText.text =
            asset.category;

        categoryText.fontSize =
            18;

        categoryText.color =
            Color.gray;

        button.onClick
            .AddListener(() =>
        {
            placementSystem
                .selectedAsset =
                asset;
        });
    }

    TMP_Text CreateTMPText(
        Transform parent,
        string text,
        int size)
    {
        GameObject obj =
            CreateUIObject(
                "Text",
                parent
            );

        TMP_Text tmp =
            obj.AddComponent<
                TextMeshProUGUI>();

        tmp.text = text;

        tmp.fontSize =
            size;

        tmp.color =
            Color.white;

        return tmp;
    }

    Button CreateButton(
        Transform parent,
        string label,
        Vector2 size,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPos)
    {
        GameObject obj =
            CreateUIObject(
                label,
                parent
            );

        Image image =
            obj.AddComponent<
                Image>();

        image.color =
            new Color(
                0.25f,
                0.25f,
                0.25f,
                1f
            );

        Button button =
            obj.AddComponent<
                Button>();

        RectTransform rect =
            obj.GetComponent<
                RectTransform>();

        rect.anchorMin =
            anchorMin;

        rect.anchorMax =
            anchorMax;

        rect.pivot =
            new Vector2(
                0,
                1
            );

        rect.anchoredPosition =
            anchoredPos;

        rect.sizeDelta =
            size;

        TMP_Text text =
            CreateTMPText(
                obj.transform,
                label,
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

        return button;
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

        palettePanel.sizeDelta =
            paletteExpanded
            ? new Vector2(
                340,
                0
            )
            : new Vector2(
                40,
                0
            );
    }
}