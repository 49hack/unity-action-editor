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

            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, new GUIContent(property.displayName));

            var itemWidth = (rect.width - labelRect.width) * 0.5f;
            var propName = property.FindPropertyRelative(SharedValue.PropNamePropertyName);
            var propNameRect = new Rect(rect.x + labelRect.width, rect.y, itemWidth * 0.95f, EditorGUIUtility.singleLineHeight);
            propName.stringValue = EditorGUI.TextField(propNameRect, propName.stringValue);

            var valueProp = property.FindPropertyRelative(SharedValue.PropNameValue);
            var valueType = GetPropertyType(valueProp);
            var valueRect = new Rect(propNameRect.x + itemWidth, rect.y, itemWidth, rect.height);
            EditorGUIEx.PropertyField(valueRect, valueProp, GUIContent.none, true, valueType);
        }

        static System.Type GetPropertyType(SerializedProperty property, bool isArrayListType = false)
        {
            var fieldInfo = GetFieldInfo(property);

            if (isArrayListType && property.isArray && property.propertyType != SerializedPropertyType.String)
                return fieldInfo.FieldType.IsArray
                    ? fieldInfo.FieldType.GetElementType()
                    : fieldInfo.FieldType.GetGenericArguments()[0];
            return fieldInfo.FieldType;
        }

        static FieldInfo GetFieldInfo(SerializedProperty property)
        {
            FieldInfo GetField(System.Type type, string path)
            {
                return type.GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            }

            var parentType = property.serializedObject.targetObject.GetType();
            var splits = property.propertyPath.Split('.');
            var fieldInfo = GetField(parentType, splits[0]);
            for (var i = 1; i < splits.Length; i++)
            {
                if (splits[i] == "Array")
                {
                    i += 2;
                    if (i >= splits.Length)
                        continue;

                    var type = fieldInfo.FieldType.IsArray
                        ? fieldInfo.FieldType.GetElementType()
                        : fieldInfo.FieldType.GetGenericArguments()[0];

                    fieldInfo = GetField(type, splits[i]);
                }
                else
                {
                    fieldInfo = i + 1 < splits.Length && splits[i + 1] == "Array"
                        ? GetField(parentType, splits[i])
                        : GetField(fieldInfo.FieldType.BaseType, splits[i]);
                }

                if (fieldInfo == null)
                    throw new System.Exception("Invalid FieldInfo. " + property.propertyPath);

                parentType = fieldInfo.FieldType;
            }

            return fieldInfo;
        }
    }
}