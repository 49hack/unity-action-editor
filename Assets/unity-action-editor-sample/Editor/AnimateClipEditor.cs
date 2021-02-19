using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor.Sample
{
    [CustomClipEditor(typeof(AnimateClip))]
    public class AnimateClipEditor : ClipEditor
    {
        protected override void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            base.DrawContents(rect, serializedObject);

            var clipRect = new Rect(rect.x, rect.y + 5f, rect.width, EditorGUIUtility.singleLineHeight);
            var clipProp = serializedObject.FindProperty(AnimateClip.PropNameClip);
            DrawContext(clipRect, clipProp, GUIContent.none, typeof(AnimationClip));
        }
    }
}