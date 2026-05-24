using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryUI : MonoBehaviour
{
    public TMP_Text titleText;

    public Button collapseButton;

    public RectTransform contentRoot;

    private bool expanded = true;

    public void Setup(string categoryName)
    {
        titleText.text = categoryName;

        collapseButton.onClick.AddListener(
            Toggle
        );
    }

    void Toggle()
    {
        expanded = !expanded;

        contentRoot.gameObject.SetActive(
            expanded
        );
    }
}
