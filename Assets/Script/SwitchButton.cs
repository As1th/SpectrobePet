using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SwitchButton : MonoBehaviour
{
    public GameManager manager;
    private DragGameSprite Spectrobe;
    private SpriteRenderer icon;
    public float ringTime = 0.2f;
    public SpriteRenderer key1;
    public SpriteRenderer key2;
    public SpriteRenderer key3;
    public Sprite key1icon;
    public Sprite key2icon;
    public Sprite key3icon;
    // Start is called before the first frame update
    void Start()
    {
        Spectrobe = manager.Spectrobe;
        icon = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame

    void OnMouseDown()
    {
        SwitchMode();
    }
    void SwitchMode()
    {
        if (Spectrobe.switchMode)
        {
            Spectrobe.switchMode = false;
        
            StartCoroutine(SmoothCloseRing());
            icon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Spectrobe.rotateMode = false;
            Spectrobe.resetIcon(Spectrobe.rotateIcon);
            Spectrobe.switchMode = true;
            key1.sprite = key1icon; key2.sprite = key2icon; key3.sprite = key3icon;
            StartCoroutine(SmoothOpenRing());
            icon.color = new Color(1, 1, 1, 0.6f);

        }


    }

    IEnumerator SmoothOpenRing()
    {
        // Activate the menu and set its scale to zero.
        Spectrobe.Ring.SetActive(true);
        Spectrobe.Ring.transform.localScale = Vector3.zero;
        Vector3 targetScale = new Vector3(0.75f, 0.75f, 0.75f);
        float t = 0;
        while (t < ringTime)
        {
            t += Time.deltaTime;
            Spectrobe.Ring.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t / ringTime);
            yield return null;
        }
        Spectrobe.Ring.transform.localScale = targetScale;
    }

    IEnumerator SmoothCloseRing()
    {
        Vector3 initialScale = Spectrobe.Ring.transform.localScale;
        float t = 0;
        while (t < ringTime)
        {
            t += Time.deltaTime;
            Spectrobe.Ring.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t / ringTime);
            yield return null;
        }
        Spectrobe.Ring.transform.localScale = Vector3.zero;
        Spectrobe.Ring.SetActive(false);
    }
}
