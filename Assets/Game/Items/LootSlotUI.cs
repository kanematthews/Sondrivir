using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image icon;

    public TMP_Text stackText;

    [Header("Item")]
    public ItemStack stack;

    [Header("Container")]
    public Container parentContainer;

    // SLOT INDEX INSIDE CONTAINER

    public int slotIndex = -1;

    [Header("Auto Find")]
    public bool usePlayerEquipment = false;

    // =====================================
    // RARITY GLOW
    // =====================================

    private UIRarityGlow rarityGlow;

    // =========================================
    // START
    // =========================================

    private void Start()
    {
        // GET RARITY GLOW

        if (icon != null)
        {
            rarityGlow =
                icon.GetComponent<UIRarityGlow>();
        }

        // AUTO ASSIGN EQUIPMENT CONTAINER

        if (usePlayerEquipment)
        {
            PlayerInventory player =
                FindFirstObjectByType<PlayerInventory>();

            if (player != null)
            {
                parentContainer =
                    player.GetComponent<EquipmentContainer>();
            }
        }

        Refresh();
    }

    // =========================================
    // SET ITEM
    // =========================================

    public void SetItem(ItemStack newStack)
    {
        stack = newStack;

        // WRITE INTO REAL CONTAINER

        if (
            parentContainer != null &&
            slotIndex >= 0 &&
            slotIndex < parentContainer.slots.Count)
        {
            parentContainer.slots[slotIndex] =
                newStack;
        }

        Refresh();

        // UPDATE PLAYER STATS

        PlayerStats stats =
            FindFirstObjectByType<PlayerStats>();

        if (stats != null)
        {
            stats.RecalculateStats();
        }
    }

    // =========================================
    // REFRESH
    // =========================================

    public void Refresh()
    {
        // INVALID

        if (
            parentContainer == null ||
            slotIndex < 0 ||
            slotIndex >= parentContainer.slots.Count)
        {
            Clear();
            return;
        }

        stack =
            parentContainer.slots[slotIndex];

        // EMPTY

        if (
            stack == null ||
            stack.item == null)
        {
            Clear();
            return;
        }

        // =====================================
        // ICON
        // =====================================

        if (icon != null)
        {
            icon.gameObject.SetActive(true);

            icon.enabled = true;

            icon.sprite =
                stack.item.icon;

            icon.color =
                Color.white;
        }

        // =====================================
        // RARITY GLOW
        // =====================================

        if (rarityGlow != null)
        {
            ItemRarity finalRarity =
                stack.rarity;

            // ONLY LOOT CONTAINERS
            // NOT PLAYER INVENTORY

            if (
                stack.containerInstance != null &&
                parentContainer.containerName
    .Contains("Loot"))
            {
                finalRarity =
                    stack.containerInstance
                        .GetHighestRarity();
            }

            rarityGlow.rarity =
                finalRarity;
        }

        // =====================================
        // STACK TEXT
        // =====================================

        if (stackText != null)
        {
            if (
                stack.item.stackable &&
                stack.amount > 1)
            {
                stackText.text =
                    stack.amount.ToString();
            }
            else
            {
                stackText.text = "";
            }
        }
    }

    // =====================================
    // DISPLAY RARITY
    // =====================================

    ItemRarity GetDisplayRarity()
    {
        // INVALID

        if (
            stack == null ||
            stack.item == null)
        {
            return ItemRarity.Common;
        }

        // NORMAL ITEMS

        if (
            !stack.item.isContainer ||
            stack.containerInstance == null)
        {
            return stack.rarity;
        }

        // =================================
        // BAGS
        // FIND HIGHEST RARITY INSIDE
        // =================================

        ItemRarity highest =
            stack.rarity;

        foreach (
            ItemStack bagStack
            in stack.containerInstance.slots)
        {
            if (
                bagStack == null ||
                bagStack.item == null)
            {
                continue;
            }

            if (
                (int)bagStack.rarity >
                (int)highest)
            {
                highest =
                    bagStack.rarity;
            }
        }

        return highest;
    }

    // =========================================
    // CLEAR
    // =========================================

    public void Clear()
    {
        stack = null;

        // =====================================
        // ICON
        // =====================================

        if (icon != null)
        {
            icon.sprite = null;

            icon.enabled = false;

            icon.gameObject.SetActive(false);
        }

        // =====================================
        // RESET RARITY
        // =====================================

        if (rarityGlow != null)
        {
            rarityGlow.rarity =
                ItemRarity.Common;
        }

        // =====================================
        // STACK TEXT
        // =====================================

        if (stackText != null)
        {
            stackText.text = "";
        }
    }
}