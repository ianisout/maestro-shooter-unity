using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpecial : MonoBehaviour
{
    [SerializeField] Slider specialSlider;

    public bool GetSpecial()
    {
        if (specialSlider.value == 100)
        {
            return true;
        }
        return false;
    }

    public void ResetSpecial()
    {
        specialSlider.value = 0;
    }
}
