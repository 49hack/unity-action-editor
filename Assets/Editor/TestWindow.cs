using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ActionEditor;
using System.Reflection;

public class TestWindow : EditorWindow
{
    [MenuItem("Window/Test check Window")]
    public static void Create()
    {
        var window = (TestWindow)GetWindow(typeof(TestWindow));
        window.Show();
    }

    [SerializeField] TestSerializedObject m_Serialized;

    public class Main
    {
        public Check check;
    }
    public class Check
    {
        public string value;
    }

    private void OnGUI()
    {
        m_Serialized = (TestSerializedObject)EditorGUILayout.ObjectField("Asset", m_Serialized, typeof(TestSerializedObject), false);
        if (m_Serialized == null)
            return;

        if(GUILayout.Button("Execute"))
        {
            var clip = ScriptableObject.CreateInstance(typeof(ActionEditor.Sample.FloatClip)) as ActionEditor.Sample.FloatClip;
            var blackboard = ScriptableObject.CreateInstance(typeof(ActionEditor.Blackborad)) as ActionEditor.Blackborad;
            ActionEditor.Utility.UpdateBlackboardReference(clip, blackboard);
            Debug.Log(clip.IsValid);
            DestroyImmediate(clip);
            DestroyImmediate(blackboard);
        }

        if (GUILayout.Button("Do"))
        {
            var instance = new Main();
            var fieldInfoList = instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach(var fieldInfo in fieldInfoList)
            {
                Debug.Log(fieldInfo.FieldType.BaseType.BaseType.BaseType);
            }
        }

    }
}
