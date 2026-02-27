using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    int state;
    public static Fade _Instance;

    public static Fade Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = FindObjectOfType(typeof(Fade)) as Fade;
            }
            return _Instance;
        }
    }

    public void StartFade()
    {
        state = 1;
        StartCoroutine("OpenFade");
    }

    IEnumerator OpenFade()
    {
        while (gameObject.GetComponent<CanvasGroup>().alpha<1)
        {
            gameObject.GetComponent<CanvasGroup>().alpha = gameObject.GetComponent<CanvasGroup>().alpha +  0.000000001f;
            if (gameObject.GetComponent<CanvasGroup>().alpha>0.93f)
            {
                gameObject.GetComponent<CanvasGroup>().alpha = 1;
                state = 2;
                StartCoroutine("CloseFade");
            }
        }
        yield return null;
    }

    IEnumerator CloseFade()
    {
        while (gameObject.GetComponent<CanvasGroup>().alpha > 1)
        {
            gameObject.GetComponent<CanvasGroup>().alpha = gameObject.GetComponent<CanvasGroup>().alpha - Time.deltaTime * 1;
            if (gameObject.GetComponent<CanvasGroup>().alpha == 1)
            {
                state = 0;
            }
        }
        yield return null;
    }
}
