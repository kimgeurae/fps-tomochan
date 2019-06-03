using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CodeMonkey;
using CodeMonkey.Utils;

public class HealthBarFade : MonoBehaviour
{

    private const float DAMAGED_HEALTH_FADE_TIMER_MAX = 1f;
    private HealthSystem healthSystem;
    private Image barImage;
    private Image damagedBarImage;
    private Color damagedColor;
    private float damagedHealthFadeTimer;

    private void Awake()
    {
        barImage = transform.Find("HealthBar").GetComponent<Image>();
        damagedBarImage = transform.Find("DamagedHealthBar").GetComponent<Image>();
        damagedColor = damagedBarImage.color;
        damagedColor.a = 0f;
        damagedBarImage.color = damagedColor;
    }

    private void Start()
    {
        healthSystem = new HealthSystem(100);
        SetHealth(healthSystem.GetHealthNormalized());
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealed += HealthSystem_OnHealed;

        CMDebug.ButtonUI(new Vector2(-100, -50), "Damage", () => healthSystem.Damage(10));
        CMDebug.ButtonUI(new Vector2(+100, -50), "Heal", () => healthSystem.Heal(10));
    }

    private void Update()
    {
        if (damagedColor.a > 0)
        {
            damagedHealthFadeTimer -= Time.deltaTime;
            if (damagedHealthFadeTimer < 0)
            {
                float fadeAmount = 5f;
                damagedColor.a -= fadeAmount * Time.deltaTime;
                damagedBarImage.color = damagedColor;
            }
        }
    }

    private void HealthSystem_OnHealed(object sender, EventArgs e)
    {
        SetHealth(healthSystem.GetHealthNormalized());
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        if (damagedColor.a <= 0)
        {
            // Damage bar is invisible
            damagedBarImage.fillAmount = barImage.fillAmount;
        }
        damagedColor.a = 1;
        damagedBarImage.color = damagedColor;
        damagedHealthFadeTimer = DAMAGED_HEALTH_FADE_TIMER_MAX;

        SetHealth(healthSystem.GetHealthNormalized());
    }

    private void SetHealth(float healthNormalized)
    {
        barImage.fillAmount = healthNormalized;
    }

}
