using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Directional Light")]
    public Light directionalLight;

    [Header("Cycle")]
    public float fullDayLength = 1200f;

    public float timeOfDay = 0.5f;

    [Header("Intensity")]
    public float dayIntensity = 1f;

    public float nightIntensity = 0.18f;

    [Header("Ambient")]
    public Color dayAmbient =
        new Color(
            0.55f,
            0.62f,
            0.72f);

    public Color nightAmbient =
        new Color(
            0.08f,
            0.10f,
            0.16f);

    [Header("Fog")]
    public Color dayFog =
        new Color(
            0.65f,
            0.74f,
            0.82f);

    public Color nightFog =
        new Color(
            0.05f,
            0.07f,
            0.12f);

    [Header("Sun Colors")]
    public Color daylightColor =
        new Color(
            1f,
            0.95f,
            0.85f);

    public Color sunsetColor =
        new Color(
            1f,
            0.65f,
            0.4f);

    public Color moonlightColor =
        new Color(
            0.50f,
            0.60f,
            0.80f);

    void Start()
    {
        RenderSettings.fog = true;

        RenderSettings.fogMode =
            FogMode.Linear;

        RenderSettings.fogStartDistance = 120f;

        RenderSettings.fogEndDistance = 350f;
    }

    void Update()
    {
        if (directionalLight == null)
        {
            return;
        }

        timeOfDay +=
            Time.deltaTime /
            fullDayLength;

        if (timeOfDay >= 1f)
        {
            timeOfDay = 0f;
        }

        UpdateLighting();
    }

    void UpdateLighting()
    {
        float sunAngle =
            timeOfDay * 360f - 90f;

        directionalLight.transform.rotation =
            Quaternion.Euler(
                sunAngle,
                -30f,
                0f);

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

        directionalLight.intensity =
            Mathf.Lerp(
                nightIntensity,
                dayIntensity,
                dayFactor);

        RenderSettings.ambientLight =
            Color.Lerp(
                nightAmbient,
                dayAmbient,
                dayFactor);

        RenderSettings.fogColor =
            Color.Lerp(
                nightFog,
                dayFog,
                dayFactor);

        if (dayFactor > 0.25f)
        {
            directionalLight.color =
                Color.Lerp(
                    sunsetColor,
                    daylightColor,
                    dayFactor);
        }
        else
        {
            directionalLight.color =
                Color.Lerp(
                    moonlightColor,
                    sunsetColor,
                    dayFactor * 4f);
        }
    }
}
