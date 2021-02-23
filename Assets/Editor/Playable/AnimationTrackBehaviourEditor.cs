using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [System.Serializable]
    [CustomTrackEditor(typeof(AnimationTrackBehaviour))]
    public class AnimationTrackBehaviourEditor : TrackBehaviourEditor
    {
        protected override Color BackgroundColor { get { return new Color(0f, 0f, 0f, 0.5f); } }


        protected override void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            var propName = serializedObject.FindProperty(TrackBehaviour.PropNameTrackName);
            var nameRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, EditorGUIUtility.singleLineHeight);
            propName.stringValue = EditorGUI.TextField(nameRect, propName.stringValue);
        }
    }
}