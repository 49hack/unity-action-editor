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

        public static string PropNameValue { get { return nameof(m_Value); } }

        public float Value { get { return m_Value; } }
    }
}