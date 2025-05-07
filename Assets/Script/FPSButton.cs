using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FPSButton : MonoBehaviour
{
    public PerformanceSettings settings;
    
    private SpriteRenderer icon;
    public Sprite Icon30;
    public Sprite Icon60;
    public 
    // Start is called before the first frame update
    void Start()
    {
        
        icon = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame

    void OnMouseDown()
    {
        Toggle();
    }
    void Toggle()
    {
        
        if (settings.TargetFPS == 30)
        {
            settings.TargetFPS = 60;
            icon.sprite = Icon60;
        }
        else
        {
            settings.TargetFPS = 30;
            icon.sprite = Icon30;

        }


    }
}
