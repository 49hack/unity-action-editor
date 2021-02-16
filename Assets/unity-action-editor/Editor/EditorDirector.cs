﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    using Runtime;

    public class EditorBindingHolder : IBindingProvider
    {
        public bool IsEnable { get { return false; } }

        public UnityEngine.Object Find(string key, System.Type type, int index)
        {
            return null;
        }

        public bool ToSerializeData(UnityEngine.Object obj, out (string key, int index) result)
        {
            result = ("", 0);
            return false;
        }
    }


    public class EditorDirector : IDirector
    {
        SequenceContext m_Context;
        Sequence m_Sequence;
        EditorBindingHolder m_BindingHolder = new EditorBindingHolder();

        public static EditorDirector Create(Sequence sequence)
        {
            var director = new EditorDirector();
            director.m_Sequence = sequence;
            return director;
        }

        public Status Status { get { return m_Context == null ? Status.Stoppped : m_Context.Status; } }
        public Sequence Sequence { get { return m_Sequence; } }
        public IBindingProvider BindingProvider { get { return m_BindingHolder; } }
        public float CurrentTime { get { return m_Context == null ? 0f : m_Context.Current; } set { if (m_Context == null) return; m_Context.Current = value; } }
        public float CurrentFrame { get { return m_Context == null ? 0f : m_Context.CurrentFrame; } set { if (m_Context == null) return; m_Context.CurrentFrame = value; } }
        public float Length { get { return m_Context == null ? 0f : m_Context.Length; } }
        public float TotalFrame { get { return m_Sequence == null ? 0f : m_Sequence.TotalFrame; } }

        public void Prepare(Sequence sequence = null, TickMode mode = TickMode.Auto)
        {
            if (m_Context != null)
            {
                m_Context.Dispose();
                m_Context = null;
            }

            if (sequence != null)
            {
                m_Sequence = sequence;
            }

            if (m_Sequence == null)
                return;

            m_Context = m_Sequence.CreateContext(m_BindingHolder);
        }

        public void Play(float? time = null)
        {
            m_Context?.Play(time == null ? m_Context.Current : time.Value);
        }

        public void Stop()
        {
            m_Context?.Stop();
        }

        public void Pause()
        {
            m_Context?.Pause();
        }

        public void Resume()
        {
            m_Context?.Resume();
        }

        public void Tick(float deltaTime)
        {
            m_Context?.Tick(deltaTime);
        }

        public void Dispose()
        {
            m_Context?.Dispose();
            m_Context = null;
        }
    }
}