using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public PlayerBehavior _playerBehavior;
    public TextMeshProUGUI Score;

    public ScoreUI(PlayerBehavior playerBehavior)
    {
        _playerBehavior = playerBehavior;
    }

    public void Update()
    {
        Score.text = _playerBehavior.score.ToString();
    }
}
