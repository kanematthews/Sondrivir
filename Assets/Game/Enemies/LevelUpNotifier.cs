using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// =========================================================
// LEVEL UP NOTIFIER
// =========================================================
// Spawns a centre-top notification when the player levels up.
// Called by PlayerStats.LevelUp() via GetComponent.
// Fades in, holds, then fades out — no jarring pop.
// =========================================================

public class LevelUpNotifier : MonoBehaviour
{
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI subText;
    private CanvasGroup      group;

    private void Awake()
    {
        BuildUI();
    }

    // =====================================================
    // BUILD
    // =====================================================

    private void BuildUI()
    {
        // ── CONTAINER ─────────────────────────────────

        GameObject container =
            new GameObject("LevelUpNotification");

        container.transform.SetParent(
            transform, false);

        RectTransform crt =
            container.AddComponent<RectTransform>();

        // Top-centre, 120px below top edge so it clears
        // any minimap or quest tracker in the corners.
        crt.anchorMin        = new Vector2(0.5f, 1f);
        crt.anchorMax        = new Vector2(0.5f, 1f);
        crt.pivot            = new Vector2(0.5f, 1f);
        crt.anchoredPosition = new Vector2(0f, -120f);
        crt.sizeDelta        = new Vector2(480f, 84f);

        group = container.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        // ── BACKGROUND ────────────────────────────────

        GameObject bg = new GameObject("BG");
        bg.transform.SetParent(container.transform, false);

        RectTransform bgRT = bg.AddComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        Image bgImg = bg.AddComponent<Image>();
        bgImg.color  = new Color(0.04f, 0.04f, 0.07f, 0.92f);

        // Gold left accent
        GameObject accent = new GameObject("Accent");
        accent.transform.SetParent(container.transform, false);

        RectTransform aRT = accent.AddComponent<RectTransform>();
        aRT.anchorMin        = new Vector2(0f, 0.1f);
        aRT.anchorMax        = new Vector2(0f, 0.9f);
        aRT.pivot            = new Vector2(0f, 0.5f);
        aRT.anchoredPosition = Vector2.zero;
        aRT.sizeDelta        = new Vector2(4f, 0f);

        accent.AddComponent<Image>().color =
            new Color(1f, 0.80f, 0.22f, 1f);

        // ── TITLE TEXT  e.g.  ✦ LEVEL 10 ✦ ──────────

        GameObject titleGO =
            new GameObject("LevelUpTitle");

        titleGO.transform.SetParent(
            container.transform, false);

        RectTransform tRT =
            titleGO.AddComponent<RectTransform>();

        tRT.anchorMin        = new Vector2(0f, 0.5f);
        tRT.anchorMax        = new Vector2(1f, 1f);
        tRT.offsetMin        = new Vector2(16f, 0f);
        tRT.offsetMax        = new Vector2(-16f, 0f);

        titleText = titleGO.AddComponent<TextMeshProUGUI>();

        titleText.text      = "";
        titleText.fontSize  = 26f;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color     = new Color(1f, 0.82f, 0.22f, 1f);
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.outlineWidth = 0.18f;
        titleText.outlineColor = Color.black;
        titleText.enableWordWrapping = false;

        // ── SUB TEXT  e.g.  +3 Stat Points available ─

        GameObject subGO =
            new GameObject("LevelUpSub");

        subGO.transform.SetParent(
            container.transform, false);

        RectTransform sRT =
            subGO.AddComponent<RectTransform>();

        sRT.anchorMin  = new Vector2(0f, 0f);
        sRT.anchorMax  = new Vector2(1f, 0.5f);
        sRT.offsetMin  = new Vector2(16f, 0f);
        sRT.offsetMax  = new Vector2(-16f, 0f);

        subText = subGO.AddComponent<TextMeshProUGUI>();

        subText.text      = "";
        subText.fontSize  = 14f;
        subText.color     = new Color(0.88f, 0.84f, 0.72f, 1f);
        subText.alignment = TextAlignmentOptions.Center;
        subText.enableWordWrapping = false;

        container.SetActive(false);
    }

    // =====================================================
    // SHOW
    // =====================================================

    public void ShowLevelUpMessage(
        int level,
        int statPoints)
    {
        StopAllCoroutines();

        StartCoroutine(
            ShowRoutine(level, statPoints));
    }

    // =====================================================
    // ROUTINE — fade in → hold → fade out
    // =====================================================

    private IEnumerator ShowRoutine(
        int level,
        int statPoints)
    {
        titleText.text =
            "✦  LEVEL " + level + "  ✦";

        subText.text =
            statPoints > 0
            ? "+" + statPoints +
              " stat point" +
              (statPoints > 1 ? "s" : "") +
              " available"
            : "You've grown stronger!";

        group.transform.gameObject.SetActive(true);
        group.alpha = 0f;

        // Fade in (0.4s)
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Clamp01(t / 0.4f);
            yield return null;
        }

        group.alpha = 1f;

        // Hold (4s)
        yield return new WaitForSeconds(4f);

        // Fade out (0.8s)
        t = 0f;
        while (t < 0.8f)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Clamp01(1f - (t / 0.8f));
            yield return null;
        }

        group.alpha = 0f;

        group.transform.gameObject.SetActive(false);
    }
}
