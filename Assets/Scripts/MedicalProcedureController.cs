using UnityEngine;
using System.Collections;
// 在另一个脚本中调用
public class MedicalProcedureController : MonoBehaviour
{
    public float startTime;
    public float endTime;
    public AnimationManager animationManager;

    IEnumerator Start()
    {


        yield return animationManager.Play(startTime, endTime, 1f);
        Debug.LogError("12124241");
    }
}