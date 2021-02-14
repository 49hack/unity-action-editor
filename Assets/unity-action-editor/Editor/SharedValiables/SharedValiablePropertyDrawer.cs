using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [CustomPropertyDrawer(typeof(SharedValue), true)]
    public class SharedValiablePropertyDrawer : PropertyDrawer
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
            using (new Utility.ColorScope(valueType == SharedValueType.Flexible ? Color.blue : GUI.color))
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

                case SharedValueType.Flexible:
                    {
                        // TOOD: 本当はリストから選択したい
                        var nameProp = property.FindPropertyRelative(SharedValue.PropNamePropertyName);
                        using (new Utility.ColorScope(string.IsNullOrEmpty(nameProp.stringValue) ? Color.red : GUI.color))
                        {
                            EditorGUI.PropertyField(valueRect, nameProp, GUIContent.none);
                        }
                    }
                    break;
            }
        }
    }
}