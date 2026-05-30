using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// =========================================================
// FIX PANEL VISUALS  —  MMO > Fix Panel Visuals
// =========================================================
// Adds missing components directly to the scene objects
// at edit time so they are saved and always present.
//
// Fixes:
//   1. StatsPanel — adds VLG + ContentSizeFitter + Image
//      so the background auto-sizes to fit stat rows
//   2. InventoryUI — fixes anchor to stretch-fill its
//      parent and ensures a visible dark background
// =========================================================

public static class FixPanelVisuals
{
    [MenuItem("MMO/Fix Panel Visuals")]
    public static void Run()
    {
        int n = 0;

        FixStatsPanel(ref n);
        FixInventoryBackground(ref n);
        FixLootBackground(ref n);

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement
                .SceneManager.GetActiveScene());

        Debug.Log(
            "[FixPanelVisuals] Done — " +
            n + " changes. Save scene (Ctrl+S).");
    }

    // =====================================================
    // STATS PANEL
    // =====================================================

    static void FixStatsPanel(ref int n)
    {
        GameObject stats =
            GameObject.Find("StatsPanel");

        if (stats == null)
        {
            Debug.LogWarning(
                "[FixPanelVisuals] StatsPanel not found.");
            return;
        }

        // ── BACKGROUND IMAGE ──────────────────────────

        Image bg = stats.GetComponent<Image>();

        if (bg == null)
        {
            bg = stats.AddComponent<Image>();
            n++;
        }

        bg.color =
            new Color(0.07f, 0.07f, 0.11f, 0.95f);

        // Remove any sprite so it renders as a flat colour
        bg.sprite = null;

        bg.type = Image.Type.Simple;

        EditorUtility.SetDirty(stats);
        n++;

        // ── VERTICAL LAYOUT GROUP ─────────────────────

        VerticalLayoutGroup vlg =
            stats.GetComponent<VerticalLayoutGroup>();

        if (vlg == null)
        {
            vlg = stats.AddComponent<VerticalLayoutGroup>();
            n++;
        }

        vlg.padding =
            new RectOffset(8, 8, 6, 8);

        vlg.spacing            = 1f;
        vlg.childAlignment     = TextAnchor.UpperLeft;
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth      = true;
        vlg.childControlHeight     = true;

        EditorUtility.SetDirty(stats);
        n++;

        // ── CONTENT SIZE FITTER ───────────────────────
        // This makes the panel height equal to the sum
        // of all stat rows — no fixed height, no gaps.

        ContentSizeFitter csf =
            stats.GetComponent<ContentSizeFitter>();

        if (csf == null)
        {
            csf = stats.AddComponent<ContentSizeFitter>();
            n++;
        }

        csf.horizontalFit =
            ContentSizeFitter.FitMode.Unconstrained;

        csf.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        EditorUtility.SetDirty(stats);
        n++;

        // ── RECT TRANSFORM — remove fixed height ──────
        // Set sizeDelta y to 0 so CSF controls height.

        RectTransform rt =
            stats.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.sizeDelta =
                new Vector2(rt.sizeDelta.x, 0);
            n++;
        }

        Debug.Log(
            "[FixPanelVisuals] StatsPanel: " +
            "VLG + CSF + Image applied.");
    }

    // =====================================================
    // INVENTORY BACKGROUND
    // =====================================================

    static void FixInventoryBackground(ref int n)
    {
        GameObject inv =
            GameObject.Find("InventoryUI");

        if (inv == null) return;

        // ── ANCHOR: stretch to fill InventoryColumn ───

        RectTransform rt =
            inv.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.anchorMin        = Vector2.zero;
            rt.anchorMax        = Vector2.one;
            rt.offsetMin        = Vector2.zero;
            rt.offsetMax        = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.localScale       = Vector3.one;
            n++;
        }

        // ── BACKGROUND IMAGE ──────────────────────────

        Image bg = inv.GetComponent<Image>();

        if (bg == null)
        {
            bg = inv.AddComponent<Image>();
            n++;
        }

        // Slightly darker than the slot colour
        bg.color  = new Color(0.07f, 0.07f, 0.11f, 0.95f);
        bg.sprite = null;
        bg.type   = Image.Type.Simple;

        EditorUtility.SetDirty(inv);
        n++;

        Debug.Log(
            "[FixPanelVisuals] InventoryUI: " +
            "anchor fixed + background applied.");
    }

    // =====================================================
    // LOOT BACKGROUND
    // =====================================================

    static void FixLootBackground(ref int n)
    {
        GameObject loot =
            GameObject.Find("LootUI");

        if (loot == null) return;

        Image bg = loot.GetComponent<Image>();

        if (bg == null)
        {
            bg = loot.AddComponent<Image>();
            n++;
        }

        bg.color  = new Color(0.07f, 0.07f, 0.11f, 0.95f);
        bg.sprite = null;
        bg.type   = Image.Type.Simple;

        EditorUtility.SetDirty(loot);
        n++;
    }
}
