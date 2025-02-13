using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
       
    }

    void OnMouseDown()
    {
        Application.Quit();
    }

    // Update is called once per frame

   
}
