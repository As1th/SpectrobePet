using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject monitorButton;
    // Start is called before the first frame update
    void Start()
    {
        if (Display.displays.Length < 2) {
            monitorButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
