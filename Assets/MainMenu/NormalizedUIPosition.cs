using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class NormalizedUIPosition : MonoBehaviour
{
    [Header("Normalized Position")]
    [Range(0f, 1f)]
    public float normalizedX = 0.5f;

    [Range(0f, 1f)]
    public float normalizedY = 0.5f;

    [Header("Normalized Size")]
    [Range(0f, 1f)]
    public float normalizedWidth = 0.2f;

    [Range(0f, 1f)]
    public float normalizedHeight = 0.08f;

    private RectTransform rect;
    private RectTransform parentRect;

    private void Update()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();

        if (parentRect == null)
            parentRect = rect.parent as RectTransform;

        if (parentRect == null)
            return;

        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        // Position
        float posX = (normalizedX - 0.5f) * parentWidth;
        float posY = (normalizedY - 0.5f) * parentHeight;

        rect.anchoredPosition = new Vector2(posX, posY);

        // Size
        float width = parentWidth * normalizedWidth;
        float height = parentHeight * normalizedHeight;

        rect.sizeDelta = new Vector2(width, height);
    }
}