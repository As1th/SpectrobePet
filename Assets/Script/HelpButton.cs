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
        Application.OpenURL("https://github.com/As1th/SpectrobePet/blob/main/README.md#basic-controls");
    }

    // Update is called once per frame

   
}
