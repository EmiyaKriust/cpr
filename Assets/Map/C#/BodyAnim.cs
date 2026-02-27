using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnim : MonoBehaviour
{
    public Animator bodyAnimation;


    public static BodyAnim _Instance;
    public static BodyAnim Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = FindObjectOfType(typeof(BodyAnim)) as BodyAnim;
            }
            return _Instance;
        }
    }

    public void anim1(string name)
    {
        bodyAnimation.Play(name);
    }
}
