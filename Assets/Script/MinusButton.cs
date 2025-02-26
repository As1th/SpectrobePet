using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MinusButton : MonoBehaviour
{
    public GameManager manager;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    void OnMouseDown()
    {
        if (manager.currentSpectrobes.Count > 1)
        {
            GameObject mon = manager.SelectedSpectrobe.gameObject;
            manager.currentSpectrobes.Remove(mon);


            manager.SelectedSpectrobe = manager.currentSpectrobes[0].GetComponent<DragGameSprite>();
            manager.SelectedSpectrobe.OnMouseUp();

            manager.Menu.GetComponent<UIFollowGameObject>().targetObject = manager.SelectedSpectrobe.transform;
            manager.Ring.GetComponent<UIFollowGameObject>().targetObject = manager.SelectedSpectrobe.transform;
            //manager.SelectedSpectrobe.GetComponentInChildren<Outline>().enabled = true;

            foreach (GameObject min in manager.currentMinerals)
            {
                min.GetComponentInChildren<ScreenSpaceCollisionDetector>().otherObjects.Remove(mon.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);
            }
            Destroy(mon);
        }
            
    }

    // Update is called once per frame

   
}
