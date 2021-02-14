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

        public ClipContext(Clip clip, float frameRate)
        {
            m_Clip = clip;
            m_FrameRate = frameRate;
        }

        public bool IsPlayingAt(float time)
        {
            return time >= BeginTime && time <= EndTime;
        }

        public void OnSetTime(float time)
        {
            if (IsPlayingAt(time) && !IsPlaying)
            {
                OnBegin();
            }
            if (!IsPlayingAt(time) && IsPlaying)
            {
                OnEnd();
            }

            m_CurrentTime = time;
            m_Clip.OnSetTime(time);
        }

        public void OnBegin()
        {
            m_Clip.OnBegin();
        }

        public void OnEnd()
        {
            m_Clip.OnEnd();
        }
        public void OnProgress(float toTime)
        {
            if(IsPlayingAt(toTime) && !IsPlaying)
            {
                OnBegin();
            }
            if(!IsPlayingAt(toTime) && IsPlaying)
            {
                OnEnd();
            }

            var fromTime = m_CurrentTime;
            m_CurrentTime = toTime;

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