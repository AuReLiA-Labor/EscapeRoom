using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Inventory UI")]
    public InventoryUI inventoryUI;
    
    [Header("Core UI")]
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text messageText;

    [Header("Panels")]
    public GameObject welcomePanel;
    public GameObject failPanel;
    public GameObject completePanel;

    [Header("Buttons / Hints")]
    public Button signButton;
    public Button startButton;      // “Start / Play” on your welcome
    public GameObject useKeyHint;

    void Awake()
    {
        // Safe defaults on load
        if (failPanel) failPanel.SetActive(false);
        if (completePanel) completePanel.SetActive(false);
        if (useKeyHint) useKeyHint.SetActive(false);
        if (signButton) signButton.gameObject.SetActive(false); // hidden until allowed
    }

    void Start()
    {
        if (startButton) startButton.onClick.AddListener(HideWelcome);
        // If you want the welcome to be visible on scene start:
        if (welcomePanel && !welcomePanel.activeSelf) ShowWelcome();
    }

    public void SetTimer(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        if (timerText) timerText.text = $"{m:00}:{s:00}";
    }

    public void SetScore(int value)
    {
        if (scoreText) scoreText.text = $"Points: {value}";
    }

    public void ShowWelcome()
    {
        if (welcomePanel) welcomePanel.SetActive(true);
        if (messageText)
            messageText.text = "Welcome! This hiring system has been corrupted by unconscious bias. Find the true facts to restore it and escape!";
    }

    public void HideWelcome()
    {
        if (welcomePanel) welcomePanel.SetActive(false);
    }

    public void ShowFail()
    {
        if (failPanel) failPanel.SetActive(true);
    }

    public void ShowComplete()
    {
        if (completePanel) completePanel.SetActive(true);
    }

    // Instant hint
    public void ShowHint(string text)
    {
        if (messageText) messageText.text = text;
    }

    // Optional: timed hint that auto-clears after 'seconds'
    public void ShowHint(string text, float seconds)
    {
        ShowHint(text);
        CancelInvoke(nameof(ClearHint));
        Invoke(nameof(ClearHint), Mathf.Max(0.1f, seconds));
    }

    void ClearHint()
    {
        if (messageText) messageText.text = string.Empty;
    }

    public void SetSigningButtonVisible(bool visible)
    {
        if (signButton) signButton.gameObject.SetActive(visible);
    }

    public void SetUseKeyHintVisible(bool visible)
    {
        if (useKeyHint) useKeyHint.SetActive(visible);
    }
    public void ToggleInventoryUI()
    {
        if (inventoryUI) inventoryUI.TogglePanel();
    }
}
