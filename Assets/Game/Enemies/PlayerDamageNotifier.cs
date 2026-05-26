using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerDamageNotifier : MonoBehaviour
{
    [Header("References")]
    public GameObject damageTextPrefab;

    public Transform damageAnchor;

    public void ShowDamage(int amount)
    {
        if (
            damageTextPrefab == null ||
            damageAnchor == null)
        {
            Debug.LogWarning(
                "Damage notifier references missing.");

            return;
        }

        GameObject textObj =
            Instantiate(
                damageTextPrefab,
                damageAnchor);

        textObj.SetActive(true);

        TMP_Text text =
            textObj.GetComponent<TMP_Text>();

        if (text != null)
        {
            text.text =
                "-" + amount;

            text.color = Color.red;
        }

        StartCoroutine(
            AnimateDamage(textObj));
    }

    IEnumerator AnimateDamage(
        GameObject obj)
    {
        RectTransform rect =
            obj.GetComponent<RectTransform>();

        TMP_Text text =
            obj.GetComponent<TMP_Text>();

        // START POSITION
        Vector2 startPos =
            new Vector2(
                Random.Range(-10f, 10f),
                Random.Range(-5f, 5f));

        rect.anchoredPosition =
            startPos;

        // RANDOM BURST DIRECTION
        Vector2 burstDirection =
            new Vector2(
                Random.Range(35f, 60f), // RIGHT
                Random.Range(50f, 80f)  // UP
            );

        float duration = 1f;

        float timer = 0f;

        Color startColor =
            text.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                timer / duration;

            // CURVED BOUNCE
            float curve =
                Mathf.Sin(t * Mathf.PI);

            rect.anchoredPosition =
                startPos +
                (burstDirection * t) +
                new Vector2(
                    0,
                    curve * 15f);

            // FADE OUT
            Color color =
                startColor;

            color.a =
                Mathf.Lerp(
                    1f,
                    0f,
                    t);

            text.color = color;

            // SLIGHT SCALE POP
            float scale =
                Mathf.Lerp(
                    1.2f,
                    1f,
                    t);

            rect.localScale =
                Vector3.one * scale;

            yield return null;
        }

        Destroy(obj);
    }
}