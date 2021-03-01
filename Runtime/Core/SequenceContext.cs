using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Runtime
{
    public class SequenceContext
    {
        public delegate void ChangeStatus(SequenceStatus status);

        protected SequenceBehaviour m_Sequence;
        protected SequenceStatus m_State;
        protected float m_ElapsedTime;

        TrackContext[] m_TrackContexts;

        public event ChangeStatus OnChangeStatus;

        public SequenceStatus Status { get { return m_State; } }
        public float Current
        {
            get
            {
                return m_ElapsedTime;
            }
            set
            {
                m_ElapsedTime = Mathf.Clamp(value, 0f, m_Sequence.TotalFrame / m_Sequence.FrameRate);
                SetTime(Current);
            }
        }

        public float CurrentFrame
        {
            get
            {
                return m_ElapsedTime * m_Sequence.FrameRate;
            }
            set
            {
                Current = value / m_Sequence.FrameRate;
            }
        }

        public float Length
        {
            get
            {
                return m_Sequence.TotalFrame / m_Sequence.FrameRate;
            }
        }

        public bool IsPlaying { get { return m_State == SequenceStatus.Playing; } }

        public SequenceContext(SequenceBehaviour sequence, TrackBehaviour[] tracks, IReadOnlyList<Blackboard> blackboards)
        {
            SetState(SequenceStatus.Stoppped);

            m_Sequence = sequence;
            m_TrackContexts = new TrackContext[tracks.Length];
            for(int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i] = tracks[i].CreateContext(sequence.FrameRate, sequence, blackboards);
            }
        }

        void SetState(SequenceStatus state)
        {
            m_State = state;
            OnChangeStatus?.Invoke(state);
        }

        public void Play(float time)
        {
            if (m_Sequence == null)
                return;

            Current = time;
            SetState(SequenceStatus.Playing);

            m_Sequence.OnPlay();

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Play();
            }
        }

        public void Stop()
        {
            if (m_Sequence == null)
                return;

            SetState(SequenceStatus.Stoppped);

            m_Sequence.OnStop();

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Stop();
            }
        }

        public void Pause()
        {
            if (m_Sequence == null)
                return;

            SetState(SequenceStatus.Paused);

            m_Sequence.OnPause();

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Pause();
            }
        }

        public void Resume()
        {
            if (m_Sequence == null)
                return;

            SetState(SequenceStatus.Playing);

            m_Sequence.OnResume();

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Resume();
            }
        }

        protected virtual void SetTime(float time)
        {
            if (m_Sequence == null)
                return;

            m_Sequence.OnSetTime(time);

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].SetTime(time);
            }
        }

        public void Tick(float deltaTime)
        {
            if (m_Sequence == null)
                return;

            if (m_State != SequenceStatus.Playing && m_State != SequenceStatus.Interrupted)
            {
                return;
            }

            var prevTime = Current;
            var nextTime = Current + deltaTime;

            m_Sequence.OnProgress(prevTime, nextTime);

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Progress(nextTime);
            }

            Current = nextTime;

            if (Current >= Length)
            {
                Stop();
            }
        }

        public void Interrupt()
        {
            m_Sequence?.OnInterrupt();

            if(m_TrackContexts != null)
            {
                for(int i = 0; i < m_TrackContexts.Length; i++)
                {
                    m_TrackContexts[i].Interrupt();
                }
            }

            SetState(SequenceStatus.Interrupted);
        }

        public void Dispose()
        {
            m_Sequence?.OnDispose();

            if (m_TrackContexts != null)
            {
                for (int i = 0; i < m_TrackContexts.Length; i++)
                {
                    m_TrackContexts[i].Dispose();
                }
            }
        }
    }
}