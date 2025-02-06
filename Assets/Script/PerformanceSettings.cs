using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PerformanceSettings : MonoBehaviour
{
    // Serialized field allows you to modify it in the Inspector.
    [SerializeField]
    private int targetFPS = 30;

    // Public property so you can also change it from code if needed.
    public int TargetFPS
    {
        get => targetFPS;
        set
        {
            targetFPS = value;
            UpdateTargetFPS();
        }
    }

    void Start()
    {
        // Set the initial FPS and disable V-Sync.
        QualitySettings.vSyncCount = 0;
        UpdateTargetFPS();
    }

    // This method applies the current targetFPS setting.
    void UpdateTargetFPS()
    {
        Application.targetFrameRate = targetFPS;
    }

    // OnValidate is called in the Editor when the script is loaded or a value is changed in the Inspector.
    void OnValidate()
    {
        UpdateTargetFPS();
    }
}
