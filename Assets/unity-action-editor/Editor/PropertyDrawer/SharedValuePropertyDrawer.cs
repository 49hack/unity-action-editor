using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [CustomPropertyDrawer(typeof(SharedValue), true)]
    public class SharedValuePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = 0f;
            if (!string.IsNullOrEmpty(label.text))
            {
                var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                EditorGUI.LabelField(labelRect, label.text);
                labelWidth = labelRect.width;
            }

            float SelectorWidth = Mathf.Min(EditorGUIUtility.singleLineHeight, position.width);
            var valueRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth - SelectorWidth, position.height);

            var valueTypeProp = property.FindPropertyRelative(SharedValue.PropNameValueType);
            var modeEnumValues = System.Enum.GetValues(typeof(SharedValueType));
            var valueType = (SharedValueType)modeEnumValues.GetValue(valueTypeProp.enumValueIndex);
            using (new Utility.ColorScope(valueType == SharedValueType.Blackboard ? new Color(0.25f, 0.25f, 0.5f) : GUI.color))
            {
                var selectorRect = new Rect(valueRect.x + valueRect.width, valueRect.y, SelectorWidth, valueRect.height);
                EditorGUI.PropertyField(selectorRect, valueTypeProp, GUIContent.none);
            }

            valueType = (SharedValueType)modeEnumValues.GetValue(valueTypeProp.enumValueIndex);
            switch(valueType)
            {
                case SharedValueType.Fixed:
                    {
                        var valueProp = property.FindPropertyRelative(SharedValue.PropNameValue);
                        EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
                    }
                    break;

                case SharedValueType.Blackboard:
                    {
                        var nameProp = property.FindPropertyRelative(SharedValue.PropNamePropertyName);
                        var blackboradProp = property.FindPropertyRelative(SharedValue.PropNameBlackboard);
                        if(blackboradProp.objectReferenceValue == null)
                        {
                            EditorGUI.PropertyField(valueRect, nameProp, GUIContent.none);
                            break;
                        }

                        var blackboard = (Blackborad)blackboradProp.objectReferenceValue;
                        var propNames = blackboard.GetPropertyNames(null);
                        var currentIndex = FindIndex(propNames, nameProp.stringValue);
                        bool isForceSet = currentIndex < 0;
                        currentIndex = Mathf.Clamp(currentIndex, 0, propNames.Length - 1);
                        var nextIndex = EditorGUI.Popup(valueRect, currentIndex, propNames);
                        if(isForceSet || currentIndex != nextIndex)
                        {
                            nameProp.stringValue = propNames[nextIndex];
                        }
                    }
                    break;
            }
        }

        int FindIndex(string[] array, string value)
        {
            for(int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                    return i;
            }
            return -1;
        }
    }
}