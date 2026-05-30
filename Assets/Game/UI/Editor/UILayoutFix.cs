using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// =========================================================
// SONDRIVIR — UI LAYOUT FIX
// =========================================================
// MMO > Fix UI Layout
//
// Audited issues fixed in one pass:
//
//  STRUCTURE
//   • Inner Canvas ConstantPixelSize → ScaleWithScreenSize 1920×1080
//   • InventoryColumn scale 1.661 → 1  (was fighting child scale 0.602)
//   • InventoryUI/EquipmentWindow scale reset to 1
//
//  HUD (bottom of screen)
//   • Health bar: bottom-left, wider, taller, proper colour
//   • Mana  bar: bottom-right, wider, taller, proper colour
//   • Bar tracks styled (dark track behind fill)
//   • HUDFrame hidden (was 736px off-screen)
//   • PlayerDamageAnchor: centred above bars
//
//  DIALOGUE  (was partially off-screen, 300px wide)
//   • Bottom-centre, 740×300, always fully visible
//   • ChoiceContainer: proper VLG spacing, padding
//   • Buttons: 36px tall, readable font
//   • NPC name prominent gold text
//
//  INVENTORY  (was nested with conflicting scales)
//   • Right side of screen, 280×420, clean
//   • Grid: 50px cells, 6px gap, 10px padding
//
//  EQUIPMENT  (was top-left off anchor)
//   • Left side of screen, 340×480, clean
//   • All slot backgrounds darkened
//
//  LOOT       (was off-centre, below screen)
//   • Screen centre + 80px up, 240×300
//
//  STATS PANEL (was at x=-779, off screen left)
//   • Positioned inside Equipment window logic;
//     standalone panel moved to left side below equip
//
//  TOOLTIP
//   • Background opacity 0.39 → 0.95
//   • Width 300px, height auto via CSF
//   • Font sizes: name 18, rarity 12, desc 13, stats 13
//
//  DYNAMIC UI (scripts that spawn things)
//   • QuestTrackerUI  — text sizes via QuestTrackerUI constants
//   • Stat rows       — via StatRowUI font sizing
//   • Loot slots      — via grid sizing
// =========================================================

public static class UILayoutFix
{
    // ── PALETTE (same as UIOverhaul) ─────────────────────

    static readonly Color BgDeep    = new Color(0.04f, 0.04f, 0.07f, 0.96f);
    static readonly Color BgPanel   = new Color(0.07f, 0.07f, 0.11f, 0.96f);
    static readonly Color BgInset   = new Color(0.04f, 0.04f, 0.07f, 1.00f);
    static readonly Color BgSlot    = new Color(0.06f, 0.06f, 0.10f, 1.00f);
    static readonly Color BgHdr     = new Color(0.10f, 0.09f, 0.04f, 1.00f);

    static readonly Color Gold      = new Color(1.00f, 0.80f, 0.22f, 1.00f);
    static readonly Color TxtMain   = new Color(0.95f, 0.92f, 0.82f, 1.00f);
    static readonly Color TxtDim    = new Color(0.60f, 0.58f, 0.52f, 1.00f);
    static readonly Color TxtHdr    = new Color(1.00f, 0.80f, 0.22f, 1.00f);

    static readonly Color HpFill    = new Color(0.78f, 0.14f, 0.14f, 1.00f);
    static readonly Color HpTrack   = new Color(0.18f, 0.04f, 0.04f, 1.00f);
    static readonly Color MpFill    = new Color(0.20f, 0.40f, 0.88f, 1.00f);
    static readonly Color MpTrack   = new Color(0.05f, 0.10f, 0.26f, 1.00f);

    static readonly Color BtnNormal = new Color(0.12f, 0.11f, 0.09f, 1.00f);
    static readonly Color BtnHover  = new Color(0.24f, 0.21f, 0.12f, 1.00f);
    static readonly Color BtnPress  = new Color(0.34f, 0.28f, 0.10f, 1.00f);
    static readonly Color BtnTxt    = new Color(0.96f, 0.88f, 0.64f, 1.00f);

    // ── ENTRY POINT ──────────────────────────────────────

    [MenuItem("MMO/Fix UI Layout")]
    public static void Run()
    {
        int n = 0;

        FixCanvasScaler(ref n);
        FixScales(ref n);
        FixHUD(ref n);
        FixDialogue(ref n);
        FixInventory(ref n);
        FixEquipment(ref n);
        FixLoot(ref n);
        FixStatsPanel(ref n);
        FixTooltip(ref n);

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement
                .SceneManager.GetActiveScene());

        Debug.Log("[UILayoutFix] Done — " + n + " fixes applied.");
    }

    // =====================================================
    // 1. FIX CANVAS SCALER
    // =====================================================
    // Inner canvas uses ConstantPixelSize — this means UI
    // doesn't scale with resolution. Switch it to
    // ScaleWithScreenSize at 1920×1080 to match HUDRoot.

    static void FixCanvasScaler(ref int n)
    {
        GameObject bottomHUD = Find("BottomHUD");
        if (bottomHUD == null) return;

        Transform canvasTF = bottomHUD.transform.Find("Canvas");
        if (canvasTF == null) return;

        CanvasScaler scaler =
            canvasTF.GetComponent<CanvasScaler>();

        if (scaler != null)
        {
            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(1920f, 1080f);

            scaler.screenMatchMode =
                CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            scaler.matchWidthOrHeight = 0.5f;

            EditorUtility.SetDirty(canvasTF.gameObject);
            n++;

            Debug.Log(
                "[UILayoutFix] Canvas scaler fixed → " +
                "ScaleWithScreenSize 1920×1080");
        }
    }

    // =====================================================
    // 2. FIX SCALES
    // =====================================================
    // InventoryColumn has scale 1.661 fighting
    // InventoryUI's scale 0.602 — reset everything.

    static void FixScales(ref int n)
    {
        string[] toReset =
        {
            "InventoryColumn",
            "InventoryUI",
            "InventoryUIManager",
            "EquipmentWindow",
            "LootUI",
            "LootUIManager",
            "DialoguePanel",
            "StatsPanel"
        };

        foreach (string name in toReset)
        {
            GameObject go = Find(name);
            if (go == null) continue;

            if (go.transform.localScale != Vector3.one)
            {
                go.transform.localScale = Vector3.one;
                EditorUtility.SetDirty(go);
                n++;
                Debug.Log("[UILayoutFix] Scale reset: " + name);
            }
        }
    }

    // =====================================================
    // 3. FIX HUD BARS
    // =====================================================
    // Health left, mana right, both at bottom.
    // Bars are 320×26. Tracks are the immediate parents.

    static void FixHUD(ref int n)
    {
        // ── HIDE HUDFRAME (off-screen sprite) ─────────

        GameObject frame = Find("HUDFrame");
        if (frame != null)
        {
            Image img = frame.GetComponent<Image>();
            if (img != null) { img.color = Color.clear; n++; }
        }

        // ── HEALTH ────────────────────────────────────

        GameObject hfill = Find("HealthFill");
        if (hfill != null)
        {
            // Style fill
            Image fillImg = hfill.GetComponent<Image>();
            if (fillImg != null) { fillImg.color = HpFill; n++; }

            // The track is the immediate parent
            Transform track = hfill.transform.parent;
            if (track != null)
            {
                // Reposition track: bottom-left
                RectTransform trt =
                    track.GetComponent<RectTransform>();
                if (trt != null)
                {
                    trt.anchorMin        = new Vector2(0f, 0f);
                    trt.anchorMax        = new Vector2(0f, 0f);
                    trt.pivot            = new Vector2(0f, 0f);
                    trt.anchoredPosition = new Vector2(24f, 28f);
                    trt.sizeDelta        = new Vector2(320f, 26f);
                    n++;
                }

                Image trackImg = track.GetComponent<Image>();
                if (trackImg != null) { trackImg.color = HpTrack; n++; }
            }

            // Reset fill rect to fill parent
            RectTransform frt =
                hfill.GetComponent<RectTransform>();
            if (frt != null)
            {
                frt.anchorMin        = Vector2.zero;
                frt.anchorMax        = Vector2.one;
                frt.offsetMin        = Vector2.zero;
                frt.offsetMax        = Vector2.zero;
                frt.anchoredPosition = Vector2.zero;
                n++;
            }

            // Add label above bar if not present
            AddBarLabel(hfill.transform.parent?.gameObject,
                        "HP", HpFill, true, ref n);
        }

        // ── MANA ──────────────────────────────────────

        GameObject mfill = Find("ManaFill");
        if (mfill != null)
        {
            Image fillImg = mfill.GetComponent<Image>();
            if (fillImg != null) { fillImg.color = MpFill; n++; }

            Transform track = mfill.transform.parent;
            if (track != null)
            {
                // Reposition track: bottom-right
                RectTransform trt =
                    track.GetComponent<RectTransform>();
                if (trt != null)
                {
                    trt.anchorMin        = new Vector2(1f, 0f);
                    trt.anchorMax        = new Vector2(1f, 0f);
                    trt.pivot            = new Vector2(1f, 0f);
                    trt.anchoredPosition = new Vector2(-24f, 28f);
                    trt.sizeDelta        = new Vector2(320f, 26f);
                    n++;
                }

                Image trackImg = track.GetComponent<Image>();
                if (trackImg != null) { trackImg.color = MpTrack; n++; }
            }

            RectTransform frt =
                mfill.GetComponent<RectTransform>();
            if (frt != null)
            {
                frt.anchorMin        = Vector2.zero;
                frt.anchorMax        = Vector2.one;
                frt.offsetMin        = Vector2.zero;
                frt.offsetMax        = Vector2.zero;
                frt.anchoredPosition = Vector2.zero;
                n++;
            }

            AddBarLabel(mfill.transform.parent?.gameObject,
                        "MP", MpFill, false, ref n);
        }

        // ── DAMAGE ANCHOR — above centre ──────────────

        GameObject dmgAnchor = Find("PlayerDamageAnchor");
        if (dmgAnchor != null)
        {
            RectTransform rt =
                dmgAnchor.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin        = new Vector2(0.5f, 0.5f);
                rt.anchorMax        = new Vector2(0.5f, 0.5f);
                rt.pivot            = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, 0f);
                rt.sizeDelta        = new Vector2(200f, 200f);
                n++;
            }
        }
    }

    // ── Add "HP" / "MP" tiny label left/right of bar ──

    static void AddBarLabel(
        GameObject trackGO,
        string label,
        Color col,
        bool leftSide,
        ref int n)
    {
        if (trackGO == null) return;

        // Don't double-add
        if (DeepFind(trackGO, label + "Label") != null) return;

        GameObject labelGO = new GameObject(label + "Label");
        labelGO.transform.SetParent(trackGO.transform, false);

        RectTransform lrt =
            labelGO.AddComponent<RectTransform>();

        if (leftSide)
        {
            // Label sits above the left edge
            lrt.anchorMin        = new Vector2(0f, 1f);
            lrt.anchorMax        = new Vector2(0f, 1f);
            lrt.pivot            = new Vector2(0f, 0f);
            lrt.anchoredPosition = new Vector2(0f, 4f);
            lrt.sizeDelta        = new Vector2(120f, 18f);
        }
        else
        {
            lrt.anchorMin        = new Vector2(1f, 1f);
            lrt.anchorMax        = new Vector2(1f, 1f);
            lrt.pivot            = new Vector2(1f, 0f);
            lrt.anchoredPosition = new Vector2(0f, 4f);
            lrt.sizeDelta        = new Vector2(120f, 18f);
        }

        TMP_Text txt = labelGO.AddComponent<TextMeshProUGUI>();
        txt.text      = label;
        txt.fontSize  = 11f;
        txt.fontStyle = FontStyles.Bold;
        txt.color     = new Color(col.r, col.g, col.b, 0.80f);
        txt.alignment = leftSide
            ? TextAlignmentOptions.MidlineLeft
            : TextAlignmentOptions.MidlineRight;
        txt.enableWordWrapping = false;

        EditorUtility.SetDirty(trackGO);
        n++;
    }

    // =====================================================
    // 4. FIX DIALOGUE PANEL
    // =====================================================
    // Was (0, -150) from bottom anchor — partially off-screen.
    // Make it wider, position on-screen, better layout.

    static void FixDialogue(ref int n)
    {
        GameObject panel = Find("DialoguePanel");
        if (panel == null) return;

        // ── PANEL POSITION & SIZE ─────────────────────

        RectTransform rt = panel.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin        = new Vector2(0.5f, 0f);
            rt.anchorMax        = new Vector2(0.5f, 0f);
            rt.pivot            = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 10f);
            rt.sizeDelta        = new Vector2(740f, 290f);
            n++;
        }

        // Panel BG
        Image panelBg = panel.GetComponent<Image>();
        if (panelBg != null) { panelBg.color = BgDeep; n++; }

        // ── NPC NAME ──────────────────────────────────

        TMP_Text nameText = DeepFindTMP(panel, "NPCNameText");
        if (nameText != null)
        {
            nameText.fontSize  = 20f;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color     = Gold;

            // Name strip bg
            Image nameBg =
                nameText.transform.parent?.GetComponent<Image>();
            if (nameBg != null) { nameBg.color = BgHdr; n++; }

            // Name strip height
            RectTransform nameRT =
                nameText.transform.parent?.GetComponent<RectTransform>();
            if (nameRT != null)
            {
                nameRT.sizeDelta =
                    new Vector2(nameRT.sizeDelta.x, 38f);
                n++;
            }
            n++;
        }

        // ── DIALOGUE TEXT ─────────────────────────────

        TMP_Text dText = DeepFindTMP(panel, "DialogueText");
        if (dText != null)
        {
            dText.fontSize    = 16f;
            dText.color       = TxtMain;
            dText.lineSpacing = 6f;

            // Make it fill more of the panel
            RectTransform dtRT = dText.GetComponent<RectTransform>();
            if (dtRT != null)
            {
                // Stretch horizontally with margin
                dtRT.anchorMin        = new Vector2(0f, 0f);
                dtRT.anchorMax        = new Vector2(1f, 1f);
                dtRT.offsetMin        = new Vector2(16f, 50f);
                dtRT.offsetMax        = new Vector2(-16f, -46f);
                dtRT.anchoredPosition = Vector2.zero;
                n++;
            }
            n++;
        }

        // ── CHOICE CONTAINER ──────────────────────────

        GameObject cc = DeepFind(panel, "ChoiceContainer");
        if (cc != null)
        {
            RectTransform ccRT =
                cc.GetComponent<RectTransform>();
            if (ccRT != null)
            {
                // Pin to bottom of dialogue panel
                ccRT.anchorMin        = new Vector2(0f, 0f);
                ccRT.anchorMax        = new Vector2(1f, 0f);
                ccRT.pivot            = new Vector2(0.5f, 0f);
                ccRT.anchoredPosition = new Vector2(0f, 0f);
                ccRT.sizeDelta        = new Vector2(0f, 116f);
                n++;
            }

            // Layout
            VerticalLayoutGroup vlg =
                cc.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
                vlg = cc.AddComponent<VerticalLayoutGroup>();

            vlg.spacing             = 5f;
            vlg.padding             = new RectOffset(12, 12, 8, 8);
            vlg.childForceExpandWidth  = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlHeight     = true;
            vlg.childControlWidth      = true;
            vlg.childAlignment         = TextAnchor.LowerCenter;
            n++;

            // Style any pre-existing buttons
            foreach (Transform child in cc.transform)
            {
                StyleBtn(child.gameObject, 34f, ref n);
            }
        }

        // ── BACK BUTTON ───────────────────────────────

        GameObject backBtn = DeepFind(panel, "BackButton");
        if (backBtn != null)
        {
            StyleBtn(backBtn, 28f, ref n);
        }

        // ── SUBMIT BUTTON ─────────────────────────────

        GameObject subBtn = DeepFind(panel, "SubmitButton");
        if (subBtn != null)
        {
            StyleBtn(subBtn, 28f, ref n);
        }
    }

    // =====================================================
    // 5. FIX INVENTORY
    // =====================================================

    static void FixInventory(ref int n)
    {
        // InventoryColumn wrapper — reset scale, reposition
        GameObject col = Find("InventoryColumn");
        if (col != null)
        {
            col.transform.localScale = Vector3.one;

            RectTransform crt = col.GetComponent<RectTransform>();
            if (crt != null)
            {
                // Right side, vertically centred
                crt.anchorMin        = new Vector2(1f, 0.5f);
                crt.anchorMax        = new Vector2(1f, 0.5f);
                crt.pivot            = new Vector2(1f, 0.5f);
                crt.anchoredPosition = new Vector2(-16f, 0f);
                crt.sizeDelta        = new Vector2(290f, 450f);
                n++;
            }

            // Remove weird negative VLG spacing
            VerticalLayoutGroup vlg =
                col.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                vlg.spacing = 8f;
                vlg.padding = new RectOffset(0, 0, 0, 0);
                n++;
            }
        }

        // InventoryUI window itself
        GameObject inv =
            Find("InventoryUI") ?? Find("InventoryUIManager");
        if (inv == null) return;

        inv.transform.localScale = Vector3.one;

        SetPanelBg(inv, BgPanel, ref n);
        StyleWindowHeader(inv, "INVENTORY", ref n);

        // Set size if it's freestanding (not inside InventoryColumn)
        if (col == null)
        {
            RectTransform rt = inv.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin        = new Vector2(1f, 0.5f);
                rt.anchorMax        = new Vector2(1f, 0.5f);
                rt.pivot            = new Vector2(1f, 0.5f);
                rt.anchoredPosition = new Vector2(-16f, 0f);
                rt.sizeDelta        = new Vector2(290f, 420f);
                n++;
            }
        }

        // Weight text
        TMP_Text wt = DeepFindTMP(inv, "WeightText");
        if (wt != null)
        {
            wt.fontSize = 12f;
            wt.color    = TxtDim;
            n++;
        }

        // Slot grid
        GameObject grid =
            DeepFind(inv, "slotContainer") ??
            DeepFind(inv, "ItemArea");
        if (grid != null)
        {
            Image bg = grid.GetComponent<Image>();
            if (bg != null) { bg.color = BgInset; n++; }

            GridLayoutGroup glg =
                grid.GetComponent<GridLayoutGroup>();
            if (glg != null)
            {
                glg.cellSize  = new Vector2(50f, 50f);
                glg.spacing   = new Vector2(6f, 6f);
                glg.padding   = new RectOffset(10, 10, 10, 10);
                n++;
            }
        }

        StyleDeepBtn(inv, "CloseButton", ref n);
    }

    // =====================================================
    // 6. FIX EQUIPMENT WINDOW
    // =====================================================

    static void FixEquipment(ref int n)
    {
        GameObject eq = Find("EquipmentWindow");
        if (eq == null) return;

        eq.transform.localScale = Vector3.one;
        SetPanelBg(eq, BgPanel, ref n);

        // Left side, vertically centred
        RectTransform rt = eq.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin        = new Vector2(0f, 0.5f);
            rt.anchorMax        = new Vector2(0f, 0.5f);
            rt.pivot            = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(16f, 0f);
            rt.sizeDelta        = new Vector2(340f, 480f);
            n++;
        }

        // Remove old VLG that collapses the panel
        VerticalLayoutGroup eqVLG =
            eq.GetComponent<VerticalLayoutGroup>();
        if (eqVLG != null)
        {
            Object.DestroyImmediate(eqVLG);
            n++;
        }

        StyleWindowHeader(eq, "CHARACTER", ref n);

        // All equipment slots
        string[] slotNames =
        {
            "HeadSlot","ChestSlot","LegsSlot","FeetSlot",
            "MainHandSlot","OffhandSlot",
            "RingSlot","AmuletSlot","CapeSlot","BagSlot"
        };

        foreach (string sn in slotNames)
        {
            GameObject slot =
                DeepFind(eq, sn) ?? Find(sn);
            if (slot == null) continue;

            Image img = slot.GetComponent<Image>();
            if (img != null)
            {
                img.color = BgSlot;
                n++;
            }

            // Increase slot size slightly
            RectTransform srt =
                slot.GetComponent<RectTransform>();
            if (srt != null && srt.sizeDelta.x < 70f)
            {
                srt.sizeDelta = new Vector2(70f, 70f);
                n++;
            }
        }

        // Stats panel inside equipment
        GameObject sp = DeepFind(eq, "StatsPanel");
        if (sp != null) StyleStatsPanel(sp, ref n);

        StyleDeepBtn(eq, "CloseButton", ref n);
    }

    // =====================================================
    // 7. FIX LOOT WINDOW
    // =====================================================

    static void FixLoot(ref int n)
    {
        GameObject loot =
            Find("LootUI") ?? Find("LootUIManager");
        if (loot == null) return;

        loot.transform.localScale = Vector3.one;
        SetPanelBg(loot, BgPanel, ref n);

        // Centre of screen, slight upward offset
        RectTransform rt = loot.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin        = new Vector2(0.5f, 0.5f);
            rt.anchorMax        = new Vector2(0.5f, 0.5f);
            rt.pivot            = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, 80f);
            rt.sizeDelta        = new Vector2(260f, 320f);
            n++;
        }

        StyleWindowHeader(loot, "LOOT", ref n);

        GameObject itemArea =
            DeepFind(loot, "ItemArea") ??
            DeepFind(loot, "slotContainer");
        if (itemArea != null)
        {
            Image bg = itemArea.GetComponent<Image>();
            if (bg != null) { bg.color = BgInset; n++; }

            GridLayoutGroup glg =
                itemArea.GetComponent<GridLayoutGroup>();
            if (glg != null)
            {
                glg.cellSize  = new Vector2(50f, 50f);
                glg.spacing   = new Vector2(6f, 6f);
                glg.padding   = new RectOffset(10, 10, 10, 10);
                n++;
            }
        }

        StyleDeepBtn(loot, "SubmitButton", "TAKE ALL", ref n);
        StyleDeepBtn(loot, "CloseButton",  ref n);
    }

    // =====================================================
    // 8. FIX STATS PANEL (standalone)
    // =====================================================

    static void FixStatsPanel(ref int n)
    {
        GameObject sp = Find("StatsPanel");
        if (sp == null) return;

        StyleStatsPanel(sp, ref n);

        // If standalone (not inside equipment window),
        // position it below the equipment window on the left
        bool isChild =
            sp.transform.parent != null &&
            sp.transform.parent.name == "EquipmentWindow";

        if (!isChild)
        {
            RectTransform rt =
                sp.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin        = new Vector2(0f, 0.5f);
                rt.anchorMax        = new Vector2(0f, 0.5f);
                rt.pivot            = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(16f, -260f);
                rt.sizeDelta        = new Vector2(320f, 460f);
                n++;
            }
        }
    }

    static void StyleStatsPanel(
        GameObject sp,
        ref int n)
    {
        sp.transform.localScale = Vector3.one;
        SetPanelBg(sp, BgPanel, ref n);

        TMP_Text spTitle = DeepFindTMP(sp, "Title");
        if (spTitle != null)
        {
            spTitle.text      = "STATS";
            spTitle.fontSize  = 15f;
            spTitle.fontStyle = FontStyles.Bold;
            spTitle.color     = TxtHdr;
            n++;
        }

        // All TMP_Text in panel
        foreach (TMP_Text t in
            sp.GetComponentsInChildren<TMP_Text>(true))
        {
            if (t.name.Contains("Label") ||
                t.name.Contains("Name")  ||
                t.name == "StatsText")
            {
                t.fontSize = 13f;
                t.color    = TxtDim;
                n++;
            }
            else if (t.name.Contains("Value"))
            {
                t.fontSize = 13f;
                t.color    = TxtMain;
                n++;
            }
        }
    }

    // =====================================================
    // 9. FIX TOOLTIP
    // =====================================================

    static void FixTooltip(ref int n)
    {
        // ToolTip is the manager, TooltipRoot is the visual
        GameObject ttRoot = Find("TooltipRoot");
        if (ttRoot == null) ttRoot = Find("ToolTip");
        if (ttRoot == null) return;

        // Background — was 0.39 alpha (nearly invisible)
        Image bg =
            ttRoot.GetComponent<Image>() ??
            ttRoot.GetComponentInChildren<Image>();

        if (bg != null)
        {
            bg.color = new Color(0.03f, 0.03f, 0.07f, 0.97f);
            n++;
        }

        // Width
        RectTransform rt = ttRoot.GetComponent<RectTransform>();
        if (rt != null && rt.sizeDelta.x < 280f)
        {
            rt.sizeDelta = new Vector2(280f, rt.sizeDelta.y);
            n++;
        }

        // Content size fitter for height
        ContentSizeFitter csf =
            ttRoot.GetComponent<ContentSizeFitter>();
        if (csf == null)
            csf = ttRoot.AddComponent<ContentSizeFitter>();
        csf.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;
        n++;

        // VLG padding
        VerticalLayoutGroup vlg =
            ttRoot.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.padding = new RectOffset(14, 14, 12, 12);
            vlg.spacing = 6f;
            n++;
        }

        // Texts
        GameObject ttScope =
            Find("ToolTip") ?? ttRoot;

        FixTMPInScope(ttScope, "ItemNameText",
            18f, FontStyles.Bold, TxtMain, ref n);

        FixTMPInScope(ttScope, "RarityText",
            12f, FontStyles.Italic, TxtDim, ref n);

        FixTMPInScope(ttScope, "DescriptionText",
            13f, FontStyles.Normal, TxtDim, ref n);

        FixTMPInScope(ttScope, "StatsText",
            13f, FontStyles.Normal, TxtMain, ref n);

        // Separator between rarity and description
        // (add if not present)
        if (ttRoot != null)
            EnsureTooltipSeparator(ttRoot, ref n);
    }

    static void EnsureTooltipSeparator(
        GameObject root,
        ref int n)
    {
        if (DeepFind(root, "TooltipSep") != null)
            return;

        // Find the DescriptionText and insert a separator before it
        TMP_Text desc = DeepFindTMP(root, "DescriptionText");
        if (desc == null) return;

        GameObject sep = new GameObject("TooltipSep");
        sep.transform.SetParent(
            desc.transform.parent, false);

        sep.transform.SetSiblingIndex(
            desc.transform.GetSiblingIndex());

        LayoutElement le = sep.AddComponent<LayoutElement>();
        le.minHeight       = 1f;
        le.preferredHeight = 1f;

        sep.AddComponent<RectTransform>();

        Image img = sep.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.10f);

        EditorUtility.SetDirty(root);
        n++;
    }

    // =====================================================
    // SHARED HELPERS
    // =====================================================

    static void SetPanelBg(
        GameObject go,
        Color col,
        ref int n)
    {
        Image img = go.GetComponent<Image>();
        if (img != null) { img.color = col; n++; }

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
                        col.r * 0.80f,
                        col.g * 0.80f,
                        col.b * 0.80f,
                        col.a);
                    n++;
                }
            }
        }
    }

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

        Image hdrBg =
            t.transform.parent?.GetComponent<Image>();
        if (hdrBg != null &&
            t.transform.parent.gameObject != root)
        {
            hdrBg.color = BgHdr;
            n++;
        }
        n++;
    }

    static void StyleBtn(
        GameObject btn,
        float height,
        ref int n)
    {
        if (btn == null) return;

        Image img = btn.GetComponent<Image>();
        if (img != null) { img.color = BtnNormal; n++; }

        Button b = btn.GetComponent<Button>();
        if (b != null)
        {
            ColorBlock cb       = b.colors;
            cb.normalColor      = BtnNormal;
            cb.highlightedColor = BtnHover;
            cb.pressedColor     = BtnPress;
            cb.selectedColor    = BtnNormal;
            b.colors = cb;
            n++;
        }

        RectTransform rt = btn.GetComponent<RectTransform>();
        if (rt != null && height > 0f)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
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
        if (btn != null) StyleBtn(btn, 32f, ref n);
    }

    static void StyleDeepBtn(
        GameObject root,
        string name,
        string label,
        ref int n)
    {
        GameObject btn = DeepFind(root, name);
        if (btn == null) return;

        StyleBtn(btn, 36f, ref n);

        TMP_Text t = btn.GetComponentInChildren<TMP_Text>(true);
        if (t != null) { t.text = label; n++; }
    }

    static void FixTMPInScope(
        GameObject scope,
        string tmName,
        float size,
        FontStyles style,
        Color col,
        ref int n)
    {
        TMP_Text t = DeepFindTMP(scope, tmName);
        if (t == null) return;
        t.fontSize  = size;
        t.fontStyle = style;
        t.color     = col;
        n++;
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
