using UnityEngine;
using UnityEngine.EventSystems;

public class BagItemClick :
    MonoBehaviour,
    IPointerClickHandler
{
    private LootSlotUI slotUI;

    void Awake()
    {
        slotUI =
            GetComponent<LootSlotUI>();
    }

    public void OnPointerClick(
        PointerEventData eventData)
    {
        // RIGHT CLICK
        if (
            eventData.button !=
            PointerEventData.InputButton.Right)
        {
            return;
        }

        if (
            slotUI == null ||
            slotUI.stack == null)
        {
            return;
        }

        ItemStack stack =
            slotUI.stack;

        if (
            stack.item == null ||
            !stack.item.isContainer)
        {
            return;
        }

        if (
            stack.containerInstance == null)
        {
            return;
        }

        NestedContainerManager.instance
            .ToggleContainer(
                stack.containerInstance);
    }
}