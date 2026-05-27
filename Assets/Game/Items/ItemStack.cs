using System;

[Serializable]
public class ItemStack
{
    public ItemData item;

    public ItemContainerInstance containerInstance;

    public int amount = 1;

    // GENERATED RARITY
    public ItemRarity rarity =
        ItemRarity.Common;
}