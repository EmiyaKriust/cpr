using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviour
{

    #region STATIC
    static private NetworkManager m_instance;
    public static NetworkManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject gameobj = new GameObject("NetworkManager");
                m_instance = gameobj.AddComponent<NetworkManager>();
            }
            return m_instance;
        }
        set
        {
            m_instance = value;
        }
    }

    #endregion STATIC



    #region MONO METHODS

    private void Awake()
    {
        if (m_instance != null)
        {
           Debug.LogError("There is more than one NetworkManager!");
        }
        else
        {
            m_instance = this;
        }
        DontDestroyOnLoad(Instance.gameObject);
    }

    public void Update()
    {
        NetworkInvokeHelper.Update();
    }

    public void FixedUpdate()
    {
      //  CheckTimeout(Time.fixedDeltaTime);
    }

       
    #endregion MONO METHODS


 


   
    


    #region UDP通信
    Dictionary<int, Action<Bluetooth_Buffer>> msgDispatcher = new Dictionary<int, Action<Bluetooth_Buffer>>();

    /// <summary>
    /// 添加消息处理器 
    /// </summary>
    public void AddMsgHandler(Action<Bluetooth_Buffer> handler)
    {
        msgDispatcher[0] = handler;
        //Debug.Log("添加消息处理器:类型" + connect_type + " 主命令[" + mainCmd + "]");
    }

    ///// <summary>
    ///// 移除消息处理器
    ///// </summary>
    ///// <param name="mainCmd"></param>mainCmd
    ///// <param name="handler"></param>
    //public void removeUdpMsgHandler(Connection_Type connect_type, int mainCmd, Action<TCPSocket, TCP_Buffer> handler)
    //{

    //    if (msgDispatcher.ContainsKey(mainCmd))
    //        msgDispatcher.Remove(mainCmd);
    //    EditorDebug.LogError("------type:" + connect_type + "----msg {" + mainCmd + "} no handler!!!!!!");
    //}


    public void hanldeMsg(Bluetooth_Buffer buffer)
    {
       // var mainCmd = buffer.MainCmd;
        if (msgDispatcher.ContainsKey(0))
            msgDispatcher[0](buffer);

    }
    #endregion INTERFACE
}