using UnityEditor;
using UnityEngine;

// =========================================================
// CREATE GOLD COIN ASSET
// =========================================================
// Run via:  MMO > Create Gold Coin Item
//
// Creates a GoldCoin ItemData ScriptableObject asset at
// Assets/Game/Items/GoldCoin.asset
// Gold is stackable, weightless, worth 1g each.
// =========================================================

public static class CreateGoldCoinAsset
{
    private const string AssetPath =
        "Assets/Game/Items/GoldCoin.asset";

    [MenuItem("MMO/Create Gold Coin Item")]
    public static void Create()
    {
        // Don't overwrite if it already exists
        ItemData existing =
            AssetDatabase.LoadAssetAtPath<ItemData>(
                AssetPath);

        if (existing != null)
        {
            Debug.Log(
                "[GoldCoin] Asset already exists at " +
                AssetPath);

            Selection.activeObject = existing;

            return;
        }

        ItemData coin = ScriptableObject.CreateInstance<ItemData>();

        coin.itemID      = "gold_coin";
        coin.itemName    = "Gold Coin";
        coin.description = "Currency of the realm.";
        coin.itemType    = ItemType.Misc;
        coin.stackable   = true;
        coin.maxStack    = 999999;
        coin.weight      = 0f;        // gold never weighs you down
        coin.goldValue   = 1;
        coin.equippable  = false;
        coin.isContainer = false;

        AssetDatabase.CreateAsset(coin, AssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "[GoldCoin] Created GoldCoin asset at " +
            AssetPath);

        Selection.activeObject = coin;
    }

    // ── Public getter so other Editor scripts can load it ──

    public static ItemData Load()
    {
        return AssetDatabase.LoadAssetAtPath<ItemData>(
            AssetPath);
    }
}
