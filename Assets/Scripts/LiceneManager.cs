using UnityEngine;
using License;
using System;
using MsgBoxBase = System.Windows.Forms.MessageBox; //引用命名空间下消息类
using WinForms = System.Windows.Forms;              //引用命名空间
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LiceneManager : MonoBehaviour
{

    public GameObject LicenePlane;
    public Button start;
    private string cpuid;
    // Use this for initialization
    void Start()
    {
        start.onClick.AddListener(entryScene);
        LicenesCheck lc = new LicenesCheck();
        string licPath = Application.dataPath;
        cpuid = lc.GetProcessorId();
        Debug.Log(cpuid);
    }
    void licenseCheck()
    {
        LicenesCheck lc = new LicenesCheck();
        string licPath = Application.dataPath;
        cpuid =lc. GetProcessorId();
        Debug.Log(cpuid);
        int isLicOk = lc.Verification(licPath);
        Debug.Log("" + isLicOk);

        switch (isLicOk)
        {
            /// 0 许可证文件正确，且在授权期限内
            case 0:
                SceneManager.LoadScene("Main");
                break;
            /// 1 没有许可证文件
            case 1:
               
                break;
            /// 2 许可证文件已失时效
            case 2:
              
                break;
                break;
            /// 3 许可证文件非法！
            case 3:
               
                break;
            default:
                quit();
                break;
        }
    }

    void entryScene()
    {
        SceneManager.LoadScene("Main");
    }

    void quit()
    {
        Application.Quit();
    }

}