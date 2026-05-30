using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// =========================================================
// SONDRIVIR UI THEMER
// =========================================================
// Run via:  MMO > Apply UI Theme
//
// Walks the active scene and applies a unified dark-fantasy
// MMO visual theme to every major UI panel. Safe to re-run.
//
// PANELS COVERED:
//   • Bottom HUD   (health/mana bars, hotbar frame)
//   • Dialogue     (NPC dialogue panel + choice buttons)
//   • Inventory    (window background, title, weight text)
//   • Equipment    (window background, title, all slots)
//   • Loot         (window background, title)
//   • Stats        (window background, title, stat rows)
//   • Item Tooltip (background, text hierarchy)
//   • Quest Tracker (already built by QuestTrackerUISetup)
// =========================================================

public static class UIThemer
{
    // =====================================================
    // PALETTE
    // =====================================================

    // Backgrounds
    static readonly Color PanelBg =
        new Color(0.06f, 0.06f, 0.09f, 0.96f);

    static readonly Color PanelBgLight =
        new Color(0.10f, 0.10f, 0.14f, 0.94f);

    static readonly Color SlotBg =
        new Color(0.08f, 0.08f, 0.12f, 1f);

    static readonly Color SlotBgEmpty =
        new Color(0.05f, 0.05f, 0.08f, 0.85f);

    // Borders / accents
    static readonly Color Gold =
        new Color(1f, 0.78f, 0.22f, 1f);

    static readonly Color GoldDim =
        new Color(1f, 0.78f, 0.22f, 0.35f);

    static readonly Color BorderSubtle =
        new Color(1f, 1f, 1f, 0.07f);

    // Text
    static readonly Color TextPrimary =
        new Color(0.95f, 0.92f, 0.82f, 1f);

    static readonly Color TextDim =
        new Color(0.65f, 0.63f, 0.58f, 1f);

    static readonly Color TextHeader =
        new Color(1f, 0.78f, 0.22f, 1f);

    // Bars
    static readonly Color HealthRed =
        new Color(0.75f, 0.13f, 0.13f, 1f);

    static readonly Color HealthRedDark =
        new Color(0.28f, 0.05f, 0.05f, 1f);

    static readonly Color ManaBlue =
        new Color(0.18f, 0.38f, 0.82f, 1f);

    static readonly Color ManaBlueDark =
        new Color(0.06f, 0.13f, 0.32f, 1f);

    // Buttons
    static readonly Color ButtonBg =
        new Color(0.14f, 0.13f, 0.11f, 1f);

    static readonly Color ButtonBorder =
        new Color(1f, 0.78f, 0.22f, 0.55f);

    static readonly Color ButtonText =
        new Color(0.95f, 0.88f, 0.65f, 1f);

    // Dialogue
    static readonly Color DialogueBg =
        new Color(0.04f, 0.04f, 0.07f, 0.97f);

    static readonly Color DialogueNameBg =
        new Color(0.10f, 0.08f, 0.03f, 1f);

    // =====================================================
    // ENTRY POINT
    // =====================================================

    [MenuItem("MMO/Apply UI Theme")]
    public static void Apply()
    {
        int changes = 0;

        changes += ThemeBottomHUD();
        changes += ThemeDialogue();
        changes += ThemeInventory();
        changes += ThemeEquipment();
        changes += ThemeLoot();
        changes += ThemeStats();
        changes += ThemeTooltip();

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement
                .SceneManager.GetActiveScene());

        Debug.Log(
            "[UIThemer] Done — " +
            changes +
            " objects restyled.");
    }

    // =====================================================
    // BOTTOM HUD
    // =====================================================

    static int ThemeBottomHUD()
    {
        int n = 0;

        // ── HUDFRAME background ───────────────────────

        GameObject frame =
            Find("HUDFrame");

        if (frame != null)
        {
            Image img =
                frame.GetComponent<Image>();

            if (img != null)
            {
                img.color =
                    new Color(
                        0.05f,
                        0.04f,
                        0.07f,
                        0.0f);   // hide old sprite bg

                n++;
            }
        }

        // ── HEALTH BAR ───────────────────────────────

        // Background track
        GameObject healthBg =
            FindInChildren("HealthFill")
            ?.transform.parent?.gameObject;

        if (healthBg != null)
        {
            Image img =
                healthBg.GetComponent<Image>();

            if (img != null)
            {
                img.color = HealthRedDark;

                n++;
            }
        }

        // Fill
        GameObject healthFill =
            Find("HealthFill");

        if (healthFill != null)
        {
            Image img =
                healthFill.GetComponent<Image>();

            if (img != null)
            {
                img.color = HealthRed;

                n++;
            }
        }

        // ── MANA BAR ─────────────────────────────────

        GameObject manaFill =
            Find("ManaFill");

        if (manaFill != null)
        {
            Image img =
                manaFill.GetComponent<Image>();

            if (img != null)
            {
                img.color = ManaBlue;

                n++;
            }

            // Parent track
            Image parentImg =
                manaFill
                    .transform.parent
                    ?.GetComponent<Image>();

            if (parentImg != null)
            {
                parentImg.color = ManaBlueDark;

                n++;
            }
        }

        // ── BOTTOM HUD CANVAS BG ─────────────────────

        GameObject bottomCanvas =
            FindPath("HUDRoot/BottomHUD/Canvas");

        if (bottomCanvas != null)
        {
            // Apply subtle dark bg to any Image on the canvas root
            Image img =
                bottomCanvas.GetComponent<Image>();

            if (img != null)
            {
                img.color =
                    new Color(
                        0.04f, 0.04f, 0.07f, 0.0f);

                n++;
            }
        }

        return n;
    }

    // =====================================================
    // DIALOGUE PANEL
    // =====================================================

    static int ThemeDialogue()
    {
        int n = 0;

        GameObject panel =
            Find("DialoguePanel");

        if (panel == null)
        {
            return n;
        }

        // ── PANEL BG ─────────────────────────────────

        Image panelImg =
            panel.GetComponent<Image>();

        if (panelImg != null)
        {
            panelImg.color = DialogueBg;

            n++;
        }

        // ── NPC NAME ─────────────────────────────────

        TMP_Text npcName =
            FindTMPInChildren(
                panel,
                "NPCNameText");

        if (npcName != null)
        {
            npcName.color = Gold;

            npcName.fontSize = 20f;

            npcName.fontStyle =
                FontStyles.Bold;

            // Name container bg
            Image nameBg =
                npcName
                    .transform.parent
                    ?.GetComponent<Image>();

            if (nameBg != null)
            {
                nameBg.color = DialogueNameBg;

                n++;
            }

            n++;
        }

        // ── DIALOGUE TEXT ─────────────────────────────

        TMP_Text dialogueText =
            FindTMPInChildren(
                panel,
                "DialogueText");

        if (dialogueText != null)
        {
            dialogueText.color = TextPrimary;

            dialogueText.fontSize = 15f;

            n++;
        }

        // ── CHOICE BUTTONS ───────────────────────────

        GameObject choiceContainer =
            FindChildByName(
                panel,
                "ChoiceContainer");

        if (choiceContainer != null)
        {
            StyleChoiceButtons(
                choiceContainer,
                ref n);
        }

        // ── BACK BUTTON ──────────────────────────────

        GameObject backBtn =
            FindChildByName(panel, "BackButton");

        if (backBtn != null)
        {
            StyleButton(backBtn, ref n);
        }

        return n;
    }

    // =====================================================
    // INVENTORY
    // =====================================================

    static int ThemeInventory()
    {
        int n = 0;

        GameObject inv =
            Find("InventoryUI");

        if (inv == null)
        {
            // Try the window name directly
            inv = Find("InventoryUIManager");
        }

        if (inv == null)
        {
            return n;
        }

        // ── WINDOW BG ────────────────────────────────

        StyleWindowPanel(inv, "INVENTORY", ref n);

        // ── WEIGHT TEXT ───────────────────────────────

        TMP_Text weightText =
            FindTMPInChildren(inv, "WeightText");

        if (weightText != null)
        {
            weightText.color = TextDim;

            weightText.fontSize = 11f;

            n++;
        }

        // ── SLOT CONTAINER ───────────────────────────

        GameObject slotContainer =
            FindChildByName(inv, "slotContainer");

        if (slotContainer != null)
        {
            Image bg =
                slotContainer.GetComponent<Image>();

            if (bg != null)
            {
                bg.color = SlotBg;

                n++;
            }
        }

        // ── CLOSE BUTTON ─────────────────────────────

        GameObject closeBtn =
            FindChildByName(inv, "CloseButton");

        if (closeBtn != null)
        {
            StyleButton(closeBtn, ref n);
        }

        return n;
    }

    // =====================================================
    // EQUIPMENT
    // =====================================================

    static int ThemeEquipment()
    {
        int n = 0;

        GameObject eq =
            Find("EquipmentWindow");

        if (eq == null)
        {
            return n;
        }

        // ── WINDOW BG ────────────────────────────────

        StyleWindowPanel(eq, "EQUIPMENT", ref n);

        // ── SLOT BACKGROUNDS ─────────────────────────

        string[] slotNames =
        {
            "HeadSlot", "ChestSlot", "LegsSlot",
            "FeetSlot", "MainHandSlot", "OffhandSlot",
            "RingSlot", "AmuletSlot", "CapeSlot", "BagSlot"
        };

        foreach (string slotName in slotNames)
        {
            GameObject slot =
                FindChildByName(eq, slotName);

            if (slot == null)
            {
                slot = Find(slotName);
            }

            if (slot == null)
            {
                continue;
            }

            Image slotImg =
                slot.GetComponent<Image>();

            if (slotImg != null)
            {
                slotImg.color = SlotBgEmpty;

                n++;
            }
        }

        // ── STATS PANEL ───────────────────────────────

        GameObject statsPanel =
            FindChildByName(eq, "StatsPanel");

        if (statsPanel != null)
        {
            Image bg =
                statsPanel.GetComponent<Image>();

            if (bg != null)
            {
                bg.color = PanelBgLight;

                n++;
            }

            TMP_Text statsText =
                FindTMPInChildren(
                    statsPanel,
                    "StatsText");

            if (statsText != null)
            {
                statsText.color = TextPrimary;

                statsText.fontSize = 12f;

                n++;
            }
        }

        return n;
    }

    // =====================================================
    // LOOT
    // =====================================================

    static int ThemeLoot()
    {
        int n = 0;

        GameObject loot =
            Find("LootUI");

        if (loot == null)
        {
            loot = Find("LootUIManager");
        }

        if (loot == null)
        {
            return n;
        }

        StyleWindowPanel(loot, "LOOT", ref n);

        // ── ITEM AREA ─────────────────────────────────

        GameObject itemArea =
            FindChildByName(loot, "ItemArea");

        if (itemArea != null)
        {
            Image bg =
                itemArea.GetComponent<Image>();

            if (bg != null)
            {
                bg.color = SlotBg;

                n++;
            }
        }

        // ── TAKE ALL BUTTON ───────────────────────────

        GameObject submitBtn =
            FindChildByName(loot, "SubmitButton");

        if (submitBtn != null)
        {
            StyleButton(submitBtn, ref n);
        }

        return n;
    }

    // =====================================================
    // STATS PANEL
    // =====================================================

    static int ThemeStats()
    {
        int n = 0;

        // Stats is usually inside EquipmentWindow or its own panel
        GameObject stats =
            Find("StatsPanel");

        if (stats == null)
        {
            return n;
        }

        Image bg =
            stats.GetComponent<Image>();

        if (bg != null)
        {
            bg.color = PanelBgLight;

            n++;
        }

        // Title
        TMP_Text title =
            FindTMPInChildren(stats, "Title");

        if (title != null)
        {
            title.color = TextHeader;

            title.fontStyle = FontStyles.Bold;

            title.fontSize = 14f;

            n++;
        }

        // All stat row labels and values
        TMP_Text[] allText =
            stats.GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text t in allText)
        {
            if (t.name.Contains("Label") ||
                t.name.Contains("Name"))
            {
                t.color = TextDim;

                t.fontSize = 12f;

                n++;
            }
            else if (t.name.Contains("Value"))
            {
                t.color = TextPrimary;

                t.fontSize = 12f;

                n++;
            }
        }

        return n;
    }

    // =====================================================
    // TOOLTIP
    // =====================================================

    static int ThemeTooltip()
    {
        int n = 0;

        GameObject tooltip =
            Find("ToolTip");

        if (tooltip == null)
        {
            tooltip = Find("TooltipRoot");
        }

        if (tooltip == null)
        {
            return n;
        }

        // ── BACKGROUND ───────────────────────────────

        Image bg =
            tooltip.GetComponent<Image>();

        if (bg == null)
        {
            bg = tooltip
                .GetComponentInChildren<Image>();
        }

        if (bg != null)
        {
            bg.color =
                new Color(
                    0.04f,
                    0.04f,
                    0.08f,
                    0.97f);

            n++;
        }

        // Search in parent too
        if (tooltip.name == "TooltipRoot")
        {
            Image parentBg =
                tooltip
                    .transform.parent
                    ?.GetComponent<Image>();

            if (parentBg != null)
            {
                parentBg.color =
                    new Color(
                        0.04f,
                        0.04f,
                        0.08f,
                        0.97f);

                n++;
            }
        }

        // ── ITEM NAME ────────────────────────────────

        TMP_Text nameText =
            FindTMPInChildren(
                tooltip,
                "ItemNameText");

        if (nameText == null)
        {
            // try parent scope
            nameText = Find("ItemNameText")
                ?.GetComponent<TMP_Text>();
        }

        if (nameText != null)
        {
            nameText.fontSize = 16f;

            nameText.fontStyle = FontStyles.Bold;

            // colour set per-rarity at runtime,
            // just make sure it's not black by default
            if (nameText.color == Color.black)
            {
                nameText.color = TextPrimary;
            }

            n++;
        }

        // ── RARITY TEXT ───────────────────────────────

        TMP_Text rarityText =
            FindTMPInChildren(
                tooltip,
                "RarityText");

        if (rarityText == null)
        {
            rarityText = Find("RarityText")
                ?.GetComponent<TMP_Text>();
        }

        if (rarityText != null)
        {
            rarityText.fontSize = 11f;

            rarityText.fontStyle =
                FontStyles.Italic;

            n++;
        }

        // ── DESCRIPTION ───────────────────────────────

        TMP_Text desc =
            FindTMPInChildren(
                tooltip,
                "DescriptionText");

        if (desc == null)
        {
            desc = Find("DescriptionText")
                ?.GetComponent<TMP_Text>();
        }

        if (desc != null)
        {
            desc.color = TextDim;

            desc.fontSize = 12f;

            n++;
        }

        // ── STATS TEXT ────────────────────────────────

        TMP_Text statsText =
            FindTMPInChildren(
                tooltip,
                "StatsText");

        if (statsText == null)
        {
            statsText = Find("StatsText")
                ?.GetComponent<TMP_Text>();
        }

        if (statsText != null)
        {
            statsText.color = TextPrimary;

            statsText.fontSize = 12f;

            n++;
        }

        return n;
    }

    // =====================================================
    // HELPERS — WINDOW PANEL
    // =====================================================

    static void StyleWindowPanel(
        GameObject root,
        string headerText,
        ref int n)
    {
        // Root background
        Image bg =
            root.GetComponent<Image>();

        if (bg != null)
        {
            bg.color = PanelBg;

            n++;
        }

        // All child Images that look like backgrounds
        // (named "Background" or "Image")
        foreach (Transform child in root.transform)
        {
            if (child.name == "Background" ||
                child.name == "Image" ||
                child.name == "BG")
            {
                Image img =
                    child.GetComponent<Image>();

                if (img != null)
                {
                    img.color = PanelBgLight;

                    n++;
                }
            }
        }

        // Title text
        TMP_Text title =
            FindTMPInChildren(root, "Title");

        if (title != null)
        {
            title.text = headerText;

            title.color = TextHeader;

            title.fontStyle = FontStyles.Bold;

            title.fontSize = 15f;

            // Title container background
            Image titleBg =
                title
                    .transform.parent
                    ?.GetComponent<Image>();

            if (titleBg != null &&
                title.transform.parent.name
                    != root.name)
            {
                titleBg.color =
                    new Color(
                        0.08f, 0.07f, 0.04f, 1f);

                n++;
            }

            n++;
        }
    }

    // =====================================================
    // HELPERS — CHOICE BUTTONS
    // =====================================================

    static void StyleChoiceButtons(
        GameObject container,
        ref int n)
    {
        // Style buttons already in the container
        foreach (Transform child in container.transform)
        {
            StyleButton(child.gameObject, ref n);
        }
    }

    // =====================================================
    // HELPERS — SINGLE BUTTON
    // =====================================================

    static void StyleButton(
        GameObject btn,
        ref int n)
    {
        if (btn == null)
        {
            return;
        }

        // Background image
        Image img =
            btn.GetComponent<Image>();

        if (img != null)
        {
            img.color = ButtonBg;

            n++;
        }

        // Button component colors
        Button button =
            btn.GetComponent<Button>();

        if (button != null)
        {
            ColorBlock cb =
                button.colors;

            cb.normalColor = ButtonBg;

            cb.highlightedColor =
                new Color(
                    0.22f,
                    0.20f,
                    0.14f,
                    1f);

            cb.pressedColor =
                new Color(
                    0.30f,
                    0.26f,
                    0.12f,
                    1f);

            cb.selectedColor = ButtonBg;

            button.colors = cb;

            n++;
        }

        // Label text
        TMP_Text label =
            btn.GetComponentInChildren<TMP_Text>(true);

        if (label != null)
        {
            label.color = ButtonText;

            label.fontSize = 13f;

            n++;
        }
    }

    // =====================================================
    // SCENE FINDERS
    // =====================================================

    static GameObject Find(string name)
    {
        return GameObject.Find(name);
    }

    static GameObject FindPath(string path)
    {
        // Walk the path manually
        string[] parts =
            path.Split('/');

        GameObject current =
            GameObject.Find(parts[0]);

        if (current == null)
        {
            return null;
        }

        for (int i = 1; i < parts.Length; i++)
        {
            Transform child =
                current.transform.Find(parts[i]);

            if (child == null)
            {
                return null;
            }

            current = child.gameObject;
        }

        return current;
    }

    static GameObject FindInChildren(
        string name)
    {
        // Broad search across all scene objects
        GameObject[] all =
            Resources.FindObjectsOfTypeAll
            <GameObject>();

        foreach (GameObject go in all)
        {
            if (go.name == name &&
                go.scene.isLoaded)
            {
                return go;
            }
        }

        return null;
    }

    static GameObject FindChildByName(
        GameObject root,
        string name)
    {
        if (root == null)
        {
            return null;
        }

        Transform result =
            root.transform.Find(name);

        if (result != null)
        {
            return result.gameObject;
        }

        // Deep search
        foreach (Transform child
            in root.GetComponentsInChildren
                <Transform>(true))
        {
            if (child.name == name)
            {
                return child.gameObject;
            }
        }

        return null;
    }

    static TMP_Text FindTMPInChildren(
        GameObject root,
        string name)
    {
        if (root == null)
        {
            return null;
        }

        foreach (TMP_Text t
            in root.GetComponentsInChildren
                <TMP_Text>(true))
        {
            if (t.name == name)
            {
                return t;
            }
        }

        return null;
    }
}
