using UnityEngine;

public class TargetRingPulse : MonoBehaviour
{
    public Renderer ringRenderer;

    public Color baseColor = new Color(0.2f, 0.7f, 1f, 0.6f);

    public float pulseSpeed = 2f;

    public float minAlpha = 0.65f;
    public float maxAlpha = 1f;

    private Material ringMaterial;

    void Start()
    {
        ringMaterial = ringRenderer.material;
    }

    void Update()
    {
        float pulse =
            Mathf.Lerp(minAlpha, maxAlpha,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);

        Color c = baseColor;
        c.a = pulse;

        ringMaterial.color = c;
    }
}