using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace ActionEditor.Sample
{
    [ParentTrack(typeof(AnimateTrack))]
    public class AnimateClip : Clip
    {
        public static string PropNameClip { get { return nameof(m_Clip); } }

        [SerializeField] AnimationClip m_Clip;

        PlayableGraph m_Graph;
        AnimationClipPlayable m_Playable;

        public AnimationClipPlayable Playable { get { return m_Playable; } }

        public override void OnCreate(Sequence sequence, IBindingProvider bindingProvider)
        {
            m_Graph = ((PlayableSequence)sequence).Graph;
        }

        public override void OnBegin()
        {
            m_Playable = AnimationClipPlayable.Create(m_Graph, m_Clip);
            m_Playable.SetTime(0f);
        }

        public override void OnEnd()
        {
            if (m_Playable.IsValid())
                m_Playable.Destroy();
        }

        public override void OnSetTime(float time)
        {
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