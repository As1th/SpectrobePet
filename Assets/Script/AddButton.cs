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
        var mon = Instantiate(Spectrobe);
        manager.currentSpectrobes.Add(mon);
        foreach(GameObject min in manager.currentMinerals)
        {
            min.GetComponentInChildren<ScreenSpaceCollisionDetector>().otherObjects.Add(mon.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);
        }
            
    }

    // Update is called once per frame

   
}
