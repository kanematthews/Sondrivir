using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// =========================================================
// QUEST TRACKER UI SETUP
// =========================================================
// Run via:  MMO > Setup Quest Tracker UI
//
// Places the tracker on HUDRoot (ScaleWithScreenSize 1920x1080)
// anchored to the TOP-RIGHT corner — standard MMO layout.
// Safe to re-run: removes any existing tracker first.
// =========================================================

public static class QuestTrackerUISetup
{
    [MenuItem("MMO/Setup Quest Tracker UI")]
    public static void Run()
    {
        // ── FIND PARENT ───────────────────────────────
        // We place on HUDRoot directly — it uses
        // ScaleWithScreenSize 1920×1080 so positions
        // are consistent across all resolutions.

        GameObject hudRoot =
            GameObject.Find("HUDRoot");

        if (hudRoot == null)
        {
            Debug.LogError(
                "[QuestTrackerSetup] Could not find 'HUDRoot' in scene.");
            return;
        }

        // Canvas component must exist on HUDRoot
        Canvas canvas =
            hudRoot.GetComponent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError(
                "[QuestTrackerSetup] HUDRoot has no Canvas component.");
            return;
        }

        Transform parent = hudRoot.transform;

        // ── REMOVE OLD TRACKER ────────────────────────

        Transform existing =
            FindDeep(parent, "QuestTracker");

        if (existing != null)
        {
            Object.DestroyImmediate(
                existing.gameObject);
        }

        // ── ROOT PANEL ────────────────────────────────

        GameObject tracker =
            CreateGO("QuestTracker", parent);

        RectTransform rt =
            tracker.GetComponent<RectTransform>();

        // Anchor: top-left, pivot: top-left
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot     = new Vector2(0f, 1f);

        // 10px from left edge, 10px from top
        rt.anchoredPosition = new Vector2(10f, -10f);
        rt.sizeDelta        = new Vector2(210f, 0f);

        // Height driven by children
        ContentSizeFitter rootFitter =
            tracker.AddComponent<ContentSizeFitter>();
        rootFitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        VerticalLayoutGroup rootVLG =
            tracker.AddComponent<VerticalLayoutGroup>();
        rootVLG.childAlignment      = TextAnchor.UpperRight;
        rootVLG.spacing             = 0f;
        rootVLG.padding             = new RectOffset(0, 0, 0, 0);
        rootVLG.childForceExpandWidth  = true;
        rootVLG.childForceExpandHeight = false;
        rootVLG.childControlWidth      = true;
        rootVLG.childControlHeight     = true;

        // ── DARK BACKGROUND ───────────────────────────

        GameObject bg = CreateGO("Background", tracker.transform);

        LayoutElement bgLayout = bg.AddComponent<LayoutElement>();
        bgLayout.ignoreLayout = true;

        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = new Vector2(-4f, -4f);
        bgRT.offsetMax = new Vector2(4f,  4f);

        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.04f, 0.04f, 0.08f, 0.88f);

        bg.transform.SetAsFirstSibling();

        // ── RIGHT-EDGE GOLD ACCENT BAR ────────────────

        GameObject edge = CreateGO("EdgeAccent", tracker.transform);

        LayoutElement edgeLayout = edge.AddComponent<LayoutElement>();
        edgeLayout.ignoreLayout = true;

        RectTransform edgeRT = edge.GetComponent<RectTransform>();
        edgeRT.anchorMin = new Vector2(1f, 0f);
        edgeRT.anchorMax = new Vector2(1f, 1f);
        edgeRT.pivot     = new Vector2(1f, 0.5f);
        edgeRT.anchoredPosition = Vector2.zero;
        edgeRT.sizeDelta        = new Vector2(3f, 0f);

        Image edgeImg = edge.AddComponent<Image>();
        edgeImg.color = new Color(1f, 0.75f, 0.20f, 0.9f);

        // ── HEADER ────────────────────────────────────

        GameObject header = CreateGO("Header", tracker.transform);

        LayoutElement headerLayout =
            header.AddComponent<LayoutElement>();
        headerLayout.minHeight       = 30f;
        headerLayout.preferredHeight = 30f;

        // Label
        GameObject labelGO = CreateGO("Label", header.transform);

        RectTransform labelRT =
            labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin  = Vector2.zero;
        labelRT.anchorMax  = Vector2.one;
        labelRT.offsetMin  = new Vector2(10f, 0f);
        labelRT.offsetMax  = new Vector2(-10f, 0f);

        TMP_Text label = labelGO.AddComponent<TextMeshProUGUI>();
        label.text      = "⚔  QUESTS";
        label.fontSize  = 11f;
        label.fontStyle = FontStyles.Bold;
        label.color     = new Color(1f, 0.78f, 0.22f, 0.95f);
        label.alignment = TextAlignmentOptions.MidlineRight;
        label.enableWordWrapping = false;

        // Separator
        GameObject sep = CreateGO("Separator", tracker.transform);

        LayoutElement sepLayout = sep.AddComponent<LayoutElement>();
        sepLayout.minHeight       = 1f;
        sepLayout.preferredHeight = 1f;

        Image sepImg = sep.AddComponent<Image>();
        sepImg.color = new Color(1f, 0.75f, 0.20f, 0.30f);

        // ── OBJECTIVE CONTAINER ───────────────────────

        GameObject container =
            CreateGO("ObjectiveContainer", tracker.transform);

        ContentSizeFitter containerFitter =
            container.AddComponent<ContentSizeFitter>();
        containerFitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        VerticalLayoutGroup containerVLG =
            container.AddComponent<VerticalLayoutGroup>();
        containerVLG.childAlignment      = TextAnchor.UpperRight;
        containerVLG.spacing             = 2f;
        containerVLG.padding             = new RectOffset(10, 12, 5, 8);
        containerVLG.childForceExpandWidth  = true;
        containerVLG.childForceExpandHeight = false;
        containerVLG.childControlWidth      = true;
        containerVLG.childControlHeight     = true;

        // ── WIRE COMPONENT ────────────────────────────

        QuestTrackerUI trackerUI =
            tracker.AddComponent<QuestTrackerUI>();
        trackerUI.objectiveContainer = container.transform;

        tracker.SetActive(false);

        // ── FINISH ────────────────────────────────────

        EditorUtility.SetDirty(hudRoot);
        UnityEditor.SceneManagement
            .EditorSceneManager
            .MarkSceneDirty(hudRoot.scene);

        Debug.Log(
            "[QuestTrackerSetup] Quest Tracker built at top-right of HUDRoot.");

        Selection.activeGameObject = tracker;
    }

    // ─────────────────────────────────────────────────────

    static Transform FindDeep(Transform root, string name)
    {
        foreach (Transform t in
            root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name) return t;
        }
        return null;
    }

    static GameObject CreateGO(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.AddComponent<RectTransform>();
        go.transform.SetParent(parent, false);
        return go;
    }
}
