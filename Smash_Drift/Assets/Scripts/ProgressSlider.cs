using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressSlider : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;

    public void SetProgress(float value)
    {
        if (progressSlider != null)
        {
            progressSlider.value = value;
        }
    }
    
    public void ResetValue()
    {
        progressSlider.value = 0f;
    }
}
