using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor.Sample
{
    [CustomClipEditor(typeof(FloatClip))]
    public class FloatClipEditor : ClipEditor
    {
        protected override void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            base.DrawContents(rect, serializedObject);

            var labelRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, new GUIContent("Value"));

            var valueRect = new Rect(labelRect.x, labelRect.y + 2f + EditorGUIUtility.singleLineHeight, labelRect.width - 4f, EditorGUIUtility.singleLineHeight);
            var valueProp = serializedObject.FindProperty(FloatClip.PropNameValue);
            valueProp.floatValue = EditorGUI.DelayedFloatField(valueRect, valueProp.floatValue);
        }
    }
}