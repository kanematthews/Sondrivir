using UnityEngine;

public class ItemContainerInstance : Container
{
    [Header("Source")]
    public ItemData sourceItem;

    // =====================================
    // AWAKE
    // =====================================

    protected override void Awake()
    {
        base.Awake();
    }

    // =====================================
    // INITIALIZE
    // =====================================

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

    // =====================================
    // GET HIGHEST RARITY
    // =====================================

    public ItemRarity GetHighestRarity()
    {
        ItemRarity highest =
            ItemRarity.Common;

        foreach (ItemStack stack in slots)
        {
            // INVALID

            if (
                stack == null ||
                stack.item == null)
            {
                continue;
            }

            // DIRECT ITEM

            if (stack.rarity > highest)
            {
                highest = stack.rarity;
            }

            // NESTED BAG CHECK

            if (stack.containerInstance != null)
            {
                ItemRarity nestedHighest =
                    stack.containerInstance
                        .GetHighestRarity();

                if (nestedHighest > highest)
                {
                    highest = nestedHighest;
                }
            }
        }

        return highest;
    }
}