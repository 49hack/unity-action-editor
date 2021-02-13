using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class Clip : ScriptableObject
    {
        [SerializeField, HideInInspector] float m_BeginFrame;
        [SerializeField, HideInInspector] float m_EndFrame;

        public static string PropNameBeginFrame { get { return nameof(m_BeginFrame); } }
        public static string PropNameEndFrame { get { return nameof(m_EndFrame); } }

        public float BeginFrame { get { return m_BeginFrame; } }
        public float EndFrame { get { return m_EndFrame; } }

        public void PostCreate(float beginFrame)
        {
            m_BeginFrame = beginFrame;
            m_EndFrame = m_BeginFrame + 10f;
        }
    }
}