using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace ActionEditor
{
    using Runtime;

    [CreateAssetMenu(menuName = "Action Editor/Playable Sequence", fileName = "PlayableSequence")]
    public class PlayableSequence : SequenceBehaviour
    {
        [SerializeField] SharedBlendableAnimatorContext m_BlendableAnimator;

        PlayableAnimator m_Animator;
        PlayableGraph m_Graph;
        AnimationLayerMixerPlayable m_ChildMixer;

        public PlayableGraph Graph { get { return m_Graph; } }
        public AnimationLayerMixerPlayable ChildMixer { get { return m_ChildMixer; } }
        internal float AnimatorWeight { get; set; }
        internal AvatarMask AvatarMask { get; set; }

        public override Runtime.SequenceContext CreateContext(IReadOnlyList<Blackboard> blackboards)
        {
            this.OnCreate(blackboards);

            var ctx = new Runtime.PlayableSequenceContext(this, m_Tracks, blackboards);

            if (m_Animator != null)
            {
                m_Animator.SetSequenceAvatarMask(AvatarMask);

                if (!Application.isPlaying)
                {
                    m_Animator.SetAnimatorWeight(AnimatorWeight);
                    m_Animator.SetSequenceWeight(1f);
                }
            }

            return ctx;
        }

        public override void OnCreate(IReadOnlyList<Blackboard> blackboards)
        {
            Blackboard.Bind(blackboards, m_BlendableAnimator);

            if (m_BlendableAnimator.Value == null)
                return;

            var animator = m_BlendableAnimator.Value;
            animator.Initialize();

            m_Animator = animator;
            m_Graph = m_Animator.Graph;
            m_ChildMixer = m_Animator.SequenceMixer;
        }

        public override void OnDispose()
        {
        }
        private void OnDestroy()
        {
            OnDispose();
        }

        bool CanOperation()
        {
            if (m_Animator == null)
                return false;

            // in a runtime, all operating is done in BlendableAnimator.
            if (Application.isPlaying)
                return false;

            return true;
        }

        public override void OnSetTime(float time)
        {
            if(CanOperation())
                m_Animator.Evaluate();
        }

        public override void OnPlay()
        {
            if (CanOperation())
                m_Animator.Play();
        }

        public override void OnStop()
        {
            if (CanOperation())
                m_Animator.Stop();
        }
        public override void OnPause()
        {
            if (CanOperation())
                m_Animator.Pause();
        }
        public override void OnResume()
        {
            if (CanOperation())
                m_Animator.Resume();
        }

        public override void OnProgress(float fromTime, float toTime)
        {
            if (CanOperation())
                m_Animator.Evaluate(toTime - fromTime);
        }
    }

}