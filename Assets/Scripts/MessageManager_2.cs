using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager_2 : MonoBehaviour
{
    public Text Content;
    public static MessageManager_2 _Instance;
    public static MessageManager_2 Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = FindObjectOfType(typeof(MessageManager_2)) as MessageManager_2;
            }
            return _Instance;
        }
    }
    public void Awake()
    {


    }
    public void info(string ContentText)
    {

        Content.text = ContentText;
    }
}
