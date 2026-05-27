using UnityEngine;
using UnityEngine.EventSystems;

public class LootSlotHover :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private LootSlotUI slotUI;

    // =====================================
    // AWAKE
    // =====================================

    void Awake()
    {
        slotUI =
            GetComponent<LootSlotUI>();
    }

    // =====================================
    // POINTER ENTER
    // =====================================

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (
            slotUI == null ||
            slotUI.stack == null ||
            slotUI.stack.item == null)
        {
            return;
        }

        if (ItemTooltipUI.instance != null)
        {
            ItemTooltipUI.instance.Show(
                slotUI.stack);
        }
    }

    // =====================================
    // POINTER EXIT
    // =====================================

    public void OnPointerExit(
        PointerEventData eventData)
    {
        if (ItemTooltipUI.instance != null)
        {
            ItemTooltipUI.instance.Hide();
        }
    }

    // =====================================
    // DISABLE
    // =====================================

    void OnDisable()
    {
        if (ItemTooltipUI.instance != null)
        {
            ItemTooltipUI.instance.Hide();
        }
    }
}