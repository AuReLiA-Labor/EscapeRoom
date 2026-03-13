using System; // <-- add this
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextWritter : MonoBehaviour
{
    private TMP_Text uiText;
    private string textToWrite;
    private float timePerCharacter;
    private float timer;
    private int characterIndex;

    // NEW: keep a callback for when a write completes
    private Action onComplete;

    // Kept same structure, just added optional callback + proper resets
    public void AddWriter(TMP_Text uiText, string textToWrite, float timePerCharacter, Action onComplete = null)
    {
        this.uiText = uiText;
        this.textToWrite = textToWrite;
        this.timePerCharacter = timePerCharacter;
        this.onComplete = onComplete;

        // reset state for a clean start
        this.timer = timePerCharacter;
        this.characterIndex = 0;
        if (this.uiText != null) this.uiText.text = string.Empty;
    }

    private void Update()
    {
        if (uiText != null)
        {
            // countdown, not up
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer += timePerCharacter;
                characterIndex++;

                if (characterIndex <= textToWrite.Length)
                {
                    uiText.text = textToWrite.Substring(0, characterIndex);
                }
                else
                {
                    // finished: clear and notify
                    uiText = null;
                    var cb = onComplete; // avoid double invoke
                    onComplete = null;
                    cb?.Invoke();
                }
            }
        }
    }
}