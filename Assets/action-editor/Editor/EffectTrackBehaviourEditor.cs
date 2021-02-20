using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [System.Serializable]
    [CustomTrackEditor(typeof(EffectTrackBehaviour))]
    public class EffectTrackBehaviourEditor : TrackBehaviourEditor
    {
        protected override Color BackgroundColor { get { return new Color(0f, 0f, 0f, 0.5f); } }


        protected override void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            //var propName = serializedObject.FindProperty(Track.PropNameTrackName);
            //var nameRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, EditorGUIUtility.singleLineHeight);
            //propName.stringValue = EditorGUI.TextField(nameRect, propName.stringValue);

            var effectRect = new Rect(rect.x + 2f, rect.y + 6f, rect.width - 4f, EditorGUIUtility.singleLineHeight);
            var effectProp = serializedObject.FindProperty(EffectTrackBehaviour.PropNameEffect);
            DrawContext(effectRect, effectProp, new GUIContent("Effect"), typeof(GameObject));

            var locatorRect = new Rect(effectRect.x, effectRect.y + effectRect.height + 5f, effectRect.width, EditorGUIUtility.singleLineHeight);
            var locatorProp = serializedObject.FindProperty(EffectTrackBehaviour.PropNameLocator);
            DrawContext(locatorRect, locatorProp, new GUIContent("Locator"), typeof(Transform));
        }
    }
}