using System;
using UnityEngine;
using TMPro;

public class Book : MonoBehaviour
{
    public float targetPct = 1.0f;
    public float glowAmount = 0.0f;
    public float glowLerpSpeed = 3.0f;
    public float outlineAmount = 0.0f;
    public float openLerpSpeed = 1.0f;
    public float openThreshold = 0.02f;
    public bool toOpen;

    public PlaceLerper placeLerper => _placeLerper;
    public bool isHovered { get; private set; }
    public bool isOpen { get; private set; }
    public float normalizedPct => currentPct / targetPct;

    [ContextMenu("Set Values")]
    public void SetValues() => LerpValues(true);

    public void LerpValues(bool set = false)
    {
        // Lerp book open pct
        if (!set) currentPct = Mathf.Lerp(currentPct, toOpen ? targetPct : 0.0f, Time.deltaTime * openLerpSpeed);
        else currentPct = toOpen ? targetPct : 0.0f;
        animator.SetFloat("normalizedTime", currentPct);
        isOpen = toOpen && (1 - normalizedPct) < openThreshold;

        // Lerp light intensities
        foreach (Light light in lights)
        {
            if (!set) light.intensity = Mathf.Lerp(light.intensity, glowAmount * glowIntensity, Time.deltaTime * glowLerpSpeed);
            else light.intensity = glowAmount * glowIntensity;
        }

        // Lerp outline color
        Color targetColor = outlineColor;
        targetColor.a *= outlineAmount;
        if (!set) outline.OutlineColor = Color.Lerp(outline.OutlineColor, targetColor, Time.deltaTime * outlineLerpSpeed);
        else outline.OutlineColor = targetColor;
    }

    public void SetContentTitle(String text_) => contentTitleText.text = text_;

    public void SetContentDescription(String text_) => contentDescriptionText.text = text_;

    [Header("References")]
    [SerializeField] private Outline outline;
    [SerializeField] private Animator animator;
    [SerializeField] private Light[] lights;
    [SerializeField] private PlaceLerper _placeLerper;

    [Header("Content References")]
    [SerializeField] private GameObject contentUI;
    [SerializeField] private TextMeshProUGUI contentTitleText;
    [SerializeField] private TextMeshProUGUI contentDescriptionText;

    [Header("Config")]
    [SerializeField] private float glowIntensity = 50.0f;
    [SerializeField] private Color outlineColor = new Color(200.0f, 100.0f, 100.0f, 255.0f);
    [SerializeField] private float outlineLerpSpeed = 3.0f;

    private float currentPct;

    private void Start()
    {
        placeLerper.SetPlace("Default");
    }

    private void Update()
    {
        // Set UI
        SetContentTitle("");
        SetContentDescription("");
        contentUI.SetActive(isOpen);

        // Lerp position and all variables
        placeLerper.Lerp();
        LerpValues();
    }

    private void OnMouseOver() => isHovered = true;

    private void OnMouseExit() => isHovered = false;
}
