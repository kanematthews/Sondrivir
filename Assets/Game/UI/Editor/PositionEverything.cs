using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// =========================================================
// POSITION EVERYTHING  —  MMO > Position Everything
// =========================================================
// Run this any time you need to reset UI positions.
// It destroys conflicting layout components, then places
// every panel at the correct screen position.
//
// TARGET LAYOUT
//  [Quests  ]  [  LEVEL UP ALERT  ]  [  Equipment ]
//  [Stats   ]                         [  Inventory ]
//
//            [HP ======]  [MP ======]
//         [  Dialogue panel (when open)  ]
// =========================================================

public static class PositionEverything
{
    [MenuItem("MMO/Position Everything")]
    public static void Run()
    {
        int n = 0;

        DestroyBadLayoutComponents(ref n);
        FixInnerCanvas(ref n);
        RestoreHUDFrame(ref n);
        PositionPanels(ref n);
        PositionBars(ref n);
        FixEquipmentSlots(ref n);
        ZeroAllZ(ref n);

        Canvas.ForceUpdateCanvases();

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement
                .SceneManager.GetActiveScene());

        Debug.Log(
            "[PositionEverything] Done — " +
            n + " changes. Save the scene (Ctrl+S).");
    }

    // =====================================================
    // 1. DESTROY conflicting layout components on BottomHUD
    // =====================================================

    static void DestroyBadLayoutComponents(ref int n)
    {
        GameObject bottomHUD =
            GameObject.Find("BottomHUD");

        if (bottomHUD == null) return;

        ContentSizeFitter csf =
            bottomHUD.GetComponent<ContentSizeFitter>();

        if (csf != null)
        {
            Object.DestroyImmediate(csf);
            n++;
        }

        HorizontalLayoutGroup hlg =
            bottomHUD
                .GetComponent<HorizontalLayoutGroup>();

        if (hlg != null)
        {
            Object.DestroyImmediate(hlg);
            n++;
        }

        RectTransform rt =
            bottomHUD.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.anchorMin        = Vector2.zero;
            rt.anchorMax        = Vector2.one;
            rt.offsetMin        = Vector2.zero;
            rt.offsetMax        = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.pivot            = new Vector2(0.5f, 0.5f);
            n++;
        }

        EditorUtility.SetDirty(bottomHUD);
    }

    // =====================================================
    // 2. INNER CANVAS — stretch to fill BottomHUD
    // =====================================================

    static void FixInnerCanvas(ref int n)
    {
        GameObject bottomHUD =
            GameObject.Find("BottomHUD");

        if (bottomHUD == null) return;

        Transform canvasTF =
            bottomHUD.transform.Find("Canvas");

        if (canvasTF == null) return;

        RectTransform rt =
            canvasTF.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.anchorMin        = Vector2.zero;
            rt.anchorMax        = Vector2.one;
            rt.offsetMin        = Vector2.zero;
            rt.offsetMax        = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.pivot            = new Vector2(0.5f, 0.5f);
            n++;
        }

        CanvasScaler scaler =
            canvasTF.GetComponent<CanvasScaler>();

        if (scaler != null)
        {
            scaler.uiScaleMode =
                CanvasScaler.ScaleMode
                    .ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(1920, 1080);

            scaler.screenMatchMode =
                CanvasScaler.ScreenMatchMode
                    .MatchWidthOrHeight;

            scaler.matchWidthOrHeight = 0.5f;
            n++;
        }

        EditorUtility.SetDirty(canvasTF.gameObject);
    }

    // =====================================================
    // 3. RESTORE HUDFRAME action bar sprite
    // =====================================================

    static void RestoreHUDFrame(ref int n)
    {
        GameObject frame = GameObject.Find("HUDFrame");
        if (frame == null) return;

        // Move it on screen at the bottom
        RectTransform rt =
            frame.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.anchorMin        = new Vector2(0.5f, 0f);
            rt.anchorMax        = new Vector2(0.5f, 0f);
            rt.pivot            = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 0f);
            rt.sizeDelta        = new Vector2(1920f, 100f);
            n++;
        }

        // Restore sprite visibility
        Image img = frame.GetComponent<Image>();

        if (img != null && img.color.a < 0.1f)
        {
            img.color = Color.white;
            n++;
        }

        EditorUtility.SetDirty(frame);
    }

    // =====================================================
    // 4. POSITION PANELS
    // =====================================================

    static void PositionPanels(ref int n)
    {
        // ── TOP-LEFT ──────────────────────────────────

        // Stats — top-left, below quest tracker
        SetRect("StatsPanel",
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(20, -160),
            new Vector2(220, 300),
            ref n);

        // ── TOP-RIGHT ─────────────────────────────────

        // Equipment window — top-right, 20px margin
        // Width 320 to fully contain 300px slot content
        SetRect("EquipmentWindow",
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(-20, -10),
            new Vector2(320, 230),
            ref n);

        // Inventory column — top-right, directly below equipment
        SetRect("InventoryColumn",
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(-20, -250),
            new Vector2(320, 260),
            ref n);

        // InventoryUI stretches to fill its column
        SetRectStretch("InventoryUI", ref n);

        // ── CENTRE ────────────────────────────────────

        // Loot window — screen centre
        SetRect("LootUI",
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0, 60),
            new Vector2(260, 300),
            ref n);

        // ── BOTTOM-CENTRE ─────────────────────────────

        // Dialogue — above health bar area
        SetRect("DialoguePanel",
            new Vector2(0.5f, 0),
            new Vector2(0.5f, 0),
            new Vector2(0.5f, 0),
            new Vector2(0, 86),
            new Vector2(680, 240),
            ref n);
    }

    // =====================================================
    // 5. POSITION BARS — bottom centre, HP left, MP right
    // =====================================================

    static void PositionBars(ref int n)
    {
        SetRect("HealthFill",
            new Vector2(0.5f, 0),
            new Vector2(0.5f, 0),
            new Vector2(0.5f, 0),
            new Vector2(-160, 46),
            new Vector2(280, 24),
            ref n);

        SetRect("ManaFill",
            new Vector2(0.5f, 0),
            new Vector2(0.5f, 0),
            new Vector2(0.5f, 0),
            new Vector2(160, 46),
            new Vector2(280, 24),
            ref n);

        SetRect("HPLabel",
            new Vector2(0.5f, 0),
            new Vector2(0.5f, 0),
            new Vector2(0, 0),
            new Vector2(-295, 74),
            new Vector2(80, 16),
            ref n);

        SetRect("MPLabel",
            new Vector2(0.5f, 0),
            new Vector2(0.5f, 0),
            new Vector2(0, 0),
            new Vector2(215, 74),
            new Vector2(80, 16),
            ref n);
    }

    // =====================================================
    // 6. FIX EQUIPMENT SLOTS container
    //    It was a fixed 300x100 box at x=150 — stretch it
    //    to fill EquipmentWindow so slots never overflow
    // =====================================================

    static void FixEquipmentSlots(ref int n)
    {
        GameObject eq = GameObject.Find("EquipmentWindow");
        if (eq == null) return;

        // Find EquipmentSlots child
        Transform slots = null;

        foreach (Transform child in
            eq.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "EquipmentSlots")
            {
                slots = child;
                break;
            }
        }

        if (slots == null) return;

        RectTransform rt =
            slots.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.anchorMin        = Vector2.zero;
            rt.anchorMax        = Vector2.one;
            rt.offsetMin        = Vector2.zero;
            rt.offsetMax        = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.pivot            = new Vector2(0.5f, 0.5f);
            EditorUtility.SetDirty(slots.gameObject);
            n++;
        }
    }

    // =====================================================
    // 7. ZERO ALL Z-POSITIONS
    // =====================================================

    static void ZeroAllZ(ref int n)
    {
        string[] names =
        {
            "InventoryColumn", "InventoryUI",
            "EquipmentWindow", "EquipmentSlots",
            "DialoguePanel", "LootUI", "StatsPanel",
            "HealthFill", "ManaFill", "HUDFrame",
            "PlayerDamageAnchor", "HPLabel", "MPLabel",
            "ChoiceContainer", "NPCNameText", "DialogueText"
        };

        foreach (string name in names)
        {
            GameObject go = GameObject.Find(name);
            if (go == null) continue;

            Vector3 lp = go.transform.localPosition;
            if (Mathf.Abs(lp.z) < 0.001f) continue;

            go.transform.localPosition =
                new Vector3(lp.x, lp.y, 0);

            EditorUtility.SetDirty(go);
            n++;
        }
    }

    // =====================================================
    // HELPERS
    // =====================================================

    static void SetRect(
        string name,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 pos,
        Vector2 size,
        ref int n)
    {
        GameObject go = GameObject.Find(name);

        if (go == null)
        {
            Debug.LogWarning(
                "[PositionEverything] Not found: " +
                name);
            return;
        }

        RectTransform rt =
            go.GetComponent<RectTransform>();

        if (rt == null) return;

        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        rt.localScale       = Vector3.one;
        rt.localPosition    = new Vector3(
            rt.localPosition.x,
            rt.localPosition.y,
            0);

        EditorUtility.SetDirty(go);
        n++;
    }

    // Makes the object stretch to fill its parent exactly
    static void SetRectStretch(string name, ref int n)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) return;

        RectTransform rt =
            go.GetComponent<RectTransform>();

        if (rt == null) return;

        rt.anchorMin        = Vector2.zero;
        rt.anchorMax        = Vector2.one;
        rt.offsetMin        = Vector2.zero;
        rt.offsetMax        = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        rt.localScale       = Vector3.one;

        EditorUtility.SetDirty(go);
        n++;
    }
}
