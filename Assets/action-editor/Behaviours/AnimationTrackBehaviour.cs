using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace ActionEditor.Sample
{
    public class AnimationTrackBehaviour : TrackBehaviour
    {
        [SerializeField] SharedAnimatorContext m_Animator = new SharedAnimatorContext();

        public static string PropNameAnimator { get { return nameof(m_Animator); } }

        PlayableGraph m_Graph;
        AnimationPlayableOutput m_PlayableOutput;
        AnimationMixerPlayable m_Mixer;

        public override void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            Blackboard.Bind(blackboards, m_Animator);
            Debug.Assert(m_Animator.Value != null, "Animator is null.");

            m_Graph = ((PlayableSequence)sequence).Graph;
            m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            m_PlayableOutput = AnimationPlayableOutput.Create(m_Graph, "Animation", m_Animator.Value);
            m_Mixer = AnimationMixerPlayable.Create(m_Graph, 2, false);

            m_PlayableOutput.SetSourcePlayable(m_Mixer);
        }

        public override void OnChangeClip(ClipBehaviour fromClip, float fromWeight, ClipBehaviour toClip, float toWeight)
        {
            m_Graph.Disconnect(m_Mixer, 0);
            m_Graph.Disconnect(m_Mixer, 1);

            //Debug.LogError(string.Format("fromWeight: {0}, toWeight: {1}", fromWeight, toWeight));
            if(fromClip != null)
            {
                var playable = ((AnimationClipBehaviour)fromClip).Playable;
                m_Mixer.ConnectInput(0, playable, 0, fromWeight);
            }

            if(toClip != null)
            {
                var playable = ((AnimationClipBehaviour)toClip).Playable;
                m_Mixer.ConnectInput(1, playable, 0, toWeight);
            }

            if(m_Graph.GetTimeUpdateMode() == DirectorUpdateMode.Manual || !Application.isPlaying)
            {
                var target = m_PlayableOutput.GetTarget();
                if (target != null)
                    target.Rebind();
            }

            m_Mixer.SetInputWeight(0, fromWeight);
            m_Mixer.SetInputWeight(1, toWeight);
        }

        public override void OnChangeWight(ClipBehaviour fromClip, float fromWeight, ClipBehaviour toClip, float toWeight)
        {
            //Debug.LogError(string.Format("fromWeight: {0}, toWeight: {1}", fromWeight, toWeight));

            m_Mixer.SetInputWeight(0, fromWeight);
            m_Mixer.SetInputWeight(1, toWeight);
        }

        public override void OnDispose()
        {
            if (m_Graph.IsValid())
                m_Graph.Destroy();
        }
    }
}