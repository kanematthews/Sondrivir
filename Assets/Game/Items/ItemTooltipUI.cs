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

        switch (stack.rarity)
        {
            case ItemRarity.Common:

                itemNameText.color =
                    Color.white;

                break;

            case ItemRarity.Uncommon:

                itemNameText.color =
                    new Color(
                        0.4f,
                        1f,
                        0.4f);

                break;

            case ItemRarity.Rare:

                itemNameText.color =
                    new Color(
                        0.3f,
                        0.6f,
                        1f);

                break;

            case ItemRarity.Epic:

                itemNameText.color =
                    new Color(
                        0.75f,
                        0.35f,
                        1f);

                break;

            case ItemRarity.Legendary:

                itemNameText.color =
                    new Color(
                        1f,
                        0.82f,
                        0.2f);

                break;
        }

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
                stack.rarity.ToString();

            switch (stack.rarity)
            {
                case ItemRarity.Common:

                    rarityText.color =
                        Color.white;

                    break;

                case ItemRarity.Uncommon:

                    rarityText.color =
                        new Color(
                            0.4f,
                            1f,
                            0.4f);

                    break;

                case ItemRarity.Rare:

                    rarityText.color =
                        new Color(
                            0.3f,
                            0.6f,
                            1f);

                    break;

                case ItemRarity.Epic:

                    rarityText.color =
                        new Color(
                            0.75f,
                            0.35f,
                            1f);

                    break;

                case ItemRarity.Legendary:

                    rarityText.color =
                        new Color(
                            1f,
                            0.82f,
                            0.2f);

                    break;
            }
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
        if (tooltipRoot == null)
        {
            return;
        }

        Canvas canvas =
            tooltipRoot.GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            return;
        }

        RectTransform canvasRect =
            canvas.transform as RectTransform;

        Camera cam =
            canvas.renderMode ==
            RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        Vector2 localMousePosition;

        RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                cam,
                out localMousePosition);

        // =====================================
        // TOOLTIP SIZE
        // =====================================

        float tooltipWidth =
            tooltipRoot.rect.width;

        float tooltipHeight =
            tooltipRoot.rect.height;

        // =====================================
        // CANVAS BOUNDS
        // =====================================

        float canvasWidth =
            canvasRect.rect.width;

        float canvasHeight =
            canvasRect.rect.height;

        // =====================================
        // OFFSET
        // =====================================

        float horizontalOffset = 18f;

        float verticalOffset = 18f;

        Vector2 position =
            localMousePosition;

        // =====================================
        // DEFAULT
        // RIGHT + BELOW CURSOR
        // =====================================

        position.x += horizontalOffset;
        position.y -= verticalOffset;

        // =====================================
        // RIGHT EDGE
        // MOVE LEFT OF CURSOR
        // =====================================

        float rightEdge =
            position.x + tooltipWidth;

        if (rightEdge > canvasWidth * 0.5f)
        {
            position.x =
                localMousePosition.x -
                tooltipWidth -
                horizontalOffset;
        }

        // =====================================
        // LEFT EDGE
        // =====================================

        float leftEdge =
            position.x;

        if (leftEdge < -canvasWidth * 0.5f)
        {
            position.x =
                -canvasWidth * 0.5f + 8f;
        }

        // =====================================
        // BOTTOM EDGE
        // MOVE ABOVE CURSOR
        // =====================================

        float bottomEdge =
            position.y - tooltipHeight;

        if (bottomEdge < -canvasHeight * 0.5f)
        {
            position.y =
                localMousePosition.y +
                tooltipHeight +
                verticalOffset;
        }

        // =====================================
        // TOP EDGE
        // =====================================

        float topEdge =
            position.y;

        if (topEdge > canvasHeight * 0.5f)
        {
            position.y =
                canvasHeight * 0.5f - 8f;
        }

        // =====================================
        // APPLY
        // =====================================

        tooltipRoot.anchoredPosition =
            position;
    }
}