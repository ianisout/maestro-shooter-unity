using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDisplay : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] Slider healthSlider;
    [SerializeField] Health playerHealth;

    [Header("Special")]
    [SerializeField] Slider specialSlider;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI scoreText;

    ScoreKeeper scoreKeeper;
    Image[] images;

    byte color = 55;

    void Awake()
    {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        images = FindObjectsOfType<Image>();
    }

    void Start()
    {
        healthSlider.maxValue = playerHealth.GetHealth();
        specialSlider.maxValue = 100;
    }

    void Update()
    {
        healthSlider.value = playerHealth.GetHealth();
        scoreText.text = scoreKeeper.GetScore().ToString("0");
        
        RaiseSpecial();
    }

    void RaiseSpecial()
    {
        if (specialSlider.value < 100)
        {
            specialSlider.value += 0.25f;
        }

        if (Math.Round(specialSlider.value % 10) == 0/*  && color < 255 */)
        {
            color += 4;

            images[1].color = new Color32(0,0,0, color);
        }
    }

    public Color32 ResetSliderColor()
    {
        color = 55;
        images[1].color = new Color32(0,0,0, 55);
        return images[1].color = new Color32(0,0,0, 55);;
    }
}
