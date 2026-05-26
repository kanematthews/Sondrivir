using UnityEngine;
using UnityEngine.UI;

public class LootSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image icon;

    [Header("Item")]
    public ItemStack stack;

    public LootBag sourceBag;
}