using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIRarityGlow : MonoBehaviour
{
    public ItemRarity rarity =
        ItemRarity.Common;

    private Image image;

    private Material runtimeMaterial;

    private bool initialized = false;

    // =====================================
    // COLORS
    // =====================================

    readonly Color uncommonColor =
        new Color(0.13f, 0.77f, 0.37f);

    readonly Color rareColor =
        new Color(0.23f, 0.51f, 0.96f);

    readonly Color epicColor =
        new Color(0.66f, 0.33f, 0.97f);

    readonly Color legendaryColor =
        new Color(0.98f, 0.75f, 0.09f);

    // =====================================
    // AWAKE
    // =====================================

    void Awake()
    {
        EnsureInitialized();
    }

    // =====================================
    // ENSURE INITIALIZED
    // Called from Awake AND SetRarity so it
    // works regardless of call order
    // =====================================

    void EnsureInitialized()
    {
        if (initialized) return;

        image = GetComponent<Image>();

        if (image.material == null)
        {
            Debug.LogWarning(
                "UIRarityGlow: No material on Image.",
                this);
            return;
        }

        runtimeMaterial =
            Instantiate(image.material);

        image.material =
            runtimeMaterial;

        // Lock tiling — prevents the 48x48 bug
        runtimeMaterial.SetTextureScale(
            "_MainTex",
            Vector2.one);

        runtimeMaterial.SetTextureOffset(
            "_MainTex",
            Vector2.zero);

        // Outline width — 6px for visibility
        runtimeMaterial.SetFloat(
            "_OutlineWidth",
            6f);

        // Start invisible
        runtimeMaterial.SetFloat(
            "_OutlineAlpha",
            0f);

        initialized = true;
    }

    // =====================================
    // SET RARITY
    // Called by LootSlotUI.Refresh / Clear
    // =====================================

    public void SetRarity(ItemRarity newRarity)
    {
        // Make sure material exists even if
        // SetRarity is called before Awake fires
        EnsureInitialized();

        rarity = newRarity;

        if (runtimeMaterial == null) return;

        if (rarity == ItemRarity.Common)
        {
            runtimeMaterial.SetFloat(
                "_OutlineAlpha",
                0f);
        }
        else
        {
            // Push the colour immediately so the
            // first frame isn't white/wrong
            runtimeMaterial.SetColor(
                "_OutlineColor",
                GetRarityColor());
        }
    }

    // =====================================
    // UPDATE
    // =====================================

    void Update()
    {
        if (!initialized)         return;
        if (runtimeMaterial == null) return;

        if (rarity == ItemRarity.Common)
        {
            runtimeMaterial.SetFloat(
                "_OutlineAlpha",
                0f);
            return;
        }

        // Pulse — speed 0.7x, never fades to zero
        float pulse =
            Mathf.PingPong(
                Time.time * 0.7f,
                1f);

        // Min 0.6 so it stays visible at dimmest point
        float alpha =
            Mathf.Lerp(0.1f, 1.0f, pulse);

        runtimeMaterial.SetColor(
            "_OutlineColor",
            GetRarityColor());

        runtimeMaterial.SetFloat(
            "_OutlineAlpha",
            alpha);
    }

    // =====================================
    // GET COLOR
    // =====================================

    Color GetRarityColor()
    {
        switch (rarity)
        {
            case ItemRarity.Uncommon:
                return uncommonColor;

            case ItemRarity.Rare:
                return rareColor;

            case ItemRarity.Epic:
                return epicColor;

            case ItemRarity.Legendary:
                return legendaryColor;

            default:
                return Color.white;
        }
    }

    // =====================================
    // CLEANUP
    // =====================================

    void OnDestroy()
    {
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }
}