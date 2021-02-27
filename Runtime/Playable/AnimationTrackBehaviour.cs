using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace ActionEditor
{
    [ParentSequence(typeof(PlayableSequence))]
    [MenuTitle("Playable/Animation")]
    public class AnimationTrackBehaviour : TrackBehaviour
    {
        public static string PropNameAvatarMask { get { return nameof(m_AvatarMask); } }

        [SerializeField] SharedAvatarMaskContext m_AvatarMask = new SharedAvatarMaskContext();

        AnimationLayerMixerPlayable m_Mixer;
        

        public override void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            Blackboard.Bind(blackboards, m_AvatarMask);

            var playableSequence = (PlayableSequence)sequence;
            m_Mixer = playableSequence.ChildMixer;

            // マスクが設定されていればアニメーターも動かす
            playableSequence.AnimatorWeight = m_AvatarMask.Value == null ? 0f : 1f;
            playableSequence.AvatarMask = m_AvatarMask.Value;
        }

        public override void OnChangeClip(ClipBehaviour fromClip, float fromWeight, ClipBehaviour toClip, float toWeight)
        {
            if (!m_Mixer.IsValid())
                return;

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
            if (!m_Mixer.IsValid())
                return;

            m_Mixer.SetInputWeight(0, fromWeight);
            m_Mixer.SetInputWeight(1, toWeight);
        }

        public override void OnDispose()
        {
        }
    }
}