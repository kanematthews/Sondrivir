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

    // =========================================
    // START
    // =========================================

    private void Start()
    {
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

        // ICON
        if (icon != null)
        {
            icon.enabled = true;

            icon.sprite =
                stack.item.icon;
        }

        // STACK TEXT
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

    // =========================================
    // CLEAR
    // =========================================

    public void Clear()
    {
        stack = null;

        if (icon != null)
        {
            icon.enabled = false;

            icon.sprite = null;
        }

        if (stackText != null)
        {
            stackText.text = "";
        }
    }
}