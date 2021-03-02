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

            var clipRect = new Rect(rect.x, rect.y + 2f, rect.width, EditorGUIUtility.singleLineHeight);
            var clipProp = serializedObject.FindProperty(AnimationClipBehaviour.PropNameClip);
            DrawContext(clipRect, clipProp, GUIContent.none, typeof(UnityEngine.AnimationClip));

            using (new Utility.LabelWidthScope(Mathf.Min(rect.width * 0.3f, 50f)))
            {
                var speedRect = new Rect(rect.x, rect.y + clipRect.height + 8f, rect.width * 0.5f - 2f, EditorGUIUtility.singleLineHeight);
                var speedProp = serializedObject.FindProperty(AnimationClipBehaviour.PropNameSpeed);
                EditorGUI.PropertyField(speedRect, speedProp);

                var offsetRect = new Rect(rect.x + speedRect.width + 4f, rect.y + clipRect.height + 8f, rect.width * 0.5f - 2f, EditorGUIUtility.singleLineHeight);
                var offsetProp = serializedObject.FindProperty(AnimationClipBehaviour.PropNameOffset);
                EditorGUI.PropertyField(offsetRect, offsetProp);
            }
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

            var speedProp = SerializedObject.FindProperty(AnimationClipBehaviour.PropNameSpeed);
            if (speedProp == null)
                return;

            var offsetProp = SerializedObject.FindProperty(AnimationClipBehaviour.PropNameOffset);
            if (offsetProp == null)
                return;

            var length = (animClip.length * (1f / speedProp.floatValue)) - offsetProp.floatValue;
            var totalFrame = length * FrameRate;
            EndFrame = BeginFrame + totalFrame;

            SerializedObject.ApplyModifiedProperties();
        }
    }
}