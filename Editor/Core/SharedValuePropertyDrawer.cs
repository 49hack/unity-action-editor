using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ActionEditor
{
    [CustomPropertyDrawer(typeof(SharedValue), true)]
    public class SharedValuePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative(SharedValue.PropNameValue);
            return EditorGUIEx.GetPropertyHeight(valueProp);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;

            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth * 0.5f, EditorGUIUtility.singleLineHeight);
            if (!string.IsNullOrEmpty(label.text))
            {
                EditorGUI.LabelField(labelRect, label.text);
            }

            var nameWidth = (rect.width - labelRect.width) * 0.3f;
            var propName = property.FindPropertyRelative(SharedValue.PropNamePropertyName);
            var propNameRect = new Rect(rect.x + labelRect.width, rect.y, nameWidth * 0.95f, EditorGUIUtility.singleLineHeight);
            propName.stringValue = EditorGUI.TextField(propNameRect, propName.stringValue);

            var itemWidth = (rect.width - labelRect.width) * 0.7f;
            var valueProp = property.FindPropertyRelative(SharedValue.PropNameValue);
            var valueType = EditorGUIEx.GetPropertyType(valueProp);
            var valueOffset = valueProp.propertyType == SerializedPropertyType.Generic ? 13f : 0f;
            var valueRect = new Rect(propNameRect.x + nameWidth + valueOffset, rect.y, itemWidth - valueOffset, rect.height);
            using (new Utility.LabelWidthScope(valueRect.width * 0.5f))
            {
                var valueName = valueProp.propertyType == SerializedPropertyType.Generic ? valueType.Name : "";
                EditorGUI.PropertyField(valueRect, valueProp, new GUIContent(valueName), true);
            }
        }

        
    }
}