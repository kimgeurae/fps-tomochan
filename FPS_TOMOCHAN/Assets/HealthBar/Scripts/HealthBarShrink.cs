using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CodeMonkey;
using CodeMonkey.Utils;

public class HealthBarShrink : MonoBehaviour
{

    private const float DAMAGED_HEALTH_SHRINK_TIMER_MAX = 1f;

    [HideInInspector]
    public HealthSystem healthSystem;
    private Image barImage;
    private Image damagedBarImage;
    private float damagedHealthShrinkTimer;
    public SetMaxHealth setMaxHealth;

    private void Awake()
    {
        barImage = transform.Find("HealthBar").GetComponent<Image>();
        damagedBarImage = transform.Find("DamagedHealthBar").GetComponent<Image>();
    }

    private void Start()
    {
        healthSystem = new HealthSystem(setMaxHealth.maximumHealth);
        SetHealth(healthSystem.GetHealthNormalized());
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealed += HealthSystem_OnHealed;
        damagedBarImage.fillAmount = barImage.fillAmount;

        // Test Buttons
        // CMDebug.ButtonUI(new Vector2(-100, -50), "Damage", () => healthSystem.Damage(10));
        // CMDebug.ButtonUI(new Vector2(+100, -50), "Heal", () => healthSystem.Heal(10));
    }

    private void Update()
    {
        damagedHealthShrinkTimer -= Time.deltaTime;
        if (damagedHealthShrinkTimer < 0)
        {
            if (barImage.fillAmount < damagedBarImage.fillAmount)
            {
                float shrinkSpeed = 1f;
                damagedBarImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
    }

    private void HealthSystem_OnHealed(object sender, EventArgs e)
    {
        SetHealth(healthSystem.GetHealthNormalized());
        damagedBarImage.fillAmount = barImage.fillAmount;
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        damagedHealthShrinkTimer = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
        SetHealth(healthSystem.GetHealthNormalized());
    }

    private void SetHealth(float healthNormalized)
    {
        barImage.fillAmount = healthNormalized;
    }

}
