using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Assistant : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TextWritter textToWrite;

    // Put your rounds here (edit in Inspector or inline)
    [SerializeField] private string[] rounds = new[]
    {
        "This is the assistant for the game you want to play now \nseee you another time !",
        "This is the assistant for the game you want to play now \nseee you another time !",
        "This is the assistant for the game you want to play now \nseee you another time !"
    };

    private int roundIndex = 0;

    private void Awake()
    {
        messageText = GameObject.Find("MessageText").GetComponent<TMP_Text>();
        if (textToWrite == null) textToWrite = GetComponent<TextWritter>();
    }

    private void Start()
    {
        PlayNextRound();
    }

    private void PlayNextRound()
    {
        if (rounds == null || rounds.Length == 0) return;

        if (roundIndex >= rounds.Length) roundIndex = 0; // loop when full count reached
        string next = rounds[roundIndex++];
        // 1f keeps your original pacing (1 char per second). Adjust if you want it faster.
        textToWrite.AddWriter(messageText, next, 1f, PlayNextRound);
    }
}