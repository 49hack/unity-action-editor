using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    using Runtime;

    public class EditorDirector : IDirector
    {
        SequenceContext m_Context;
        SequenceBehaviour m_Sequence;

        public static EditorDirector Create(SequenceBehaviour sequence)
        {
            var director = new EditorDirector();
            director.m_Sequence = sequence;
            return director;
        }

        public SequenceStatus Status { get { return m_Context == null ? SequenceStatus.Initial : m_Context.Status; } }
        public SequenceBehaviour Sequence { get { return m_Sequence; } set { m_Sequence = value; } }
        public IReadOnlyList<Blackboard> Blackboard { get { return null; } }
        public float CurrentTime { get { return m_Context == null ? 0f : m_Context.Current; } set { if (m_Context == null) return; m_Context.Current = value; } }
        public float CurrentFrame { get { return m_Context == null ? 0f : m_Context.CurrentFrame; } set { if (m_Context == null) return; m_Context.CurrentFrame = value; } }
        public float Length { get { return m_Context == null ? 0f : m_Context.Length; } }
        public float TotalFrame { get { return m_Sequence == null ? 0f : m_Sequence.TotalFrame; } }

        public SequenceContext Prepare(SequenceBehaviour sequence = null, TickMode mode = TickMode.Auto)
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
                return null;

            m_Context = m_Sequence.CreateContext(Blackboard);
            return m_Context;
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