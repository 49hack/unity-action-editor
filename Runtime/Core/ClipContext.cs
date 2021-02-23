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
        public ClipBehaviour Clip { get { return m_Clip; } }
        ClipBehaviour m_Clip;
        float m_CurrentTime;
        float m_FrameRate;
        bool m_IsBegined = false;

        public ClipContext(ClipBehaviour clip, float frameRate, SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
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

            m_CurrentTime = time;

            if (isPlayingAt && !m_IsBegined)
            {
                Begin();
            }
            if (!isPlayingAt && m_IsBegined)
            {
                End();
            }

            var clipTime = Mathf.Clamp(time - BeginTime, 0f, EndTime - BeginTime);
            if (isPlayingAt)
                m_Clip.OnSetTime(clipTime);
        }

        public void Begin()
        {
            var clipTime = Mathf.Clamp(m_CurrentTime - BeginTime, 0f, EndTime - BeginTime);
            m_IsBegined = true;
            m_Clip.OnBegin(clipTime, m_CurrentTime);
        }

        public void End()
        {
            var clipTime = Mathf.Clamp(m_CurrentTime - BeginTime, 0f, EndTime - BeginTime);
            m_IsBegined = false;
            m_Clip.OnEnd(clipTime, m_CurrentTime);
        }
        public void Progress(float toTime)
        {
            var fromTime = m_CurrentTime;

            SetTime(toTime);

            if (!IsPlaying)
                return;

            var clipFromTime = Mathf.Clamp(fromTime - BeginTime, 0f, EndTime - BeginTime);
            var clipToTime = Mathf.Clamp(toTime - BeginTime, 0f, EndTime - BeginTime);
            m_Clip.OnProgress(clipFromTime, clipToTime);
        }

        public void Interrupt()
        {
            var clipTime = Mathf.Clamp(m_CurrentTime - BeginTime, 0f, EndTime - BeginTime);
            m_Clip?.OnInterrupt(clipTime, BeginTime);
        }

        public void Dispose()
        {
            m_Clip.OnDispose();
        }
    }
}