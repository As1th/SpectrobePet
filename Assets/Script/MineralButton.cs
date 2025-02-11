using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MineralButton : MonoBehaviour
{
    public GameManager manager;
    private DragGameSprite Spectrobe;
    private SpriteRenderer icon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    void OnMouseDown()
    {
        manager.SpawnMineral();
    }
    
}
