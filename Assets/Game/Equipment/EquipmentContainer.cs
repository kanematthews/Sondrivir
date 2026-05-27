using UnityEngine;

public class EquipmentContainer : Container
{
    protected override void Awake()
    {
        slotCount = 10;

        base.Awake();

        containerName = "Equipment";
    }

    // =========================================
    // CAN PLACE ITEM
    // =========================================

    public bool CanPlaceItem(
        int slotIndex,
        ItemStack stack)
    {
        if (
            stack == null ||
            stack.item == null)
        {
            return false;
        }

        if (!stack.item.equippable)
        {
            return false;
        }

        EquipmentSlotType requiredSlot =
            GetSlotType(slotIndex);

        return
            stack.item.equipmentSlotType ==
            requiredSlot;
    }

    // =========================================
    // SLOT TYPE MAP
    // =========================================

    public EquipmentSlotType GetSlotType(
        int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                return EquipmentSlotType.Head;

            case 1:
                return EquipmentSlotType.Cape;

            case 2:
                return EquipmentSlotType.Amulet;

            case 3:
                return EquipmentSlotType.Chest;

            case 4:
                return EquipmentSlotType.Legs;

            case 5:
                return EquipmentSlotType.Feet;

            case 6:
                return EquipmentSlotType.MainHand;

            case 7:
                return EquipmentSlotType.OffHand;

            case 8:
                return EquipmentSlotType.Ring;

            case 9:
                return EquipmentSlotType.Bag;
        }

        return EquipmentSlotType.Head;
    }
}