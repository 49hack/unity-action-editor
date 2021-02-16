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

        public void Enable()
        {
            m_TypeListCache = null;
            m_TypeNameCache = null;
        }
        public void Disable()
        {
            m_TypeListCache = null;
            m_TypeNameCache = null;
        }

        public void Draw(Window owner, Sequence sequence)
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                m_IsFoldout = EditorGUIEx.Foldout(m_IsFoldout, new GUIContent("Blackboard"));
                if (!m_IsFoldout)
                    return;

                if (m_TypeListCache == null || m_TypeListCache.Length <= 0)
                {
                    m_TypeListCache = Utility.GetSubClasses<SharedObject>().Where(o => !o.IsGenericType).ToArray();
                }
                if (m_TypeNameCache == null || m_TypeNameCache.Length <= 0)
                {
                    m_TypeNameCache = m_TypeListCache.Select(o => { var title = Utility.GetAttribute<MenuTitle>(o); return title != null ? title.Name : o.Name; }).ToArray();
                }

                using (new EditorGUI.IndentLevelScope(1))
                {
                    var blackboardObject = new SerializedObject(sequence.Blackborad);
                    blackboardObject.Update();
                    var objectsProp = blackboardObject.FindProperty(Blackborad.PropNameSharedObjects);

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

                            var index = objectsProp.arraySize;
                            objectsProp.InsertArrayElementAtIndex(index);
                            var item = objectsProp.GetArrayElementAtIndex(index);
                            item.objectReferenceValue = value;

                            blackboardObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(sequence.Blackborad);
                            AssetDatabase.SaveAssets();
                        }
                    }

                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                    {
                        int deleteIndex = -1;
                        for (int i = 0; i < objectsProp.arraySize; i++)
                        {
                            var item = objectsProp.GetArrayElementAtIndex(i);
                            if (item.objectReferenceValue == null)
                                continue;

                            var itemSo = new SerializedObject(item.objectReferenceValue);
                            itemSo.Update();

                            using (var check = new EditorGUI.ChangeCheckScope())
                            {
                                var valueProp = itemSo.FindProperty(SharedObject.PropNameValue);

                                var propertyHeight = EditorGUIEx.GetPropertyHeight(valueProp);

                                var fullRect = GUILayoutUtility.GetRect(1, propertyHeight + 8, GUILayout.ExpandWidth(true));
                                var rect = new Rect(fullRect.x + 10f, fullRect.y, fullRect.width - 10, fullRect.height);
                                var backColor = Color.Lerp(Color.white, Color.gray, (i % 2) == 0 ? 0.5f : 0.75f);
                                EditorGUI.DrawRect(rect, backColor);

                                var labelWidth = EditorGUIUtility.labelWidth;
                                var typeNameRect = new Rect(rect.x, rect.y + 2f, labelWidth, EditorGUIUtility.singleLineHeight);
                                EditorGUI.LabelField(typeNameRect, ToName(item.objectReferenceValue.GetType()));

                                var contentWidth = (rect.width - labelWidth - 4);

                                var propNameRect = new Rect(typeNameRect.x + typeNameRect.width + 1f, rect.y + 4f, contentWidth * 0.45f, EditorGUIUtility.singleLineHeight);
                                var nameProp = itemSo.FindProperty(SharedObject.PropNamePropertyName);
                                nameProp.stringValue = EditorGUI.DelayedTextField(propNameRect, nameProp.stringValue);

                                var valueRect = new Rect(propNameRect.x + propNameRect.width + 1f, rect.y + 4f, contentWidth * 0.45f, EditorGUIUtility.singleLineHeight);
                                EditorGUIEx.PropertyField(valueRect, valueProp, GUIContent.none);
                                
                                var deleteRect = new Rect(valueRect.x + valueRect.width + 1f, rect.y + 4f, contentWidth * 0.1f, EditorGUIUtility.singleLineHeight);
                                if (GUI.Button(deleteRect, "Delete"))
                                {
                                    deleteIndex = i;
                                }

                                //if (check.changed)
                                {
                                    
                                }
                            }

                            itemSo.ApplyModifiedProperties();
                        }

                        if (deleteIndex >= 0)
                        {
                            var item = objectsProp.GetArrayElementAtIndex(deleteIndex);
                            if (item.objectReferenceValue != null)
                            {
                                Object.DestroyImmediate(item.objectReferenceValue, true);
                            }
                            Utility.RemoveAt(objectsProp, deleteIndex);
                            blackboardObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(sequence.Blackborad);
                            AssetDatabase.SaveAssets();
                        }
                    }

                    blackboardObject.ApplyModifiedProperties();
                }
            }
        }

        string ToName(System.Type type)
        {
            for(int i = 0; i < m_TypeListCache.Length; i++)
            {
                if (m_TypeListCache[i] == type)
                    return m_TypeNameCache[i];
            }
            return type.Name;
        }

        public bool TryCreate(Sequence sequence)
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
                return true;
            }
            return false;
        }
    }
}