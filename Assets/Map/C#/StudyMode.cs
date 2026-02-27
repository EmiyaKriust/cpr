using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StudyMode : MonoBehaviour
{
    [Header("按压语音")]
    public AudioClip[] Audio_presstip;
    public AudioSource MainAudio;
    [Header("信息面板")]
    public GameObject InfoPanel;
    public Text InfoPanel_Content_Text;
    public Text InfoPanel_Btn_Text;
    public Button InfoPanel_Btn;
    public GameObject massagePanel;
    public GameObject massagePanel_2;

    [Header("全局变量")]
    private string currentSceneName;

    [Header("环顾四周，是否搬运")]
    public bool isSafety;
    public GameObject ChangePlace;
    public GameObject oneStepPanel;
    public Button lookButton;
    public Button transf_Yes;
    public Button transf_No;
    public GameObject camera1;

    float time;
    public GameObject SpeakPanel;
    public GameObject pressingPanel;
    public GameObject pressingPanel2;
    public GameObject ChuiqiPanel;
    public int StepNumber = 0;

    public GameObject endPanel;
    public GameObject endPanel2;
    public Button continueButton;
    public Text endPanelText;

    [Header("动画")]
    public AnimationManager character_Ani;

    public GameObject schedulePanel;
    public Sprite OverSprite;
    public Sprite NoOverSprite;

    [Header("初始动画")]
    public GameObject StartCamera;
    public GameObject StartMan;

    [Header("考核")]
    public bool isPlayAnim = false;
    public int Score;
    public bool isend_Exam = true;
    private int repertNumber = 0;

    bool waitButtonOn = false;
    public UnityEvent OnButtonClicked;

    public static StudyMode _Instance;
    public static StudyMode Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = FindObjectOfType(typeof(StudyMode)) as StudyMode;
            }
            return _Instance;
        }
    }

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < schedulePanel.transform.childCount; i++)
        {
            schedulePanel.transform.GetChild(i).GetComponent<Image>().sprite = NoOverSprite;
        }
        CloseAllUI();
        StartCoroutine(FlowManager());

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(() =>
            {
                StartCoroutine(repeattrain());
                DataLogic.Instance.IsPeopleChuiQi = true;
                endPanel2.SetActive(false);
            });
        }
    }

    public IEnumerator FlowManager()
    {
        CloseAllUI();
        if (currentSceneName == "StudyMode")
        {
            yield return Pre_animation();
            Debug.LogWarning("步骤1 结束");
            yield return AssessConsciousness();
            Debug.LogWarning("步骤2 结束");
            yield return CheckTR();
            Debug.LogWarning("步骤3 结束");
            yield return CryForHelp();
            Debug.LogWarning("步骤4 结束");
            yield return PressTrain();
            Debug.LogWarning("步骤5 结束");
            yield return CleanMouth();
            Debug.LogWarning("步骤6 结束");
            yield return ChuiQi();
            Debug.LogWarning("步骤7 结束");
            yield return endStudy1();
            Debug.LogWarning("结束面板1");

            // 只进行一次重复训练
            yield return repeattrain();
            yield return repeatBreathing();

            Debug.LogWarning("重复训练");
            yield return endStudy2();
        }
        else
        {
            yield return Pre_animation();
            Debug.LogWarning("步骤1 结束");
            yield return AnYa_Pre();
            Debug.LogWarning("步骤2 结束 - 前四个步骤完成");

            // 在考核模式下设置StepNumber为4，确保DataLogic能识别按压步骤
            StepNumber = 4;
            Debug.Log("考核模式：设置StepNumber为4，准备开始按压训练");

            // 考核模式下的循环
            for (int i = 0; i < 5; i++) // 5个循环
            {
                Debug.LogWarning($"开始第 {i + 1} 个CPR循环");
                yield return AnYaTrain();
                yield return ChuiQiTrain();

                // 每次循环后重置相关状态
                DataLogic.Instance.IsPeopleBreathing = false;
                DataLogic.Instance.IsPeopleChuiQi = false;
                DataLogic.Instance.isAnYaIng = false;
                DataLogic.Instance.isChuiQiIng = false;

                // 重置计数
                DataLogic.Instance.Number = 0;
                DataLogic.Instance.BreathingNumber = 0;
                DataLogic.Instance.Number_Count = 0;
                DataLogic.Instance.BreathingNumber_Count = 0;

                // 重置按压和吹气数据列表
                DataLogic.Instance.DetailsData = new List<int>();
                DataLogic.Instance.DetailsDataYY = new List<int>();

                yield return new WaitForSeconds(1f); // 循环间的延迟
            }

            yield return endStudy3();
        }
    }

    public void tip(int i)
    {
        MainAudio.Stop();
        if (i < Audio_presstip.Length)
        {
            MainAudio.clip = Audio_presstip[i];
            MainAudio.Play();
            
        }
    }

    public IEnumerator Pre_animation()
    {
        if (currentSceneName == "StudyMode")
        {
            yield return null;
            StartMan.SetActive(true);
            StartCamera.SetActive(true);
            camera1.SetActive(false);
            yield return new WaitForSeconds(8f);

            oneStepPanel.SetActive(true);
            massagePanel.SetActive(true);
            schedulePanel.SetActive(true);

            StartCamera.SetActive(false);
            camera1.SetActive(true);
            StartMan.SetActive(false);
            character_Ani.gameObject.SetActive(true);

            MassageManage.Instance.info("Step One", @"Confirm Scene Safety...");

            waitButtonOn = false;
            UnityAction action = () => waitButtonOn = true;
            lookButton.onClick.AddListener(action);
            yield return new WaitUntil(() => waitButtonOn);
            lookButton.onClick.RemoveListener(action);
            massagePanel.SetActive(false);

            camera1.GetComponent<Animator>().SetBool("Play", true);
            yield return new WaitForSeconds(10f);
            oneStepPanel.SetActive(false);
            ChangePlace.SetActive(true);

            int yesorno = 0;
            UnityAction yes = () => yesorno = 1;
            UnityAction no = () => yesorno = 2;
            transf_Yes.onClick.AddListener(yes);
            transf_No.onClick.AddListener(no);
            yield return new WaitUntil(() => yesorno != 0);
            transf_Yes.onClick.RemoveListener(yes);
            transf_No.onClick.RemoveListener(no);
            ChangePlace.SetActive(false);

            if (isSafety == false)
            {
                if (yesorno == 2)
                {
                    InfoPanel.gameObject.SetActive(true);
                    InfoPanel_Btn_Text.text = "End Training";
                    InfoPanel_Content_Text.text = "Failed to move the casualty to a safe location.";
                    InfoPanel_Btn.onClick.AddListener(() => {
                        SceneManager.LoadScene("Main");
                    });
                    StopAllCoroutines();
                }
            }
            else if (isSafety == true)
            {
                if (yesorno == 1)
                {
                    InfoPanel.gameObject.SetActive(true);
                    InfoPanel_Btn_Text.text = "Confirm";
                    InfoPanel_Content_Text.text = "Scene is safe, no need to move the casualty.";
                    waitButtonOn = false;
                    UnityAction waitInfoButton = () => waitButtonOn = true;
                    InfoPanel_Btn.onClick.AddListener(waitInfoButton);
                    yield return new WaitUntil(() => waitButtonOn);
                    InfoPanel.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            yield return null;
            StartMan.SetActive(true);
            StartCamera.SetActive(true);
            camera1.SetActive(false);
            yield return new WaitForSeconds(8f);

            oneStepPanel.SetActive(true);
            StartCamera.SetActive(false);
            camera1.SetActive(true);
            StartMan.SetActive(false);
            character_Ani.gameObject.SetActive(true);

            waitButtonOn = false;
            UnityAction action = () => waitButtonOn = true;
            lookButton.onClick.AddListener(action);
            yield return new WaitUntil(() => waitButtonOn);
            lookButton.onClick.RemoveListener(action);
            massagePanel.SetActive(false);

            camera1.GetComponent<Animator>().SetBool("Play", true);
            yield return new WaitForSeconds(10f);
            oneStepPanel.SetActive(false);
            ChangePlace.SetActive(true);

            int yesorno = 0;
            UnityAction yes = () => yesorno = 1;
            UnityAction no = () => yesorno = 2;
            transf_Yes.onClick.AddListener(yes);
            transf_No.onClick.AddListener(no);
            yield return new WaitUntil(() => yesorno != 0);
            transf_Yes.onClick.RemoveListener(yes);
            transf_No.onClick.RemoveListener(no);
            ChangePlace.SetActive(false);

            if (isSafety == false)
            {
                if (yesorno == 2)
                {
                    InfoPanel.gameObject.SetActive(true);
                    InfoPanel_Btn_Text.text = "End Training";
                    InfoPanel_Content_Text.text = "Failed to move casualty to a safe location.";
                    InfoPanel_Btn.onClick.AddListener(() => {
                        SceneManager.LoadScene("Main");
                    });
                    StopAllCoroutines();
                }
            }
            else if (isSafety == true)
            {
                if (yesorno == 1)
                {
                    InfoPanel.gameObject.SetActive(true);
                    InfoPanel_Btn_Text.text = "Confirm";
                    InfoPanel_Content_Text.text = "Scene is safe, no need to move the casualty.";
                    waitButtonOn = false;
                    UnityAction waitInfoButton = () => waitButtonOn = true;
                    InfoPanel_Btn.onClick.AddListener(waitInfoButton);
                    yield return new WaitUntil(() => waitButtonOn);
                    InfoPanel.gameObject.SetActive(false);
                }
            }
        }
    }

    public IEnumerator AssessConsciousness()
    {
        character_Ani.gameObject.SetActive(true);
        massagePanel.SetActive(true);
        MassageManage.Instance.info("Step Two", @"Check Responsiveness...");
        ChangeStepIcon();

        yield return character_Ani.Play(0f, 1.75f, 1);
        StepNumber = 1;

        // 重置状态
        DataLogic.Instance.IsConsciousness = false;
        yield return new WaitUntil(() => DataLogic.Instance.IsConsciousness);

        massagePanel.SetActive(false);
        SpeakPanel.SetActive(true);
        yield return Speak.Instance.info("Rescuer", "Comrade, are you okay?", 0);
        yield return character_Ani.Play(1.75f, 3.7f, 1);
        yield return Speak.Instance.info("Patient", "……", 1);

        SpeakPanel.SetActive(false);
        ChangeStepIcon();
    }

    public IEnumerator CheckTR()
    {
        yield return null;
        massagePanel.SetActive(true);
        massagePanel_2.SetActive(true);

        MassageManage.Instance.info("Step Three", @"Check Pulse and Breathing...");
        MessageManager_2.Instance.info("Place index and middle fingers...");

        StepNumber = 2;

        // 重置状态
        DataLogic.Instance.IsCheckPulse = false;
        DataLogic.Instance.IsBreathe = false;
        yield return new WaitUntil(() => DataLogic.Instance.IsBreathe && DataLogic.Instance.IsCheckPulse);

        massagePanel.SetActive(false);
        massagePanel_2.SetActive(false);
        SpeakPanel.SetActive(true);
        yield return Speak.Instance.info("Rescuer", "The patient currently has no pulse.", 4);
        yield return character_Ani.Play(3.7f, 5.11f);
        yield return new WaitForSeconds(2f);

        SpeakPanel.SetActive(false);
        ChangeStepIcon();
    }

    public IEnumerator CryForHelp()
    {
        yield return null;
        massagePanel.SetActive(true);
        massagePanel_2.SetActive(true);

        MassageManage.Instance.info("Step Four", @"Activate Emergency Response...");
        MessageManager_2.Instance.info("Press the button...");

        StepNumber = 3;

        // 重置状态
        DataLogic.Instance.IsCall = false;
        yield return new WaitUntil(() => DataLogic.Instance.IsCall);

        massagePanel.SetActive(false);
        massagePanel_2.SetActive(false);
        SpeakPanel.SetActive(true);
        yield return Speak.Instance.info("Rescuer", "Someone here is unconscious.Please call 120 and look for an AED in a public area.", 2);
        yield return character_Ani.Play(5.11f, 6.38f);
        yield return Speak.Instance.info("Bystander", "Understood, I've called 120.", 3);

        SpeakPanel.SetActive(false);
        ChangeStepIcon();
    }

    public IEnumerator PressTrain()
    {
        yield return null;
        massagePanel.SetActive(true);
        pressingPanel.SetActive(true);
        pressingPanel2.SetActive(true);

        PressingPanel.Instance.info("Compression Count：0", "Rate：0", "Compression timer starts automatically after pressing the manikin. You need to perform 30 compressions.");
        MassageManage.Instance.info("Step Five", @"High-Quality Chest Compressions...");

        yield return character_Ani.Play(6.38f, 7f);
        StepNumber = 4;

        // 重置状态
        DataLogic.Instance.IsPeopleBreathing = false;
        yield return new WaitUntil(() => DataLogic.Instance.IsPeopleBreathing);

        ChangeStepIcon();
    }

    public IEnumerator CleanMouth()
    {
        yield return null;
        massagePanel.SetActive(true);
        massagePanel_2.SetActive(true);
        pressingPanel.SetActive(false);
        pressingPanel2.SetActive(false);

        MassageManage.Instance.info("Step Six", @"Clear Airway...");
        MessageManager_2.Instance.info("Place hands on both sides...");

        StepNumber = 5;

        // 重置状态
        DataLogic.Instance.IsRemoveForeign = false;
        yield return new WaitUntil(() => DataLogic.Instance.IsRemoveForeign);

        massagePanel.SetActive(false);
        massagePanel_2.SetActive(false);
        yield return character_Ani.Play(10.11f, 16.33f, 1);

        SpeakPanel.SetActive(true);
        yield return Speak.Instance.info("Rescuer", "Airway cleared and opened.", 6);
        yield return new WaitForSeconds(3.5f);

        SpeakPanel.SetActive(false);
        ChangeStepIcon();
    }

    public IEnumerator ChuiQi()
    {
        yield return null;
        SpeakPanel.SetActive(true);
        yield return Speak.Instance.info("Rescuer", "Compressions completed. Ready for rescue breaths.", 7);
        yield return new WaitForSeconds(5f);

        SpeakPanel.SetActive(false);
        ChuiqiPanel.SetActive(true);
        massagePanel.SetActive(true);
        pressingPanel.SetActive(true);

        PressingPanel.Instance.info("Breath Count：0", "Tidal Volume：0", "Begin rescue breaths.");
        MassageManage.Instance.info("Step Eight", @"8. Mouth-to-Mouth Ventilation...");

        StepNumber = 6;

        // 重置状态
        DataLogic.Instance.IsPeopleChuiQi = false;
        DataLogic.Instance.IsEND = false;
        yield return new WaitUntil(() => DataLogic.Instance.IsPeopleChuiQi);

        ChuiqiPanel.SetActive(false);
        ChangeStepIcon();
    }

    public IEnumerator AnYa_Pre()
    {
        yield return null;
        isPlayAnim = false;
        character_Ani.gameObject.SetActive(true);
        isend_Exam = true;

        // 重置所有考核状态
        DataLogic.Instance.IsConsciousness_int = 0;
        DataLogic.Instance.IsRemoveForeign_int = 0;
        DataLogic.Instance.IsCheckPulse_int = 0;
        DataLogic.Instance.IsCall_int = 0;
        DataLogic.Instance.IsBreathe_int = 0;

        // 重置按压相关状态
        DataLogic.Instance.Number = 0;
        DataLogic.Instance.Number_Count = 0;
        DataLogic.Instance.BreathingNumber = 0;
        DataLogic.Instance.BreathingNumber_Count = 0;
        DataLogic.Instance.DetailsData = new List<int>();
        DataLogic.Instance.DetailsDataYY = new List<int>();
        DataLogic.Instance.isAnYaIng = false;
        DataLogic.Instance.isChuiQiIng = false;
        DataLogic.Instance.IsPeopleBreathing = false;
        DataLogic.Instance.IsPeopleChuiQi = false;

        while (isend_Exam)
        {
            if (!isPlayAnim)
            {
                if (DataLogic.Instance.IsConsciousness_int == 1)
                {
                    isPlayAnim = true;
                    yield return character_Ani.Play(0f, 1.75f, 1);
                    SpeakPanel.SetActive(true);
                    yield return Speak.Instance.info("Rescuer", "Comrade, are you okay?", 0);
                    yield return character_Ani.Play(1.75f, 3.7f, 1);
                    yield return Speak.Instance.info("Patient", "……", 1);
                    yield return new WaitForSeconds(2f);
                    SpeakPanel.SetActive(false);
                    DataLogic.Instance.IsConsciousness_int = 2;
                    isPlayAnim = false;
                }
                if (DataLogic.Instance.IsRemoveForeign_int == 1)
                {
                    isPlayAnim = true;
                    yield return character_Ani.Play(0f, 1.75f, 1);
                    yield return character_Ani.Play(10.11f, 16.33f, 1);
                    SpeakPanel.SetActive(true);
                    yield return Speak.Instance.info("Rescuer", "Airway cleared and opened.", 6);
                    yield return new WaitForSeconds(3.5f);
                    SpeakPanel.SetActive(false);
                    DataLogic.Instance.IsRemoveForeign_int = 2;
                    isPlayAnim = false;
                }
                if (DataLogic.Instance.IsCheckPulse_int == 1)
                {
                    isPlayAnim = true;
                    SpeakPanel.SetActive(true);
                    yield return Speak.Instance.info("Rescuer", "The patient currently has no pulse.", 4);
                    yield return character_Ani.Play(3.7f, 5.11f);
                    yield return new WaitForSeconds(2f);
                    SpeakPanel.SetActive(false);
                    DataLogic.Instance.IsCheckPulse_int = 2;
                    isPlayAnim = false;
                }
                if (DataLogic.Instance.IsCall_int == 1)
                {
                    isPlayAnim = true;
                    SpeakPanel.SetActive(true);
                    yield return Speak.Instance.info("Rescuer", "Someone here is unconscious.Please call 120 and look for an AED in a public area.", 2);
                    yield return character_Ani.Play(5.11f, 6.38f);
                    yield return new WaitForSeconds(5f);
                    yield return Speak.Instance.info("Bystander", "Understood, I've called 120.", 3);
                    yield return new WaitForSeconds(4f);
                    SpeakPanel.SetActive(false);
                    DataLogic.Instance.IsCall_int = 2;
                    isPlayAnim = false;
                }

                // 检查是否所有前四个步骤都完成
                if (DataLogic.Instance.IsRemoveForeign_int == 2 &&
                    DataLogic.Instance.IsConsciousness_int == 2 &&
                    DataLogic.Instance.IsCheckPulse_int == 2 &&
                    DataLogic.Instance.IsCall_int == 2)
                {
                    isend_Exam = false;
                }
            }
            yield return null;
        }
    }
    public IEnumerator AnYaTrain()
    {
        yield return null;

        CloseAllUI();

        // 在考核模式下，确保StepNumber正确设置为4（按压步骤）
        if (currentSceneName != "StudyMode")
        {
            StepNumber = 4;
            Debug.Log("考核模式：AnYaTrain中设置StepNumber为4");
        }

        // 重置按压相关状态
        DataLogic.Instance.isAnYaIng = false;
        DataLogic.Instance.IsPeopleBreathing = false;
        DataLogic.Instance.Number = 0;
        DataLogic.Instance.Number_Count = 0;
        DataLogic.Instance.DetailsData = new List<int>();
        DataLogic.Instance.time_xx = 0;

        // 重置键盘输入状态
        DataLogic.Instance.isCompressing = false;
        DataLogic.Instance.simulatingCompression = false;

        // 显示按压UI
        pressingPanel.SetActive(true);
        pressingPanel2.SetActive(true);

        if (PressingPanel.Instance != null)
        {
            PressingPanel.Instance.RePressin();
            PressingPanel.Instance.reChuiQIliangPanel();
            PressingPanel.Instance.info("Compression Count：0", "Rate：0", "Press SPACE key to start compressions. Need 30 compressions.");
        }

        isPlayAnim = true;
        yield return character_Ani.Play(6.38f, 7f);
        isPlayAnim = false;

        Debug.Log("等待按压完成（30次按压）...当前状态：isAnYaIng=" + DataLogic.Instance.isAnYaIng + ", IsPeopleBreathing=" + DataLogic.Instance.IsPeopleBreathing);

        // 等待按压完成（30次按压）
        yield return new WaitUntil(() => DataLogic.Instance.IsPeopleBreathing);

        isPlayAnim = false;
        Debug.Log("按压训练完成，按压次数：" + DataLogic.Instance.Number_Count);

        pressingPanel.SetActive(false);
        pressingPanel2.SetActive(false);
    }

    public IEnumerator ChuiQiTrain()
    {
        yield return null;

        Debug.Log("开始吹气训练 - 考核模式");

        CloseAllUI();

        // 在考核模式下，设置StepNumber为6（吹气步骤）
        if (currentSceneName != "StudyMode")
        {
            StepNumber = 6;
            Debug.Log("考核模式：ChuiQiTrain中设置StepNumber为6");
        }

        // 重置吹气相关状态 - 关键修复！
        DataLogic.Instance.isChuiQiIng = false;
        DataLogic.Instance.IsPeopleChuiQi = false;
        DataLogic.Instance.BreathingNumber = 0;
        DataLogic.Instance.BreathingNumber_Count = 0;
        DataLogic.Instance.DetailsDataYY = new List<int>();

        // 重置键盘输入状态
        DataLogic.Instance.isBreathing = false;
        DataLogic.Instance.simulatingBreath = false;

        // 显示吹气UI
        ChuiqiPanel.SetActive(true);
        pressingPanel2.SetActive(false);
        pressingPanel.SetActive(true);

        if (PressingPanel.Instance != null)
        {
            PressingPanel.Instance.RePressin();
            PressingPanel.Instance.reChuiQIliangPanel();
            PressingPanel.Instance.info("Breath Count：0", "Tidal Volume：0", "Press SPACE key for rescue breaths. Need 2 breaths.");
        }

        isPlayAnim = true;
        yield return character_Ani.Play(19.33f, 20.7f);
        isPlayAnim = false;

        Debug.Log("等待吹气完成（2次吹气）...当前状态：isChuiQiIng=" + DataLogic.Instance.isChuiQiIng + ", IsPeopleChuiQi=" + DataLogic.Instance.IsPeopleChuiQi);

        // 等待吹气完成（2次吹气）
        yield return new WaitUntil(() => DataLogic.Instance.IsPeopleChuiQi);

        // 吹气完成后再关闭面板
        ChuiqiPanel.SetActive(false);
        pressingPanel.SetActive(false);
        isPlayAnim = false;
        Debug.Log("吹气训练完成，吹气次数：" + DataLogic.Instance.BreathingNumber_Count);
    }

    public void CloseAllUI()
    {
        InfoPanel.SetActive(false);
        oneStepPanel.SetActive(false);
        ChangePlace.SetActive(false);
        massagePanel.SetActive(false);
        SpeakPanel.SetActive(false);
        pressingPanel.SetActive(false);
        pressingPanel2.SetActive(false);
        ChuiqiPanel.SetActive(false);
        schedulePanel.SetActive(false);
        massagePanel_2.SetActive(false);
    }

    IEnumerator endStudy1()
    {
        // 等待完成标志
        yield return new WaitUntil(() => DataLogic.Instance.IsEND);
        yield return new WaitForSeconds(2f);

        ChangeStepIcon();
        StepNumber = 8;
        ChuiqiPanel.SetActive(false);
        pressingPanel.SetActive(false);
        massagePanel.SetActive(false);

        SpeakPanel.SetActive(true);
        yield return Speak.Instance.info("Rescuer", "Patient successfully revived after 4 cycles.", 8);
        yield return new WaitForSeconds(4f);

        endPanel2.SetActive(true);
        waitButtonOn = false;
        UnityAction action = () => waitButtonOn = true;
        continueButton.onClick.AddListener(action);
        yield return new WaitUntil(() => waitButtonOn);
        continueButton.onClick.RemoveListener(action);
    }

    IEnumerator endStudy2()
    {
        // 等待完成标志
        yield return new WaitUntil(() => DataLogic.Instance.IsEND);
        yield return new WaitForSeconds(2f);

        ChangeStepIcon();
        StepNumber = 8;
        ChuiqiPanel.SetActive(false);
        pressingPanel.SetActive(false);
        massagePanel.SetActive(false);

        SpeakPanel.SetActive(true);
        yield return Speak.Instance.info("Rescuer", "Patient successfully revived after 5 cycles.", 8);
        yield return new WaitForSeconds(4f);

        endPanel.SetActive(true);
        yield return null;
    }

    IEnumerator endStudy3()
    {
        yield return new WaitForSeconds(2f);

        StepNumber = 8;
        ChuiqiPanel.SetActive(false);
        pressingPanel.SetActive(false);
        massagePanel.SetActive(false);

        SpeakPanel.SetActive(true);
        yield return Speak.Instance.info("Rescuer", "Patient successfully revived after 5 cycles.", 8);
        yield return new WaitForSeconds(4f);

        endPanel.SetActive(true);
        yield return null;
    }

    //重复练习
    IEnumerator repeattrain()
    {
        Debug.Log("开始重复训练 - 按压");

        // 重置所有状态
        DataLogic.Instance.IsPeopleBreathing = false;
        DataLogic.Instance.Number = 0;
        DataLogic.Instance.isAnYaIng = false;

        massagePanel.SetActive(false);
        ChuiqiPanel.SetActive(false);
        yield return new WaitForSeconds(3.5f);

        SpeakPanel.SetActive(false);
        massagePanel.SetActive(true);
        pressingPanel.SetActive(true);
        pressingPanel2.SetActive(true);

        PressingPanel.Instance.RePressin(); // 重置按压面板
        PressingPanel.Instance.info("Compression Count：0", "Rate：0", "Compression timer starts automatically after pressing the manikin. You need to perform 30 compressions.");
        MassageManage.Instance.info("Step Seven", @"5. High-Quality Chest Compressions...");

        StepNumber = 4;

        yield return character_Ani.Play(6.38f, 7f);

        // 等待按压完成
        yield return new WaitUntil(() => DataLogic.Instance.IsPeopleBreathing);

        Debug.Log("重复训练 - 按压完成");
    }

    IEnumerator repeatBreathing()
    {
        Debug.Log("开始重复训练 - 吹气");

        // 重置所有状态
        DataLogic.Instance.IsPeopleChuiQi = false;
        DataLogic.Instance.BreathingNumber = 0;
        DataLogic.Instance.isChuiQiIng = false;

        pressingPanel.SetActive(false);
        pressingPanel2.SetActive(false);
        massagePanel.SetActive(false);

        SpeakPanel.SetActive(true);
        yield return Speak.Instance.info("Rescuer", "Compressions completed. Ready for rescue breaths.", 7);
        yield return new WaitForSeconds(5f);

        SpeakPanel.SetActive(false);
        massagePanel.SetActive(true);
        pressingPanel.SetActive(true);
        ChuiqiPanel.SetActive(true);

        PressingPanel.Instance.reChuiQIliangPanel(); // 重置吹气面板
        PressingPanel.Instance.info("Breath Count：0", "Tidal Volume：0", "Begin rescue breaths.");
        MassageManage.Instance.info("Step Eight", @"8. Mouth-to-Mouth Ventilation...");

        StepNumber = 6;

        yield return character_Ani.Play(19.33f, 20.7f);

        // 等待吹气完成
        yield return new WaitUntil(() => DataLogic.Instance.IsPeopleChuiQi);

        ChuiqiPanel.SetActive(false);
        Debug.Log("重复训练 - 吹气完成");
    }

    public void ChangeStepIcon()
    {
        if (StepNumber < schedulePanel.transform.childCount)
        {
            Transform stepIcon = schedulePanel.transform.GetChild(StepNumber);
            stepIcon.GetComponent<Image>().sprite = OverSprite;
            Color fade = new Color(137, 105, 5, 0);
            stepIcon.GetChild(0).GetComponent<Text>().color = fade;
            if (StepNumber != 6)
            {
                stepIcon.GetChild(1).GetComponent<Image>().color = Color.white;
            }
        }
    }
}