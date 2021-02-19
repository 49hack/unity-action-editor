using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ActionEditor
{
    [System.Serializable]
    public class BlackboardEditor
    {
        [SerializeField] bool m_IsFoldout;

        public void Draw(IReadOnlyList<Blackboard> blackboards)
        {
            if (blackboards == null)
                return;

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                m_IsFoldout = EditorGUIEx.Foldout(m_IsFoldout, new GUIContent("Blackboard"));
                if (!m_IsFoldout)
                    return;

                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    for (int bi = 0; bi < blackboards.Count; bi++)
                    {
                        var blackboard = blackboards[bi];
                        if (blackboard == null)
                            continue;

                        var serializedObject = new SerializedObject(blackboard);
                        serializedObject.Update();

                        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                        {
                            var foldoutProp = serializedObject.FindProperty(Blackboard.PropNameFoldout);
                            foldoutProp.boolValue = EditorGUIEx.Foldout(foldoutProp.boolValue, new GUIContent(blackboard.GetType().Name));
                            if (!foldoutProp.boolValue)
                            {
                                serializedObject.ApplyModifiedProperties();
                                return;
                            }

                            var feilds = BlackboardEditorGUI.CollectFields(blackboard);

                            for (int i = 0; i < feilds.Count; i++)
                            {
                                var field = feilds[i];
                                var attr = field.GetCustomAttribute<MenuTitle>();
                                var typeName = attr == null ? field.FieldType.Name : attr.Name;
                                var fieldProp = serializedObject.FindProperty(field.Name);
                                if (fieldProp == null)
                                    continue;

                                var propValue = fieldProp.FindPropertyRelative(SharedValue.PropNameValue);

                                var height = EditorGUIEx.GetPropertyHeight(propValue);
                                var secureRect = GUILayoutUtility.GetRect(1f, height + 2f, GUILayout.ExpandWidth(true));
                                var rect = new Rect(secureRect.x, secureRect.y, secureRect.width, height);

                                var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                                EditorGUI.LabelField(labelRect, new GUIContent(fieldProp.displayName));

                                var itemWidth = (rect.width - labelRect.width) * 0.5f;
                                var propName = fieldProp.FindPropertyRelative(SharedValue.PropNamePropertyName);
                                var propNameRect = new Rect(rect.x + labelRect.width, rect.y, itemWidth * 0.95f, EditorGUIUtility.singleLineHeight);
                                propName.stringValue = EditorGUI.TextField(propNameRect, propName.stringValue);

                                var valueType = field.FieldType.BaseType.GetField(SharedValue.PropNameValue, BindingFlags.Instance | BindingFlags.NonPublic).FieldType;
                                var valueRect = new Rect(propNameRect.x + itemWidth, rect.y, itemWidth, rect.height);
                                EditorGUIEx.PropertyField(valueRect, propValue, GUIContent.none, true, valueType);
                            }

                            serializedObject.ApplyModifiedProperties();
                        }
                        EditorGUILayout.Space();
                    }
                }
            }
        }
    }
}