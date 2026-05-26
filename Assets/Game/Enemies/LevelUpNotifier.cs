using System.Collections;
using TMPro;
using UnityEngine;

public class LevelUpNotifier : MonoBehaviour
{
    private TextMeshProUGUI notificationText;

    void Awake()
    {
        CreateNotificationUI();
    }

    void CreateNotificationUI()
    {
        GameObject notificationObj =
            new GameObject(
                "LevelUpNotification");

        notificationObj.transform.SetParent(
            transform,
            false);

        RectTransform rect =
            notificationObj
            .AddComponent<RectTransform>();

        // BELOW TOP CENTER
        rect.anchorMin =
            new Vector2(0.5f, 1f);

        rect.anchorMax =
            new Vector2(0.5f, 1f);

        rect.pivot =
            new Vector2(0.5f, 1f);

        rect.anchoredPosition =
            new Vector2(0, -80);

        rect.sizeDelta =
            new Vector2(1000, 80);

        notificationText =
            notificationObj
            .AddComponent<TextMeshProUGUI>();

        notificationText.text = "";

        notificationText.fontSize = 20;

        notificationText.alignment =
            TextAlignmentOptions.Center;

        notificationText.color =
            new Color32(
                255,
                230,
                120,
                255);

        // MMO OUTLINE
        notificationText.outlineWidth =
            0.2f;

        notificationText.outlineColor =
            Color.black;

        notificationObj.SetActive(false);
    }

    public void ShowLevelUpMessage(
        int level,
        int statPoints)
    {
        Debug.Log(
            "LEVEL UP NOTIFICATION");

        StopAllCoroutines();

        StartCoroutine(
            ShowRoutine(
                level,
                statPoints));
    }

    IEnumerator ShowRoutine(
        int level,
        int statPoints)
    {
        notificationText.gameObject
            .SetActive(true);

        notificationText.text =
            "Congratulations! You reached level "
            + level +
            ". You now have "
            + statPoints +
            " points to spend on attribute increases!";

        yield return
            new WaitForSeconds(8f);

        notificationText.gameObject
            .SetActive(false);
    }
}