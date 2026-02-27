using UnityEngine.Events;
using System;
#if  UNITY_STANDALONE_WIN //|| UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

/// <summary>
/// 文件管理器
/// </summary>
public class FileFrom
{
    //static FileFormSelectedEvent onFileFormSelected = new FileFormSelectedEvent();

    //static void Invoke(string path)
    //{
    //    onFileFormSelected.Invoke(path);
    //    onFileFormSelected.RemoveAllListeners();
    //}

    /// <summary>
    /// 打开文件选择面板
    /// </summary>
    /// <param name="title"></param>
    /// <param name="multiselect"></param>
    /// <param name="filter"></param>
    /// <param name="defext"></param>
    /// <returns></returns>
    public static string OpenFile(string title, bool multiselect, string filter, string defext)
    {
        try
        {
            return OpenFile("C:\\", title, multiselect, filter, defext);
        }
        catch (Exception e)
        {
            EditorDebug.Log(e.ToString());
            return OpenFileOld(title, multiselect, filter);
        }
    }

    ///// <summary>
    ///// 使用事件返回面板
    ///// </summary>
    ///// <param name="title"></param>
    ///// <param name="multiselect"></param>
    ///// <param name="filter"></param>
    ///// <param name="defext"></param>
    ///// <param name="endselect"></param>
    //public static void OpenFile(string title, bool multiselect, string filter, string defext, UnityAction<string> endselect)
    //{
    //    string path = OpenFile(title, multiselect, filter, defext);
    //    Invoke(path);
    //}

    /// <summary>
    /// 打开保存文件面板
    /// </summary>
    /// <param name="title"></param>
    /// <param name="multiselect"></param>
    /// <param name="filter"></param>
    /// <param name="defext"></param>
    /// <returns></returns>
    public static string SaveFile(string title, bool multiselect, string filter, string defext)
    {
        try
        {
            return SaveFile("C:\\", title, multiselect, filter, defext);
        }
        catch (Exception e)
        {
            EditorDebug.Log(e.ToString());
            return SaveFileOld(title, filter);
        }
    }

    /// <summary>
    /// 打开保存至文件夹面板
    /// </summary>
    /// <param name="title"></param>
    /// <param name="multiselect"></param>
    /// <param name="filter"></param>
    /// <param name="defext"></param>
    /// <returns></returns>
    public static string SaveFolder(string title, bool multiselect, string filter, string defext)
    {
        try
        {
            return SaveFile("C:\\", title, multiselect, filter, defext);
        }
        catch (Exception e)
        {
            EditorDebug.Log(e.ToString());
            return SaveFileOld(title, filter);
        }
    }

    static string OpenFile(string path, string title, bool multiselect, string filter, string defext)
    {
#if  UNITY_STANDALONE_WIN //|| UNITY_EDITOR
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        ofn.filter = filter;

        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;

        path = path.Replace('/', '\\');
        //默认路径
        ofn.initialDir = path;
        //ofn.InitialDirectory = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";
        ofn.title = title;

        ofn.defExt = defext;//显示文件的类型
        //注意 一下项目不一定要全选 但是0x00000008项不要缺少
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;//OFN_EXPLORER|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT | 0x00000200 |OFN_NOCHANGEDIR

        if (WindowDll.GetOpenFileName(ofn))
        {
            EditorDebug.Log("Selected file with full path: {0}" + ofn.file);
        }
        return ofn.file;
#else
        return "";
#endif
    }

    static string SaveFile(string path, string title, bool multiselect, string filter, string defext)
    {
#if  UNITY_STANDALONE_WIN //|| UNITY_EDITOR
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        ofn.filter = filter;

        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;

        path = path.Replace('/', '\\');
        //默认路径
        ofn.initialDir = path;
        //ofn.InitialDirectory = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";
        ofn.title = title;

        ofn.defExt = defext;//显示文件的类型
        //注意 一下项目不一定要全选 但是0x00000008项不要缺少
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (WindowDll.GetSaveFileName(ofn))
        {
            EditorDebug.Log("Selected file with full path: {0}" + ofn.file);
        }
        return ofn.file;
#else
        return "";
#endif
    }

    public static string OpenFileOld(string title, bool multiselect, string filter)
    {
#if  UNITY_STANDALONE_WIN || UNITY_EDITOR
        //OpenFileDialog od = new OpenFileDialog();
        //od.Title = title;
        //od.Multiselect = multiselect;
        //od.Filter = filter;// "图片文件(*.jpg,*.png,*.bmp)|*.jpg;*.png;*.bmp";

        //if (od.ShowDialog() == DialogResult.OK)
        //{
        //    return od.FileName;
        //}
#endif
        return "";
    }

    public static string SaveFileOld(string title, string filter)
    {
#if  UNITY_STANDALONE_WIN || UNITY_EDITOR
        //SaveFileDialog sd = new SaveFileDialog();
        //sd.Title = title;
        //sd.Filter = filter;

        //if (sd.ShowDialog() == DialogResult.OK)
        //{
        //    return sd.FileName;
        //}
#endif
        return "";
    }
}

#if  UNITY_STANDALONE_WIN //|| UNITY_EDITOR
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}
#endif

public class WindowDll
{
#if  UNITY_STANDALONE_WIN //|| UNITY_EDITOR
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

    public static bool GetOpenFileNameI([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);
    }

    public static bool GetSaveFileNameI([In, Out] OpenFileName ofn)
    {
        return GetSaveFileName(ofn);
    }
#endif
}

/// <summary>
/// 文件格式过滤器
/// </summary>
public static class FileFilter
{
    public const string Image = "图片文件(*.jpg,*.png)\0*.jpg;*png\0";
    public const string ImageExt = "png";

    public const string Model = "模型文件(*.unity3d)\0*.unity3d\0";
    public const string ModelExt = "unity3d";

    public const string Project = "工程文件(*.yjh)\0*.yjh\0";
    public const string ProjectExt = "yjh";

    public const string Json = "Json文件(*.json)\0*.json\0";
    public const string JsonExt = "json";

    public const string Folder = "文件夹\0*.wjjgl\0";
    public const string FolderExt = "wjjgl";
}

/// <summary>
/// 文件管理器选择框选择完毕事件
/// </summary>
public class FileFormSelectedEvent : UnityEvent<string> { }
