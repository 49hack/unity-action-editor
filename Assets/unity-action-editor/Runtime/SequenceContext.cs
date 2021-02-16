using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Runtime
{
    public class SequenceContext
    {
        Sequence m_Sequence;
        TrackContext[] m_TrackContexts;
        Status m_State;
        float m_ElapsedTime;

        public Status Status { get { return m_State; } }
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

        public bool IsPlaying { get { return m_State == Status.Playing; } }

        public SequenceContext(Sequence sequence, Track[] tracks, IBindingProvider bindingProvider)
        {
            SetState(Status.Stoppped);

            m_Sequence = sequence;
            m_TrackContexts = new TrackContext[tracks.Length];
            for(int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i] = tracks[i].CreateContext(sequence.FrameRate, sequence, bindingProvider);
            }
        }

        void SetState(Status state)
        {
            m_State = state;
        }

        public void Play(float time)
        {
            if (m_Sequence == null)
                return;

            Current = time;
            SetState(Status.Playing);

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

            SetState(Status.Stoppped);

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

            SetState(Status.Paused);

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

            SetState(Status.Playing);

            m_Sequence.OnResume();

            for (int i = 0; i < m_TrackContexts.Length; i++)
            {
                m_TrackContexts[i].Resume();
            }
        }

        void SetTime(float time)
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

            if (m_State != Status.Playing)
            {
                return;
            }

            var prevTime = Current;
            Current += deltaTime;

            m_Sequence.OnSetTime(Current);
            m_Sequence.OnProgress(prevTime, Current);

            for (int i = 0; i < m_TrackContexts.Length; i++)
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