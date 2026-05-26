using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class ResponsiveCanvasScaler : MonoBehaviour
{
    [Header("Reference Resolution")]
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    [Header("Match")]
    [Range(0, 1)]
    public float matchWidthOrHeight = 0.5f;

    [Header("Clamp Scale")]
    public float minScale = 0.75f;
    public float maxScale = 1.5f;

    private CanvasScaler scaler;

    private void Awake()
    {
        scaler = GetComponent<CanvasScaler>();

        ApplyScaling();
    }

    private void Update()
    {
        ApplyScaling();
    }

    private void ApplyScaling()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        float referenceRatio = referenceResolution.x / referenceResolution.y;

        float match;

        // Ultrawide support
        if (screenRatio > referenceRatio)
        {
            match = 1f;
        }
        else
        {
            match = 0f;
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        scaler.matchWidthOrHeight = Mathf.Lerp(
            matchWidthOrHeight,
            match,
            0.5f
        );

        // Optional overall scale clamp
        float scaleFactor = Mathf.Min(
            (float)Screen.width / referenceResolution.x,
            (float)Screen.height / referenceResolution.y
        );

        scaleFactor = Mathf.Clamp(scaleFactor, minScale, maxScale);

        scaler.scaleFactor = scaleFactor;
    }
}