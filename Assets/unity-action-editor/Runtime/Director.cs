using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Runtime
{
    [ExecuteInEditMode]
    public class Director : MonoBehaviour, IDirector
    {
        [SerializeField] Sequence m_Sequence;
        [SerializeField] TickMode m_Mode;
        [SerializeField] BindingHolder m_BindingHolder;

        SequenceContext m_Context;

        public Status Status { get { return m_Context == null ? Status.Stoppped : m_Context.Status; } }
        public Sequence Sequence { get { return m_Sequence; } }
        public IBindingProvider BindingProvider { get { return m_BindingHolder; } }
        public float CurrentTime { get { return m_Context == null ? 0f : m_Context.Current; } set { if (m_Context == null) return; m_Context.Current = value; } }
        public float CurrentFrame { get { return m_Context == null ? 0f : m_Context.CurrentFrame; } set { if (m_Context == null) return; m_Context.CurrentFrame = value; } }
        public float Length { get { return m_Context == null ? 0f : m_Context.Length; } }
        public float TotalFrame { get { return m_Sequence == null ? 0f : m_Sequence.TotalFrame; } }


        public void Prepare(Sequence sequence = null)
        {
            if (sequence != null)
            {
                m_Sequence = sequence;
            }

            if (m_Context != null)
            {
                m_Context.Dispose();
                m_Context = null;
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

        void Update()
        {
            if (m_Mode != TickMode.Manual)
                Tick(Time.deltaTime);
        }

        public void Dispose()
        {
            m_Context?.Dispose();
            m_Context = null;
        }

        void OnDestroy()
        {
            Dispose();
        }
    }
}