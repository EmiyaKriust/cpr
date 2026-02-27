using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using UnityEngine.UI;
using System.Text;
/****************************************************************
*Author:       严将 
*Version:      1.0 
*UnityVersion：2019.4.5 
*Date:         CreateTime
*Description:    
*History: 
*****************************************************************/
public class Buletooth : MonoBehaviour
{
    BluetoothHelper m_helper;

    public InputField m_deviceName_ipt; // 输入设备名称

    public Button m_Scan_btn; // 连接蓝牙按钮

    public Button m_connect_btn; // 连接蓝牙按钮

    public Button m_disconnect_btn; // 断开蓝牙连接按钮

    public Button m_clearLog_btn; // 清除日子面板信息按钮

    public Button m_hideLog_btn; // 隐藏当前面板按钮，一般连接成功就关闭这些面板，进入控制

    public Button m_Send_btn; // 断开蓝牙连接按钮

    public Text m_log_txt; // 输入的日志信息

    public GameObject blueToothImage;//连接上的图标

    public bool isConnect;//是否连接
    Action<bool> connectAction = null;
    public static Buletooth _Instance;
    public static Buletooth Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = FindObjectOfType(typeof(Buletooth)) as Buletooth;
            }
            return _Instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        m_deviceName_ipt.text = "XM-15";
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (blueToothImage.activeSelf==true)
        {
            //   ConnectBuletooth("XM-15", actionConnect); 
             string textname = m_deviceName_ipt.text; 
            ConnectBuletooth(textname, actionConnect);
        }
    }

    void Awake()
    {
        

        m_Scan_btn.onClick.AddListener(() =>
        {
            m_helper.ScanNearbyDevices();
        });
        // 添加连接按钮的事件
        m_connect_btn.onClick.AddListener(() =>
        {
            m_deviceName_ipt.text = "XM-15";
            if (!string.IsNullOrEmpty(m_deviceName_ipt.text))
            {
                Log("开始连接蓝牙:" + m_deviceName_ipt.text);
                // 设置连接的设备名字
                m_helper.setDeviceName(m_deviceName_ipt.text);
                // 开始连接
                m_helper.Connect();
            }
        });
        // 添加断开连接按钮的事件
        m_disconnect_btn.onClick.AddListener(Disconnect);
        // 添加日志清除按钮的事件
        m_clearLog_btn.onClick.AddListener(() =>
        {
            m_log_txt.text = "日志";
        });
        // 隐藏面板操作事件监听
        m_hideLog_btn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        try
        {
            // 获取 BluetoothHelper 实例
            m_helper = BluetoothHelper.GetInstance();
            // 打开蓝牙
            //m_helper.EnableBluetooth(true);

            // 设置收发字符的长度，这里是重点，不设置则接，发 不了消息，要到数据缓存的一定的量才一次性发送
            m_helper.setFixedLengthBasedStream(20);
            // 连接成功的回调函数
            m_helper.OnConnected += OnConnected;
            m_helper.OnScanEnded += OnScanEnded;
            // 连接失败的回调函数
            m_helper.OnConnectionFailed += OnConnectionFailed;
            // 没有找到设备的回调函数
            m_helper.OnServiceNotFound += OnServiceNotFound;
            // 接收到消息的回调函数
            m_helper.OnDataReceived += OnDataReceived;

        }
        catch (Exception e)
        {
            Log("连接异常：" + e.Message);
            Disconnect();
       
        }
    }
    public void actionConnect(bool able)
    {

    }
    public void ConnectBuletooth(string deviceName, Action<bool> action = null)
    {
        connectAction = action;
        // 设置连接的设备名字
        m_helper.setDeviceName(deviceName);
        // 开始连接
        m_helper.Connect();
      //  blueToothImage.SetActive(true);
    }

    void OnDataReceived(BluetoothHelper helper)
    {
        Log("接收到一条新消息");
        try
        {
            Bluetooth_Buffer udpBuffer = new Bluetooth_Buffer();
            byte[] data = m_helper.ReadBytes();

            string str = "{";
            Log("str :" + str);
            for (int i = 0; i < data.Length; i++)
            {
                byte test = data[i];
                if (i > 0)
                    str += ",";
                str += String.Format("{0:X}", test);
            }
            EditorDebug.LogWriteLog("test :" + str + "}");
            udpBuffer.Unpack(data);
            if (udpBuffer.MainCmd > 0)
            {
                EditorDebug.Log("OnReceiveMessage:解包==>异步处理:" + udpBuffer.MainCmd);
                NetworkManager.Instance.hanldeMsg(udpBuffer);
                string receiveString = Encoding.UTF8.GetString(udpBuffer.Content);
            }
        }
        catch (Exception)
        {
            Log("处理信息报错");
            throw;
        }
    }

    void OnServiceNotFound(BluetoothHelper helper, string service)
    {
        blueToothImage.SetActive(true);

        Log("没有找到设备:" + service);
        // 断开连接
        Disconnect();
    }

    void OnScanEnded(BluetoothHelper helper, LinkedList<BluetoothDevice> devices)
    {
        Log("扫描成功");
        string names = "";
        LinkedListNode<BluetoothDevice> node = devices.First;
        for (int i = 0; i < 30; i++)
        {

            string bluetoothName = node.Value.DeviceName;
            names += bluetoothName;
            names += ",,,";

            if (bluetoothName == "XM-15")
            {
                // 设置连接的设备名字
                m_helper.setDeviceName(bluetoothName);
                // 开始连接
                m_helper.Connect();
                break;
            }
            node = node.Next;
            if (node == null)
                break;
        }
        Log("扫描设备名称：" + names);
    }

    void OnConnected(BluetoothHelper helper)
    {
        Log("连接成功");
        connectAction.Invoke(true);
        blueToothImage.SetActive(false);
        isConnect = true;
        // 连接成功，开始监听消息
        helper.StartListening();
    }

    void OnConnectionFailed(BluetoothHelper helper)
    {
        blueToothImage.SetActive(true);

        Log("连接失败");
        connectAction.Invoke(false);
        Disconnect();
    }

    /// 日志内容
    public void Log(string log)
    {
        m_log_txt.text += "\n:" + log;
    }

    /// /// 发送消息
    ///
    /// 消息的内容
    public void Send(string msg)
    {
        m_helper.SendData(msg);
    }
    public void SendByte(Byte[] msg)
    {
        m_helper.SendData(msg);
    }
    void OnDestroy()
    {
        Disconnect();
    }

    ///断开连接
    public void Disconnect()
    {
        blueToothImage.SetActive(true);
        isConnect = false;
        Log("断开连接");
        if (m_helper != null)
            m_helper.Disconnect();
    }

    public void Choice()
    {
        m_deviceName_ipt.gameObject.SetActive(!m_deviceName_ipt.gameObject.activeSelf);
    }

}

