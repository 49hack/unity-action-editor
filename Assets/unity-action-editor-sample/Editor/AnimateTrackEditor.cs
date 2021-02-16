using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor.Sample
{
    [System.Serializable]
    [CustomTrackEditor(typeof(AnimateTrack))]
    public class AnimateTrackEditor : TrackEditor
    {
        protected override Color BackgroundColor { get { return new Color(0f, 0f, 0f, 0.5f); } }


        protected override void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            var propName = serializedObject.FindProperty(Track.PropNameTrackName);
            var nameRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, EditorGUIUtility.singleLineHeight);
            propName.stringValue = EditorGUI.TextField(nameRect, propName.stringValue);

            var animRect = new Rect(rect.x + 2f, rect.y + nameRect.height + 5f, rect.width - 4f, EditorGUIUtility.singleLineHeight);
            var animProp = serializedObject.FindProperty(AnimateTrack.PropNameAnimator);
            DrawBinding(animRect, animProp, GUIContent.none);
        }
    }
}