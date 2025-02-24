using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AddButton : MonoBehaviour
{
    public GameManager manager;
    public GameObject Spectrobe;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    void OnMouseDown()
    {
        manager.currentSpectrobes.Add(Instantiate(Spectrobe));

            
    }

    // Update is called once per frame

   
}
