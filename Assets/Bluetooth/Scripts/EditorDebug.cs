using UnityEngine;
using System.Collections;
using System;

public class EditorDebug
{
    //public static string path = Application.persistentDataPath + "/Log.txt";
    public static string path = Application.dataPath + "/Log.txt";
    public static void Log(object message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    public static void LogWriteLog(string message)
    {
        //#if UNITY_EDITOR

        FileHelper.WriteAppendIntoFile(path, "\n" + DateTime.Now.ToLongTimeString() + "  " + message);
        //#endif
    }

    public static void LogError(object message, UnityEngine.Object obj = null)
    {
#if UNITY_EDITOR
        if (obj == null)
            Debug.LogError(message);
        else
            Debug.LogError(message, obj);
#endif
    }

    public static void LogWarning(object message, UnityEngine.Object obj = null)
    {
#if UNITY_EDITOR
        if (obj == null)
            Debug.LogWarning(message);
        else
            Debug.LogWarning(message, obj);
#endif
    }

    public static void LogException(System.Exception message)
    {
#if UNITY_EDITOR
        Debug.LogException(message);
#endif
    }
}
