using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor.Sample
{
    [System.Serializable]
    [CustomTrackEditor(typeof(FloatTrack))]
    public class FloatTrackEditor : TrackEditor
    {
        protected override Color BackgroundColor { get { return new Color(0f, 0f, 0f, 0.5f); } }

        protected override void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            var propName = serializedObject.FindProperty(Track.PropNameTrackName);
            var nameRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, EditorGUIUtility.singleLineHeight);
            propName.stringValue = EditorGUI.TextField(nameRect, propName.stringValue);

            var propValue = serializedObject.FindProperty(FloatTrack.PropNameValue);
            var valueRect = new Rect(nameRect.x, nameRect.y + EditorGUIUtility.singleLineHeight + 2f, nameRect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(valueRect, propValue);
        }
    }
}