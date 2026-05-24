#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;

public class AssetPaletteUI : MonoBehaviour
{
    [Header("References")]
    public GridPlacementSystem placementSystem;

    [Header("Assets")]
    public List<PlaceableAsset> assets =
        new List<PlaceableAsset>();

    [Header("Panel")]
    public bool expanded = true;

    public float panelWidth = 320f;

    private Vector2 scrollPosition;

    private Dictionary<
        string,
        List<PlaceableAsset>
    > categorizedAssets =
        new Dictionary<
            string,
            List<PlaceableAsset>
        >();

    private Dictionary<
        string,
        bool
    > categoryFoldouts =
        new Dictionary<
            string,
            bool
        >();

    void Start()
    {
        BuildCategories();
    }

    void BuildCategories()
    {
        categorizedAssets.Clear();

        foreach (PlaceableAsset asset in assets)
        {
            if (!categorizedAssets.ContainsKey(
                asset.category))
            {
                categorizedAssets.Add(
                    asset.category,
                    new List<PlaceableAsset>()
                );

                categoryFoldouts.Add(
                    asset.category,
                    true
                );
            }

            categorizedAssets[
                asset.category
            ].Add(asset);
        }
    }

    void OnGUI()
    {
        float xPosition =
            expanded
            ? Screen.width - panelWidth
            : Screen.width - 35;

        GUI.Box(
            new Rect(
                xPosition,
                10,
                expanded ? panelWidth : 30,
                Screen.height - 20
            ),
            ""
        );

        if (GUI.Button(
            new Rect(
                xPosition + 3,
                15,
                25,
                25
            ),
            expanded ? ">" : "<"
        ))
        {
            expanded = !expanded;
        }

        if (!expanded)
            return;

        GUI.Label(
            new Rect(
                xPosition + 40,
                18,
                200,
                20
            ),
            "Asset Palette"
        );

        GUILayout.BeginArea(
            new Rect(
                xPosition + 10,
                50,
                panelWidth - 20,
                Screen.height - 70
            )
        );

        scrollPosition =
            GUILayout.BeginScrollView(
                scrollPosition
            );

        foreach (var category
            in categorizedAssets)
        {
            DrawCategory(category.Key);
        }

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    void DrawCategory(string categoryName)
    {
        GUILayout.Space(8);

        GUILayout.BeginHorizontal("box");

        string arrow =
            categoryFoldouts[categoryName]
            ? "▼"
            : "▶";

        if (GUILayout.Button(
            $"{arrow} {categoryName}",
            GUILayout.Height(28)))
        {
            categoryFoldouts[categoryName] =
                !categoryFoldouts[
                    categoryName
                ];
        }

        GUILayout.EndHorizontal();

        if (!categoryFoldouts[categoryName])
            return;

        GUILayout.Space(4);

        foreach (PlaceableAsset asset
            in categorizedAssets[
                categoryName
            ])
        {
            DrawAssetButton(asset);
        }
    }

    void DrawAssetButton(
        PlaceableAsset asset)
    {
        GUILayout.BeginHorizontal(
            "box",
            GUILayout.Height(74)
        );

#if UNITY_EDITOR
        Texture preview =
            AssetPreview.GetAssetPreview(
                asset.prefab
            );
#else
        Texture preview =
            Texture2D.grayTexture;
#endif

        if (preview == null)
        {
            preview = Texture2D.grayTexture;
        }

        if (GUILayout.Button(
            preview,
            GUILayout.Width(64),
            GUILayout.Height(64)))
        {
            placementSystem.selectedAsset =
                asset;
        }

        GUILayout.BeginVertical();

        string displayName =
            asset.displayName;

        if (displayName.Length > 30)
        {
            displayName =
                displayName.Substring(0, 30)
                + "...";
        }

        GUILayout.Label(displayName);

        GUILayout.Space(4);

        GUILayout.Label(
            asset.category
        );

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}