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

        BlendableAnimator m_Animator;

        public PlayableGraph Graph { get { return m_Animator.Graph; } }
        public AnimationMixerPlayable ChildMixer { get { return m_Animator.SequenceMixer; } }

        public override Runtime.SequenceContext CreateContext(IReadOnlyList<Blackboard> blackboards)
        {
            this.OnCreate(blackboards);

            return new Runtime.PlayableSequenceContext(this, m_Tracks, blackboards);
        }

        public override void OnCreate(IReadOnlyList<Blackboard> blackboards)
        {
            Blackboard.Bind(blackboards, m_BlendableAnimator);

            if (m_BlendableAnimator.Value == null)
                return;

            var animator = m_BlendableAnimator.Value;
            var initialized = animator.IsInitialized;
            if (!initialized)
                animator.Initialize();

            m_Animator = animator;

            if(!initialized)
            {
                animator.SetSequenceWeight(1f);
            }
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