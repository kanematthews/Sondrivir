using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GraphicsSettingsManager : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;

    public UniversalRenderPipelineAsset urpAsset;

    [Header("Performance")]
    public int targetFPS = 144;

    [Header("Anti Aliasing")]
    [Range(0, 8)]
    public int antiAliasing = 4;

    [Header("Render Scale")]
    [Range(0.5f, 2f)]
    public float renderScale = 1.15f;

    [Header("Brightness")]
    [Range(0.5f, 2f)]
    public float brightness = 1f;

    [Header("Fog")]
    public bool enableFog = true;

    [Header("Pixel MMO Style")]
    public bool stylizedMMOLighting = true;

    void Start()
    {
        ApplySettings();
    }

    public void ApplySettings()
    {
        Application.targetFrameRate =
            targetFPS;

        QualitySettings.vSyncCount = 0;

        QualitySettings.antiAliasing =
            antiAliasing;

        QualitySettings.anisotropicFiltering =
            AnisotropicFiltering.ForceEnable;

        QualitySettings.shadows =
            UnityEngine.ShadowQuality.All;

        QualitySettings.shadowResolution =
            UnityEngine.ShadowResolution.VeryHigh;

        QualitySettings.shadowDistance =
            120f;

        if (mainCamera != null)
        {
            mainCamera.allowHDR = true;

            mainCamera.allowMSAA =
                antiAliasing > 0;

            mainCamera.nearClipPlane = 0.3f;

            mainCamera.farClipPlane = 500f;

            mainCamera.fieldOfView = 50f;
        }

        if (urpAsset != null)
        {
            urpAsset.renderScale =
                renderScale;

            urpAsset.msaaSampleCount =
                antiAliasing;

            urpAsset.shadowDistance =
                120f;

            urpAsset.supportsHDR = true;

            urpAsset.supportsCameraDepthTexture = true;
        }

        RenderSettings.ambientIntensity =
            brightness;

        RenderSettings.ambientLight =
            new Color(
                0.52f,
                0.60f,
                0.70f);

        RenderSettings.fog = enableFog;

        RenderSettings.fogMode =
            FogMode.Linear;

        RenderSettings.fogColor =
            new Color(
                0.62f,
                0.70f,
                0.78f);

        RenderSettings.fogStartDistance = 120f;

        RenderSettings.fogEndDistance = 350f;

        if (stylizedMMOLighting)
        {
            Shader.globalMaximumLOD = 300;
        }
    }

    public void SetAntiAliasing(int value)
    {
        antiAliasing = value;

        ApplySettings();
    }

    public void SetBrightness(float value)
    {
        brightness = value;

        ApplySettings();
    }

    public void SetRenderScale(float value)
    {
        renderScale = value;

        ApplySettings();
    }
}
