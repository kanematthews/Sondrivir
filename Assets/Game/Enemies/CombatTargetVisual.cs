using UnityEngine;

public class CombatTargetVisual : MonoBehaviour
{
    [Header("Renderer")]
    public Renderer targetRenderer;

    [Header("Pulse Settings")]
    public Color pulseColor = Color.red;

    public float pulseSpeed = 4f;

    private Material materialInstance;

    private Color originalColor;

    private bool pulsing;

    void Start()
    {
        materialInstance = targetRenderer.material;

        originalColor = materialInstance.color;
    }

    void Update()
    {
        if (pulsing)
        {
            float pulse =
                (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

            materialInstance.color =
                Color.Lerp(originalColor, pulseColor, pulse);
        }
    }

    public void StartPulse()
    {
        pulsing = true;
    }

    public void StopPulse()
    {
        pulsing = false;

        materialInstance.color = originalColor;
    }
}