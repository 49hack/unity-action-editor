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
            var effectRect = new Rect(rect.x + 2f, rect.y + 6f, rect.width - 4f, EditorGUIUtility.singleLineHeight);
            var effectProp = serializedObject.FindProperty(EffectTrackBehaviour.PropNameEffect);
            DrawContext(effectRect, effectProp, new GUIContent("Effect"), typeof(GameObject));

            var locatorRect = new Rect(effectRect.x, effectRect.y + effectRect.height + 5f, effectRect.width, EditorGUIUtility.singleLineHeight);

            var toggleRect = new Rect(locatorRect.x, locatorRect.y, EditorGUIUtility.singleLineHeight, locatorRect.height);
            var toggleProp = serializedObject.FindProperty(EffectTrackBehaviour.PropNameHasLocator);
            toggleProp.boolValue = EditorGUI.ToggleLeft(toggleRect, "", toggleProp.boolValue);
            if(toggleProp.boolValue)
            {
                var itemWidth = (locatorRect.width - toggleRect.width);
                var selectRect = new Rect(locatorRect.x + toggleRect.width, locatorRect.y, itemWidth * 0.75f, locatorRect.height);
                var locatorProp = serializedObject.FindProperty(EffectTrackBehaviour.PropNameLocatorArray);
                DrawContext(selectRect, locatorProp, GUIContent.none, typeof(Transform[]));

                var indexRect = new Rect(selectRect.x + selectRect.width, selectRect.y, itemWidth * 0.25f, locatorRect.height);
                var indexProp = serializedObject.FindProperty(EffectTrackBehaviour.PropNameLocatorIndex);
                indexProp.intValue = EditorGUI.IntField(indexRect, "", indexProp.intValue);
            }
        }
    }
}