using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MassageManage : MonoBehaviour
{
    public Text title;
    public Text Content;
    public static MassageManage _Instance;
    public static MassageManage Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = FindObjectOfType(typeof(MassageManage)) as MassageManage;
            }
            return _Instance;
        }
    }
    public void Awake()
    {
        

    }
    public void info(string titleText, string ContentText)
    {
        title.text = titleText;
        Content.text = ContentText;
    }
}
