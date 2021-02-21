using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ActionEditor
{
    internal static class BlackboardEditorGUI
    {
        public static void DrawContext(IReadOnlyList<Blackboard> blackboardList, Rect position, SerializedProperty property, GUIContent label, System.Type type, float titleWidth = 80f)
        {
            using (new Utility.LabelWidthScope(titleWidth))
            {
                var labelWidth = 0f;
                if (!string.IsNullOrEmpty(label.text))
                {
                    var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                    EditorGUI.LabelField(labelRect, label.text);
                    labelWidth = labelRect.width;
                }

                const float SelectorWidth = 20f;

                var valueRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth - SelectorWidth, position.height);

                var nameProp = property.FindPropertyRelative(SharedValueContext.PropNamePropertyName);
                if (blackboardList == null || blackboardList.Count <= 0)
                {
                    EditorGUI.PropertyField(valueRect, nameProp, GUIContent.none);
                    return;
                }

                var isForceSet = false;
                var sharedTypeProp = property.FindPropertyRelative(SharedValueContext.PropNameSharedType);
                var sharedTypeValues = System.Enum.GetValues(typeof(SharedValueType));
                var sharedType = (SharedValueType)sharedTypeValues.GetValue(sharedTypeProp.enumValueIndex);
                var sharedTypeRect = new Rect(valueRect.x + valueRect.width, valueRect.y, SelectorWidth, valueRect.height);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.PropertyField(sharedTypeRect, sharedTypeProp, GUIContent.none);
                    isForceSet = check.changed;
                }

                switch (sharedType)
                {
                    case SharedValueType.Blackboard:
                        {
                            var fieldInfos = CollectFields(blackboardList, type);
                            var propNames = GetBlackboardNameList(fieldInfos);
                            if (propNames.Length < 0)
                            {
                                EditorGUI.PropertyField(valueRect, nameProp, GUIContent.none);
                                return;
                            }

                            var currentIndex = FindIndex(propNames, nameProp.stringValue);
                            isForceSet |= currentIndex < 0 || string.IsNullOrEmpty(nameProp.stringValue);
                            currentIndex = Mathf.Clamp(currentIndex, 0, propNames.Length - 1);
                            var nextIndex = EditorGUI.Popup(valueRect, currentIndex, propNames);
                            if (isForceSet || currentIndex != nextIndex)
                            {
                                if (propNames.Length > nextIndex)
                                    nameProp.stringValue = propNames[nextIndex];
                            }
                        }
                        break;

                    case SharedValueType.Runtime:
                        {
                            EditorGUI.PropertyField(valueRect, nameProp, GUIContent.none);
                        }
                        break;

                    case SharedValueType.Fixed:
                        {
                            var fixedValueProp = property.FindPropertyRelative(SharedValueContext.PropNameFixedValue);
                            EditorGUI.PropertyField(valueRect, fixedValueProp, GUIContent.none);
                        }
                        break;
                }
            }
        }

        public static (Blackboard blackboard, List<FieldInfo>)[] CollectFields(IReadOnlyList<Blackboard> blackboardList, System.Type valueType)
        {
            var result = new (Blackboard blackboard, List<FieldInfo>)[blackboardList.Count];

            for (int bi = 0; bi < blackboardList.Count; bi++)
            {
                var blackboard = blackboardList[bi];
                var fields = CollectFields(blackboard, valueType);
                result[bi] = (blackboard, fields);
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

        static string[] GetBlackboardNameList((Blackboard blackboard, List<FieldInfo> fields)[] fieldInfos)
        {
            var result = new List<string>();

            for (int bi = 0; bi < fieldInfos.Length; bi++)
            {
                var blackboard = fieldInfos[bi].blackboard;

                for (int i = 0; i < fieldInfos[bi].fields.Count; i++)
                {
                    var field = fieldInfos[bi].fields[i];
                    var sharedValue = field.GetValue(blackboard);
                    var type = sharedValue.GetType();
                    var nameField = typeof(SharedValue).GetField(SharedValue.PropNamePropertyName, BindingFlags.Instance | BindingFlags.NonPublic);
                    result.Add((string)nameField.GetValue(sharedValue));
                }
            }

            return result.ToArray();
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