using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AssetButtonUI : MonoBehaviour
{
    [Header("UI")]
    public RawImage previewImage;

    public TMP_Text nameText;

    public TMP_Text categoryText;

    public Button selectButton;

    public void Setup(
        PlaceableAsset asset,
        GridPlacementSystem placementSystem)
    {
        if (asset == null)
        {
            Debug.LogError(
                "AssetButtonUI received NULL asset."
            );

            return;
        }

        SetupPreview(asset);

        SetupText(asset);

        selectButton.onClick.RemoveAllListeners();

        selectButton.onClick.AddListener(() =>
        {
            placementSystem.selectedAsset =
                asset;
        });
    }

    void SetupPreview(
        PlaceableAsset asset)
    {
        if (previewImage == null)
        {
            Debug.LogError(
                "PreviewImage reference missing."
            );

            return;
        }

        if (asset.previewTexture != null)
        {
            previewImage.texture =
                asset.previewTexture;
        }
        else
        {
            previewImage.texture =
                Texture2D.grayTexture;

            Debug.LogWarning(
                $"No preview texture assigned for: {asset.displayName}"
            );
        }
    }

    void SetupText(
        PlaceableAsset asset)
    {
        if (nameText != null)
        {
            string displayName =
                asset.displayName;

            if (displayName.Length > 30)
            {
                displayName =
                    displayName.Substring(0, 30)
                    + "...";
            }

            nameText.text =
                displayName;
        }

        if (categoryText != null)
        {
            categoryText.text =
                asset.category;
        }
    }
}