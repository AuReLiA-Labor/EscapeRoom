using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_updated : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text messageText;

    [Header("Panels")]
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject completePanel;

    [Header("Actions")]
    [SerializeField] private Button signButton;

    public void SetTimer(float seconds)
    {
        if (!timerText) return;
        int s = Mathf.CeilToInt(seconds);
        int m = s / 60;
        int r = s % 60;
        timerText.text = $"{m:00}:{r:00}";
    }

    public void SetScore(int score)
    {
        if (scoreText) scoreText.text = $"Points: {score}";
    }

    public void ShowHint(string msg)
    {
        if (messageText) messageText.text = msg;
    }

    public void SetSigningButtonVisible(bool visible)
    {
        if (signButton) signButton.gameObject.SetActive(visible);
    }

    public void ShowWelcome(bool show) { if (welcomePanel) welcomePanel.SetActive(show); }
    public void ShowFail(bool show)    { if (failPanel) failPanel.SetActive(show); }
    public void ShowComplete(bool show){ if (completePanel) completePanel.SetActive(show); }
}