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
            slotUI.stack.item == null ||
            slotUI.sourceBag == null)
        {
            Debug.Log(
                "Loot transfer failed: Missing references.");

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

        Debug.Log(
            "Trying to loot: " +
            slotUI.stack.item.itemName);

        // TRY ADDING ITEM
        bool success =
            inventory.AddItem(
                slotUI.stack.item,
                slotUI.stack.amount);

        if (!success)
        {
            Debug.Log(
                "Could not loot item.");

            return;
        }

        Debug.Log(
            "Loot success!");

        // REMOVE FROM BAG
        slotUI.sourceBag.items.Remove(
            slotUI.stack);

        // REFRESH LOOT UI
        if (LootUI.instance != null)
        {
            LootUI.instance.Refresh();
        }

        // REFRESH INVENTORY UI
        if (InventoryUI.instance != null)
        {
            InventoryUI.instance.Refresh();
        }

        // DESTROY EMPTY BAG
        if (
            slotUI.sourceBag.items.Count <= 0)
        {
            Destroy(
                slotUI.sourceBag.gameObject);

            if (LootUI.instance != null)
            {
                LootUI.instance.Close();
            }
        }
    }
}