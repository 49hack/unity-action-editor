using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class TrackBehaviour : ScriptableObject
    {
        [SerializeField] string m_TrackName;

        [SerializeField] ClipBehaviour[] m_Clips = new ClipBehaviour[0];

        public static string PropNameTrackName { get { return nameof(m_TrackName); } }
        public static string PropNameClips { get { return nameof(m_Clips); } }

        public Runtime.TrackContext CreateContext(float frameRate, SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            return new Runtime.TrackContext(this, m_Clips, frameRate, sequence, blackboards);
        }

        public virtual void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards) { }
        public virtual void SetTime(float time) { }
        public virtual void OnPlay() { }
        public virtual void OnStop() { }
        public virtual void OnPause() { }
        public virtual void OnResume() { }
        public virtual void OnProgress(float fromTime, float toTime) { }
        public virtual void OnChangeClip(ClipBehaviour fromClip, float fromWeight, ClipBehaviour toClip, float toWeight) { }
        public virtual void OnChangeWight(ClipBehaviour fromClip, float fromWeight, ClipBehaviour toClip, float toWeight) { }
        public virtual void OnDispose() { }
    }
}
