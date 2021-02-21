using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace ActionEditor
{
    public class AnimationTrackBehaviour : TrackBehaviour
    {
        AnimationMixerPlayable m_Mixer;

        public override void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            m_Mixer = ((PlayableSequence)sequence).ChildMixer;
        }

        public override void OnChangeClip(ClipBehaviour fromClip, float fromWeight, ClipBehaviour toClip, float toWeight)
        {
            m_Mixer.DisconnectInput(0);
            m_Mixer.DisconnectInput(1);

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

            m_Mixer.SetInputWeight(0, fromWeight);
            m_Mixer.SetInputWeight(1, toWeight);
        }

        public override void OnChangeWight(ClipBehaviour fromClip, float fromWeight, ClipBehaviour toClip, float toWeight)
        {
            m_Mixer.SetInputWeight(0, fromWeight);
            m_Mixer.SetInputWeight(1, toWeight);
        }

        public override void OnDispose()
        {
        }
    }
}