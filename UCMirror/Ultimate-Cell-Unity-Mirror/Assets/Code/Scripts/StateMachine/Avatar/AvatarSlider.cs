using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class AvatarSlider : MonoBehaviour
{
    Slider slider;
    public Slider Slider
    {
        get
        {
            if(!slider)slider = transform.GetChild(0).GetComponent<Slider>();
            return slider;
        }
    }
    public void SetSliderValue(float value)
    {
        Slider.value = value;
    }
    public void ShakingAnimation(float duration)
    {
        float strength = 10;
        int vibrato = 10;
        float randomness = 90;
        bool fadeOut = true;
        bool snapping = true;
        transform.GetComponent<RectTransform>().DOShakePosition(duration,strength,vibrato,randomness,fadeOut,snapping);
    }
}