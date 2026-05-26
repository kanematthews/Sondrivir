using UnityEngine;

public class WaterLightingController : MonoBehaviour
{
    [Header("Water")]
    public Renderer waterRenderer;

    private Material waterMaterial;

    private UnityEngine.Light directionalLight;

    [Header("Colors")]
    public Color dayColor =
        new Color(
            0.26f,
            0.48f,
            0.68f);

    public Color nightColor =
        new Color(
            0.16f,
            0.25f,
            0.42f);

    [Header("Moon")]
    public Color moonTint =
        new Color(
            0.72f,
            0.82f,
            1f);

    [Range(0f, 1f)]
    public float moonTintStrength = 0.08f;

    [Header("Brightness")]
    public float dayBrightness = 1.05f;

    public float nightBrightness = 0.82f;

    [Header("Smoothness")]
    public float daySmoothness = 0.12f;

    public float nightSmoothness = 0.22f;

    void Start()
    {
        directionalLight =
            FindFirstObjectByType<UnityEngine.Light>();

        if (waterRenderer != null)
        {
            waterMaterial =
                waterRenderer.material;
        }
    }

    void Update()
    {
        if (
            directionalLight == null ||
            waterMaterial == null)
        {
            return;
        }

        float lightDot =
            Vector3.Dot(
                directionalLight.transform.forward,
                Vector3.down);

        float dayFactor =
            Mathf.Clamp01(lightDot);

        dayFactor =
            Mathf.SmoothStep(
                0,
                1,
                dayFactor);

        Color waterColor =
            Color.Lerp(
                nightColor,
                dayColor,
                dayFactor);

        if (dayFactor < 0.4f)
        {
            float moonFactor =
                1f - (dayFactor / 0.4f);

            waterColor =
                Color.Lerp(
                    waterColor,
                    moonTint,
                    moonFactor *
                    moonTintStrength);
        }

        float brightness =
            Mathf.Lerp(
                nightBrightness,
                dayBrightness,
                dayFactor);

        waterColor *= brightness;

        if (waterMaterial.HasProperty("_BaseColor"))
        {
            waterMaterial.SetColor(
                "_BaseColor",
                waterColor);
        }

        if (waterMaterial.HasProperty("_Smoothness"))
        {
            float smoothness =
                Mathf.Lerp(
                    nightSmoothness,
                    daySmoothness,
                    dayFactor);

            waterMaterial.SetFloat(
                "_Smoothness",
                smoothness);
        }
    }
}
