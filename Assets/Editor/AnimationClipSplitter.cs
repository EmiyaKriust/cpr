using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AnimationClipSplitter : EditorWindow
{
    private AnimationClip originalClip;
    private List<AnimationClip> splitClips = new List<AnimationClip>();
    private List<Vector2> timeRanges = new List<Vector2>();
    private string savePath = "Assets/";
    private bool preserveRootMotion = true;

    [MenuItem("Tools/Animation Clip Splitter")]
    public static void ShowWindow()
    {
        GetWindow<AnimationClipSplitter>("Animation Splitter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Animation Clip Splitter", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        originalClip = (AnimationClip)EditorGUILayout.ObjectField("Original Clip", originalClip, typeof(AnimationClip), false);
        if (EditorGUI.EndChangeCheck())
        {
            splitClips.Clear();
            timeRanges.Clear();
        }

        savePath = EditorGUILayout.TextField("Save Path", savePath);
        preserveRootMotion = EditorGUILayout.Toggle("Preserve Root Motion", preserveRootMotion);

        if (GUILayout.Button("Add Split Segment"))
        {
            timeRanges.Add(new Vector2(0, originalClip ? originalClip.length : 1));
        }

        for (int i = 0; i < timeRanges.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label($"Segment {i + 1}");

            if (originalClip)
            {
                Vector2 range = timeRanges[i];
                range = EditorGUILayout.Vector2Field("Time Range (Start-End)", range);
                EditorGUILayout.MinMaxSlider(ref range.x, ref range.y, 0, originalClip.length);
                timeRanges[i] = range;
            }
            else
            {
                Vector2 range = timeRanges[i];
                range = EditorGUILayout.Vector2Field("Time Range (Start-End)", range);
                timeRanges[i] = range;
            }

            if (GUILayout.Button("Remove"))
            {
                timeRanges.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndVertical();
        }

        if (originalClip && timeRanges.Count > 0)
        {
            if (GUILayout.Button("Split Animation"))
            {
                SplitAnimationClip();
            }
        }

        if (splitClips.Count > 0)
        {
            GUILayout.Label("Generated Clips:", EditorStyles.boldLabel);
            foreach (var clip in splitClips)
            {
                EditorGUILayout.ObjectField(clip.name, clip, typeof(AnimationClip), false);
            }
        }
    }

    private void SplitAnimationClip()
    {
        splitClips.Clear();

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        for (int i = 0; i < timeRanges.Count; i++)
        {
            var range = timeRanges[i];
            float startTime = Mathf.Min(range.x, range.y);
            float endTime = Mathf.Max(range.x, range.y);

            AnimationClip newClip = new AnimationClip();
            newClip.name = $"{originalClip.name}_Part{i + 1}";

            // 复制动画曲线
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(originalClip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(originalClip, binding);
                AnimationCurve newCurve = new AnimationCurve();

                foreach (Keyframe key in curve.keys)
                {
                    if (key.time >= startTime && key.time <= endTime)
                    {
                        float newTime = key.time - startTime;
                        Keyframe newKey = new Keyframe(newTime, key.value, key.inTangent, key.outTangent);
                        newCurve.AddKey(newKey);
                    }
                }

                // 特别处理旋转曲线
                if (binding.propertyName.Contains("LocalRotation") || binding.propertyName.Contains("RootQ"))
                {
                    for (int j = 0; j < newCurve.keys.Length; j++)
                    {
                        Keyframe key = newCurve.keys[j];
                        key.value = Mathf.Repeat(key.value, 360f); // 确保旋转值在合理范围内
                        newCurve.MoveKey(j, key);
                    }
                }

                AnimationUtility.SetEditorCurve(newClip, binding, newCurve);
            }

            // 处理动画事件
            AnimationEvent[] events = AnimationUtility.GetAnimationEvents(originalClip);
            List<AnimationEvent> newEvents = new List<AnimationEvent>();

            foreach (AnimationEvent evt in events)
            {
                if (evt.time >= startTime && evt.time <= endTime)
                {
                    AnimationEvent newEvent = new AnimationEvent
                    {
                        time = evt.time - startTime,
                        functionName = evt.functionName,
                        stringParameter = evt.stringParameter,
                        floatParameter = evt.floatParameter,
                        intParameter = evt.intParameter,
                        objectReferenceParameter = evt.objectReferenceParameter,
                        messageOptions = evt.messageOptions
                    };
                    newEvents.Add(newEvent);
                }
            }

            AnimationUtility.SetAnimationEvents(newClip, newEvents.ToArray());

            // 设置根运动属性
            if (preserveRootMotion)
            {
                newClip.legacy = originalClip.legacy;
                AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(originalClip);
                AnimationUtility.SetAnimationClipSettings(newClip, settings);
            }

            // 保存新剪辑
            string path = Path.Combine(savePath, newClip.name + ".anim");
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(newClip, path);

            splitClips.Add(newClip);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Split {originalClip.name} into {timeRanges.Count} clips");
    }
}