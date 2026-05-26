using UnityEngine;

public class LootUI : MonoBehaviour
{
    public static LootUI instance;

    [Header("UI")]
    public GameObject window;

    public Transform itemContainer;

    public GameObject lootSlotPrefab;

    private LootBag currentBag;

    void Awake()
    {
        instance = this;

        Close();
    }

    public void Open(LootBag bag)
    {
        currentBag = bag;

        window.SetActive(true);

        Refresh();
    }

    public void Close()
    {
        currentBag = null;

        window.SetActive(false);
    }

    public void Refresh()
    {
        // CLEAR OLD SLOTS
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        if (currentBag == null)
        {
            return;
        }

        // CREATE SLOT FOR EACH ITEM
        foreach (ItemStack stack in currentBag.items)
        {
            GameObject slot =
                Instantiate(
                    lootSlotPrefab,
                    itemContainer);

            LootSlotUI slotUI =
                slot.GetComponent<LootSlotUI>();

            if (
                slotUI != null &&
                slotUI.icon != null &&
                stack.item != null)
            {
                slotUI.icon.sprite =
                    stack.item.icon;

                    slotUI.stack =
                    stack;

                slotUI.sourceBag =
                    currentBag;
            }

            LootSlotHover hover =
    slot.GetComponent<LootSlotHover>();

if (hover != null)
{
    hover.stack =
    stack;
}
        }
    }
}