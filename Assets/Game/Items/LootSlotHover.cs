using UnityEngine;
using UnityEngine.EventSystems;

public class LootSlotHover :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public ItemData item;

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (
            ItemTooltipUI.instance != null &&
            item != null)
        {
            ItemTooltipUI.instance.Show(
                item);
        }
    }

    public void OnPointerExit(
        PointerEventData eventData)
    {
        if (ItemTooltipUI.instance != null)
        {
            ItemTooltipUI.instance.Hide();
        }
    }
}