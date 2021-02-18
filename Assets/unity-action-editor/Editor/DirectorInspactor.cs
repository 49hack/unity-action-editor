using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace ActionEditor
{
    using Runtime;

    [CustomEditor(typeof(Director))]
    public class DirectorInspactor : Editor
    {
        ReorderableList m_BlackboardList;

        void OnEnable()
        {
            m_BlackboardList = new ReorderableList(serializedObject, serializedObject.FindProperty(Director.PropNameBlackboardList));
            m_BlackboardList.drawHeaderCallback = (rect) => {
                 EditorGUI.LabelField(rect, "Blackboard");
             };
            m_BlackboardList.drawElementCallback = (rect, index, isActive, isFocused) => {
                var item = m_BlackboardList.serializedProperty.GetArrayElementAtIndex(index);
                item.objectReferenceValue = EditorGUI.ObjectField(rect, item.objectReferenceValue, typeof(Blackboard), true);
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            serializedObject.Update();

            m_BlackboardList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}