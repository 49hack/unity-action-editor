using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ActionEditor
{
    public static class EditorGUIEx
    {
        public static float GetPropertyHeight(SerializedProperty property)
        {
            switch(property.propertyType)
            {
                case SerializedPropertyType.Vector4:
                    return EditorGUIUtility.singleLineHeight;

                case SerializedPropertyType.Vector3:
                    return EditorGUIUtility.singleLineHeight;

                case SerializedPropertyType.Vector2:
                    return EditorGUIUtility.singleLineHeight;

                case SerializedPropertyType.Rect:
                    return EditorGUIUtility.singleLineHeight;

                case SerializedPropertyType.Quaternion:
                    return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUI.GetPropertyHeight(property);
        }

        public static void PropertyField(Rect position, SerializedProperty property, GUIContent label, bool isIncludeChildren = false, System.Type type = null)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector4:
                    Vector4Oneline(position, property, label);
                    break;

                case SerializedPropertyType.Vector3:
                    Vector3Oneline(position, property, label);
                    break;

                case SerializedPropertyType.Vector2:
                    Vector2Oneline(position, property, label);
                    break;

                case SerializedPropertyType.Rect:
                    RectOneline(position, property, label);
                    break;

                case SerializedPropertyType.Quaternion:
                    QuaternionOneline(position, property, label);
                    break;

                case SerializedPropertyType.ObjectReference:
                    var propType = type == null ? typeof(Object) : type;
                    var prev = property.objectReferenceValue;
                    property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, propType, true);
                    break;

                default:
                    EditorGUI.PropertyField(position, property, label, isIncludeChildren);
                    break;
            }
        }

        public static void Vector2Oneline(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = 0f;
            if (!string.IsNullOrEmpty(label.text))
            {
                var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                EditorGUI.LabelField(labelRect, label.text);
                labelWidth = labelRect.width;
            }

            var perWidth = (position.width - labelWidth) / 2;

            var x = position.x + labelWidth;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var charWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x;
                var xLabelRect = new Rect(x, position.y, 25f, position.height);
                var xValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(xLabelRect, "X");
                var valX = EditorGUI.DelayedFloatField(xValueRect, property.vector2Value.x);
                x += perWidth;

                var yLabelRect = new Rect(x, position.y, 25f, position.height);
                var yValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(yLabelRect, "Y");
                var valY = EditorGUI.DelayedFloatField(yValueRect, property.vector2Value.y);
                x += perWidth;

                if (check.changed)
                {
                    property.vector2Value = new Vector2(valX, valY);
                }
            }
        }

        public static void Vector3Oneline(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = 0f;
            if (!string.IsNullOrEmpty(label.text))
            {
                var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                EditorGUI.LabelField(labelRect, label.text);
                labelWidth = labelRect.width;
            }

            var perWidth = (position.width - labelWidth) / 3;

            var x = position.x + labelWidth;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var charWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x;
                var xLabelRect = new Rect(x, position.y, 25f, position.height);
                var xValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(xLabelRect, "X");
                var valX = EditorGUI.DelayedFloatField(xValueRect, property.vector3Value.x);
                x += perWidth;

                var yLabelRect = new Rect(x, position.y, 25f, position.height);
                var yValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(yLabelRect, "Y");
                var valY = EditorGUI.DelayedFloatField(yValueRect, property.vector3Value.y);
                x += perWidth;

                var zLabelRect = new Rect(x, position.y, 25f, position.height);
                var zValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(zLabelRect, "Z");
                var valZ = EditorGUI.DelayedFloatField(zValueRect, property.vector3Value.z);
                x += perWidth;

                if (check.changed)
                {
                    property.vector3Value = new Vector3(valX, valY, valZ);
                }
            }
        }

        public static void Vector4Oneline(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = 0f;
            if (!string.IsNullOrEmpty(label.text))
            {
                var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                EditorGUI.LabelField(labelRect, label.text);
                labelWidth = labelRect.width;
            }

            var perWidth = (position.width - labelWidth) / 4;

            var x = position.x + labelWidth;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var charWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x;
                var xLabelRect = new Rect(x, position.y, 25f, position.height);
                var xValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(xLabelRect, "X");
                var valX = EditorGUI.DelayedFloatField(xValueRect, property.vector4Value.x);
                x += perWidth;

                var yLabelRect = new Rect(x, position.y, 25f, position.height);
                var yValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(yLabelRect, "Y");
                var valY = EditorGUI.DelayedFloatField(yValueRect, property.vector4Value.y);
                x += perWidth;

                var zLabelRect = new Rect(x, position.y, 25f, position.height);
                var zValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(zLabelRect, "Z");
                var valZ = EditorGUI.DelayedFloatField(zValueRect, property.vector4Value.z);
                x += perWidth;

                var wLabelRect = new Rect(x, position.y, 25f, position.height);
                var wValueRect = new Rect(x + charWidth, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(wLabelRect, "W");
                var valW = EditorGUI.DelayedFloatField(wValueRect, property.vector4Value.w);

                if (check.changed)
                {
                    property.vector4Value = new Vector4(valX, valY, valZ, valW);
                }
            }
        }

        public static void QuaternionOneline(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = 0f;
            if (!string.IsNullOrEmpty(label.text))
            {
                var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                EditorGUI.LabelField(labelRect, label.text);
                labelWidth = labelRect.width;
            }

            var perWidth = (position.width - labelWidth) / 4;

            var x = position.x + labelWidth;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var charWidth = EditorStyles.label.CalcSize(new GUIContent("W")).x;
                var xLabelRect = new Rect(x, position.y, 25f, position.height);
                var xValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(xLabelRect, "X");
                var valX = EditorGUI.DelayedFloatField(xValueRect, property.quaternionValue.x);
                x += perWidth;

                var yLabelRect = new Rect(x, position.y, 25f, position.height);
                var yValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(yLabelRect, "Y");
                var valY = EditorGUI.DelayedFloatField(yValueRect, property.quaternionValue.y);
                x += perWidth;

                var zLabelRect = new Rect(x, position.y, 30f, position.height);
                var zValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(zLabelRect, "Z");
                var valZ = EditorGUI.DelayedFloatField(zValueRect, property.quaternionValue.z);
                x += perWidth;

                var wLabelRect = new Rect(x, position.y, 30f, position.height);
                var wValueRect = new Rect(x + charWidth, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(wLabelRect, "W");
                var valW = EditorGUI.DelayedFloatField(wValueRect, property.quaternionValue.w);

                if (check.changed)
                {
                    property.quaternionValue = new Quaternion(valX, valY, valZ, valW);
                }
            }
        }

        public static void RectOneline(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = 0f;
            if (!string.IsNullOrEmpty(label.text))
            {
                var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                EditorGUI.LabelField(labelRect, label.text);
                labelWidth = labelRect.width;
            }

            var perWidth = (position.width - labelWidth) / 4;

            var x = position.x + labelWidth;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var charWidth = EditorStyles.label.CalcSize(new GUIContent("W")).x;
                var xLabelRect = new Rect(x, position.y, 25f, position.height);
                var xValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(xLabelRect, "X");
                var valX = EditorGUI.DelayedFloatField(xValueRect, property.rectValue.x);
                x += perWidth;

                var yLabelRect = new Rect(x, position.y, 25f, position.height);
                var yValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(yLabelRect, "Y");
                var valY = EditorGUI.DelayedFloatField(yValueRect, property.rectValue.y);
                x += perWidth;

                var zLabelRect = new Rect(x, position.y, 30f, position.height);
                var zValueRect = new Rect(x + charWidth + 1, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(zLabelRect, "W");
                var valWidth = EditorGUI.DelayedFloatField(zValueRect, property.rectValue.width);
                x += perWidth;

                var wLabelRect = new Rect(x, position.y, 30f, position.height);
                var wValueRect = new Rect(x + charWidth, position.y, perWidth - charWidth - 1, position.height);
                EditorGUI.LabelField(wLabelRect, "H");
                var valHeight = EditorGUI.DelayedFloatField(wValueRect, property.rectValue.height);

                if (check.changed)
                {
                    property.rectValue = new Rect(valX, valY, valWidth, valHeight);
                }
            }
        }

        public static bool Foldout(bool display, GUIContent content)
        {
            const float Height = 24f;
            const float ToggleSize = 13f;

            var rect = GUILayoutUtility.GetRect(1f, Height, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, Color.gray);

            var labelRect = new Rect(rect.x + ToggleSize + 6f, rect.y, rect.width - ToggleSize - 10f, rect.height);
            EditorGUI.LabelField(labelRect, content);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 5f, ToggleSize, ToggleSize);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }
    }
}