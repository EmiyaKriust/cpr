using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Text;

///////////////////////////////////////////////////////////////////////////
//结构定义

/// <summary>
/// 网络缓冲
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public struct TCP_Buffer
{
    public byte Head { private set; get; }
    public short MainCmd { set; get; }
    public short SubCmd { set; get; }

    public short Length { private set; get; }

    public byte[] Content { set; get; }

    public void Unpack(byte[] bytes, bool head = false)
    {
      
        int index = 0;
        Content = NetUtil.GetValue<byte[]>(bytes, ref index);
        Length = (short)Content.Length;

        //Head = NetUtil.GetValue<byte>(bytes, ref index);
        //MainCmd = NetUtil.GetValue<short>(bytes, ref index);
        //SubCmd = NetUtil.GetValue<short>(bytes, ref index);
        //Length = NetUtil.GetValue<short>(bytes, ref index);
        //if (!head)
        //    Content = NetUtil.GetValue<byte[]>(bytes, ref index);

        //Debug.Log("SubCmd:" + SubCmd + ",MainCmd:" + MainCmd);
    }

    internal byte[] Pack()
    {
        Length = (short)Content.Length;
        byte[] bys = new byte[Length];
        Array.Copy(Content, 0, bys, 0, Length);

        //Head = 88;
        //Length = (short)Content.Length;
        //byte[] bys = new byte[1 + 2 + 2 + 2 + Length];
        //bys[0] = Head;
        //Array.Copy(NetUtil.GetBytes<short>(MainCmd), 0, bys, 1, 2);
        //Array.Copy(NetUtil.GetBytes<short>(SubCmd), 0, bys, 3, 2);
        //Array.Copy(NetUtil.GetBytes<short>(Length), 0, bys, 5, 2);
        //Array.Copy(Content, 0, bys, 7, Length);
        //if (bys.Length < 7)
        //{
        //    Debug.Log("Pack  SubCmd:" + SubCmd + ",Length:" + Length);
        //}
        return bys;
    }
}

/// <summary>
/// 网络缓冲
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public struct TCP_EasyBuffer
{
  
    public short Length { private set; get; }

    public byte[] Content { set; get; }

    public void Unpack(byte[] bytes, bool head = false)
    {

        int index = 0;
        Content = NetUtil.GetValue<byte[]>(bytes, ref index);

        // Debug.Log("SubCmd:" + SubCmd + ",MainCmd:" + MainCmd);
    }

    internal byte[] Pack()
    {
      
        Length = (short)Content.Length;
        byte[] bys = new byte[Length];
       
        
        Array.Copy(Content, 0, bys, 0, Length);
        
          //  Debug.Log("Pack  SubCmd:" + SubCmd + ",Length:" + Length);
        
        return bys;
    }
}


public struct UDP_Buffer
{
    public long Head { private set; get; }

    public int Length { private set; get; }

    public int HeadLh { private set; get; }

    public byte[] Content { set; get; }

    public void Unpack(byte[] bytes, bool head = false)
    {
        int index = 0;
       // Head = NetUtil.GetValue<long>(bytes, ref index);
        Length = NetUtil.GetValue<int>(bytes, ref index);
        HeadLh = NetUtil.GetValue<int>(bytes, ref index);
        if (!head)
              Content = NetUtil.GetValue<byte[]>(bytes, ref index);
    }

    internal byte[] Pack()
    {
        Head = 255;
        Length = (int)Content.Length;
        byte[] bys = new byte[2 + 2 + 4 + Length];
        bys[0] = 255;
        bys[1] = 255;
        Array.Copy(NetUtil.GetBytes<int>(Length), 0, bys, 4, 4);
        Array.Copy(Content, 0, bys, 8, Length);
        return bys;
    }
}

/// <summary>
/// 蓝牙数据解析
/// 数据共20位 前3位是头，第四位 是命令  中间是数据 最后一位是结束符
/// </summary>
public struct Bluetooth_Buffer
{
    public long Head { private set; get; }

    public short MainCmd { set; get; }
    public int Length { private set; get; }


    public byte[] Content { set; get; }

    public byte EndTag { private set; get; }

    public void Unpack(byte[] bytes, bool head = false)
    {
        int index = 3;
        MainCmd = NetUtil.GetValue<byte>(bytes, ref index);
        Content = NetUtil.GetValue<byte[]>(bytes, ref index);
        
    }

    internal byte[] Pack()
    {
        //F0//
        Head = 240;
        Length = (int)Content.Length;
        byte[] bys = new byte[3 + 1 + Length];
        bys[0] = 240;
        bys[1] = 240;
        bys[2] = 240;
        Array.Copy(NetUtil.GetBytes<byte>(MainCmd), 0, bys, 3, 1);
        Array.Copy(Content, 0, bys, 4, Length);
        return bys;
    }
}

public class NetUtil : MonoBehaviour
{
    //无效数值
    public static readonly byte INVALID_BYTE = ((byte)(0xff));                  //无效数值
    public static readonly ushort INVALID_USHORT = ((ushort)(0xffff));          //无效数值
    public static readonly uint INVALID_UINT = ((uint)(0xffffffff));	        //无效数值
    //数据类型
    public static readonly byte DK_MAPPED = 0X01;					            //映射类型
    public static readonly byte DK_ENCRYPT = 0X02;						        //加密类型
    public static readonly byte DK_COMPRESS = 0X04;						        //压缩类型

    public static readonly int MDM_KN_COMMAND = 0;                              //内核命令
    public static readonly int SUB_KN_DETECT_SOCKET = 1;                        //检测命令
    public static readonly int SUB_KN_VALIDATE_SOCKET = 2;                      //验证命令

    public const int SOCKET_TCP_BUFFER = 32767;                                 //最大包体长度

    //public static readonly int SOCKET_TCP_PACKET = SOCKET_TCP_BUFFER - (TCP_HEAD);

    public static readonly byte[] ENCODE_MAP = new byte[256] {
        0x70, 0x2F, 0x40, 0x5F, 0x44, 0x8E,
        0x6E, 0x45, 0x7E, 0xAB, 0x2C, 0x1F, 0xB4, 0xAC, 0x9D, 0x91, 0x0D,
        0x36, 0x9B, 0x0B, 0xD4, 0xC4, 0x39, 0x74, 0xBF, 0x23, 0x16, 0x14,
        0x06, 0xEB, 0x04, 0x3E, 0x12, 0x5C, 0x8B, 0xBC, 0x61, 0x63, 0xF6,
        0xA5, 0xE1, 0x65, 0xD8, 0xF5, 0x5A, 0x07, 0xF0, 0x13, 0xF2, 0x20,
        0x6B, 0x4A, 0x24, 0x59, 0x89, 0x64, 0xD7, 0x42, 0x6A, 0x5E, 0x3D,
        0x0A, 0x77, 0xE0, 0x80, 0x27, 0xB8, 0xC5, 0x8C, 0x0E, 0xFA, 0x8A,
        0xD5, 0x29, 0x56, 0x57, 0x6C, 0x53, 0x67, 0x41, 0xE8, 0x00, 0x1A,
        0xCE, 0x86, 0x83, 0xB0, 0x22, 0x28, 0x4D, 0x3F, 0x26, 0x46, 0x4F,
        0x6F, 0x2B, 0x72, 0x3A, 0xF1, 0x8D, 0x97, 0x95, 0x49, 0x84, 0xE5,
        0xE3, 0x79, 0x8F, 0x51, 0x10, 0xA8, 0x82, 0xC6, 0xDD, 0xFF, 0xFC,
        0xE4, 0xCF, 0xB3, 0x09, 0x5D, 0xEA, 0x9C, 0x34, 0xF9, 0x17, 0x9F,
        0xDA, 0x87, 0xF8, 0x15, 0x05, 0x3C, 0xD3, 0xA4, 0x85, 0x2E, 0xFB,
        0xEE, 0x47, 0x3B, 0xEF, 0x37, 0x7F, 0x93, 0xAF, 0x69, 0x0C, 0x71,
        0x31, 0xDE, 0x21, 0x75, 0xA0, 0xAA, 0xBA, 0x7C, 0x38, 0x02, 0xB7,
        0x81, 0x01, 0xFD, 0xE7, 0x1D, 0xCC, 0xCD, 0xBD, 0x1B, 0x7A, 0x2A,
        0xAD, 0x66, 0xBE, 0x55, 0x33, 0x03, 0xDB, 0x88, 0xB2, 0x1E, 0x4E,
        0xB9, 0xE6, 0xC2, 0xF7, 0xCB, 0x7D, 0xC9, 0x62, 0xC3, 0xA6, 0xDC,
        0xA7, 0x50, 0xB5, 0x4B, 0x94, 0xC0, 0x92, 0x4C, 0x11, 0x5B, 0x78,
        0xD9, 0xB1, 0xED, 0x19, 0xE9, 0xA1, 0x1C, 0xB6, 0x32, 0x99, 0xA3,
        0x76, 0x9E, 0x7B, 0x6D, 0x9A, 0x30, 0xD6, 0xA9, 0x25, 0xC7, 0xAE,
        0x96, 0x35, 0xD0, 0xBB, 0xD2, 0xC8, 0xA2, 0x08, 0xF3, 0xD1, 0x73,
        0xF4, 0x48, 0x2D, 0x90, 0xCA, 0xE2, 0x58, 0xC1, 0x18, 0x52, 0xFE,
        0xDF, 0x68, 0x98, 0x54, 0xEC, 0x60, 0x43, 0x0F
    };

    public static readonly byte[] DECODE_MAP = new byte[256] {
        0x51, 0xA1, 0x9E, 0xB0, 0x1E, 0x83,
        0x1C, 0x2D, 0xE9, 0x77, 0x3D, 0x13, 0x93, 0x10, 0x45, 0xFF, 0x6D,
        0xC9, 0x20, 0x2F, 0x1B, 0x82, 0x1A, 0x7D, 0xF5, 0xCF, 0x52, 0xA8,
        0xD2, 0xA4, 0xB4, 0x0B, 0x31, 0x97, 0x57, 0x19, 0x34, 0xDF, 0x5B,
        0x41, 0x58, 0x49, 0xAA, 0x5F, 0x0A, 0xEF, 0x88, 0x01, 0xDC, 0x95,
        0xD4, 0xAF, 0x7B, 0xE3, 0x11, 0x8E, 0x9D, 0x16, 0x61, 0x8C, 0x84,
        0x3C, 0x1F, 0x5A, 0x02, 0x4F, 0x39, 0xFE, 0x04, 0x07, 0x5C, 0x8B,
        0xEE, 0x66, 0x33, 0xC4, 0xC8, 0x59, 0xB5, 0x5D, 0xC2, 0x6C, 0xF6,
        0x4D, 0xFB, 0xAE, 0x4A, 0x4B, 0xF3, 0x35, 0x2C, 0xCA, 0x21, 0x78,
        0x3B, 0x03, 0xFD, 0x24, 0xBD, 0x25, 0x37, 0x29, 0xAC, 0x4E, 0xF9,
        0x92, 0x3A, 0x32, 0x4C, 0xDA, 0x06, 0x5E, 0x00, 0x94, 0x60, 0xEC,
        0x17, 0x98, 0xD7, 0x3E, 0xCB, 0x6A, 0xA9, 0xD9, 0x9C, 0xBB, 0x08,
        0x8F, 0x40, 0xA0, 0x6F, 0x55, 0x67, 0x87, 0x54, 0x80, 0xB2, 0x36,
        0x47, 0x22, 0x44, 0x63, 0x05, 0x6B, 0xF0, 0x0F, 0xC7, 0x90, 0xC5,
        0x65, 0xE2, 0x64, 0xFA, 0xD5, 0xDB, 0x12, 0x7A, 0x0E, 0xD8, 0x7E,
        0x99, 0xD1, 0xE8, 0xD6, 0x86, 0x27, 0xBF, 0xC1, 0x6E, 0xDE, 0x9A,
        0x09, 0x0D, 0xAB, 0xE1, 0x91, 0x56, 0xCD, 0xB3, 0x76, 0x0C, 0xC3,
        0xD3, 0x9F, 0x42, 0xB6, 0x9B, 0xE5, 0x23, 0xA7, 0xAD, 0x18, 0xC6,
        0xF4, 0xB8, 0xBE, 0x15, 0x43, 0x70, 0xE0, 0xE7, 0xBC, 0xF1, 0xBA,
        0xA5, 0xA6, 0x53, 0x75, 0xE4, 0xEB, 0xE6, 0x85, 0x14, 0x48, 0xDD,
        0x38, 0x2A, 0xCC, 0x7F, 0xB1, 0xC0, 0x71, 0x96, 0xF8, 0x3F, 0x28,
        0xF2, 0x69, 0x74, 0x68, 0xB7, 0xA3, 0x50, 0xD0, 0x79, 0x1D, 0xFC,
        0xCE, 0x8A, 0x8D, 0x2E, 0x62, 0x30, 0xEA, 0xED, 0x2B, 0x26, 0xB9,
        0x81, 0x7C, 0x46, 0x89, 0x73, 0xA2, 0xF7, 0x72
    };

    public static bool mappedBuffer(byte[] buffer, int wDataSize)
    {
        //变量定义
        //TCP_Info pInfo = new TCP_Info();
        //int cbCheckCode = 0;
        ////映射数据
        //for (var i = Marshal.SizeOf(pInfo); i < wDataSize; i++)
        //{
        //    cbCheckCode += buffer[i];
        //    buffer[i] = ENCODE_MAP[buffer[i]];
        //}

        ////设置数据
        //pInfo.cbCheckCode = (byte)(~cbCheckCode + 1);
        //pInfo.wPacketSize = (ushort)wDataSize;
        //pInfo.cbDataKind |= DK_MAPPED;

        //var bInfo = StructToBytes(pInfo);
        //Array.Copy(bInfo, buffer, bInfo.Length);
        return true;
    }
    public static bool unMappedBuffer(byte[] buffer, int size)
    {
        //var Tcp_Info = typeof(TCP_Info);
        //TCP_Info pInfo = (TCP_Info)BytesToStruct(buffer, Tcp_Info, Marshal.SizeOf(Tcp_Info));
        ////映射
        //if ((pInfo.cbDataKind & DK_MAPPED) != 0)
        //{
        //    byte cbCheckCode = pInfo.cbCheckCode;

        //    for (var i = Marshal.SizeOf(pInfo); i < size; i++)
        //    {
        //        cbCheckCode += DECODE_MAP[buffer[i]];
        //        buffer[i] = DECODE_MAP[buffer[i]];
        //    }
        //    //效验
        //    if (cbCheckCode != 0)
        //        return false;
        //}
        return true;
    }

    //结构体转字节数组
    public static byte[] StructToBytes(object structObj, int size = 0)
    {
        if (size == 0)
        {
            size = Marshal.SizeOf(structObj);
        }
        IntPtr buffer = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(structObj, buffer, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(buffer, bytes, 0, size);
            return bytes;
        }
        catch (Exception ex)
        {
            Debug.LogError("struct to bytes error:" + ex);
         //   DebugHelper.BugReport(BugType.Net, "struct to bytes error:" + ex.ToString());
            return null;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    //字节数组转结构体
    public static object BytesToStruct(byte[] bytes, Type strcutType, int nSize)
    {
        if (bytes == null)
        {
            Debug.LogError("null bytes!!!!!!!!!!!!!");
        }
        int size = Marshal.SizeOf(strcutType);
        IntPtr buffer = Marshal.AllocHGlobal(nSize);
        //Debug.LogError("Type: " + strcutType.ToString() + "---TypeSize:" + size + "----packetSize:" + nSize);
        try
        {
            Marshal.Copy(bytes, 0, buffer, nSize);
            return Marshal.PtrToStructure(buffer, strcutType);
        }
        catch (Exception ex)
        {
           // DebugHelper.BugReport(BugType.Net, "Type: " + strcutType.ToString() + "---TypeSize:" + size + "----packetSize:" + nSize + ex.ToString());
            Debug.LogError("Type: " + strcutType.ToString() + "---TypeSize:" + size + "----packetSize:" + nSize);
            return null;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public static bool CheckStructSize<T>(int size)
    {
        return true;

        int structSize = Marshal.SizeOf(typeof(T));
        if (size != structSize)
            throw new Exception("---CheckStructSize failed!!! nSize(" + size + ")!=structSize(" + structSize + ")...");
        return true;
    }


    public static byte[] StringToBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public static string BytesToString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    public static T GetValue<T>(byte[] bytes, ref int index, int length = 0)
    {
        try
        {
            object result;
            Type type = typeof(T);
            if (type == typeof(byte))
            {
                result = bytes[index++];

            }
            else if (type == typeof(short))
            {
                result = BitConverter.ToInt16(bytes, index);
                index += sizeof(short);
            }
            else if (type == typeof(ushort))
            {
                result = BitConverter.ToUInt16(bytes, index);
                index += sizeof(ushort);
            }
            else if (type == typeof(int))
            {
                result = BitConverter.ToInt32(bytes, index);
                index += sizeof(int);
            }
            else if (type == typeof(uint))
            {
                result = BitConverter.ToUInt32(bytes, index);
                index += sizeof(uint);
            }
            else if (type == typeof(long))
            {
                result = BitConverter.ToInt64(bytes, index);
                index += sizeof(long);
            }
            else if (type == typeof(ulong))
            {
                result = BitConverter.ToUInt64(bytes, index);
                index += sizeof(long);
            }
            else if (type == typeof(string))
            {
                if (length > 0)
                {
                    byte[] buffer = new byte[length * 2];
                    Buffer.BlockCopy(bytes, index, buffer, 0, length * 2);
                    string temp = System.Text.Encoding.Unicode.GetString(buffer);
                    int end_idx = temp.IndexOf("\0");
                    if (end_idx >= 0 && end_idx < length)
                    {
                        result = temp.Substring(0, end_idx);
                    }
                    else
                    {
                        result = temp;
                    }
                    index += length * 2;
                }
                else
                {
                    result = string.Empty;
                }
            }
            else if (type == typeof(double))
            {
                result = BitConverter.ToDouble(bytes, index);
                index += sizeof(double);
            }
            else if (type == typeof(float))
            {
                result = BitConverter.ToDouble(bytes, index);
                index += sizeof(float);
            }
            else if (type == typeof(bool))
            {
                result = BitConverter.ToBoolean(bytes, index);
                index += sizeof(bool);
            }
            else
            {
                throw new Exception("need type: " + type.ToString());
                //Debug.LogError("need type: " + type.ToString());
            }
            return (T)result;
        }
        catch (Exception ex)
        {
            Debug.LogError("Get value exception:" + ex.Message);
            return default(T);
        }
    }

    public static T[] GetValue<T>(byte[] bytes, int length, ref int index)
    {
        T[] result = new T[length];
        for (int i = 0; i < length; ++i)
        {
            result[i] = GetValue<T>(bytes, ref index);
        }
        return result;
    }

    public static T[,] GetValue2<T>(byte[] bytes, int row, int column, ref int index)
    {
        T[,] result = new T[row, column];
        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < column; ++j)
            {
                result[i, j] = GetValue<T>(bytes, ref index);
            }
        }
        return result;
    }

    public static T[][] GetValue22<T>(byte[] bytes, int row, int column, ref int index)
    {
        T[][] result = new T[row][];
        for (int i = 0; i < row; ++i)
        {
            result[i] = GetValue<T>(bytes, column, ref index);
        }
        return result;
    }

    public static void PushValue(ref MemoryStream stream, object obj, int length = 0)
    {
        Type type = obj.GetType();
        byte[] result;

        if (type == typeof(byte))
        {
            result = new byte[1];
            result[0] = (byte)obj;
        }
        else if (type == typeof(byte[]))
        {
            result = (byte[])obj;
        }
        else if (type == typeof(bool))
        {
            result = BitConverter.GetBytes((bool)obj);
        }
        else if (type == typeof(short))
        {
            result = BitConverter.GetBytes((short)obj);
        }
        else if (type == typeof(ushort))
        {
            result = BitConverter.GetBytes((ushort)obj);
        }
        else if (type == typeof(int))
        {
            result = BitConverter.GetBytes((int)obj);
        }
        else if (type == typeof(uint))
        {
            result = BitConverter.GetBytes((uint)obj);
        }
        else if (type == typeof(long))
        {
            result = BitConverter.GetBytes((long)obj);
        }
        else if (type == typeof(ulong))
        {
            result = BitConverter.GetBytes((ulong)obj);
        }
        else if (type == typeof(float))
        {
            result = BitConverter.GetBytes((float)obj);
        }
        else if (type == typeof(double))
        {
            result = BitConverter.GetBytes((double)obj);
        }
        else if (type == typeof(string))
        {
            if (length == 0)
            {
                result = System.Text.Encoding.Unicode.GetBytes((string)obj);
            }
            else
            {
                result = new byte[length * 2];
                string target = (string)obj;
                System.Text.Encoding.Unicode.GetBytes(target, 0, target.Length, result, 0);
            }
        }
        else
        {
            Debug.LogError("push value undefined type:" + type);
            return;
        }
        stream.Write(result, 0, result.Length);
    }

    /// <summary>
    /// 提供HiSocket消息解析，二进制解析的问题，BitConvert有问题
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static T GetValue<T>(byte[] bytes, ref int offset/*, bool bIsHighFirst = true*/)
    {
        bool bIsHighFirst = true;
        try
        {
            Type type = typeof(T);
            object result;
            if (type == typeof(byte))
            {
                result = bytes[0 + offset];
                offset += 1;
                return (T)result;
            }
            else if (type == typeof(short))
            {

                if (bIsHighFirst)
                {
                    result = (short)(((bytes[1 + offset] & 0xFF))
                        | ((bytes[0 + offset] & 0xFF) << 8));
                }
                else
                {
                    //低位在前 高位在后//
                    result = (short)((((bytes[1 + offset] & 0xFF) << 8))
                       | ((bytes[0 + offset] & 0xFF)));
                }

                offset += 2;
                return (T)result;
            }
            else if (type == typeof(int))
            {
                if (bIsHighFirst)
                {
                    result = (int)((bytes[3 + offset] & 0xFF)
                       | ((bytes[2 + offset] & 0xFF) << 8)
                       | ((bytes[1 + offset] & 0xFF) << 16)
                       | ((bytes[0 + offset] & 0xFF) << 24));
                }
                else
                {
                    //低位在前 高位在后//
                    result = (int)(((bytes[3 + offset] & 0xFF) << 24)
                        | ((bytes[2 + offset] & 0xFF) << 16)
                        | ((bytes[1 + offset] & 0xFF) << 8)
                        | ((bytes[0 + offset] & 0xFF)));
                }

                offset += 4;
                return (T)result;
            }
            else if (type == typeof(double))
            {
                result = BitConverter.ToDouble(bytes, offset);
                offset += sizeof(double);
                return (T)result;
            }
            else if (type == typeof(float))
            {
                result = BitConverter.ToSingle(bytes, offset);
                offset += sizeof(float);
                return (T)result;
            }
            else if (type == typeof(byte[]))
            {
                int len = bytes.Length - offset;
                byte[] value = new byte[len];
                Array.Copy(bytes, offset, value, 0, len);
                offset += len;
                result = value;
                return (T)result;
            }
            else
            {
                throw new Exception("没有解析该类型: " + type.ToString());
            }
        }
        catch
        {
            return default(T);
        }
    }

    public static byte[] GetBytes<T>(object target)
    {
        byte[] result = new byte[0];
        try
        {
            Type type = typeof(T);
            if (type == typeof(short) || type == typeof(ushort))
            {
                short value = (short)target;
                result = new byte[2];
                result[0] = (byte)((value >> 8) & 0xFF);//高8位
                result[1] = (byte)(value & 0xFF);//低位
            }
            if (type == typeof(int))
            {
                int value = (int)target;
                result = new byte[4];
                result[0] = (byte)((value >> 24) & 0xFF);
                result[1] = (byte)((value >> 16) & 0xFF);
                result[2] = (byte)((value >> 8) & 0xFF);//高8位
                result[3] = (byte)(value & 0xFF);//低位
            }
        }
        catch
        {
            result = new byte[0];
        }

        return result;
    }
}

