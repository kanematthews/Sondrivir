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

    // WHICH CONTAINER THIS SLOT BELONGS TO
    public Container parentContainer;

    // SLOT INDEX INSIDE CONTAINER
    public int slotIndex = -1;
}