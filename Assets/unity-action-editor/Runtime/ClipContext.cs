using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Runtime
{
    public class ClipContext
    {
        public float BeginTime { get { return m_Clip.BeginFrame / m_FrameRate; } }
        public float EndTime { get { return m_Clip.EndFrame / m_FrameRate; } }
        public bool IsPlaying { get { return IsPlayingAt(m_CurrentTime); } }
        public Clip Clip { get { return m_Clip; } }
        Clip m_Clip;
        float m_CurrentTime;
        float m_FrameRate;
        bool m_IsBegined = false;

        public ClipContext(Clip clip, float frameRate, Sequence sequence, IReadOnlyList<Blackboard> blackboards)
        {
            m_Clip = clip;
            m_Clip.OnCreate(sequence, blackboards);
            m_FrameRate = frameRate;
        }

        public bool IsPlayingAt(float time)
        {
            return time >= BeginTime && time <= EndTime;
        }

        public void SetTime(float time)
        {
            var isPlayingAt = IsPlayingAt(time);

            if (isPlayingAt && !m_IsBegined)
            {
                Begin();
            }
            if (!isPlayingAt && m_IsBegined)
            {
                End();
            }

            m_CurrentTime = time;

            if (isPlayingAt)
                m_Clip.OnSetTime(time);
        }

        public void Begin()
        {
            m_IsBegined = true;
            m_Clip.OnBegin();
        }

        public void End()
        {
            m_IsBegined = false;
            m_Clip.OnEnd();
        }
        public void Progress(float toTime)
        {
            var fromTime = m_CurrentTime;

            SetTime(toTime);

            if (!IsPlaying)
                return;

            m_Clip.OnProgress(fromTime, toTime);
        }
        public void Dispose()
        {
            m_Clip.OnDispose();
        }
    }
}