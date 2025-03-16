using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private Transform car;
    private ProgressSlider progressSlider;
    private float startZ;
    private float finishZ;
    private float maxProgress = 0f; // Stores the max filled value
    
    
    public void Initialize(Transform carTransform, ProgressSlider slider)
    {
        car = carTransform;
        progressSlider = slider;

        if (car != null)
        {
            startZ = car.position.z;
            finishZ = transform.position.z;
        }
    }

    void Update()
    {
        if (car == null || progressSlider == null) return;

        float totalDistance = finishZ - startZ;
        float currentDistance = car.position.z - startZ;

        // Calculate progress as a percentage
        float progress = Mathf.Clamp01(currentDistance / totalDistance);

        // Prevent slider from decreasing past the max recorded value
        maxProgress = Mathf.Max(maxProgress, progress);
        progressSlider.SetProgress(maxProgress);
    }
    
    public void ResetProgress()
    {
        maxProgress = 0f;
        if (progressSlider != null)
        {
            progressSlider.SetProgress(0f);
        }
    }
}
