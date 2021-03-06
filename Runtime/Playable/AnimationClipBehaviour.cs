﻿using System.Collections;
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
        public static string PropNameSpeed { get { return nameof(m_Speed); } }
        public static string PropNameOffset { get { return nameof(m_Offset); } }

        [SerializeField] SharedAnimationClipContext m_Clip;
        [SerializeField] float m_Speed = 1f;
        [SerializeField] float m_Offset = 0f;

        PlayableGraph m_Graph;
        AnimationClipPlayable m_Playable;

        public AnimationClipPlayable Playable { get { return m_Playable; } }

        public override void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            m_Graph = ((PlayableSequence)sequence).Graph;
            Blackboard.Bind(blackboards, m_Clip);
        }

        public override void OnBegin(float time, float absoluteTime, float duration)
        {
            if (!m_Graph.IsValid())
                return;

            m_Playable = AnimationClipPlayable.Create(m_Graph, m_Clip.Value);
            m_Playable.SetTime(m_Offset);
        }

        public override void OnEnd(float time, float absoluteTime, float duration)
        {
            if (m_Playable.IsValid())
                m_Playable.Destroy();
        }

        public override void OnSetTime(float time, float duration)
        {
            if (m_Playable.IsValid())
            {
                m_Playable.SetTime(m_Offset + time * m_Speed);
            }
        }

#if UNITY_EDITOR
        public override void OnProgress(float fromTime, float toTime, float duration)
        {
            if (!Application.isPlaying)
                OnSetTime(toTime, duration);
        }
#endif
    }
}