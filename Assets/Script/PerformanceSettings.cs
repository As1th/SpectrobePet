using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PerformanceSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UniversalRenderPipelineAsset urpAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        if (urpAsset != null)
        {
            urpAsset.renderScale = 0.5f;
        }
        Application.targetFrameRate = 30; // Cap FPS
        QualitySettings.vSyncCount = 0; // Disable V-Sync for better FPS control
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
