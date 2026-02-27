using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class KeyConfigData
{
    public string actionName;
    public KeyCode keyCode;
    public string description;
}

public class KeyConfigManager : MonoBehaviour
{
    [Header("按键配置")]
    public List<KeyConfigData> keyConfigs = new List<KeyConfigData>
    {
        new KeyConfigData { actionName = "CheckConsciousness", keyCode = KeyCode.Q, description = "Check consciousness" },
        new KeyConfigData { actionName = "CheckBreath", keyCode = KeyCode.W, description = "Check pulse/Check breathing" },
        new KeyConfigData { actionName = "EmergencyCall", keyCode = KeyCode.E, description = "Emergency Call" },
        new KeyConfigData { actionName = "ClearAirway", keyCode = KeyCode.R, description = "Clear airway" },
        new KeyConfigData { actionName = "TiltHead", keyCode = KeyCode.T, description = "Head tilt–chin lift" },
        new KeyConfigData { actionName = "StartCPR", keyCode = KeyCode.Space, description = "Start compressions" },
        new KeyConfigData { actionName = "RescueBreath", keyCode = KeyCode.Space, description = "Rescue breaths" },
    };

    [Header("UI显示")]
    public Text keyDisplayText; // 直接把你的Text拖到这里

    void Start()
    {
        UpdateDisplay();
    }

    // 更新显示
    void UpdateDisplay()
    {
        if (keyDisplayText != null)
        {
            keyDisplayText.text = GetKeyConfigDisplayText();
        }
    }

    // 获取按键配置的显示文本
    string GetKeyConfigDisplayText()
    {
        string text = "Button Configuration：\n\n";

        foreach (var config in keyConfigs)
        {
            string keyName = GetKeyDisplayName(config.keyCode);
            text += $"{config.description}: {keyName}\n";
        }

        return text;
    }

    // 获取按键的友好显示名称
    string GetKeyDisplayName(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Space:
                return "Space";
            default:
                return keyCode.ToString();
        }
    }
}