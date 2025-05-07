using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MonitorButton : MonoBehaviour
{
    private int currentMonitor = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }

    void OnMouseDown()
    {
        currentMonitor++;
        if(currentMonitor >= Display.displays.Length)
        {
            currentMonitor = 0;
        }
        Camera.main.targetDisplay = currentMonitor;
    }

    // Update is called once per frame

   
}
