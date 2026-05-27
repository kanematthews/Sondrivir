using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    // =====================================
    // SINGLETON
    // =====================================

    public static ItemTooltipUI instance;

    // =====================================
    // ROOT
    // =====================================

    [Header("UI")]
    public RectTransform tooltipRoot;

    // =====================================
    // TEXT
    // =====================================

    public TMP_Text itemNameText;

    public TMP_Text rarityText;

    public TMP_Text descriptionText;

    public TMP_Text statsText;

    // =====================================
    // ICON
    // =====================================

    public Image icon;

    // =====================================
    // FOLLOW SETTINGS
    // =====================================

    [Header("Mouse Follow")]
    public Vector2 offset =
        new Vector2(32f, -12f);

    // =====================================
    // AWAKE
    // =====================================

    void Awake()
    {
        instance = this;

        Hide();
    }

    // =====================================
    // UPDATE
    // =====================================

    void Update()
    {
        if (
            tooltipRoot != null &&
            tooltipRoot.gameObject.activeSelf)
        {
            FollowMouse();
        }
    }

    // =====================================
    // SHOW
    // =====================================

    public void Show(ItemStack stack)
    {
        if (
            stack == null ||
            stack.item == null ||
            tooltipRoot == null)
        {
            return;
        }

        ItemData item =
            stack.item;

        tooltipRoot.gameObject.SetActive(true);

        // =================================
        // BASIC INFO
        // =================================

        if (itemNameText != null)
        {
            itemNameText.text =
                item.itemName;

            // STACK COUNT
            if (
                item.stackable &&
                stack.amount > 1)
            {
                itemNameText.text +=
                    " x" + stack.amount;
            }
        }

        if (rarityText != null)
        {
            rarityText.text =
                item.rarity.ToString();
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                item.description;
        }

        if (
            icon != null &&
            item.icon != null)
        {
            icon.sprite =
                item.icon;
        }

        // =================================
        // BUILD STATS
        // =================================

        string stats = "";

        // =================================
        // WEAPON
        // =================================

        if (
            item.itemType == ItemType.Weapon &&
            item.damage > 0)
        {
            stats +=
                "Damage: +" +
                item.damage +
                "\n";
        }

        if (
            item.itemType == ItemType.Weapon &&
            item.attackSpeed > 0f)
        {
            stats +=
                "Attack Speed: " +
                item.attackSpeed
                .ToString("F2") +
                "\n";
        }

        if (
            item.itemType == ItemType.Weapon &&
            item.attackRange > 0f)
        {
            stats +=
                "Attack Range: " +
                item.attackRange
                .ToString("F1") +
                "\n";
        }

        // =================================
        // ARMOR
        // =================================

        if (item.armor > 0)
        {
            stats +=
                "Armor: +" +
                item.armor +
                "\n";
        }

        // =================================
        // PRIMARYS
        // =================================

        if (item.bonusStrength > 0)
        {
            stats +=
                "Strength: +" +
                item.bonusStrength +
                "\n";
        }

        if (item.bonusDexterity > 0)
        {
            stats +=
                "Dexterity: +" +
                item.bonusDexterity +
                "\n";
        }

        if (item.bonusIntellect > 0)
        {
            stats +=
                "Intellect: +" +
                item.bonusIntellect +
                "\n";
        }

        if (item.bonusVitality > 0)
        {
            stats +=
                "Vitality: +" +
                item.bonusVitality +
                "\n";
        }

        // =================================
        // RESOURCES
        // =================================

        if (item.bonusHealth > 0)
        {
            stats +=
                "Health: +" +
                item.bonusHealth +
                "\n";
        }

        if (item.bonusMana > 0)
        {
            stats +=
                "Mana: +" +
                item.bonusMana +
                "\n";
        }

        // =================================
        // REGEN
        // =================================

        if (item.bonusHPRegen > 0)
        {
            stats +=
                "HP Regen: +" +
                item.bonusHPRegen +
                "\n";
        }

        if (item.bonusMPRegen > 0)
        {
            stats +=
                "MP Regen: +" +
                item.bonusMPRegen +
                "\n";
        }

        // =================================
        // CRIT
        // =================================

        if (item.bonusCritChance > 0)
        {
            stats +=
                "Crit Chance: +" +
                (item.bonusCritChance * 100f)
                .ToString("F0") +
                "%\n";
        }

        // =================================
        // MOVEMENT
        // =================================

        if (item.bonusMoveSpeed > 0)
        {
            stats +=
                "Move Speed: +" +
                item.bonusMoveSpeed
                .ToString("F1") +
                "\n";
        }

        // =================================
        // REQUIREMENTS
        // =================================

        if (item.requiredStrength > 0)
        {
            stats +=
                "\nRequires Strength: " +
                item.requiredStrength;
        }

        if (item.requiredDexterity > 0)
        {
            stats +=
                "\nRequires Dexterity: " +
                item.requiredDexterity;
        }

        if (item.requiredIntelligence > 0)
        {
            stats +=
                "\nRequires Intelligence: " +
                item.requiredIntelligence;
        }

        // =================================
        // APPLY
        // =================================

        if (statsText != null)
        {
            statsText.text =
                stats.TrimEnd();
        }

        FollowMouse();
    }

    // =====================================
    // HIDE
    // =====================================

    public void Hide()
    {
        if (tooltipRoot != null)
        {
            tooltipRoot.gameObject.SetActive(false);
        }
    }

    // =====================================
    // FOLLOW MOUSE
    // =====================================

    void FollowMouse()
    {
        Vector2 mouse =
            Input.mousePosition;

        float width =
            tooltipRoot.rect.width;

        float height =
            tooltipRoot.rect.height;

        float cursorPadding = 18f;

        Vector2 position =
            mouse;

        // =================================
        // DEFAULT POSITION
        // RIGHT + SLIGHTLY DOWN
        // =================================

        position.x += cursorPadding;
        position.y -= cursorPadding;

        // =================================
        // RIGHT EDGE
        // FLIP TO LEFT SIDE
        // =================================

        if (
            position.x + width >
            Screen.width)
        {
            position.x =
                mouse.x -
                width -
                cursorPadding;
        }

        // =================================
        // BOTTOM EDGE
        // FLIP ABOVE CURSOR
        // =================================

        if (
            position.y - height <
            0)
        {
            position.y =
                mouse.y +
                height * 0.15f;
        }

        // =================================
        // HARD CLAMP
        // =================================

        position.x =
            Mathf.Clamp(
                position.x,
                8f,
                Screen.width - width - 8f);

        position.y =
            Mathf.Clamp(
                position.y,
                height + 8f,
                Screen.height - 8f);

        tooltipRoot.position =
            position;
    }
}