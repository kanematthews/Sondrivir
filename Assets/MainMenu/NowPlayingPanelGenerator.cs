using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NowPlayingPanelGenerator : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip musicClip;

    private AudioSource audioSource;

    private readonly List<Image> bars =
        new List<Image>();

    private readonly float[] spectrum =
        new float[512];

    private bool paused;

    private TMP_Text pauseText;

    void Start()
    {
        GeneratePanel();
    }

    void Update()
    {
        UpdateSpectrum();
    }

    void GeneratePanel()
    {
        //
        // AUDIO SOURCE
        //
        GameObject audioObj =
            new GameObject(
                "MusicSource"
            );

        audioObj.transform.SetParent(
            transform,
            false
        );

        audioSource =
            audioObj.AddComponent<AudioSource>();

        audioSource.clip =
            musicClip;

        audioSource.loop =
            true;

        audioSource.playOnAwake =
            true;

        audioSource.volume =
            0.4f;

        audioSource.Play();

        //
        // UI
        //
        CreateBars();

        CreatePauseButton();

        CreateVolumeSlider();
    }

    void CreateBars()
    {
        GameObject container =
            new GameObject(
                "BarsContainer"
            );

        container.transform.SetParent(
            transform,
            false
        );

        RectTransform rect =
            container.AddComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.anchorMax =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.pivot =
            new Vector2(
                0.5f,
                0.5f
            );

        //
        // PERFECT POSITION
        //
        rect.anchoredPosition =
            new Vector2(
                0,
                5
            );

        //
        // FITS INSIDE PANEL
        //
        rect.sizeDelta =
            new Vector2(
                210,
                10
            );

        //
        // MASK
        //
        container.AddComponent<RectMask2D>();

        //
        // MANY SMALL BARS
        //
        int barCount = 36;

        float spacing =
            rect.sizeDelta.x /
            barCount;

        for (
            int i = 0;
            i < barCount;
            i++
        )
        {
            GameObject bar =
                new GameObject(
                    "Bar_" + i
                );

            bar.transform.SetParent(
                container.transform,
                false
            );

            Image image =
                bar.AddComponent<Image>();

            image.color =
                new Color32(
                    40,
                    170,
                    255,
                    255
                );

            RectTransform barRect =
                image.rectTransform;

            barRect.anchorMin =
                new Vector2(
                    0,
                    0
                );

            barRect.anchorMax =
                new Vector2(
                    0,
                    0
                );

            barRect.pivot =
                new Vector2(
                    0,
                    0
                );

            //
            // FITTED INSIDE WIDTH
            //
            barRect.anchoredPosition =
                new Vector2(
                    i * spacing,
                    0
                );

            //
            // TINY BARS
            //
            barRect.sizeDelta =
                new Vector2(
                    1.5f,
                    2f
                );

            bars.Add(image);
        }
    }

    void CreatePauseButton()
    {
        GameObject buttonObj =
            new GameObject(
                "PauseButton"
            );

        buttonObj.transform.SetParent(
            transform,
            false
        );

        Image image =
            buttonObj.AddComponent<Image>();

        image.color =
            new Color(
                0,
                0,
                0,
                0
            );

        Button button =
            buttonObj.AddComponent<Button>();

        RectTransform rect =
            buttonObj.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.anchorMax =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.pivot =
            new Vector2(
                0.5f,
                0.5f
            );

        //
        // SLIGHTLY LOWER
        //
        rect.anchoredPosition =
            new Vector2(
                0,
                -10
            );

        rect.sizeDelta =
            new Vector2(
                14,
                14
            );

        //
        // ICON
        //
        GameObject textObj =
            new GameObject(
                "Icon"
            );

        textObj.transform.SetParent(
            buttonObj.transform,
            false
        );

        pauseText =
            textObj.AddComponent<TextMeshProUGUI>();

        pauseText.text =
            "II";

        pauseText.fontSize =
            8;

        pauseText.color =
            new Color32(
                230,
                190,
                100,
                255
            );

        pauseText.alignment =
            TextAlignmentOptions.Center;

        RectTransform textRect =
            pauseText.rectTransform;

        textRect.anchorMin =
            Vector2.zero;

        textRect.anchorMax =
            Vector2.one;

        textRect.offsetMin =
            Vector2.zero;

        textRect.offsetMax =
            Vector2.zero;

        button.onClick
            .AddListener(
                TogglePause
            );
    }

    void CreateVolumeSlider()
    {
        GameObject sliderObj =
            new GameObject(
                "VolumeSlider"
            );

        sliderObj.transform.SetParent(
            transform,
            false
        );

        Slider slider =
            sliderObj.AddComponent<Slider>();

        RectTransform rect =
            sliderObj.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.anchorMax =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.pivot =
            new Vector2(
                0.5f,
                0.5f
            );

        //
        // LOWER SLIGHTLY
        //
        rect.anchoredPosition =
            new Vector2(
                0,
                -24
            );

        //
        // SMALLER
        //
        rect.sizeDelta =
            new Vector2(
                72,
                3
            );

        //
        // BACKGROUND
        //
        GameObject bgObj =
            new GameObject(
                "Background"
            );

        bgObj.transform.SetParent(
            sliderObj.transform,
            false
        );

        Image bg =
            bgObj.AddComponent<Image>();

        bg.color =
            new Color32(
                60,
                40,
                20,
                255
            );

        RectTransform bgRect =
            bg.rectTransform;

        bgRect.anchorMin =
            Vector2.zero;

        bgRect.anchorMax =
            Vector2.one;

        bgRect.offsetMin =
            Vector2.zero;

        bgRect.offsetMax =
            Vector2.zero;

        //
        // FILL
        //
        GameObject fillObj =
            new GameObject(
                "Fill"
            );

        fillObj.transform.SetParent(
            sliderObj.transform,
            false
        );

        Image fill =
            fillObj.AddComponent<Image>();

        fill.color =
            new Color32(
                220,
                180,
                90,
                255
            );

        RectTransform fillRect =
            fill.rectTransform;

        fillRect.anchorMin =
            new Vector2(
                0,
                0
            );

        fillRect.anchorMax =
            new Vector2(
                1,
                1
            );

        fillRect.offsetMin =
            Vector2.zero;

        fillRect.offsetMax =
            Vector2.zero;

        //
        // HANDLE
        //
        GameObject handleObj =
            new GameObject(
                "Handle"
            );

        handleObj.transform.SetParent(
            sliderObj.transform,
            false
        );

        Image handle =
            handleObj.AddComponent<Image>();

        handle.color =
            new Color32(
                255,
                220,
                140,
                255
            );

        RectTransform handleRect =
            handle.rectTransform;

        handleRect.sizeDelta =
            new Vector2(
                4,
                8
            );

        slider.fillRect =
            fillRect;

        slider.handleRect =
            handleRect;

        slider.targetGraphic =
            handle;

        slider.direction =
            Slider.Direction.LeftToRight;

        slider.minValue =
            0f;

        slider.maxValue =
            1f;

        slider.value =
            0.7f;

        slider.onValueChanged
            .AddListener(
                SetVolume
            );
    }

    void UpdateSpectrum()
    {
        if (audioSource == null)
            return;

        audioSource.GetSpectrumData(
            spectrum,
            0,
            FFTWindow.Blackman
        );

        for (
            int i = 0;
            i < bars.Count;
            i++
        )
        {
            float intensity =
                spectrum[i] * 240f;

            float targetHeight =
                Mathf.Clamp(
                    intensity,
                    1f,
                    5f
                );

            RectTransform rect =
                bars[i].rectTransform;

            Vector2 size =
                rect.sizeDelta;

            size.y =
                Mathf.Lerp(
                    size.y,
                    targetHeight,
                    Time.deltaTime * 10f
                );

            rect.sizeDelta =
                size;
        }
    }

    void TogglePause()
    {
        paused = !paused;

        if (paused)
        {
            audioSource.Pause();

            pauseText.text =
                "▶";
        }
        else
        {
            audioSource.UnPause();

            pauseText.text =
                "II";
        }
    }

    void SetVolume(
        float value
    )
    {
        audioSource.volume =
            value;
    }
}