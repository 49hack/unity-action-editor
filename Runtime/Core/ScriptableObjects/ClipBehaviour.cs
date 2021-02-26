using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace ActionEditor
{
    public abstract class ClipBehaviour : ScriptableObject
    {
        [SerializeField, HideInInspector] float m_BeginFrame;
        [SerializeField, HideInInspector] float m_EndFrame;

        public static string PropNameBeginFrame { get { return nameof(m_BeginFrame); } }
        public static string PropNameEndFrame { get { return nameof(m_EndFrame); } }

        public float BeginFrame { get { return m_BeginFrame; } }
        public float EndFrame { get { return m_EndFrame; } }

        public Runtime.ClipContext CreateContext(float frameRate, SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            return new Runtime.ClipContext(this, frameRate, sequence, blackboards);
        }

        public void PostCreate(float beginFrame)
        {
            m_BeginFrame = beginFrame;
            m_EndFrame = m_BeginFrame + 10f;
        }

        public virtual void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards) { }
        public virtual void OnInterrupt(float clipTime, float absoluteTime, float duration) { }
        public virtual void OnSetTime(float time, float duration) { }
        public virtual void OnBegin(float time, float absoluteTime, float duration) { }
        public virtual void OnEnd(float time, float absoluteTime, float duration) { }
        public virtual void OnProgress(float fromTime, float toTime, float duration) { }
        public virtual void OnDispose() { }
    }
}