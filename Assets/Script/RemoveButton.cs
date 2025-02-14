using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class RemoveButton : MonoBehaviour
{
    public GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    void OnMouseDown()
    {
       if(manager.currentSpectrobes.Count > 1)
        {
            Destroy(transform.root.gameObject.GetComponent<UIFollowGameObject>().targetObject.root.gameObject);
            Destroy(transform.root.gameObject);

        }
        else {

        }
    }

    // Update is called once per frame

   
}
