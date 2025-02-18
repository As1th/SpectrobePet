using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class RotateButton : MonoBehaviour
{
    public GameManager manager;
    private DragGameSprite Spectrobe;
    private SpriteRenderer icon;
    public float ringTime = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        Spectrobe = manager.Spectrobe;
        icon = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame

    void OnMouseDown()
    {
        RotateMode();
    }
    void RotateMode()
    {
        if (Spectrobe.rotateMode)
        {
            Spectrobe.rotateMode = false;
            StartCoroutine(SmoothCloseRing());

            icon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Spectrobe.switchMode = false;
            Spectrobe.resetIcon(Spectrobe.switchIcon);
            Spectrobe.rotateMode = true;
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
