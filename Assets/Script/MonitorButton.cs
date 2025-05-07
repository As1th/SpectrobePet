using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MonitorButton : MonoBehaviour
{
    private int currentMonitor = 0;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        StartCoroutine(BlankDisplay(currentMonitor));
        currentMonitor++;
        if(currentMonitor >= 3)
        {
            currentMonitor = 0;
        }
       
       cam.targetDisplay = currentMonitor;
      
    }

    IEnumerator BlankDisplay(int displayIndex)
    {
        var go = new GameObject("Blanker");
        var cam = go.AddComponent<Camera>();
        cam.targetDisplay = displayIndex;
        
        cam.depth = -100;         // render before your real camera
        cam.cullingMask = 0;
        yield return new WaitForSeconds(0.1f);
        yield return null;        // wait one frame
        Destroy(go);
    }


}
