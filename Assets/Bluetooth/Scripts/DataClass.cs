using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
/****************************************************************
 *Author:       严将 
 *Version:      1.0 
 *UnityVersion：2019.4.5 
 *Date:         CreateTime
 *Description:    
 *History: 
*****************************************************************/
public class DataClass 
{

}

public static class ConstantData
{
    public static byte[] ConfigCommandData = new byte[]{0xF1, 0xF2, 0xF3, 0xF4, 0xFF };
    public static byte[] SystemCommandData = new byte[] { 0x01, 0x0A, 0x0C};
    public static byte[] ControlCommandData = new byte[] { 0x11, 0x21, 0x31};
    public static byte[] UploadCommandData = new byte[] { 0xA0, 0xA1, 0xA2, 0xA3, 0xA4};
}

/// <summary>
/// 指令类型
/// </summary>
public enum CommandType
{
    ConfigCommand = 0,
    SystemCommand = 1,
    ControlCommand = 2,
    UploadCommand = 3,
    BodyCommand = 4
}

/// <summary>
/// 配置指令
/// </summary>
public enum ConfigCommandType
{ 
    CleanMCU = 0,
    SaveID = 1,
    Setting = 2,
    InitState = 3,
    SendIDTest = 4
}
/// <summary>
/// 系统类型指令
/// </summary>
public enum SystemCommandType
{
    Connect = 0,
    Reset = 1,
    Fault = 2
}
/// <summary>
/// 控制指令
/// </summary>
public enum ControlCommandType
{
    CPR = 0,
    QiGuan = 1,
    MaiBo = 2
}
/// <summary>
/// 上传指令
/// </summary>
public enum UploadCommandType
{
    CPRData = 0,
    CPRCheck = 1,
    YangTou = 2,
    /// <summary>
    /// 未用
    /// </summary>
    QiGuanData = 3,
    QiGuanPosData = 4,
}


/// <summary>
/// 1.擦除MCU的EEPROM内容
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_CleanMCU
{
    public byte XX;
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        XX = NetUtil.GetValue<byte>(bytes, ref idx);
        //idx = 18;
        //end = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};
/// <summary>
/// 2.ID号保存
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_SaveID
{
    public byte XX;
    public byte[] IDlist = new byte[7];
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        XX = NetUtil.GetValue<byte>(bytes, ref idx);
        IDlist = NetUtil.GetValue<byte>(bytes,7,ref idx);
        //idx = 18;
        //end = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};
/// <summary>
/// 3.设备配置
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_Setting
{
    public byte WW;
    public byte XX;
    public byte YY;
    public short ZZ;
    public short MM;
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        WW = NetUtil.GetValue<byte>(bytes, ref idx);
        XX = NetUtil.GetValue<byte>(bytes, ref idx);
        YY = NetUtil.GetValue<byte>(bytes, ref idx);
        ZZ = NetUtil.GetValue<short>(bytes, ref idx);
        MM = NetUtil.GetValue<short>(bytes, ref idx);
        //idx = 18;
        //end = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};

/// <summary>
/// 4.查询模拟人初始状态
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_InitState
{
    public byte WW;
    public byte XX;
    public byte YY;
    public Int32 TT;
    public byte end;
    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        WW = NetUtil.GetValue<byte>(bytes, ref idx);
        XX = NetUtil.GetValue<byte>(bytes, ref idx);
        YY = NetUtil.GetValue<byte>(bytes, ref idx);
        TT = NetUtil.GetValue<Int32>(bytes, ref idx);
        idx = 18;
        end = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};
/// <summary>
/// 5.ID号发送（测试）
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_SendIDTest
{
    public byte WW;
    public byte[] IDlist = new byte[7];
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        WW = NetUtil.GetValue<byte>(bytes, ref idx);
        IDlist = NetUtil.GetValue<byte>(bytes, 7, ref idx);
        idx = 18;
        end = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};


/// <summary>
/// 1.联机指令
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_Connect
{
    public byte WW;
    public byte[] IDlist = new byte[7];
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        WW = NetUtil.GetValue<byte>(bytes, ref idx);
        IDlist = NetUtil.GetValue<byte>(bytes, 7, ref idx);
        idx = 18;
        end = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};
/// <summary>
/// 2.系统关机、复位指令
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_Reset
{
    public byte XX;
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        XX = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};
/// <summary>
/// 3.模拟人电池电量、故障指令
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_Fault
{
    public byte XXH;
    public byte XXL;
    public byte YY;
    public byte ZZ;
    public byte WW;
    public byte PP;
    public byte BB;
    public byte end;
    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        XXH = NetUtil.GetValue<byte>(bytes, ref idx);
        XXL = NetUtil.GetValue<byte>(bytes, ref idx);
        YY = NetUtil.GetValue<byte>(bytes, ref idx);
        ZZ = NetUtil.GetValue<byte>(bytes, ref idx);
        WW = NetUtil.GetValue<byte>(bytes, ref idx);
        PP = NetUtil.GetValue<byte>(bytes, ref idx);
        BB = NetUtil.GetValue<byte>(bytes, ref idx);
        idx = 18;
        end = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};


/// <summary>
/// 1.CPR运行控制指令
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_CPR
{
    public byte XX;
    public byte YY;
    public byte ZZ;
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        XX = NetUtil.GetValue<byte>(bytes, ref idx);
        YY = NetUtil.GetValue<byte>(bytes, ref idx);
        ZZ = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};

/// <summary>
///2.气管插管运行控制指令
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_QiGuan
{
    public byte XX;
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        XX = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};

/// <summary>
///3.脉搏、瞳孔控制指令
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_MaiBo
{
    public byte YY;
    public byte ZZ;
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        YY = NetUtil.GetValue<byte>(bytes, ref idx);
        ZZ = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};

/// <summary>
///1.CPR操作数据指令
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_CPRData
{
    public byte XX1;
    public byte WW1;
    // public short YY1;
    public byte YY1; //我猜yy也是一个字节

    public byte XX2;
    public byte WW2;
    //  public short YY2;
    public byte YY2;//我猜yy也是一个字节

    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        XX1 = NetUtil.GetValue<byte>(bytes, ref idx);
        WW1 = NetUtil.GetValue<byte>(bytes, ref idx);
        YY1 = NetUtil.GetValue<byte>(bytes, ref idx);
        YY2 = NetUtil.GetValue<byte>(bytes, ref idx);

        //  YY1 = NetUtil.GetValue<short>(bytes, ref idx);
        XX2 = NetUtil.GetValue<byte>(bytes, ref idx);
        WW2 = NetUtil.GetValue<byte>(bytes, ref idx);
     //   YY2 = NetUtil.GetValue<short>(bytes, ref idx);
        idx = 19;
        end = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};

/// <summary>
///2.CPR操作前判断检测指令
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_CPRCheck
{
    public byte PP;
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        PP = NetUtil.GetValue<byte>(bytes, ref idx);
      
    }
};

/// <summary>
///3.仰头状态检测指令
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_YangTou
{
 
    public byte ZZ;
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        ZZ = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};

/// <summary>
///4.气管插管过程值数据指令（未用）
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_QiGuanData
{
    public byte XX1;
    public byte XX2;
    public byte XX3;
    public byte XX4;
    public byte XX5;
    public byte end;
    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        XX1 = NetUtil.GetValue<byte>(bytes, ref idx);
        XX2 = NetUtil.GetValue<byte>(bytes, ref idx);
        XX3 = NetUtil.GetValue<byte>(bytes, ref idx);
        XX4 = NetUtil.GetValue<byte>(bytes, ref idx);
        XX5 = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};

/// <summary>
///5.气管插管位置数据指令
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class CMD_QiGuanPosData
{
    public byte ZZ;
    public byte WW;
    public byte end;

    public void UnPack(byte[] bytes)
    {
        int idx = 0;
        ZZ = NetUtil.GetValue<byte>(bytes, ref idx);
        WW = NetUtil.GetValue<byte>(bytes, ref idx);
    }
};




