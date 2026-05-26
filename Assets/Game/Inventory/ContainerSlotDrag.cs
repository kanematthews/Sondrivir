using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerSlotDrag :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    private LootSlotUI slotUI;

    void Awake()
    {
        slotUI =
            GetComponent<LootSlotUI>();
    }

    // =========================================
    // BEGIN DRAG
    // =========================================

    public void OnBeginDrag(
        PointerEventData eventData)
    {
        if (
        slotUI == null ||
        slotUI.parentContainer == null)
    {
        return;
    }

        InventoryDragManager.instance
            .BeginDrag(
                slotUI.parentContainer,
                slotUI.stack,
                slotUI.slotIndex);
    }

    // =========================================
    // DRAGGING
    // =========================================

    public void OnDrag(
        PointerEventData eventData)
    {
        // Unity requires this
    }

    // =========================================
    // END DRAG
    // =========================================

    public void OnEndDrag(
        PointerEventData eventData)
    {
        InventoryDragManager.instance
            .ClearDrag();
    }

    // =========================================
    // DROP
    // =========================================

    public void OnDrop(
        PointerEventData eventData)
    {
        if (
            slotUI == null ||
            slotUI.parentContainer == null)
        {
            return;
        }

        InventoryDragManager.instance
            .DropOnSlot(
                slotUI.parentContainer,
                slotUI.slotIndex);
    }
}