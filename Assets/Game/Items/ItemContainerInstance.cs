using UnityEngine;

public class ItemContainerInstance : Container
{
    public ItemData sourceItem;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize(
        ItemData item)
    {
        sourceItem = item;

        containerName =
            item.itemName;

        slotCount =
            item.containerSlots;

        slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(null);
        }
    }
}