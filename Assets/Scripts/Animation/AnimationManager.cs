using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
    [Header("角色动画控制器")]
    public Animator doctor;
    public Animator patient;

    [Header("调试设置")]
    public bool debugLogs = true;

    // 运行时状态
    public string currentDoctorState;
    public string currentPatientState;
    private float segmentStartTime;
    private float segmentEndTime;
    private bool isPlayingSegment = false;
    private Coroutine playRoutine;

    // 动画长度缓存
    private float animationLength = -1f;
    public void Awake()
    {
        PauseAllAnimations();
    }

    void Update()
    {
        if (!isPlayingSegment) return;

        // 获取当前动画时间
        float currentTime = GetCurrentAnimationTime();

        // 检查是否到达结束时间
        if (currentTime >= segmentEndTime)
        {
            // 暂停在结束位置
            PauseAllAnimations();
            SeekToTime(segmentEndTime);
            isPlayingSegment = false;

            if (debugLogs) Debug.Log($"Segment completed: {currentDoctorState} ({segmentStartTime}s-{segmentEndTime}s)");
        }
    }

    /// <summary>
    /// 同步播放两个角色的指定时间段动画（协程版）
    /// </summary>
    public IEnumerator Play(float startTime, float endTime, float speed = 1f)
    {
        // 确保动画控制器有效
        if (!ValidateAnimators())
            yield break;

        // 停止当前播放
        StopCurrentPlayback();

        // 设置播放参数
        segmentStartTime = startTime;
        segmentEndTime = endTime;

        // 开始播放
        playRoutine = StartCoroutine(PlaySegmentInternal(speed));

        // 等待播放完成
        while (isPlayingSegment)
        {
            yield return null;
        }
    }

    // 内部播放逻辑
    private IEnumerator PlaySegmentInternal(float speed)
    {
        isPlayingSegment = true;

        // 获取动画长度（如果需要）
        if (animationLength < 0)
        {
            yield return CacheAnimationLength();
        }

        // 计算归一化时间
        float normalizedStart = segmentStartTime / animationLength;

        // 播放动画
        doctor.Play(currentDoctorState, -1, normalizedStart);
        patient.Play(currentPatientState, -1, normalizedStart);

        // 设置速度
        doctor.speed = speed;
        patient.speed = speed;

        // 强制立即更新动画状态
        doctor.Update(0f);
        patient.Update(0f);

        if (debugLogs)
        {
            Debug.Log($"Started segment: Doctor={currentDoctorState}, " +
                     $"Patient={currentPatientState}, " +
                     $"{segmentStartTime}s-{segmentEndTime}s, " +
                     $"Speed={speed}");
        }
    }

    // 缓存动画长度
    private IEnumerator CacheAnimationLength()
    {
        // 临时播放动画以获取长度
        doctor.Play(currentDoctorState, -1, 0f);
        doctor.speed = 1f;

        // 等待一帧确保动画状态更新
        yield return null;

        // 获取动画状态信息
        AnimatorStateInfo stateInfo = doctor.GetCurrentAnimatorStateInfo(0);
        animationLength = stateInfo.length;

        // 暂停动画
        doctor.speed = 0f;

        if (debugLogs) Debug.Log($"Cached animation length: {animationLength}s");
    }

    // 获取当前动画时间（秒）
    private float GetCurrentAnimationTime()
    {
        if (!doctor || !doctor.isActiveAndEnabled) return 0f;

        AnimatorStateInfo stateInfo = doctor.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime * animationLength;
    }

    // 停止当前播放
    private void StopCurrentPlayback()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        PauseAllAnimations();
        isPlayingSegment = false;
    }

    // 验证动画控制器
    private bool ValidateAnimators()
    {
        if (doctor == null) doctor = GetComponent<Animator>();

        if (doctor == null)
        {
            Debug.LogError("Doctor Animator not found!");
            return false;
        }

        if (patient == null)
        {
            Debug.LogError("Patient Animator not found!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 暂停所有动画
    /// </summary>
    public void PauseAllAnimations()
    {
        if (doctor) doctor.speed = 0;
        if (patient) patient.speed = 0;
    }

    /// <summary>
    /// 恢复所有动画
    /// </summary>
    public void ResumeAllAnimations(float speed = 1f)
    {
        if (doctor) doctor.speed = speed;
        if (patient) patient.speed = speed;
    }

    /// <summary>
    /// 跳转到指定时间点（双角色同步）
    /// </summary>
    public void SeekToTime(float time)
    {
        if (animationLength <= 0) return;

        float normalizedTime = Mathf.Clamp01(time / animationLength);

        if (doctor)
        {
            doctor.Play(currentDoctorState, -1, normalizedTime);
            doctor.Update(0f);
        }

        if (patient)
        {
            patient.Play(currentPatientState, -1, normalizedTime);
            patient.Update(0f);
        }

        if (debugLogs) Debug.Log($"Seeked to: {time}s (normalized: {normalizedTime})");
    }

    /// <summary>
    /// 获取当前动画时间
    /// </summary>
    public float GetCurrentTime()
    {
        return GetCurrentAnimationTime();
    }

    /// <summary>
    /// 获取动画长度
    /// </summary>
    public float GetAnimationLength()
    {
        return animationLength;
    }
}