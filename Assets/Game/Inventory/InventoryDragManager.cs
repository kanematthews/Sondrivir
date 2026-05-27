using UnityEngine;
using UnityEngine.UI;

public class InventoryDragManager : MonoBehaviour
{
    public static InventoryDragManager instance;

    [Header("Drag UI")]
    public Image dragIcon;

    [HideInInspector]
    public ItemStack draggedStack;

    [HideInInspector]
    public Container sourceContainer;

    [HideInInspector]
    public int sourceSlotIndex = -1;

    private bool isDragging = false;

    // =========================================
    // AWAKE
    // =========================================

    void Awake()
    {
        instance = this;

        ClearDrag();
    }

    // =========================================
    // UPDATE
    // =========================================

    void Update()
    {
        // FOLLOW MOUSE
        if (
            isDragging &&
            dragIcon != null)
        {
            dragIcon.transform.position =
                Input.mousePosition;
        }
    }

    // =========================================
    // BEGIN DRAG
    // =========================================

    public void BeginDrag(
        Container container,
        ItemStack stack,
        int slotIndex)
    {
        if (
            container == null ||
            stack == null ||
            stack.item == null)
        {
            return;
        }

        sourceContainer =
            container;

        draggedStack =
            stack;

        sourceSlotIndex =
            slotIndex;

        isDragging = true;

        // DRAG ICON
        if (dragIcon != null)
        {
            dragIcon.enabled = true;

            dragIcon.sprite =
                stack.item.icon;

            dragIcon.transform.position =
                Input.mousePosition;
        }

        Debug.Log(
            "Begin Drag: " +
            stack.item.itemName);
    }

    // =========================================
    // DROP
    // =========================================

    public void DropOnSlot(
        Container targetContainer,
        int targetSlotIndex)
    {
        if (
            sourceContainer == null ||
            targetContainer == null ||
            draggedStack == null)
        {
            ClearDrag();

            return;
        }

        // SAME SLOT
        if (
            sourceContainer == targetContainer &&
            sourceSlotIndex == targetSlotIndex)
        {
            ClearDrag();

            return;
        }

        ItemStack sourceStack =
            sourceContainer.slots[sourceSlotIndex];

        ItemStack targetStack =
            targetContainer.slots[targetSlotIndex];

        // INVALID
        if (
            sourceStack == null ||
            sourceStack.item == null)
        {
            ClearDrag();

            return;
        }

        // =====================================
        // BAG INSIDE ITSELF
        // =====================================

        if (
            sourceStack.item.isContainer &&
            sourceStack.containerInstance != null)
        {
            if (
                targetContainer ==
                sourceStack.containerInstance)
            {
                Debug.Log(
                    "Cannot place a bag inside itself.");

                ClearDrag();

                return;
            }
        }

        // =====================================
        // EQUIPMENT VALIDATION
        // =====================================

        EquipmentContainer equipment =
            targetContainer as EquipmentContainer;

        if (equipment != null)
        {
            bool canEquip =
                equipment.CanPlaceItem(
                    targetSlotIndex,
                    sourceStack);

            if (!canEquip)
            {
                Debug.Log(
                    "Cannot equip item there.");

                ClearDrag();

                return;
            }
        }

        // =====================================
        // EMPTY SLOT
        // =====================================

        if (targetStack == null)
        {
            targetContainer.slots[targetSlotIndex] =
                sourceStack;

            sourceContainer.slots[sourceSlotIndex] =
                null;
        }

        // =====================================
        // STACK MERGE
        // =====================================

        else if (
            sourceStack.item ==
            targetStack.item &&
            sourceStack.item.stackable)
        {
            targetStack.amount +=
                sourceStack.amount;

            sourceContainer.slots[sourceSlotIndex] =
                null;
        }

        // =====================================
        // SWAP
        // =====================================

        else
        {
            targetContainer.slots[targetSlotIndex] =
                sourceStack;

            sourceContainer.slots[sourceSlotIndex] =
                targetStack;
        }

        // =====================================
        // REFRESH SLOT UIS
        // =====================================

        RefreshAllSlots();

        // =====================================
        // REFRESH CONTAINER UIS
        // =====================================

        if (InventoryUI.instance != null)
        {
            InventoryUI.instance.Refresh();
        }

        if (LootUI.instance != null)
        {
            LootUI.instance.Refresh();
        }

        if (NestedContainerManager.instance != null)
        {
            NestedContainerManager.instance
                .RefreshAll();
        }

        // =====================================
        // RECALCULATE PLAYER STATS
        // =====================================

        PlayerStats stats =
            FindFirstObjectByType<PlayerStats>();

        if (stats != null)
        {
            stats.RecalculateStats();
        }

        // =====================================
        // DESTROY EMPTY LOOT BAG
        // =====================================

        LootBag lootBag =
            sourceContainer as LootBag;

        if (lootBag != null)
        {
            bool empty = true;

            foreach (ItemStack stack in lootBag.slots)
            {
                if (stack != null)
                {
                    empty = false;

                    break;
                }
            }

            if (empty)
            {
                Destroy(
                    lootBag.gameObject);

                if (LootUI.instance != null)
                {
                    LootUI.instance.Close();
                }
            }
        }

        Debug.Log("Drop Complete");

        ClearDrag();
    }

    // =========================================
    // REFRESH ALL SLOTS
    // =========================================

    void RefreshAllSlots()
    {
        LootSlotUI[] allSlots =
            FindObjectsByType<LootSlotUI>(
                FindObjectsSortMode.None);

        foreach (LootSlotUI slot in allSlots)
        {
            slot.Refresh();
        }
    }

    // =========================================
    // CLEAR DRAG
    // =========================================

    public void ClearDrag()
    {
        draggedStack = null;

        sourceContainer = null;

        sourceSlotIndex = -1;

        isDragging = false;

        if (dragIcon != null)
        {
            dragIcon.enabled = false;

            dragIcon.sprite = null;
        }
    }
}