using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PerformanceSettings : MonoBehaviour
{
    // Serialized field allows you to modify it in the Inspector.
    [SerializeField]
    private int targetFPS = 30;
    public TextMeshProUGUI test;

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


        // Activate all connected displays
      
       // SetupMultiMonitorCameras();
    }

    void SetupMultiMonitorCameras()
    {
        int totalDisplays = Display.displays.Length;

        if (totalDisplays < 2)
        {
            Debug.LogWarning("Only one display detected. Multi-monitor setup requires 2+ displays.");
            return;
        }

        float totalFOV = 90f; // Total horizontal FOV for the full scene
        float perCameraFOV = totalFOV / totalDisplays;

        for (int i = 0; i < totalDisplays; i++)
        {
            GameObject camObj = new GameObject($"Camera_Display_{i}");
            Camera cam = camObj.AddComponent<Camera>();

            cam.targetDisplay = i;
            cam.fieldOfView = Camera.main.fieldOfView; // Use same vertical FOV as main cam
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.depth = 0;

            // Position & rotate cameras for panoramic effect
            cam.transform.position = Camera.main.transform.position;
            cam.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(0, (i - (totalDisplays - 1) / 2f) * perCameraFOV, 0);
        }

    
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
