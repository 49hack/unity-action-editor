using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Sample
{
    [ParentTrack(typeof(FloatTrack))]
    [MenuTitle("Primitive/Float Clip")]
    public class FloatClip : Clip
    {
        [SerializeField] float m_Value;
        [SerializeField] SharedFloatContext m_SharedValue;

        public static string PropNameValue { get { return nameof(m_Value); } }
        public static string PropNameSharedValue { get { return nameof(m_SharedValue); } }

        public float Value { get { return m_SharedValue.Value; } }
        public bool IsValid { get { return m_SharedValue.HasBlackboard; } }
    }
}