using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EntryScene : MonoBehaviour
{
    [Header("ÊäÈë")]
    public InputField userName;//ÕË»§
    public InputField password;//ÃÜÂë
    public InputField register_userName;//×¢²áÕË»§
    public InputField register_password;//×¢²áÃÜÂë
    [Header("°´Å¥")]
    public Button login;//µÇÂ¼
    public Button register;//×¢²á
    public Button go_register;//È¥×¢²á
    public Button go_login;//È¥µÇÂ¼
    public Button exit1;//ÍË³ö
    public Button exit2;//ÍË³ö
    [Header("Ãæ°å")]
    public GameObject Login_Plane;
    public GameObject Register_Plane;
    public GameObject Liscene_Plane;

    public void Awake()
    {
        go_register.onClick.AddListener(() =>
        {
            Login_Plane.SetActive(false);
            Register_Plane.SetActive(true);
        });
        go_login.onClick.AddListener(()=>
        {
            Login_Plane.SetActive(true);
            Register_Plane.SetActive(false);
        });
    }
    public void OnClickExit()
    {
        userName.text = "";
        password.text = "";
       
        register_userName.text = "";
        register_password.text = "";
        Register_Plane.SetActive(false);
        Login_Plane.SetActive(false);
    }
    public void OnClickLogin()
    {
        
    }
    public void OnClickRegister()
    {

    }
}
