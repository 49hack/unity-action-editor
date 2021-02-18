﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class Track : ScriptableObject
    {
        [SerializeField] string m_TrackName;

        [SerializeField] Clip[] m_Clips = new Clip[0];

        public static string PropNameTrackName { get { return nameof(m_TrackName); } }
        public static string PropNameClips { get { return nameof(m_Clips); } }

        public Runtime.TrackContext CreateContext(float frameRate, Sequence sequence, IReadOnlyList<Blackboard> blackboards)
        {
            return new Runtime.TrackContext(this, m_Clips, frameRate, sequence, blackboards);
        }

        public virtual void OnCreate(Sequence sequence, IReadOnlyList<Blackboard> blackboards) { }
        public virtual void SetTime(float time) { }
        public virtual void OnPlay() { }
        public virtual void OnStop() { }
        public virtual void OnPause() { }
        public virtual void OnResume() { }
        public virtual void OnProgress(float fromTime, float toTime) { }
        public virtual void OnChangeClip(Clip fromClip, float fromWeight, Clip toClip, float toWeight) { }
        public virtual void OnChangeWight(Clip fromClip, float fromWeight, Clip toClip, float toWeight) { }
        public virtual void OnDispose() { }
    }
}
