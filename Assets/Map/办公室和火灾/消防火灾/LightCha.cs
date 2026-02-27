using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCha : MonoBehaviour
{
    // Start is called before the first frame update
    private Light fireLight;
    void Start()
    {
        fireLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(fire());
    }
    IEnumerator fire()
    {
        yield return new WaitForSeconds(0.5f);
        fireLight.intensity = Random.Range(1, 3);
        yield return new WaitForSeconds(0.5f);
    }
}
