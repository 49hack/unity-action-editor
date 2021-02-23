using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [CustomClipEditor(typeof(AnimationClipBehaviour))]
    public class AnimationClipBehaviourEditor : ClipBehaviourEditor
    {
        protected override void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            base.DrawContents(rect, serializedObject);

            var clipRect = new Rect(rect.x, rect.y + 5f, rect.width, EditorGUIUtility.singleLineHeight);
            var clipProp = serializedObject.FindProperty(AnimationClipBehaviour.PropNameClip);
            DrawContext(clipRect, clipProp, GUIContent.none, typeof(UnityEngine.AnimationClip));
        }

        protected override void AddContextMenu(GenericMenu menu)
        {
            base.AddContextMenu(menu);

            menu.AddItem(new GUIContent("Adjust time to fit this clip"), false, OnFitTime, null);
        }

        void OnFitTime(object dummy)
        {
            SerializedObject.Update();

            var clipSharedValue = SerializedObject.FindProperty(AnimationClipBehaviour.PropNameClip);
            var clipProp = clipSharedValue.FindPropertyRelative(SharedValueContext.PropNameFixedValue);
            var animClip = clipProp.objectReferenceValue as AnimationClip;
            if (animClip == null)
                return;

            var length = animClip.length;
            var totalFrame = length * FrameRate;
            EndFrame = BeginFrame + totalFrame;

            SerializedObject.ApplyModifiedProperties();
        }
    }
}