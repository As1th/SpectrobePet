using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class WalkButton : MonoBehaviour
{
    public GameManager manager;
    private DragGameSprite Spectrobe;
    private SpriteRenderer icon;
    // Start is called before the first frame update
    void Start()
    {
        Spectrobe = manager.Spectrobe;
        icon = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame

    void OnMouseDown()
    {
        Toggle();
    }
    void Toggle()
    {
        if (Spectrobe.walkCycle)
        {
            Spectrobe.walkCycle = false;
            icon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Spectrobe.walkCycle = true;
           
            icon.color = new Color(1, 1, 1, 0.6f);

        }


    }
}
