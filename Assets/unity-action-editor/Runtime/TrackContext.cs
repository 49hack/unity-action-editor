using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Runtime
{
    public class TrackContext
    {
        struct ClipIndices : System.IEquatable<ClipIndices>
        {
            public int First { get; private set; }
            public int Last { get; private set; }

            public ClipIndices(int first, int last)
            {
                First = first;
                Last = last;
            }

            public bool IsValid()
            {
                if (First < 0)
                    return false;
                if (Last < 0)
                    return false;
                return true;
            }

            public bool Contain(int index)
            {
                if (First == index)
                    return true;
                if (Last == index)
                    return true;
                return false;
            }

            public bool Equals(ClipIndices other)
            {
                if (First != other.First)
                    return false;
                if (Last != other.Last)
                    return false;

                return true;
            }
        }

        TrackBehaviour m_Track;
        ClipContext[] m_ClipContexts;
        ClipIndices m_LatestIndecies = new ClipIndices(-1, -1);
        float m_CurrentTime;

        public TrackContext(TrackBehaviour track, ClipBehaviour[] clips, float frameRate, SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            m_CurrentTime = -1f;
            m_Track = track;
            m_Track.OnCreate(sequence, blackboards);

            m_ClipContexts = new ClipContext[clips.Length];
            for(int i = 0; i < m_ClipContexts.Length; i++)
            {
                m_ClipContexts[i] = clips[i].CreateContext(frameRate, sequence, blackboards);
            }
        }

        public void Play()
        {
            m_Track.OnPlay();
        }
        public void Stop()
        {
            m_Track.OnStop();
        }
        public void Pause()
        {
            m_Track.OnPause();
        }
        public void Resume()
        {
            m_Track.OnResume();
        }
        public void SetTime(float time)
        {
            for (int i = 0; i < m_ClipContexts.Length; i++)
            {
                m_ClipContexts[i].SetTime(time);
            }

            m_Track.SetTime(time);

            m_CurrentTime = time;

            UpdateClip(time);
        }


        public void Progress(float time)
        {
            for (int i = 0; i < m_ClipContexts.Length; i++)
            {
                m_ClipContexts[i].Progress(time);
            }

            var fromTime = m_CurrentTime;
            m_CurrentTime = time;
            m_Track.SetTime(time);
            UpdateClip(time);
            m_Track.OnProgress(fromTime, time);
        }

        void UpdateClip(float time)
        {
            var indeceis = FindCurrentClip(time);
            if(!indeceis.IsValid())
            {
                if (!m_LatestIndecies.Equals(indeceis))
                    m_Track.OnChangeClip(null, 0f, null, 0f);

                m_LatestIndecies = indeceis;
                return;
            }

            if (indeceis.First == indeceis.Last)
            {
                if (!m_LatestIndecies.Equals(indeceis))
                    m_Track.OnChangeClip(m_ClipContexts[indeceis.First].Clip, 1f, null, 0f);
            }
            else
            {
                var min = m_ClipContexts[indeceis.Last].BeginTime;
                var max = m_ClipContexts[indeceis.First].EndTime;
                var weight = Mathf.Clamp01((time - min) / (max - min) + 0.000001f);
                if (!m_LatestIndecies.Equals(indeceis))
                {
                    m_Track.OnChangeClip(m_ClipContexts[indeceis.First].Clip, 1f - weight, m_ClipContexts[indeceis.Last].Clip, weight);
                }
                else
                {
                    m_Track.OnChangeWight(m_ClipContexts[indeceis.First].Clip, 1f - weight, m_ClipContexts[indeceis.Last].Clip, weight);
                }
            }

            m_LatestIndecies = indeceis;
        }

        ClipIndices FindCurrentClip(float time)
        {
            int first = -1;
            for(int i = 0; i < m_ClipContexts.Length; i++)
            {
                var clip = m_ClipContexts[i];
                if(clip.IsPlayingAt(time))
                {
                    first = i;
                    break;
                }
            }

            if(first < 0)
            {
                return new ClipIndices(-1, -1);
            }

            if (first + 1 >= m_ClipContexts.Length)
                return new ClipIndices(first, first);

            var nextClip = m_ClipContexts[first + 1];
            if (nextClip.IsPlayingAt(time))
                return new ClipIndices(first, first + 1);

            return new ClipIndices(first, first);
        }

        public void Dispose()
        {
            m_Track.OnDispose();

            for(int i = 0; i < m_ClipContexts.Length; i++)
            {
                m_ClipContexts[i].Dispose();
            }
        }
    }
}