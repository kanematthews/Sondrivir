using UnityEngine;
using UnityEngine.EventSystems;

public class LootSlotClick :
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
        // DOUBLE LEFT CLICK
        if (
            eventData.button ==
            PointerEventData.InputButton.Left &&
            eventData.clickCount >= 2)
        {
            TransferToInventory();
        }
    }

    void TransferToInventory()
    {
        if (
            slotUI == null ||
            slotUI.stack == null ||
            slotUI.stack.item == null)
        {
            return;
        }

        // MUST BELONG TO CONTAINER
        if (slotUI.parentContainer == null)
        {
            return;
        }

        // ONLY LOOT FROM LOOT BAGS
        LootBag lootBag =
            slotUI.parentContainer
            as LootBag;

        if (lootBag == null)
        {
            return;
        }

        PlayerInventory inventory =
            FindFirstObjectByType
            <PlayerInventory>();

        if (inventory == null)
        {
            Debug.Log(
                "No PlayerInventory found.");

            return;
        }

        // TRY ADDING ITEM
        bool success =
            inventory.AddItem(
                slotUI.stack.item,
                slotUI.stack.amount);

        if (!success)
        {
            Debug.Log(
                "Inventory full.");

            return;
        }

        // REMOVE FROM BAG
        lootBag.RemoveItem(
            slotUI.slotIndex);

        // REFRESH UI
        if (LootUI.instance != null)
        {
            LootUI.instance.Refresh();
        }

        if (InventoryUI.instance != null)
        {
            InventoryUI.instance.Refresh();
        }

        // DESTROY EMPTY BAG
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
}