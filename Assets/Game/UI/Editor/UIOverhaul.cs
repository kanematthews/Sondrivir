using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// =========================================================
// SONDRIVIR — FULL UI OVERHAUL
// =========================================================
// MMO > Full UI Overhaul
//
// Applies a complete dark-fantasy MMO visual theme:
//   • Consistent panel backgrounds with depth/layering
//   • Larger, legible font sizes everywhere
//   • Warm gold accent system
//   • Health/mana bar styling
//   • Adds a gold display widget to HUDRoot
//   • Properly repositions floating windows
//   • Styles all buttons, slots, tooltips, dialogue
// =========================================================

public static class UIOverhaul
{
    // ── PALETTE ──────────────────────────────────────────

    // Backgrounds — layered from darkest to lighter
    static readonly Color BgDeep    = new Color(0.03f, 0.03f, 0.06f, 0.97f);
    static readonly Color BgPanel   = new Color(0.06f, 0.06f, 0.10f, 0.96f);
    static readonly Color BgInset   = new Color(0.09f, 0.09f, 0.14f, 1.00f);
    static readonly Color BgSlot    = new Color(0.05f, 0.05f, 0.09f, 1.00f);

    // Accent
    static readonly Color Gold      = new Color(1.00f, 0.78f, 0.22f, 1.00f);
    static readonly Color GoldFaint = new Color(1.00f, 0.78f, 0.22f, 0.22f);
    static readonly Color Silver    = new Color(0.75f, 0.78f, 0.85f, 1.00f);

    // Text
    static readonly Color TxtMain   = new Color(0.96f, 0.92f, 0.82f, 1.00f);
    static readonly Color TxtDim    = new Color(0.62f, 0.60f, 0.55f, 1.00f);
    static readonly Color TxtHdr    = new Color(1.00f, 0.78f, 0.22f, 1.00f);

    // Bars
    static readonly Color HpFill    = new Color(0.78f, 0.13f, 0.13f, 1.00f);
    static readonly Color HpTrack   = new Color(0.22f, 0.04f, 0.04f, 1.00f);
    static readonly Color MpFill    = new Color(0.18f, 0.38f, 0.85f, 1.00f);
    static readonly Color MpTrack   = new Color(0.05f, 0.10f, 0.28f, 1.00f);

    // Buttons
    static readonly Color BtnNormal = new Color(0.13f, 0.12f, 0.10f, 1.00f);
    static readonly Color BtnHover  = new Color(0.24f, 0.21f, 0.12f, 1.00f);
    static readonly Color BtnPress  = new Color(0.33f, 0.28f, 0.10f, 1.00f);
    static readonly Color BtnTxt    = new Color(0.96f, 0.88f, 0.64f, 1.00f);

    // ── ENTRY POINT ──────────────────────────────────────

    [MenuItem("MMO/Full UI Overhaul")]
    public static void Apply()
    {
        int n = 0;

        n += OverhaulHUD();
        n += OverhaulDialogue();
        n += OverhaulInventory();
        n += OverhaulEquipment();
        n += OverhaulLoot();
        n += OverhaulTooltip();

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement
                .SceneManager.GetActiveScene());

        Debug.Log(
            "[UIOverhaul] Complete — " + n + " elements restyled.");
    }

    // =====================================================
    // HUD
    // =====================================================

    static int OverhaulHUD()
    {
        int n = 0;

        // ── HEALTH FILL ───────────────────────────────

        SetImageColor("HealthFill", HpFill, ref n);

        // HealthFill parent = track
        GameObject hf = Find("HealthFill");
        if (hf != null)
        {
            Image track =
                hf.transform.parent?.GetComponent<Image>();
            if (track != null) { track.color = HpTrack; n++; }

            // Size up bar for readability
            RectTransform hfRT =
                hf.transform.parent?.GetComponent<RectTransform>();
            if (hfRT != null)
            {
                // If it's tiny, make it taller
                if (hfRT.sizeDelta.y > 0 &&
                    hfRT.sizeDelta.y < 20f)
                {
                    hfRT.sizeDelta =
                        new Vector2(hfRT.sizeDelta.x, 22f);
                    n++;
                }
            }
        }

        // ── MANA FILL ─────────────────────────────────

        SetImageColor("ManaFill", MpFill, ref n);

        GameObject mf = Find("ManaFill");
        if (mf != null)
        {
            Image track =
                mf.transform.parent?.GetComponent<Image>();
            if (track != null) { track.color = MpTrack; n++; }

            RectTransform mfRT =
                mf.transform.parent?.GetComponent<RectTransform>();
            if (mfRT != null && mfRT.sizeDelta.y > 0 &&
                mfRT.sizeDelta.y < 20f)
            {
                mfRT.sizeDelta =
                    new Vector2(mfRT.sizeDelta.x, 22f);
                n++;
            }
        }

        // ── HUDFRAME — hide old sprite bg ─────────────

        GameObject frame = Find("HUDFrame");
        if (frame != null)
        {
            Image img = frame.GetComponent<Image>();
            if (img != null)
            {
                img.color = new Color(0, 0, 0, 0);
                n++;
            }
        }

        return n;
    }

    // =====================================================
    // DIALOGUE PANEL
    // =====================================================

    static int OverhaulDialogue()
    {
        int n = 0;

        GameObject panel = Find("DialoguePanel");
        if (panel == null) return n;

        // ── PANEL BACKGROUND ──────────────────────────

        SetPanelBg(panel, BgDeep, ref n);

        // Move to bottom-center, wider, taller
        RectTransform panelRT =
            panel.GetComponent<RectTransform>();
        if (panelRT != null)
        {
            panelRT.anchorMin        = new Vector2(0.5f, 0f);
            panelRT.anchorMax        = new Vector2(0.5f, 0f);
            panelRT.pivot            = new Vector2(0.5f, 0f);
            panelRT.anchoredPosition = new Vector2(0f, 20f);
            panelRT.sizeDelta        = new Vector2(780f, 280f);
            n++;
        }

        // ── NPC NAME ──────────────────────────────────

        TMP_Text npcName = DeepFindTMP(panel, "NPCNameText");
        if (npcName != null)
        {
            npcName.fontSize  = 22f;
            npcName.fontStyle = FontStyles.Bold;
            npcName.color     = Gold;

            // Name bar background
            Image nameBg =
                npcName.transform.parent?.GetComponent<Image>();
            if (nameBg != null)
            {
                nameBg.color = new Color(0.08f, 0.07f, 0.03f, 1f);
                n++;
            }
            n++;
        }

        // ── DIALOGUE TEXT ─────────────────────────────

        TMP_Text dText = DeepFindTMP(panel, "DialogueText");
        if (dText != null)
        {
            dText.fontSize  = 16f;
            dText.color     = TxtMain;
            dText.lineSpacing = 4f;
            n++;
        }

        // ── CHOICE CONTAINER ──────────────────────────

        GameObject cc = DeepFind(panel, "ChoiceContainer");
        if (cc != null)
        {
            Image ccBg = cc.GetComponent<Image>();
            if (ccBg != null) { ccBg.color = new Color(0,0,0,0); n++; }

            // Add a VLG if not present
            VerticalLayoutGroup vlg =
                cc.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
            {
                vlg = cc.AddComponent<VerticalLayoutGroup>();
            }
            vlg.spacing             = 5f;
            vlg.padding             = new RectOffset(8, 8, 6, 6);
            vlg.childForceExpandWidth  = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth      = true;
            vlg.childControlHeight     = false;
            n++;

            // Style existing buttons (pre-spawned)
            foreach (Transform child in cc.transform)
            {
                StyleBtn(child.gameObject, ref n);
            }
        }

        return n;
    }

    // =====================================================
    // INVENTORY
    // =====================================================

    static int OverhaulInventory()
    {
        int n = 0;

        // The InventoryUI script's window reference
        GameObject invRoot = Find("InventoryUI");
        if (invRoot == null) invRoot = Find("InventoryUIManager");
        if (invRoot == null) return n;

        // Walk up to find the actual window GameObject
        // (the window field on InventoryUI — usually a child)
        GameObject window = invRoot;

        SetPanelBg(window, BgPanel, ref n);

        // Position: right side, vertically centered
        RectTransform rt = window.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin        = new Vector2(1f, 0.5f);
            rt.anchorMax        = new Vector2(1f, 0.5f);
            rt.pivot            = new Vector2(1f, 0.5f);
            rt.anchoredPosition = new Vector2(-20f, 0f);
            rt.sizeDelta        = new Vector2(280f, 440f);
            n++;
        }

        // Title
        StyleWindowHeader(window, "INVENTORY", ref n);

        // Weight text
        TMP_Text wt = DeepFindTMP(window, "WeightText");
        if (wt != null)
        {
            wt.fontSize = 12f;
            wt.color    = TxtDim;
            n++;
        }

        // Slot container
        GameObject sc = DeepFind(window, "slotContainer");
        if (sc != null)
        {
            Image bg = sc.GetComponent<Image>();
            if (bg != null) { bg.color = BgInset; n++; }

            // Grid layout
            GridLayoutGroup grid =
                sc.GetComponent<GridLayoutGroup>();
            if (grid != null)
            {
                grid.cellSize  = new Vector2(52f, 52f);
                grid.spacing   = new Vector2(5f, 5f);
                grid.padding   = new RectOffset(8, 8, 8, 8);
                n++;
            }
        }

        // Close button
        StyleDeepBtn(window, "CloseButton", ref n);

        return n;
    }

    // =====================================================
    // EQUIPMENT WINDOW
    // =====================================================

    static int OverhaulEquipment()
    {
        int n = 0;

        GameObject eq = Find("EquipmentWindow");
        if (eq == null) return n;

        SetPanelBg(eq, BgPanel, ref n);

        // Position: left side, vertically centered
        RectTransform rt = eq.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin        = new Vector2(0f, 0.5f);
            rt.anchorMax        = new Vector2(0f, 0.5f);
            rt.pivot            = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(20f, 0f);
            rt.sizeDelta        = new Vector2(340f, 480f);
            n++;
        }

        StyleWindowHeader(eq, "CHARACTER", ref n);

        // All equipment slots
        string[] slots =
        {
            "HeadSlot","ChestSlot","LegsSlot",
            "FeetSlot","MainHandSlot","OffhandSlot",
            "RingSlot","AmuletSlot","CapeSlot","BagSlot"
        };

        foreach (string slotName in slots)
        {
            // Find slot anywhere in scene (they may be direct children)
            GameObject slot =
                DeepFind(eq, slotName) ??
                Find(slotName);

            if (slot == null) continue;

            Image img = slot.GetComponent<Image>();
            if (img != null)
            {
                img.color = BgSlot;
                n++;
            }

            // Add a subtle outline via a child border image if missing
            // (just tint the slot itself)
        }

        // Stats panel inside equipment
        GameObject sp = DeepFind(eq, "StatsPanel");
        if (sp != null)
        {
            SetPanelBg(sp, BgInset, ref n);

            TMP_Text stTxt = DeepFindTMP(sp, "StatsText");
            if (stTxt != null)
            {
                stTxt.fontSize = 13f;
                stTxt.color    = TxtMain;
                n++;
            }

            // Title
            TMP_Text spTitle = DeepFindTMP(sp, "Title");
            if (spTitle != null)
            {
                spTitle.fontSize  = 14f;
                spTitle.fontStyle = FontStyles.Bold;
                spTitle.color     = TxtHdr;
                n++;
            }
        }

        StyleDeepBtn(eq, "CloseButton", ref n);

        return n;
    }

    // =====================================================
    // LOOT WINDOW
    // =====================================================

    static int OverhaulLoot()
    {
        int n = 0;

        GameObject loot = Find("LootUI") ?? Find("LootUIManager");
        if (loot == null) return n;

        SetPanelBg(loot, BgPanel, ref n);

        // Center of screen, smaller
        RectTransform rt = loot.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin        = new Vector2(0.5f, 0.5f);
            rt.anchorMax        = new Vector2(0.5f, 0.5f);
            rt.pivot            = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, 60f);
            rt.sizeDelta        = new Vector2(260f, 320f);
            n++;
        }

        StyleWindowHeader(loot, "LOOT", ref n);

        GameObject itemArea = DeepFind(loot, "ItemArea");
        if (itemArea != null)
        {
            Image bg = itemArea.GetComponent<Image>();
            if (bg != null) { bg.color = BgInset; n++; }
        }

        StyleDeepBtn(loot, "SubmitButton", "TAKE ALL", ref n);
        StyleDeepBtn(loot, "CloseButton", ref n);

        return n;
    }

    // =====================================================
    // ITEM TOOLTIP
    // =====================================================

    static int OverhaulTooltip()
    {
        int n = 0;

        // Tooltip can be either ToolTip or inside TooltipRoot
        GameObject tt    = Find("ToolTip");
        GameObject ttRoot = Find("TooltipRoot");
        GameObject target = tt ?? ttRoot;
        if (target == null) return n;

        // Background
        Image bg = target.GetComponent<Image>() ??
                   target.GetComponentInChildren<Image>();
        if (bg != null)
        {
            bg.color = new Color(0.03f, 0.03f, 0.07f, 0.97f);
            n++;
        }

        // Size — make it wider so stats don't wrap
        RectTransform rt = target.GetComponent<RectTransform>();
        if (rt != null && rt.sizeDelta.x < 260f)
        {
            rt.sizeDelta = new Vector2(260f, rt.sizeDelta.y);
            n++;
        }

        // Texts — search both target and its parent scope
        GameObject[] scope =
            tt != null && ttRoot != null
            ? new[] { tt, ttRoot }
            : new[] { target };

        foreach (GameObject s in scope)
        {
            TMP_Text nameT = DeepFindTMP(s, "ItemNameText");
            if (nameT != null)
            {
                nameT.fontSize  = 17f;
                nameT.fontStyle = FontStyles.Bold;
                // runtime colour is set per-rarity; default warm white
                if (nameT.color == Color.black ||
                    nameT.color == Color.white)
                    nameT.color = TxtMain;
                n++;
            }

            TMP_Text rarT = DeepFindTMP(s, "RarityText");
            if (rarT != null)
            {
                rarT.fontSize  = 12f;
                rarT.fontStyle = FontStyles.Italic;
                n++;
            }

            TMP_Text descT = DeepFindTMP(s, "DescriptionText");
            if (descT != null)
            {
                descT.fontSize = 13f;
                descT.color    = TxtDim;
                n++;
            }

            TMP_Text statsT = DeepFindTMP(s, "StatsText");
            if (statsT != null)
            {
                statsT.fontSize = 13f;
                statsT.color    = TxtMain;
                n++;
            }
        }

        return n;
    }

    // =====================================================
    // SHARED HELPERS
    // =====================================================

    static void StyleWindowHeader(
        GameObject root,
        string title,
        ref int n)
    {
        TMP_Text t = DeepFindTMP(root, "Title");
        if (t == null) return;

        t.text      = title;
        t.fontSize  = 16f;
        t.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
        t.color     = TxtHdr;

        // Header strip bg
        Image hdrBg =
            t.transform.parent?.GetComponent<Image>();
        if (hdrBg != null &&
            t.transform.parent.gameObject != root)
        {
            hdrBg.color = new Color(0.08f, 0.07f, 0.03f, 1f);
            n++;
        }
        n++;
    }

    static void SetPanelBg(
        GameObject go,
        Color col,
        ref int n)
    {
        Image img = go.GetComponent<Image>();
        if (img != null) { img.color = col; n++; }

        // Also darken any child named Background / BG / Image
        foreach (Transform c in go.transform)
        {
            if (c.name == "Background" ||
                c.name == "BG" ||
                c.name == "Image")
            {
                Image ci = c.GetComponent<Image>();
                if (ci != null)
                {
                    ci.color = new Color(
                        col.r * 0.85f,
                        col.g * 0.85f,
                        col.b * 0.85f,
                        col.a);
                    n++;
                }
            }
        }
    }

    static void StyleBtn(
        GameObject btn,
        ref int n)
    {
        if (btn == null) return;

        Image img = btn.GetComponent<Image>();
        if (img != null) { img.color = BtnNormal; n++; }

        Button b = btn.GetComponent<Button>();
        if (b != null)
        {
            ColorBlock cb  = b.colors;
            cb.normalColor      = BtnNormal;
            cb.highlightedColor = BtnHover;
            cb.pressedColor     = BtnPress;
            cb.selectedColor    = BtnNormal;
            b.colors = cb;
            n++;
        }

        TMP_Text lbl =
            btn.GetComponentInChildren<TMP_Text>(true);
        if (lbl != null)
        {
            lbl.color    = BtnTxt;
            lbl.fontSize = 14f;
            n++;
        }
    }

    static void StyleDeepBtn(
        GameObject root,
        string name,
        ref int n)
    {
        GameObject btn = DeepFind(root, name);
        if (btn != null) StyleBtn(btn, ref n);
    }

    static void StyleDeepBtn(
        GameObject root,
        string name,
        string label,
        ref int n)
    {
        GameObject btn = DeepFind(root, name);
        if (btn == null) return;
        StyleBtn(btn, ref n);
        TMP_Text t = btn.GetComponentInChildren<TMP_Text>(true);
        if (t != null) { t.text = label; n++; }
    }

    static void SetImageColor(
        string goName,
        Color col,
        ref int n)
    {
        GameObject go = Find(goName);
        if (go == null) return;
        Image img = go.GetComponent<Image>();
        if (img != null) { img.color = col; n++; }
    }

    // ── Scene lookups ─────────────────────────────────

    static GameObject Find(string name)
        => GameObject.Find(name);

    static GameObject DeepFind(
        GameObject root, string name)
    {
        if (root == null) return null;
        foreach (Transform t in
            root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name) return t.gameObject;
        }
        return null;
    }

    static Transform DeepFind(
        Transform root, string name)
    {
        foreach (Transform t in
            root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name) return t;
        }
        return null;
    }

    static TMP_Text DeepFindTMP(
        GameObject root, string name)
    {
        if (root == null) return null;
        foreach (TMP_Text t in
            root.GetComponentsInChildren<TMP_Text>(true))
        {
            if (t.name == name) return t;
        }
        return null;
    }
}
