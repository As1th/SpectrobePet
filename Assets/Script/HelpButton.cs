using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
       
    }

    void OnMouseDown()
    {
        Application.OpenURL("https://asith.itch.io/spectrobe-pet");
    }

    // Update is called once per frame

   
}
