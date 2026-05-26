using UnityEngine;
using UnityEngine.EventSystems;

public class LootSlotHover :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public ItemStack stack;

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (
            ItemTooltipUI.instance != null &&
            stack != null)
        {
            ItemTooltipUI.instance.Show(
                stack);
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