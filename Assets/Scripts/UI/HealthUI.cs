using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerBehavior _playerBehavior;
    public Slider healthBarSlider;

    public HealthUI(PlayerBehavior playerBehavior)
    {
        _playerBehavior = playerBehavior;
    }

    public void Update()
    {
        healthBarSlider.value = _playerBehavior._playerHealth.Health;
        healthBarSlider.maxValue = _playerBehavior._playerHealth.MaxHealth;
    }
}
