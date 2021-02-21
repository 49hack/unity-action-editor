using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace ActionEditor
{
    [ParentTrack(typeof(AnimationTrackBehaviour))]
    public class AnimationClipBehaviour : ClipBehaviour
    {
        public static string PropNameClip { get { return nameof(m_Clip); } }

        [SerializeField] SharedAnimationClipContext m_Clip;

        PlayableGraph m_Graph;
        AnimationClipPlayable m_Playable;

        public AnimationClipPlayable Playable { get { return m_Playable; } }

        public override void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            m_Graph = ((PlayableSequence)sequence).Graph;
            Blackboard.Bind(blackboards, m_Clip);
        }

        public override void OnBegin(float time, float absoluteTime)
        {
            m_Playable = AnimationClipPlayable.Create(m_Graph, m_Clip.Value);
            m_Playable.SetTime(0f);
        }

        public override void OnEnd(float time, float absoluteTime)
        {
            if (m_Playable.IsValid())
                m_Playable.Destroy();
        }

        public override void OnSetTime(float time)
        {
            if (m_Playable.IsValid())
                m_Playable.SetTime(time);
        }

#if UNITY_EDITOR
        public override void OnProgress(float fromTime, float toTime)
        {
            if (!Application.isPlaying)
                OnSetTime(toTime);
        }
#endif
    }
}