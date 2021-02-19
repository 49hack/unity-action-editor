using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Runtime
{
    
    public class Director : MonoBehaviour, IDirector
    {
        public static string PropNameBlackboardList { get { return nameof(m_BlackboardList); } }

        [SerializeField] SequenceBehaviour m_Sequence;
        [SerializeField] TickMode m_Mode;
        [SerializeField] bool m_PlayOnAwake; 
        [SerializeField,HideInInspector] List<Blackboard> m_BlackboardList = new List<Blackboard>();

        SequenceContext m_Context;

        public Status Status { get { return m_Context == null ? Status.Initial : m_Context.Status; } }
        public SequenceBehaviour Sequence { get { return m_Sequence; } }
        public IReadOnlyList<Blackboard> Blackboard { get { return m_BlackboardList; } }
        public float CurrentTime { get { return m_Context == null ? 0f : m_Context.Current; } set { if (m_Context == null) return; m_Context.Current = value; } }
        public float CurrentFrame { get { return m_Context == null ? 0f : m_Context.CurrentFrame; } set { if (m_Context == null) return; m_Context.CurrentFrame = value; } }
        public float Length { get { return m_Context == null ? 0f : m_Context.Length; } }
        public float TotalFrame { get { return m_Sequence == null ? 0f : m_Sequence.TotalFrame; } }

        void Awake()
        {
            if(m_PlayOnAwake)
            {
                Prepare(mode: TickMode.Auto);
                Play(0f);
            }
        }

        public void Prepare(SequenceBehaviour sequence = null, TickMode mode = TickMode.Auto)
        {
            m_Mode = mode;

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

            m_Context = m_Sequence.CreateContext(Blackboard);
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