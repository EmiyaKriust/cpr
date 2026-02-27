using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class Loading : MonoBehaviour
{
    // 指定加载过程中需要显示的UI元素
    public GameObject loadScreen;
    public Slider slider;
    public Text text;

 
    // Start is called before the first frame update
    public void LoadNextLevel(string name)
    {
        // 调用协程函数开始异步加载场景
        StartCoroutine(Loadlevel(name));
    }

    IEnumerator Loadlevel(string name)
    {
        // 激活加载界面
        loadScreen.SetActive(true);
        // 异步加载下一个场景，并等待加载结束
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        // 不允许立即激活场景
       operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // 在整个加载过程中更新进度条显示
            slider.value = operation.progress;
            text.text = (operation.progress * 100).ToString("F2") + "%";
            if (operation.progress >= 0.9f)
            {
                // 如果已经加载到了90%以上，则直接跳转下一个场景
                slider.value = 1;
                operation.allowSceneActivation = true;
                text.text = "Loading……";
                // text.text = "Press Any Key to Continue";
            }
            yield return null;
        }
    }


 
 
}
