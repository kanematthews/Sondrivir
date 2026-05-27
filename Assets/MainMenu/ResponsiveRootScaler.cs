using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class ResponsiveRootScaler : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    private RectTransform rect;

    private void Update()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();

        float widthScale = Screen.width / referenceResolution.x;
        float heightScale = Screen.height / referenceResolution.y;

        float scale = Mathf.Min(widthScale, heightScale);

        rect.localScale = Vector3.one * scale;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.sizeDelta = referenceResolution;
        rect.anchoredPosition = Vector2.zero;
    }
}