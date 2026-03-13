using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int score;
    public TMP_Text scoreText;

  public  void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = "Score:" + score.ToString();
    }
}
