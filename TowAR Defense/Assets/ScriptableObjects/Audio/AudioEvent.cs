using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class AudioEvent : ScriptableObject
{
    public abstract void Play(AudioSource source);
}


#if UNITY_EDITOR
[CustomEditor(typeof(AudioEvent), true)]
public class AudioEventEditor : Editor
{
    [SerializeField] private AudioSource _previewer;

    public void OnEnable()
    {
        _previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio Preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
    }

    public void OnDisable()
    {
        DestroyImmediate(_previewer.gameObject);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
        if (GUILayout.Button("Play"))
        {
            ((AudioEvent)target).Play(_previewer);
        }
        EditorGUI.EndDisabledGroup();
    }
}
#endif
