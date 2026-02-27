using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressingPanel : MonoBehaviour
{
    

    public Text Count;
    public Text Time;
    public Text Help;
 
    public GameObject pressinGame;


    public GameObject ChuiqiGame;

    public Image ChuiqiImage;
    public Sprite yesChuiqi;
    public Sprite noChuiqi;

    [Header("替换图片")]
    public Sprite red;
    public Sprite green;
    public Sprite yellow;
    public Sprite grey;
    public Sprite empty;
    public Sprite Content;
    public Slider jindu;
    public Transform force;
    public Transform wancheng;
    Color newColor = new Color(0.7490196f, 0.5764706f, 0.02745098f, 0);
    Color oldColor = new Color(0.7490196f, 0.5764706f, 0.02745098f, 1);
    Color greenColor = new Color(0.5294118f, 0.7490196f, 0.4156863f, 1);
    Color redColor = new Color(0.7490196f, 0.3333333f, 0.3333333f, 1);
    Color blueColor = new Color(0.4431373f, 0.6941177f, 0.7490196f, 1);
    public static PressingPanel _Instance;
    public static PressingPanel Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = FindObjectOfType(typeof(PressingPanel)) as PressingPanel;
            }
            return _Instance;
        }
    }
    public void Awake()
    {
        if (Help!=null)
        {
            Help.text = "按压模拟人后将会自动计时。";

        }

        RePressin();
    }
    public void info(string CountText, string TimeText, string Helptext)
    {
        Count.text = CountText;
        Time.text = TimeText;
        Help.text = Helptext;
    }
    public void Pressin(int count, int Level) //按压最大深度156 最小深度约124 32/4=8
    {
        if (pressinGame == null)
        {
            Debug.LogError("pressinGame is null!");
            return;
        }

        // 添加边界检查：确保count在有效范围内
        if (count >= pressinGame.transform.childCount)
        {
            Debug.LogWarning($"Pressin: count {count} 超出范围，最大为 {pressinGame.transform.childCount - 1}");
            return;
        }

        jindu.value = count / 30f;

        if (force != null && count < force.childCount)
        {
            force.GetChild(count).GetComponent<CanvasGroup>().alpha = 1;
        }
        else if (force != null)
        {
            Debug.LogWarning($"Pressin: force 索引 {count} 超出范围，最大为 {force.childCount - 1}");
        }

        if (Level > 9)
        {
            Level = 9;
        }
        else if (Level < 0)
        {
            Level = 0;
        }

        for (int i = 0; i < Level; i++)
        {
            if (Level > 6)
            {
                pressinGame.transform.GetChild(count).GetChild(0).GetChild(9 - i).GetComponent<Image>().sprite = red;

                if (wancheng != null && count < wancheng.childCount)
                {
                    wancheng.GetChild(count).GetComponent<Image>().sprite = Content;
                    wancheng.GetChild(count).GetChild(0).GetComponent<Text>().color = newColor;
                }
                else if (wancheng != null)
                {
                    Debug.LogWarning($"Pressin: wancheng 索引 {count} 超出范围");
                }

                if (force != null && count < force.childCount && force.GetChild(count).GetChild(0) != null)
                {
                    force.GetChild(count).GetChild(0).GetComponent<Text>().text = ((9 - Level) * 30).ToString();
                    force.GetChild(count).GetChild(0).GetComponent<Text>().color = redColor;
                }
            }
            else if (Level < 4)
            {
                pressinGame.transform.GetChild(count).GetChild(0).GetChild(9 - i).GetComponent<Image>().sprite = yellow;

                if (wancheng != null && count < wancheng.childCount)
                {
                    wancheng.GetChild(count).GetComponent<Image>().sprite = Content;
                    wancheng.GetChild(count).GetChild(0).GetComponent<Text>().color = newColor;
                }
                else if (wancheng != null)
                {
                    Debug.LogWarning($"Pressin: wancheng 索引 {count} 超出范围");
                }

                if (force != null && count < force.childCount && force.GetChild(count).GetChild(0) != null)
                {
                    force.GetChild(count).GetChild(0).GetComponent<Text>().text = (100 - (4 - Level) * 25).ToString();
                    force.GetChild(count).GetChild(0).GetComponent<Text>().color = blueColor;
                }
            }
            else
            {
                pressinGame.transform.GetChild(count).GetChild(0).GetChild(9 - i).GetComponent<Image>().sprite = green;

                if (wancheng != null && count < wancheng.childCount)
                {
                    wancheng.GetChild(count).GetComponent<Image>().sprite = Content;
                    wancheng.GetChild(count).GetChild(0).GetComponent<Text>().color = newColor;
                }
                else if (wancheng != null)
                {
                    Debug.LogWarning($"Pressin: wancheng 索引 {count} 超出范围");
                }

                if (force != null && count < force.childCount && force.GetChild(count).GetChild(0) != null)
                {
                    force.GetChild(count).GetChild(0).GetComponent<Text>().text = "100";
                    force.GetChild(count).GetChild(0).GetComponent<Text>().color = greenColor;
                }
            }
        }
    }
    public void RePressin()//重置心肺复苏的内容
    {
        jindu.value = 0;
        for (int i = 0; i < pressinGame.transform.childCount; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                pressinGame.transform.GetChild(i).GetChild(0).GetChild(j).GetComponent<Image>().sprite = grey;
            }
        }
        for (int i = 0; i < force.childCount; i++)
        {
            force.GetChild(i).GetComponent<CanvasGroup>().alpha = 0;
        }
        for (int i = 0; i < wancheng.childCount; i++)
        {
            wancheng.GetChild(i).GetComponent<Image>().sprite = empty;
            wancheng.GetChild(i).GetChild(0).GetComponent<Text>().color = oldColor;
        }

    }

    public void reChuiQIliangPanel()//重置呼吸
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                ChuiqiGame.transform.GetChild(1).GetChild(i).GetChild(0).GetChild(j).GetComponent<Image>().sprite = grey;
            }
        }
    }

    public void chuiqi(int count,int level)
    {

        if (level > 9)
        {
            level = 9;
        }
        else if (level < 0)
        {
            level = 0;
        }

        for (int i=0;i<level;i++)
        {
            if (level<3)
            {
                ChuiqiGame.transform.GetChild(1).GetChild(count).GetChild(0).GetChild(9 - i).GetComponent<Image>().sprite = yellow;

            }
            else if (level > 7)
            {
                ChuiqiGame.transform.GetChild(1).GetChild(count).GetChild(0).GetChild(9 - i).GetComponent<Image>().sprite = red;

            }
           else
            {
                ChuiqiGame.transform.GetChild(1).GetChild(count).GetChild(0).GetChild(9 - i).GetComponent<Image>().sprite = green;

            }
        }
      
    }

    public void chuiqiImagechoice(bool tf)
    {
        if (tf)
        {
            ChuiqiImage.sprite = yesChuiqi;
        }
        else
        {
            ChuiqiImage.sprite = noChuiqi;
        }
    }
}
