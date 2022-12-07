using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;
    
    Damageable playerDamageable;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerDamageable = player.GetComponent<Damageable>();
    }

    void Start()
    {
        OnPlayerHealthChanged(playerDamageable.Health, playerDamageable.MaxHealth);
    }

    private void OnEnable()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnDisable()
    {
        playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    private float CalculateSliderValue(float currentHealth, float maxHealth)
    {
        return currentHealth / maxHealth;
    }

    private void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        healthSlider.value = CalculateSliderValue(newHealth, maxHealth);
        healthBarText.text = newHealth + " / " + maxHealth;
    }
}
