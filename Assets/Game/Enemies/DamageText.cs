using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header("Animation")]
    public float lifetime = 1f;

    public float moveSpeed = 3f;

    public float gravity = 6f;

    public float horizontalSpread = 1.5f;

    private Vector3 velocity;

    private float timer;

    private TextMeshPro textMesh;

    private Color originalColor;

    private Camera cam;

    void Awake()
    {
        // GET REFERENCES IMMEDIATELY
        textMesh = GetComponent<TextMeshPro>();

        cam = Camera.main;

        originalColor = textMesh.color;

        // RANDOM BURST DIRECTION
        velocity = new Vector3(
            Random.Range(-horizontalSpread, horizontalSpread),
            moveSpeed,
            Random.Range(-horizontalSpread, horizontalSpread)
        );
    }

    public void SetDamage(int amount)
    {
        textMesh.text = amount.ToString();

        Debug.Log("Damage Text Set To: " + amount);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // FACE CAMERA
        if (cam != null)
        {
            transform.forward = cam.transform.forward;
        }

        // MOVE
        transform.position += velocity * Time.deltaTime;

        // GRAVITY
        velocity.y -= gravity * Time.deltaTime;

        // FADE OUT
        float alpha =
            Mathf.Lerp(1f, 0f, timer / lifetime);

        Color c = originalColor;
        c.a = alpha;

        textMesh.color = c;

        // DESTROY
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}