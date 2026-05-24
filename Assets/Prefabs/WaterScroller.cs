using UnityEngine;

public class WaterScroller : MonoBehaviour
{
    public Renderer targetRenderer;

    public float speedX = 0.02f;
    public float speedY = 0.01f;

    public float waveStrength = 0.01f;
    public float waveSpeed = 1f;

    void Update()
    {
        float wave =
            Mathf.Sin(
                Time.time * waveSpeed
            ) * waveStrength;

        Vector2 offset =
            new Vector2(
                Time.time * speedX,
                Time.time * speedY + wave
            );

        targetRenderer.sharedMaterial
            .SetTextureOffset(
                "_BaseMap",
                offset
            );
    }
}