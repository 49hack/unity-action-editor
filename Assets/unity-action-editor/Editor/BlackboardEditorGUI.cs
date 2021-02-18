using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ActionEditor
{
    internal static class BlackboardEditorGUI
    {
        public static void DrawBind(IReadOnlyList<Blackboard> blackboardList, Rect position, SerializedProperty property, GUIContent label, System.Type type)
        {
            using (new Utility.LabelWidthScope(80f))
            {
                var labelWidth = 0f;
                if (!string.IsNullOrEmpty(label.text))
                {
                    var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                    EditorGUI.LabelField(labelRect, label.text);
                    labelWidth = labelRect.width;
                }

                var valueRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);

                var nameProp = property.FindPropertyRelative(SharedValue.PropNamePropertyName);
                if (blackboardList == null || blackboardList.Count <= 0)
                {
                    EditorGUI.PropertyField(valueRect, nameProp, GUIContent.none);
                    return;
                }

                var fields = CollectFields(blackboardList, type);
                var propNames = GetBlackboardNameList(blackboardList, fields);
                if(propNames.Length < 0)
                {
                    EditorGUI.PropertyField(valueRect, nameProp, GUIContent.none);
                    return;
                }

                var currentIndex = FindIndex(propNames, nameProp.stringValue);
                bool isForceSet = currentIndex < 0 || string.IsNullOrEmpty(nameProp.stringValue);
                currentIndex = Mathf.Clamp(currentIndex, 0, propNames.Length - 1);
                var nextIndex = EditorGUI.Popup(valueRect, currentIndex, propNames);
                if (isForceSet || currentIndex != nextIndex)
                {
                    if(propNames.Length > nextIndex)
                        nameProp.stringValue = propNames[nextIndex];
                }
            }
        }

        public static List<FieldInfo> CollectFields(IReadOnlyList<Blackboard> blackboardList, System.Type valueType)
        {
            List<FieldInfo> result = new List<FieldInfo>();

            for (int bi = 0; bi < blackboardList.Count; bi++)
            {
                var blackboard = blackboardList[bi];
                var fields = CollectFields(blackboard, valueType);
                result.AddRange(fields);
            }

            return result;
        }

        public static List<FieldInfo> CollectFields(Blackboard blackboard, System.Type valueType = null)
        {
            List<FieldInfo> result = new List<FieldInfo>();
            if (blackboard == null)
                return result;

            var fieldInfos = blackboard.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            var sharedValueType = typeof(SharedValue);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                var fieldInfo = fieldInfos[i];
                var type = fieldInfo.FieldType;
                if (!sharedValueType.IsAssignableFrom(type))
                    continue;

                var valueInstance = fieldInfo.GetValue(blackboard);
                if (valueInstance == null)
                    continue;

                var sharedValue = (SharedValue)valueInstance;
                if (valueType != null && sharedValue.ValueType != valueType)
                    continue;

                var name = sharedValue.PropertyName;
                if (string.IsNullOrEmpty(name))
                    continue;

                result.Add(fieldInfo);
            }

            return result;
        }

        static string[] GetBlackboardNameList(IReadOnlyList<Blackboard> blackboardList, List<FieldInfo> fieldInfos)
        {
            var result = new string[fieldInfos.Count];

            for (int bi = 0; bi < blackboardList.Count; bi++)
            {
                var blackboard = blackboardList[bi];

                for (int i = 0; i < fieldInfos.Count; i++)
                {
                    var field = fieldInfos[i];
                    var sharedValue = field.GetValue(blackboard);
                    var type = sharedValue.GetType();
                    var nameField = typeof(SharedValue).GetField(SharedValue.PropNamePropertyName, BindingFlags.Instance | BindingFlags.NonPublic);
                    result[i] = (string)nameField.GetValue(sharedValue);
                }
            }

            return result;
        }

        static int FindIndex(string[] array, string value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                    return i;
            }
            return -1;
        }
    }
}