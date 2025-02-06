using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateModeButton : MonoBehaviour
{
    public GameManager manager;
    private DragGameSprite Spectrobe; 
    // Start is called before the first frame update
    void Start()
    {
        Spectrobe = manager.Spectrobe;
    }

    // Update is called once per frame

    void OnMouseDown()
    {
        RotateMode();
    }
    void RotateMode()
    {
        if (Spectrobe.rotateMode)
        {
            Spectrobe.rotateMode = false;
        }
        else
        {
            Spectrobe.rotateMode = true;
        }


    }
}
