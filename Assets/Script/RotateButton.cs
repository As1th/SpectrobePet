using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class RotateButton : MonoBehaviour
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
        RotateMode();
    }
    void RotateMode()
    {
        if (Spectrobe.rotateMode)
        {
            Spectrobe.rotateMode = false;
            Spectrobe.Ring.SetActive(false);
            icon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Spectrobe.rotateMode = true;
            Spectrobe.Ring.SetActive(true);
            icon.color = new Color(1, 1, 1, 0.6f);

        }


    }
}
