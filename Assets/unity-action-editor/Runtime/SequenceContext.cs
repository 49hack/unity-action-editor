using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Runtime
{
    public class SequenceContext
    {
        enum Status
        {
            Stoppped,
            Playing,
            Paused,
        }
        Sequence m_Sequence;
        TrackContext[] m_TrackContexts;
        Status m_State;
        float m_ElapsedTime;

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

        public float Length
        {
            get
            {
                return m_Sequence.TotalFrame / m_Sequence.FrameRate;
            }
        }

        public bool IsPlaying { get { return m_State == Status.Playing; } }

        public SequenceContext(Sequence sequence, Track[] tracks)
        {
            SetState(Status.Stoppped);

            m_Sequence = sequence;
            m_TrackContexts = new TrackContext[tracks.Length];
            for(int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i] = tracks[i].CreateContext(sequence.FrameRate);
            }
        }

        void SetState(Status state)
        {
            m_State = state;
        }

        public void Play(float time)
        {
            Current = time;
            SetState(Status.Playing);

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Play();
            }
        }

        public void Stop()
        {
            SetState(Status.Stoppped);

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Stop();
            }
        }

        public void Pause()
        {
            SetState(Status.Paused);

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Pause();
            }
        }

        public void Resume()
        {
            SetState(Status.Playing);

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Resume();
            }
        }

        void SetTime(float time)
        {
            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].SetTime(time);
            }
        }

        public void Tick(float deltaTime)
        {
            if(m_State != Status.Playing)
            {
                return;
            }

            Current += deltaTime;

            for(int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Progress(Current);
            }

            if(Current >= Length)
            {
                Stop();
            }
        }

        public void Dispose()
        {
            m_Sequence.OnDispose();
            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Dispose();
            }
        }
    }
}