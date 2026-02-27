using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speak : MonoBehaviour
{
    public Text title;
    public Text Content;
    public static Speak _Instance;
    public AudioClip[] speakAudio;
    public AudioSource MainAudio;

    // 用于跟踪当前是否正在播放
    private bool isPlaying = false;

    public static Speak Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = FindObjectOfType(typeof(Speak)) as Speak;
            }
            return _Instance;
        }
    }

    // 异步方法，返回IEnumerator以便使用协程
    public IEnumerator info(string titleText, string ContentText, int i)
    {
        // 如果正在播放，等待当前播放完成
        while (isPlaying)
        {
            yield return null;
        }

        isPlaying = true;

        MainAudio.Stop();
        title.text = titleText;
        Content.text = ContentText;

        if (speakAudio[i] != null)
        {
            MainAudio.clip = speakAudio[i];
            MainAudio.Play();

            // 等待音频播放完成
            yield return new WaitForSeconds(speakAudio[i].length);
        }
        else
        {
            yield return new WaitForSeconds(4f);
        }

        isPlaying = false;
    }

    // 检查是否正在播放
    public bool IsPlaying
    {
        get { return isPlaying; }
    }

    // 停止播放
    public void StopPlaying()
    {
        if (isPlaying)
        {
            MainAudio.Stop();
            isPlaying = false;
            StopAllCoroutines(); // 停止所有与此相关的协程
        }
    }
}