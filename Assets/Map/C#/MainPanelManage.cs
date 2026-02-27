using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanelManage : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject SenceChoicePanel;

    public GameObject onePage;
    public GameObject TwoPage;

    public GameObject right;
    public GameObject left;
    // Start is called before the first frame update
    void Start()
    {
        Init();
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Init()
    {
        SenceChoicePanel.SetActive(false);
        MainPanel.SetActive(true);
        onePage.SetActive(true);
        TwoPage.SetActive(false);
        left.SetActive(false);
        right.SetActive(true);
    }
    public void ExamineBth()
    {
        SenceChoicePanel.SetActive(true);
        MainPanel.SetActive(false);
    }
    public void leftBtn()
    {
        left.SetActive(false);
        right.SetActive(true);
        onePage.SetActive(true);
        TwoPage.SetActive(false);
    }
    public void rightBtn()
    {
        right.SetActive(false);

        left.SetActive(true);
        onePage.SetActive(false);
        TwoPage.SetActive(true);
    }
}
