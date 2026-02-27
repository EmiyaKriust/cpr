using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/****************************************************************
 *Author:       严将/张皓名
 *Version:      1.0 
 *UnityVersion：2019.4.5 
 *Date:         CreateTime
 *Description:    
 *History: 
*****************************************************************/
public class DataLogic : MonoBehaviour
{
    public bool IsRemoveForeign;//判断是否清楚异物
    public bool IsCheckPulse;//判断是否检查脉搏
    public bool IsCall;//判断是否急救呼叫
    public bool IsConsciousness;//判断是否有意识
    public bool IsBreathe;//判断是否有呼吸
    public bool IsPeopleBreathing;//判断是否进行人工呼吸
    public bool IsPeopleChuiQi;
    public bool IsYangTou;//判断是否仰头
    public bool IsEND;//判断是否完成

    [Header("考核")]
    public int IsRemoveForeign_int;//判断是否清楚异物
    public int IsCheckPulse_int;//判断是否检查脉搏
    public int IsCall_int;//判断是否急救呼叫
    public int IsConsciousness_int;//判断是否有意识
    public int IsBreathe_int;//判断是否有呼吸
    public int IsAnYa;//判断是否按压
    public int IsPeopleBreathing_int;//判断是否进行人工呼吸
    public int IsYangTou_int;//判断是否仰头
    public int IsEND_int;//判断是否完成

    public int Lunci;//判断按了几轮
    public int Lunci_anya;//判断按了几轮
    public int Lunci_chuiqi;//判断按了几轮
    public bool isAnYaIng;//是否正在按压中
    public bool isChuiQiIng;//是否正在按吹起中
    public int repeatNumber = 0;

    public List<int> DetailsData; //每次的数据;
    private List<int> DetailsDataXX_Pinlv; //每次频率的的数据;
    public List<int> DetailsDataYY; //每次的数据;
    private int intYY;
    private Dictionary<int, List<int>> dictData; //所有的按压数据
    public int Number; //按压次数
    public int Number_Count; //真实的按压次数
    public int BreathingNumber; //吹气次数
    public int BreathingNumber_Count; //真实的吹气次数
    public State state;//判断每次的按压状态
    int DetailsDataCount;//判断是否按压不足
    public int DetailsDataMax;//判断最大的按压数据
    int DetailsDataYYMax;//判断最大的吹气数据
    string helpText;

    bool reSetBool = true;//判断按压是否重置，到下次xx有数值为止是一次
    string sceneName;

    static DataLogic m_instance;

    public int YY_zongLiang;//吹气的总量

    public float time_xx;//每一次按压之间的间隔为0.5s-0.6s
    public bool time_bool;//计算时间的

    [Header("键盘输入设置")]
    public bool enableKeyboardInput = true;

    [Header("按压模拟参数")]
    public int minCompressionValue = 140;
    public int maxCompressionValue = 160;
    public int minBreathValue = 100;
    public int maxBreathValue = 150;
    public float compressionDuration = 0.5f;
    public float breathDuration = 1.0f;

    // 模拟状态
    public bool simulatingCompression = false;
    public bool simulatingBreath = false;
    private float compressionTimer = 0f;
    private float breathTimer = 0f;

    // 按键状态跟踪
    private bool isCheckingPulse = false;
    private bool isCheckingBreath = false;
    public bool isCompressing = false;
    public bool isBreathing = false;

    public static DataLogic Instance
    {
        get
        {
            if (null == m_instance)
                m_instance = new GameObject("DataLogic").AddComponent<DataLogic>();
            return m_instance;
        }
        set
        {
            m_instance = value;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        Number = 0;
        DetailsDataMax = 0;
        ResetAllBools();
    }

    // Update is called once per frame
    void Update()
    {
        time_xx += Time.deltaTime;

        // 检测键盘输入
        if (enableKeyboardInput)
        {
            CheckKeyboardInput();
        }

        // 模拟按压过程
        if (simulatingCompression)
        {
            UpdateCompressionSimulation();
        }

        // 模拟吹气过程
        if (simulatingBreath)
        {
            UpdateBreathSimulation();
        }
    }

    public enum State
    {
        正确,
        按压过大,
        按压不足,
        按压过快,
        按压过慢,
        回弹不足,
    }

    private void Init()
    {
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
        DetailsData = new List<int>();
        dictData = new Dictionary<int, List<int>>();
        DetailsDataYY = new List<int>();
    }

    private void CheckKeyboardInput()
    {
        if (!enableKeyboardInput) return;

        // 检查意识（拍肩）- Q键
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("按下Q键 - 检查意识");
            OnCheckConsciousness();
        }

        // 检查脉搏和呼吸 - W键
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("按下W键 - 检查脉搏和呼吸");
            if (sceneName != "StudyMode") // 考核模式
            {
                if (StudyMode.Instance != null && StudyMode.Instance.isPlayAnim == false)
                {
                    if (IsCheckPulse_int == 0)
                    {
                        IsCheckPulse_int = 1;
                        Debug.Log("考核模式：设置IsCheckPulse_int=1");
                    }
                    if (IsBreathe_int == 0)
                    {
                        IsBreathe_int = 1;
                        Debug.Log("考核模式：设置IsBreathe_int=1");
                    }
                }
            }
            else // 学习模式
            {
                if (StudyMode.Instance != null)
                {
                    if (StudyMode.Instance.StepNumber == 2)
                    {
                        if (!isCheckingPulse)
                        {
                            OnCheckPulse();
                            isCheckingPulse = true;
                        }
                        if (!isCheckingBreath)
                        {
                            OnCheckBreath();
                            isCheckingBreath = true;
                        }
                    }
                }
            }
        }

        // 紧急呼叫 - E键
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("按下E键 - 紧急呼叫");
            OnEmergencyCall();
        }

        // 清理气道 - R键
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("按下R键 - 清理气道");
            OnClearAirway();
        }

        // 仰头抬颏 - T键（按下/释放）
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("按下T键 - 开始仰头抬颏");
            OnTiltHead(true);
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            Debug.Log("释放T键 - 结束仰头抬颏");
            OnTiltHead(false);
        }

        // 开始CPR按压 - Space键
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("按下空格键 - 当前场景：" + sceneName + ", StepNumber=" + (StudyMode.Instance != null ? StudyMode.Instance.StepNumber.ToString() : "null"));

            if (sceneName != "StudyMode") // 考核模式
            {
                Debug.Log("考核模式：空格键按下");
                Debug.Log("当前状态：StepNumber=" + (StudyMode.Instance != null ? StudyMode.Instance.StepNumber.ToString() : "null") +
                         ", isPlayAnim=" + (StudyMode.Instance != null ? StudyMode.Instance.isPlayAnim.ToString() : "null") +
                         ", isCompressing=" + isCompressing +
                         ", isBreathing=" + isBreathing);

                if (StudyMode.Instance != null && StudyMode.Instance.isPlayAnim == false)
                {
                    // 根据当前StepNumber判断是按压还是吹气
                    if (StudyMode.Instance.StepNumber == 4)
                    {
                        // 步骤4：按压
                        if (!isCompressing)
                        {
                            Debug.Log("考核模式：开始按压");
                            StartCompression();
                            isCompressing = true;
                        }
                    }
                    else if (StudyMode.Instance.StepNumber == 6)
                    {
                        // 步骤6：吹气
                        if (!isBreathing)
                        {
                            Debug.Log("考核模式：开始吹气");
                            StartBreath();
                            isBreathing = true;
                        }
                    }
                    else
                    {
                        Debug.Log("考核模式：当前不是按压或吹气步骤，StepNumber=" + StudyMode.Instance.StepNumber);
                    }
                }
            }
            else // 学习模式
            {
                if (StudyMode.Instance != null)
                {
                    if (StudyMode.Instance.StepNumber == 4)
                    {
                        if (!isCompressing)
                        {
                            StartCompression();
                            isCompressing = true;
                        }
                    }
                    else if (StudyMode.Instance.StepNumber == 6)
                    {
                        if (!isBreathing)
                        {
                            StartBreath();
                            isBreathing = true;
                        }
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("释放空格键");
            if (isCompressing && simulatingCompression)
            {
                StopCompression();
                isCompressing = false;
            }
            else if (isBreathing && simulatingBreath)
            {
                StopBreath();
                isBreathing = false;
            }
        }

        // 重置训练 - F5键
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("按下F5键 - 重置训练");
            ResetTraining();
        }
    }

    // 重置所有bool状态
    private void ResetAllBools()
    {
        isCheckingPulse = false;
        isCheckingBreath = false;
        isCompressing = false;
        isBreathing = false;
    }

    // 模拟蓝牙消息处理的方法
    private void OnCheckConsciousness()
    {
        Debug.Log("键盘输入：检查意识（拍肩） - Q键");

        // 只有在StudyMode的学习模式下才设置IsConsciousness
        if (sceneName == "StudyMode")
        {
            if (StudyMode.Instance != null && StudyMode.Instance.StepNumber == 1)
            {
                IsConsciousness = true;
                Debug.Log("IsConsciousness设置为true");
            }
        }
        else
        {
            // 考核模式下设置IsConsciousness_int
            if (StudyMode.Instance != null && StudyMode.Instance.isPlayAnim == false)
            {
                if (IsConsciousness_int == 0)
                {
                    IsConsciousness_int = 1;
                    Debug.Log("IsConsciousness_int设置为1");
                }
            }
        }
    }

    private void OnCheckPulse()
    {
        Debug.Log("键盘输入：检查脉搏 - W键");

        // 只有在StudyMode的学习模式下才设置IsCheckPulse
        if (sceneName == "StudyMode")
        {
            if (StudyMode.Instance != null && StudyMode.Instance.StepNumber == 2)
            {
                IsCheckPulse = true;
                Debug.Log("IsCheckPulse设置为true");
            }
        }
        else
        {
            // 考核模式下设置IsCheckPulse_int
            if (StudyMode.Instance != null && StudyMode.Instance.isPlayAnim == false)
            {
                if (IsCheckPulse_int == 0)
                {
                    IsCheckPulse_int = 1;
                    Debug.Log("IsCheckPulse_int设置为1");
                }
            }
        }
    }

    private void OnCheckBreath()
    {
        Debug.Log("键盘输入：检查呼吸 - W键");

        // 只有在StudyMode的学习模式下才设置IsBreathe
        if (sceneName == "StudyMode")
        {
            if (StudyMode.Instance != null && StudyMode.Instance.StepNumber == 2)
            {
                IsBreathe = true;
                Debug.Log("IsBreathe设置为true");
            }
        }
        else
        {
            // 考核模式下设置IsBreathe_int
            if (StudyMode.Instance != null && StudyMode.Instance.isPlayAnim == false)
            {
                if (IsBreathe_int == 0)
                {
                    IsBreathe_int = 1;
                    Debug.Log("IsBreathe_int设置为1");
                }
            }
        }
    }

    private void OnEmergencyCall()
    {
        Debug.Log("键盘输入：紧急呼叫 - E键");

        // 只有在StudyMode的学习模式下才设置IsCall
        if (sceneName == "StudyMode")
        {
            if (StudyMode.Instance != null && StudyMode.Instance.StepNumber == 3)
            {
                IsCall = true;
                Debug.Log("IsCall设置为true");
            }
        }
        else
        {
            // 考核模式下设置IsCall_int
            if (StudyMode.Instance != null && StudyMode.Instance.isPlayAnim == false)
            {
                if (IsCall_int == 0)
                {
                    IsCall_int = 1;
                    Debug.Log("IsCall_int设置为1");
                }
            }
        }
    }

    private void OnClearAirway()
    {
        Debug.Log("键盘输入：清理气道 - R键");

        // 只有在StudyMode的学习模式下才设置IsRemoveForeign
        if (sceneName == "StudyMode")
        {
            if (StudyMode.Instance != null && StudyMode.Instance.StepNumber == 5)
            {
                IsRemoveForeign = true;
                Debug.Log("IsRemoveForeign设置为true");
            }
        }
        else
        {
            // 考核模式下设置IsRemoveForeign_int
            if (StudyMode.Instance != null && StudyMode.Instance.isPlayAnim == false)
            {
                if (IsRemoveForeign_int == 0)
                {
                    IsRemoveForeign_int = 1;
                    Debug.Log("IsRemoveForeign_int设置为1");
                }
            }
        }
    }

    private void OnTiltHead(bool state)
    {
        Debug.Log("键盘输入：仰头抬颏 - " + (state ? "开始" : "结束"));
        IsYangTou = state;
        if (PressingPanel.Instance != null)
        {
            PressingPanel.Instance.chuiqiImagechoice(state);
        }
    }

    private void StartCompression()
    {
        Debug.Log("StartCompression() 被调用 - 场景：" + sceneName);

        if (sceneName != "StudyMode") // 考核模式
        {
            Debug.Log("考核模式：开始按压模拟");
            simulatingCompression = true;
            compressionTimer = 0f;
            isAnYaIng = true;
            isCompressing = true;

            // 生成模拟按压数据
            int simulatedValue = UnityEngine.Random.Range(minCompressionValue, maxCompressionValue);
            if (DetailsData == null)
            {
                DetailsData = new List<int>();
            }
            DetailsData.Add(simulatedValue);

            Debug.Log("考核模式：模拟按压值=" + simulatedValue + ", DetailsData.Count=" + DetailsData.Count);
        }
        else if (sceneName == "StudyMode" && StudyMode.Instance != null && StudyMode.Instance.StepNumber == 4)
        {
            Debug.Log("学习模式：开始按压");
            simulatingCompression = true;
            compressionTimer = 0f;
            isAnYaIng = true;

            // 生成模拟按压数据
            int simulatedValue = UnityEngine.Random.Range(minCompressionValue, maxCompressionValue);
            DetailsData.Add(simulatedValue);
        }
    }

    private void StopCompression()
    {
        if (simulatingCompression)
        {
            Debug.Log("键盘输入：停止按压 - Space键");
            simulatingCompression = false;
            isAnYaIng = false;

            // 处理按压结束
            HandleCompressionEnd();
        }
    }

    private void StartBreath()
    {
        Debug.Log("StartBreath() 被调用 - 场景：" + sceneName + ", StepNumber=" + (StudyMode.Instance != null ? StudyMode.Instance.StepNumber.ToString() : "null"));

        if (sceneName != "StudyMode") // 考核模式
        {
            Debug.Log("考核模式：开始吹气模拟");
            simulatingBreath = true;
            breathTimer = 0f;
            isChuiQiIng = true;
            isBreathing = true;

            // 生成模拟吹气数据
            int simulatedValue = UnityEngine.Random.Range(minBreathValue, maxBreathValue);
            if (DetailsDataYY == null)
            {
                DetailsDataYY = new List<int>();
            }
            DetailsDataYY.Add(simulatedValue);

            Debug.Log("考核模式：模拟吹气值=" + simulatedValue + ", DetailsDataYY.Count=" + DetailsDataYY.Count);
        }
        else if (sceneName == "StudyMode" && StudyMode.Instance != null && StudyMode.Instance.StepNumber == 6)
        {
            Debug.Log("学习模式：开始吹气");
            simulatingBreath = true;
            breathTimer = 0f;
            isChuiQiIng = true;

            // 生成模拟吹气数据
            int simulatedValue = UnityEngine.Random.Range(minBreathValue, maxBreathValue);
            DetailsDataYY.Add(simulatedValue);
        }
    }

    private void StopBreath()
    {
        if (simulatingBreath)
        {
            Debug.Log("键盘输入：停止吹气 - Space键");
            simulatingBreath = false;
            isChuiQiIng = false;

            // 处理吹气结束
            HandleBreathEnd();
        }
    }

    private void UpdateCompressionSimulation()
    {
        compressionTimer += Time.deltaTime;

        // 模拟按压过程中的数据
        if (compressionTimer <= compressionDuration)
        {
            // 可以在这里实时更新按压数据
        }
    }

    private void UpdateBreathSimulation()
    {
        breathTimer += Time.deltaTime;

        if (breathTimer <= breathDuration)
        {
            // 可以在这里实时更新吹气数据
        }
    }

    private void HandleCompressionEnd()
    {
        if (sceneName == "StudyMode")
        {
            if (StudyMode.Instance != null && StudyMode.Instance.StepNumber == 4)
            {
                if (DetailsData.Count > 0)
                {
                    // 停止计时器
                    if (Number != 0)
                    {
                        if (time_xx < 0.4f) // 按压太快
                        {
                            Debug.LogError("按压太快");
                            if (StudyMode.Instance != null)
                                StudyMode.Instance.tip(0);
                        }
                        else if (time_xx > 0.7f) // 按压太慢
                        {
                            Debug.LogError("按压太慢");
                            if (StudyMode.Instance != null)
                                StudyMode.Instance.tip(1);
                        }
                        else
                        {
                            Debug.LogError("按压正常");
                        }
                    }

                    // 分析按压数据
                    for (int i = 0; i < DetailsData.Count; i++)
                    {
                        if (DetailsData[i] > DetailsDataMax)
                        {
                            DetailsDataMax = DetailsData[i];
                        }
                        if (DetailsData[i] > 148)
                        {
                            // 压力过大
                        }
                        if (DetailsData[i] < 140)
                        {
                            DetailsDataCount++;
                        }
                    }

                    if (DetailsDataCount == DetailsData.Count)
                    {
                        // 压力过小
                    }

                    // 播放按压动画
                    if (StudyMode.Instance != null && StudyMode.Instance.character_Ani != null)
                    {
                        StartCoroutine(StudyMode.Instance.character_Ani.Play(7f, 7.36f, 2));
                    }

                    // 更新按压面板
                    if (PressingPanel.Instance != null)
                    {
                        PressingPanel.Instance.Pressin(Number, UnityEngine.Random.Range(4, 6));
                    }

                    Number++;
                    Debug.LogError("学习模式按压次数: " + Number);

                    // 重置计数
                    if (Number == 31 || Number == 61)
                    {
                        DetailsData = new List<int>();
                    }

                    // 更新信息
                    if (PressingPanel.Instance != null)
                    {
                        PressingPanel.Instance.info("Count：" + Number,
                            "Rate：" + (60 / time_xx).ToString("F1") + "/Min", "");
                    }

                    // 重置计时和数据
                    time_xx = 0;
                    DetailsDataMax = 0;
                    DetailsDataCount = 0;
                    DetailsData = new List<int>();

                    // 检查是否完成30次按压
                    if (Number >= 29) // 修改：从 == 30 改为 >= 29
                    {
                        IsPeopleBreathing = true;
                        Number = 0;
                        Debug.Log("学习模式：完成30次按压，设置IsPeopleBreathing=true");
                    }
                }
            }
        }
        else // 其他场景（考核模式）
        {
            if (DetailsData.Count > 0)
            {
                // 处理按压结束逻辑
                if (Number != 0)
                {
                    if (time_xx < 0.4f)
                    {
                        if (StudyMode.Instance != null)
                            StudyMode.Instance.tip(0);
                    }
                    else if (time_xx > 0.7f)
                    {
                        if (StudyMode.Instance != null)
                            StudyMode.Instance.tip(1);
                    }
                }

                // 分析数据
                for (int i = 0; i < DetailsData.Count; i++)
                {
                    if (DetailsData[i] > DetailsDataMax)
                    {
                        DetailsDataMax = DetailsData[i];
                    }
                    if (DetailsData[i] > 148)
                    {
                        // 压力过大
                    }
                    if (DetailsData[i] < 140)
                    {
                        DetailsDataCount++;
                    }
                }

                if (DetailsDataCount == DetailsData.Count)
                {
                    // 压力过小
                }

                // 更新按压面板
                if (PressingPanel.Instance != null)
                {
                    if (Number_Count == 0)
                    {
                        PressingPanel.Instance.RePressin();
                    }

                    BreathingNumber_Count = 0;
                    BreathingNumber = 0;

                    PressingPanel.Instance.Pressin(Number, UnityEngine.Random.Range(4, 6));
                }

                Number++;
                Number_Count++;

                // 更新按压轮次
                if (Lunci_anya < 4)
                {
                    if (Number_Count == 1)
                    {
                        Lunci_anya++;
                    }
                }

                Debug.LogError("考核模式按压次数: " + Number + ", Number_Count: " + Number_Count);

                // 检查是否完成30次按压 - 正确修复！
                // 因为Number从0开始，所以完成30次按压时Number应该是30
                // 但这里有个问题：每次按压后Number++，所以当第30次按压完成后，Number=30
                if (Number >= 29) // 修改：从 >= 30 改为 >= 29
                {
                    Number = 0;
                    IsPeopleBreathing = true;
                    Debug.Log("考核模式：完成30次按压，设置IsPeopleBreathing=true");
                    if (PressingPanel.Instance != null)
                    {
                        PressingPanel.Instance.RePressin();
                    }
                }

                // 播放动画
                if (StudyMode.Instance != null && StudyMode.Instance.character_Ani != null)
                {
                    StartCoroutine(StudyMode.Instance.character_Ani.Play(7f, 7.36f, 2));
                }

                // 更新信息 - 使用Number_Count来显示，因为它是真实按压次数
                if (PressingPanel.Instance != null)
                {
                    PressingPanel.Instance.info("Compression Cycle " + Lunci_anya +
                        " Compression Count：" + Number_Count,
                        "Compression Rate：" + (60 / time_xx).ToString("F1") + "/Min", "");
                }

                // 重置
                time_xx = 0;
                DetailsDataMax = 0;
                DetailsDataCount = 0;
                DetailsData = new List<int>();
            }
        }
    }
    private void HandleBreathEnd()
    {
        if (sceneName == "StudyMode")
        {
            if (StudyMode.Instance != null && StudyMode.Instance.StepNumber == 6)
            {
                if (DetailsDataYY.Count > 0)
                {
                    // 分析吹气数据
                    for (int i = 0; i < DetailsDataYY.Count; i++)
                    {
                        if (DetailsDataYY[i] > DetailsDataYYMax)
                        {
                            DetailsDataYYMax = DetailsDataYY[i];
                        }
                        if (DetailsDataYY[i] < 100)
                        {
                            intYY++;
                        }
                        YY_zongLiang += DetailsDataYY[i];
                    }

                    if (intYY == DetailsDataYY.Count)
                    {
                        helpText = "Insufficient Breath.";
                        if (PressingPanel.Instance != null)
                        {
                            PressingPanel.Instance.info("Breath Count：" + BreathingNumber, "", helpText);
                        }
                    }

                    // 播放吹气动画
                    if (StudyMode.Instance != null && StudyMode.Instance.character_Ani != null)
                    {
                        StartCoroutine(StudyMode.Instance.character_Ani.Play(19.33f, 20.7f, 2));
                    }

                    // 更新吹气面板
                    if (PressingPanel.Instance != null)
                    {
                        PressingPanel.Instance.chuiqi(BreathingNumber, 6);
                    }

                    DetailsDataYYMax = 0;
                    YY_zongLiang = 0;
                    DetailsDataYY = new List<int>();
                    BreathingNumber++;

                    // 更新信息
                    if (PressingPanel.Instance != null)
                    {
                        PressingPanel.Instance.info("Breath Count：" + BreathingNumber, "", helpText);
                    }

                    // 检查是否完成2次吹气
                    if (BreathingNumber == 2)
                    {
                        IsPeopleChuiQi = true;
                        IsEND = true;
                        Debug.Log("学习模式：完成2次吹气，设置IsPeopleChuiQi=true");
                    }

                    // 重置
                    intYY = 0;
                    helpText = "";
                }
            }
        }
        else // 其他场景（考核模式）
        {
            if (DetailsDataYY.Count > 0)
            {
                // 分析吹气数据
                for (int i = 0; i < DetailsDataYY.Count; i++)
                {
                    if (DetailsDataYY[i] > DetailsDataYYMax)
                    {
                        DetailsDataYYMax = DetailsDataYY[i];
                    }
                    if (DetailsDataYY[i] < 100)
                    {
                        intYY++;
                    }
                    YY_zongLiang += DetailsDataYY[i];
                }

                if (intYY == DetailsDataYY.Count)
                {
                    helpText = "Insufficient breath volume.";
                }

                // 重置按压计数
                Number = 0;
                Number_Count = 0;

                // 更新吹气面板
                if (PressingPanel.Instance != null)
                {
                    if (BreathingNumber_Count == 0)
                    {
                        PressingPanel.Instance.reChuiQIliangPanel();
                    }
                    PressingPanel.Instance.chuiqi(BreathingNumber, UnityEngine.Random.Range(3,6));
                }

                // 播放吹气动画
                if (StudyMode.Instance != null && StudyMode.Instance.character_Ani != null)
                {
                    StartCoroutine(StudyMode.Instance.character_Ani.Play(19.33f, 20.7f, 2));
                }

                // 重置数据
                DetailsDataYYMax = 0;
                YY_zongLiang = 0;
                DetailsDataYY = new List<int>();
                BreathingNumber++;
                BreathingNumber_Count++;

                Debug.Log("考核模式：吹气次数 BreathingNumber=" + BreathingNumber + ", BreathingNumber_Count=" + BreathingNumber_Count);

                // 检查吹气次数 - 关键修复：需要2次吹气
                if (BreathingNumber_Count >= 2) // 完成2次吹气
                {
                    Debug.Log("考核模式：完成2次吹气，设置IsPeopleChuiQi=true");
                    IsPeopleChuiQi = true;
                    BreathingNumber = 0;
                    BreathingNumber_Count = 0;

                    if (PressingPanel.Instance != null)
                    {
                        PressingPanel.Instance.reChuiQIliangPanel();
                    }
                }

                // 检查是否吹气过多（错误条件）
                if (BreathingNumber_Count == 4)
                {
                    if (StudyMode.Instance != null)
                    {
                        StudyMode.Instance.endPanel.SetActive(true);
                        StudyMode.Instance.endPanelText.text = "Unfortunately, resuscitation was unsuccessful. The compression-to-ventilation ratio must be 30:2. Please retry the practice.";
                    }
                }

                // 更新信息
                if (PressingPanel.Instance != null)
                {
                    PressingPanel.Instance.info("Breath Count：" + BreathingNumber_Count,
                        "Tidal Volume：" + YY_zongLiang,
                        helpText);
                }

                // 重置
                intYY = 0;
                helpText = "";
            }
        }
    }

    private void ResetTraining()
    {
        Debug.Log("键盘输入：重置训练");
        clearAll();
        ResetAllBools();
        if (StudyMode.Instance != null)
        {
            StudyMode.Instance.StartCoroutine(StudyMode.Instance.FlowManager());
        }
    }

    public void clearAll()
    {
        // 重置所有bool变量
        IsRemoveForeign = false;
        IsCheckPulse = false;
        IsCall = false;
        IsConsciousness = false;
        IsBreathe = false;
        IsPeopleBreathing = false;
        IsYangTou = false;
        IsEND = false;

        IsRemoveForeign_int = 0;
        IsCheckPulse_int = 0;
        IsCall_int = 0;
        IsConsciousness_int = 0;
        IsBreathe_int = 0;
        IsPeopleBreathing_int = 0;
        IsYangTou_int = 0;
        IsEND_int = 0;

        Lunci = 0;
        Lunci_anya = 0;
        Lunci_chuiqi = 0;
        isAnYaIng = false;
        isChuiQiIng = false;
        repeatNumber = 0;

        DetailsData = new List<int>();
        DetailsDataXX_Pinlv = new List<int>();
        DetailsDataYY = new List<int>();
        intYY = 0;
        dictData = new Dictionary<int, List<int>>();
        Number = 0;
        Number_Count = 0;
        BreathingNumber = 0;
        BreathingNumber_Count = 0;
        state = State.正确;
        DetailsDataCount = 0;
        DetailsDataMax = 0;
        DetailsDataYYMax = 0;
        helpText = "";

        reSetBool = true;

        YY_zongLiang = 0;
        time_xx = 0;
        time_bool = false;

        simulatingCompression = false;
        simulatingBreath = false;
        compressionTimer = 0f;
        breathTimer = 0f;

        isCheckingPulse = false;
        isCheckingBreath = false;
        isCompressing = false;
        isBreathing = false;

        if (GameObject.Find("DataLogic") != null)
        {
            Destroy(GameObject.Find("DataLogic"));
        }
    }
}