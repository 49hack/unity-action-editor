using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [CustomClipEditor(typeof(EffectClipBehaviour))]
    public class EffectClipBehaviourEditor : ClipBehaviourEditor
    {
        protected override void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            var labelWidth = Mathf.Min(rect.width * 2f, 30f);
            using (new Utility.LabelWidthScope(labelWidth))
            {
                var scaleRect = new Rect(rect.x, rect.y - 4f, rect.width, EditorGUIUtility.singleLineHeight);
                var scaleProp = serializedObject.FindProperty(EffectClipBehaviour.PropNameScale);
                EditorGUI.PropertyField(scaleRect, scaleProp);
            }
        }
    }
}