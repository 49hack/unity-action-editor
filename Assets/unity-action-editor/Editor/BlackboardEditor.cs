using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ActionEditor
{
    [System.Serializable]
    public class BlackboardEditor
    {
        [SerializeField] Blackborad m_Blackborard;
        [SerializeField] int m_LatestSelectValueTypeIndex;
        [SerializeField] bool m_IsFoldout;

        System.Type[] m_TypeListCache;
        string[] m_TypeNameCache;

        public void Draw(Window owner, Sequence sequence)
        {
            {
                var so = new SerializedObject(sequence);
                so.Update();
                var bloackboardProp = so.FindProperty(Sequence.PropNameBlackboard);
                if (bloackboardProp.objectReferenceValue == null)
                {
                    var blackboard = ScriptableObject.CreateInstance(typeof(Blackborad));
                    blackboard.name = typeof(Blackborad).Name;
                    AssetDatabase.AddObjectToAsset(blackboard, sequence);
                    EditorUtility.SetDirty(sequence);
                    AssetDatabase.SaveAssets();
                    bloackboardProp.objectReferenceValue = blackboard;
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(sequence);
                    AssetDatabase.SaveAssets();
                }
            }

            // add
            {
                if (m_TypeListCache == null || m_TypeListCache.Length <= 0)
                {
                    m_TypeListCache = Utility.GetSubClasses<SharedObject>();
                }
                if (m_TypeNameCache == null || m_TypeNameCache.Length <= 0)
                {
                    m_TypeNameCache = m_TypeListCache.Select(o => o.Name).ToArray();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    m_LatestSelectValueTypeIndex = EditorGUILayout.Popup("Value", m_LatestSelectValueTypeIndex, m_TypeNameCache);

                    if (GUILayout.Button("Create", GUILayout.Width(60f)))
                    {
                        var type = m_TypeListCache[m_LatestSelectValueTypeIndex];
                        var value = ScriptableObject.CreateInstance(type) as SharedObject;
                        value.name = type.Name;

                        AssetDatabase.AddObjectToAsset(value, sequence.Blackborad);
                        EditorUtility.SetDirty(sequence.Blackborad);
                        AssetDatabase.SaveAssets();

                        var so = new SerializedObject(sequence.Blackborad);
                        var objectsProp = so.FindProperty(Blackborad.PropNameSharedObjects);
                        var index = objectsProp.arraySize;
                        objectsProp.InsertArrayElementAtIndex(index);
                        var item = objectsProp.GetArrayElementAtIndex(index);
                        item.objectReferenceValue = value;

                        so.ApplyModifiedProperties();
                        EditorUtility.SetDirty(sequence);
                        AssetDatabase.SaveAssets();
                    }
                }
            }

            // list
            {
                var so = new SerializedObject(sequence.Blackborad);
                var objectsProp = so.FindProperty(Blackborad.PropNameSharedObjects);
                for (int i = 0; i < objectsProp.arraySize; i++)
                {
                    var item = objectsProp.GetArrayElementAtIndex(i);
                    var itemSo = new SerializedObject(item.objectReferenceValue);

                    using (var check = new EditorGUI.ChangeCheckScope())
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(item.objectReferenceValue.GetType().Name);

                        var nameProp = itemSo.FindProperty(SharedObject.PropNamePropertyName);
                        nameProp.stringValue = EditorGUILayout.DelayedTextField(nameProp.stringValue);

                        var valueProp = itemSo.FindProperty(SharedObject.PropNameValue);
                        EditorGUILayout.PropertyField(valueProp, GUIContent.none);

                        if (check.changed)
                        {
                            itemSo.ApplyModifiedProperties();
                        }
                    }
                }
            }
        }
    }
}