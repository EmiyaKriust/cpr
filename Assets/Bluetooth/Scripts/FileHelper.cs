using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
/// <summary>
/// 文件操作
/// </summary>
public class FileHelper : MonoBehaviour
{

    /// <summary>
    /// 将文件移到固定目录
    /// </summary>
    /// <param name="srcPath"></param>
    /// <param name="targetFold"></param>
    /// <returns></returns>
    public static bool CopyFile(ref string srcPath, string targetFold)
    {
        //**文件路径就是在该固定目录下**//
        srcPath = srcPath.Replace('\\', '/');
        if (srcPath == (targetFold + System.IO.Path.GetFileName(srcPath)))
        {
            srcPath = System.IO.Path.GetFileName(srcPath);
            return true;
        }
        // string filename = System.IO.Path.GetFileName(sourcePath);
        try
        {
            Debug.Log("srcPath:" + srcPath);


            // 判断目标目录是否存在如果不存在则新建
            if (!System.IO.Directory.Exists(targetFold))
            {
                System.IO.Directory.CreateDirectory(targetFold);
            }

           
            string newpath = targetFold + "/" + System.IO.Path.GetFileName(srcPath);
            Debug.Log("targetPath+srcPath:" + newpath);
            //**判断固定目录下有没有该文件**//
            if (!System.IO.File.Exists(newpath))
            {
               
                System.IO.File.Copy(srcPath, newpath);
                srcPath = System.IO.Path.GetFileName(srcPath);
            }
            //**该固定目录下有这个文件**//
            else
            {

                Debug.Log("lujing2:" + newpath);
                File.Delete(newpath);
                Debug.Log("lujing22:" + newpath);
                System.IO.File.Copy(srcPath, newpath);
                srcPath = System.IO.Path.GetFileName(srcPath);


                //Debug.Log("lujing2:" + targetPath + System.IO.Path.GetFileNameWithoutExtension(srcPath)  + System.IO.Path.GetExtension(srcPath));
                //File.Delete(targetPath + System.IO.Path.GetExtension(srcPath));

                //Debug.Log("lujing22:" + targetPath + System.IO.Path.GetFileNameWithoutExtension(srcPath)+System.IO.Path.GetExtension(srcPath));
                //System.IO.File.Copy(srcPath, targetPath + System.IO.Path.GetFileName(srcPath));
                //// System.IO.File.Copy(srcPath, targetPath + System.IO.Path.GetFileNameWithoutExtension(srcPath) + System.IO.Path.GetExtension(srcPath));
                //srcPath = System.IO.Path.GetFileNameWithoutExtension(srcPath) + System.IO.Path.GetExtension(srcPath);
                ////   //Debug.Log("srcPathsrcPath:" + srcPath);
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
            //throw;

        }

    }

    public static bool CopyDir(ref string srcPath, string targetPath, string sign)
    {
        //**文件路径就是在该固定目录下**//
        srcPath = srcPath.Replace('\\', '/');
        //Debug.Log("srcPath:" + srcPath);
        string fileName = System.IO.Path.GetFileNameWithoutExtension(srcPath);
        //Debug.Log("fileName:" + fileName);
        if (fileName.Length < sign.Length)
            fileName += sign;
        else if (fileName.Substring(fileName.Length - 3, 3) != sign)
        {
            fileName += sign;
        }
        //Debug.Log("fileName:" + fileName);
        string ss = System.IO.Path.GetExtension(srcPath);
        //Debug.Log("ss:" + ss);
        //path = fileName + ss;
        ////Debug.Log("path:" + path);
        fileName += ss;
        if (srcPath == (targetPath + fileName))
        {
            srcPath = fileName;
            //Debug.Log("srcPathTrue:"+srcPath);
            return true;
        }
        // string filename = System.IO.Path.GetFileName(sourcePath);
        try
        {
            Debug.Log("srcPath:" + srcPath);


            // 判断目标目录是否存在如果不存在则新建
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            Debug.Log("targetPath+srcPath:" + targetPath + fileName);
            //**判断固定目录下有没有该文件**//
            if (!System.IO.File.Exists(targetPath + fileName))
            {
                Debug.Log("lujing1:" + targetPath + fileName);
                System.IO.File.Copy(srcPath, targetPath + fileName);
                srcPath = fileName;
            }
            //**该固定目录下有这个文件**//
            else
            {

                Debug.Log("lujing2:" + targetPath + fileName);
                File.Delete(targetPath + fileName);
                Debug.Log("lujing22:" + targetPath + fileName);
                System.IO.File.Copy(srcPath, targetPath + fileName);
                srcPath = fileName;


                //Debug.Log("lujing2:" + targetPath + System.IO.Path.GetFileNameWithoutExtension(srcPath)  + System.IO.Path.GetExtension(srcPath));
                //File.Delete(targetPath + System.IO.Path.GetExtension(srcPath));

                //Debug.Log("lujing22:" + targetPath + System.IO.Path.GetFileNameWithoutExtension(srcPath)+System.IO.Path.GetExtension(srcPath));
                //System.IO.File.Copy(srcPath, targetPath + System.IO.Path.GetFileName(srcPath));
                //// System.IO.File.Copy(srcPath, targetPath + System.IO.Path.GetFileNameWithoutExtension(srcPath) + System.IO.Path.GetExtension(srcPath));
                //srcPath = System.IO.Path.GetFileNameWithoutExtension(srcPath) + System.IO.Path.GetExtension(srcPath);
                ////   //Debug.Log("srcPathsrcPath:" + srcPath);
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
            //throw;

        }

    }
    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="path"></param>
    internal static void DeleteSingleFile(string path)
    {

        if (ExitFile(path))
            File.Delete(path);

    }

    internal static void DeleteFile(string path)
    {
#if (UNITY_WEBGL || UNITY_ANDROID)
        if (Directory.Exists(path))
        {
            try
            {
                DirectoryInfo direction = new DirectoryInfo(path);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    files[i].Delete();
                    //if (System.IO.Directory.Exists(files[i].Name))
                    //{
                    //    System.IO.Directory.Delete(path);
                    //}
                    //if (files[i].Name.EndsWith(".meta"))
                    //{
                    //    continue;
                    //}
                }

                if (System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.Delete(path);
                }
            }
            catch (Exception e)
            {

            }

        }

        //if (ExitFile(path))
        //    File.Delete(path);
#endif
    }

    /// <summary>
    /// 复制文件夹至另一文件夹
    /// </summary>
    /// <param name="srcPath"></param>
    /// <param name="aimPath"></param>
    public static void CopyDir(string srcPath, string aimPath)
    {
        try
        {
#if !(UNITY_WEBGL || UNITY_ANDROID)
            // 检查目标目录是否以目录分割字符结束如果不是则添加
            if (aimPath[aimPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                aimPath += System.IO.Path.DirectorySeparatorChar;
            }
            // 判断目标目录是否存在如果不存在则新建
            if (!System.IO.Directory.Exists(aimPath))
            {
                System.IO.Directory.CreateDirectory(aimPath);
            }
            // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
            // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
            // string[] fileList = Directory.GetFiles（srcPath）；
            string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
            // 遍历所有的文件和目录
            foreach (string file in fileList)
            {
                // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                if (System.IO.Directory.Exists(file))
                {
                    CopyDir(file, aimPath + System.IO.Path.GetFileName(file));
                }
                // 否则直接Copy文件
                else
                {
                    System.IO.File.Copy(file, aimPath + System.IO.Path.GetFileName(file), true);
                }
            }
#endif
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public static void CopyFile(string srcPath, string aimPath)
    {
        try
        {
            // 检查目标目录是否以目录分割字符结束如果不是则添加
            if (aimPath[aimPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                aimPath += System.IO.Path.DirectorySeparatorChar;
            }
            // 判断目标目录是否存在如果不存在则新建
            if (!System.IO.Directory.Exists(aimPath))
            {
                System.IO.Directory.CreateDirectory(aimPath);
            }
            // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
            // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
            // string[] fileList = Directory.GetFiles（srcPath）；
            //string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
            // 遍历所有的文件和目录
            //foreach (string file in fileList)
            //{
            // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
            //if (System.IO.Directory.Exists(file))
            //{
            //    CopyDir(file, aimPath + System.IO.Path.GetFileName(file));
            //}
            //// 否则直接Copy文件
            //else
            //{
            //    System.IO.File.Copy(file, aimPath + System.IO.Path.GetFileName(file), true);
            //}
            //}
            //if (System.IO.Directory.Exists(srcPath))
            //{
            System.IO.File.Copy(srcPath, aimPath + System.IO.Path.GetFileName(srcPath), true);
            //CopyDir(file, aimPath + System.IO.Path.GetFileName(file));
            //}
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    public static void CopyFile(string srcPath, string aimPath, int intPlatform)
    {
        try
        {
            // 检查目标目录是否以目录分割字符结束如果不是则添加
            if (aimPath[aimPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                aimPath += System.IO.Path.DirectorySeparatorChar;
            }
            // 判断目标目录是否存在如果不存在则新建
            if (!System.IO.Directory.Exists(aimPath))
            {
                System.IO.Directory.CreateDirectory(aimPath);
            }
            // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
            // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
            // string[] fileList = Directory.GetFiles（srcPath）；
            //string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
            // 遍历所有的文件和目录
            //foreach (string file in fileList)
            //{
            // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
            //if (System.IO.Directory.Exists(file))
            //{
            //    CopyDir(file, aimPath + System.IO.Path.GetFileName(file));
            //}
            //// 否则直接Copy文件
            //else
            //{
            //    System.IO.File.Copy(file, aimPath + System.IO.Path.GetFileName(file), true);
            //}
            //}
            //if (System.IO.Directory.Exists(srcPath))
            //{
            string fileName = System.IO.Path.GetFileName(srcPath);
            string targetPath = aimPath + fileName;
            if (intPlatform == 2)
                targetPath = System.IO.Path.ChangeExtension(targetPath, ".ar.md");
            System.IO.File.Copy(srcPath, targetPath, true);
            //CopyDir(file, aimPath + System.IO.Path.GetFileName(file));
            //}
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    /// <summary>
    ///  写入文件夹
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="infoStr">内容</param>
    /// <returns></returns>
    public static bool WriteIntoFile(string filePath, string infoStr,string fileFoldPath ="")
    {
        try
        {
            FileStream fs;

            if (!string.IsNullOrEmpty(fileFoldPath))
            {
                if (!FileHelper.ExitFolder(fileFoldPath))
                    FileHelper.CreateFolder(fileFoldPath);
            }

            FileInfo t = new FileInfo(filePath);
            if (!t.Exists)
            {
                //如果此文件不存在则创建
                fs = t.Open(FileMode.Create, FileAccess.Write);
            }
            else
            {
                //如果此文件存在则打开,清空文件内容
                fs = t.Open(FileMode.Truncate, FileAccess.Write);
                // sw = t.;
            }
            byte[] bt = System.Text.Encoding.UTF8.GetBytes(infoStr);
            fs.Write(bt, 0, bt.Length);
            //关闭流
            fs.Close();
            //销毁流
            fs.Dispose();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
            //throw;

        }
    }
    /// <summary>
    ///  写入文件夹
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="infoStr">内容</param>
    /// <returns></returns>
    public static bool WriteAppendIntoFile(string filePath, string infoStr, string fileFoldPath = "")
    {
        try
        {
            FileStream fs;

            if (!string.IsNullOrEmpty(fileFoldPath))
            {
                if (!FileHelper.ExitFolder(fileFoldPath))
                    FileHelper.CreateFolder(fileFoldPath);
            }

            FileInfo t = new FileInfo(filePath);
            if (!t.Exists)
            {
                //如果此文件不存在则创建
                fs = t.Open(FileMode.Create, FileAccess.Write);
            }
            else
            {
                //如果此文件存在则打开,清空文件内容
                fs = t.Open(FileMode.Append, FileAccess.Write);
                // sw = t.;
            }
            byte[] bt = System.Text.Encoding.UTF8.GetBytes(infoStr);
            fs.Write(bt, 0, bt.Length);
            //关闭流
            fs.Close();
            //销毁流
            fs.Dispose();
            return true;
        }
        catch (Exception e)
        {
            EditorDebug.Log(e.ToString());
            return false;
            //throw;

        }
    }

    /// <summary>
    ///  资源写入文件夹
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="infoStr">内容</param>
    /// <returns></returns>
    public static bool WriteByteIntoFile(string filePath, byte[] bt, string fileFoldPath = "")
    {
        try
        {
            FileStream fs;
            if (!string.IsNullOrEmpty(fileFoldPath))
            {
                if (!FileHelper.ExitFolder(fileFoldPath))
                    FileHelper.CreateFolder(fileFoldPath);
            }
            FileInfo t = new FileInfo(filePath);
            if (!t.Exists)
            {
                //如果此文件不存在则创建
                fs = t.Open(FileMode.Create, FileAccess.Write);
            }
            else
            {
                //如果此文件存在则打开,清空文件内容
                fs = t.Open(FileMode.Truncate, FileAccess.Write);
                // sw = t.;
            }
            fs.Write(bt, 0, bt.Length);
            //关闭流
            fs.Close();
            //销毁流
            fs.Dispose();
            //LogModel.Log("文件写入成功");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    /// <summary>
    /// 读取文件内容
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string ReadFromFile(string filePath)
    {
        try
        {
            //使用流的形式读取
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(filePath);
            }
            catch (Exception e)
            {
                //路径与名称未找到文件则直接返回空
                Debug.Log(e.ToString());
                return null;
            }
            string line;
            string filecontent = "";
            //  ArrayList arrlist = new ArrayList();
            while ((line = sr.ReadLine()) != null)
            {
                //一行一行的读取
                //将每一行的内容存入数组链表容器中
                //  arrlist.Add(line);
                filecontent += line;
            }
            //关闭流
            sr.Close();
            //销毁流
            sr.Dispose();
            //将数组链表容器返回
            return filecontent;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
            //throw;

        }
    }
    /// <summary>
    /// 读取文件内容
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static T ReadObjectFromFile<T>(string filePath)
    {
        try
        {
            T data = default(T);
            if (File.Exists(filePath))
            {
                FileStream fs = File.Open(filePath, FileMode.Open);

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                data = (T)binaryFormatter.Deserialize(fs);
                //关闭流
                fs.Close();
                //销毁流
                fs.Dispose();
                Debug.Log("文件读写成功");
            }
            return data;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return default(T);
            //throw;

        }
    }
    /// <summary>
    /// 读取文件转换为byte数组
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] ReadFileToBytes(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

        fileStream.Seek(0, SeekOrigin.Begin);

        byte[] binary = new byte[fileStream.Length]; //创建文件长度的buffer
        fileStream.Read(binary, 0, (int)fileStream.Length);

        fileStream.Close();

        fileStream.Dispose();

        fileStream = null;

        return binary;
    }


    /// <summary>
    /// 动态创建文件夹.
    /// </summary>
    /// <returns>The folder.</returns>
    /// <param name="path">文件创建目录.</param>
    /// <param name="FolderName">文件夹名(不带符号).</param>
    public static string CreateFolder(string path, string FolderName)
    {
        string FolderPath = path + FolderName;
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }
        return FolderPath;
    }

    /// <summary>
    /// 创建文件.
    /// </summary>
    /// <param name="path">完整文件夹路径.</param>
    /// <param name="name">文件的名称.</param>
    /// <param name="info">写入的内容.</param>
    public void CreateFile(string path, string name, string info)
    {
        //文件流信息
        StreamWriter sw;
        FileInfo t = new FileInfo(path + name);
        if (!t.Exists)
        {
            //如果此文件不存在则创建
            sw = t.CreateText();
        }
        else
        {
            //如果此文件存在则打开
            sw = t.AppendText();
        }
        //以行的形式写入信息
        sw.WriteLine(info);
        //关闭流
        sw.Close();
        //销毁流
        sw.Dispose();
    }

    /// <summary>
    /// 读取文件.
    /// </summary>
    /// <returns>The file.</returns>
    /// <param name="path">完整文件夹路径.</param>
    /// <param name="name">读取文件的名称.</param>
    public ArrayList LoadFile(string path, string name)
    {
        //使用流的形式读取
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + name);
        }
        catch (Exception e)
        {
            //路径与名称未找到文件则直接返回空
            Debug.Log(e.ToString());
            return null;
        }
        string line;
        ArrayList arrlist = new ArrayList();
        while ((line = sr.ReadLine()) != null)
        {
            //一行一行的读取
            //将每一行的内容存入数组链表容器中
            arrlist.Add(line);
        }
        //关闭流
        sr.Close();
        //销毁流
        sr.Dispose();
        //将数组链表容器返回
        return arrlist;
    }
    //写入模型到本地
    IEnumerator loadassetbundle(string url)
    {
        WWW w = new WWW(url);
        yield return w;
        if (w.isDone)
        {
            byte[] model = w.bytes;
            int length = model.Length;
            //写入模型到本地
            CreateassetbundleFile(Application.persistentDataPath, "Model.assetbundle", model, length);
        }
    }
    /// <summary>
    /// 获取文件下所有文件大小
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public int GetAllFileSize(string filePath)
    {
        int sum = 0;

#if !(UNITY_WEBGL || UNITY_ANDROID)
        if (!Directory.Exists(filePath))
        {
            return 0;
        }

        DirectoryInfo dti = new DirectoryInfo(filePath);

        FileInfo[] fi = dti.GetFiles();

        foreach (FileInfo f in fi)
        {

            sum += Convert.ToInt32(f.Length / 1024);
        }

        DirectoryInfo[] di = dti.GetDirectories();

        if (di.Length > 0)
        {
            for (int i = 0; i < di.Length; i++)
            {
                sum += GetAllFileSize(di[i].FullName);
            }
        }
#endif
        return sum;
    }
    /// <summary>
    /// 获取指定文件大小
    /// </summary>
    /// <param name="FilePath"></param>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public int GetFileSize(string FilePath, string FileName)
    {
        int sum = 0;
#if !(UNITY_WEBGL || UNITY_ANDROID)
        if (!Directory.Exists(FilePath))
        {
            return 0;
        }
        else
        {
            FileInfo Files = new FileInfo(@FilePath + FileName);
            sum += Convert.ToInt32(Files.Length / 1024);
        }
#endif
        return sum;
    }

    /// <summary>
    /// 创建新文件
    /// </summary>
    /// <param name="content"></param>
    /// <param name="path"></param>
    public static void WriteFile(byte[] content, string path)
    {
        byte[] stream = content;
        FileStream fs = new FileStream(path, FileMode.CreateNew);
        // Create the writer for data.
        BinaryWriter w = new BinaryWriter(fs);
        // Write data to Test.data.
        w.Write(stream);
        w.Close();
        fs.Close();
    }
    void CreateassetbundleFile(string path, string name, byte[] info, int length)
    {
#if !(UNITY_WEBGL || UNITY_ANDROID)
        //文件流信息
        //StreamWriter sw;
        Stream sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (!t.Exists)
        {
            //如果此文件不存在则创建
            sw = t.Create();
        }
        else
        {
            //如果此文件存在则打开
            //sw = t.Append();
            return;
        }
        //以行的形式写入信息
        sw.Write(info, 0, length);
        //关闭流
        sw.Close();
        //销毁流
        sw.Dispose();
#endif
    }
    //读取本地AssetBundle文件
    IEnumerator LoadAssetbundleFromLocal(string path, string name)
    {
        //Debug.Log("file:///" + path + "/" + name);

        WWW w = new WWW("file:///" + path + "/" + name);

        yield return w;

        if (w.isDone)
        {
            Instantiate(w.assetBundle.mainAsset);

        }
    }

    /// <summary>
    /// 删除文件.
    /// </summary>
    /// <param name="path">删除完整文件夹路径.</param>
    /// <param name="name">删除文件的名称.</param>
    public void DeleteFile(string path, string name)
    {
        File.Delete(path + name);
    }
    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="filesName"></param>
    /// <returns></returns>
    public bool DeleteFiles(string path, string filesName)
    {
        bool isDelete = false;
        try
        {
            if (Directory.Exists(path))
            {
                if (File.Exists(path + "\\" + filesName))
                {
                    File.Delete(path + "\\" + filesName);
                    isDelete = true;
                }
            }
        }
        catch
        {
            return isDelete;
        }
        return isDelete;
    }

    /// <summary>
    /// 是否存在文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool ExitFile(string path)
    {
        return File.Exists(path);
    }

    public static bool ExitFolder(string v)
    {
        return Directory.Exists(v);
    }

    public static void CreateFolder(string v)
    {
        if (!ExitFolder(v))
            Directory.CreateDirectory(v);
    }
}
