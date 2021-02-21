using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace ActionEditor
{
    using Runtime;

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Director))]
    [RequireComponent(typeof(PlayableAnimatorBlackboard))]
    public class PlayableAnimator : MonoBehaviour
    {
        [SerializeField] RuntimeAnimatorController m_AnimatorController;

        Animator m_Animator;
        Director m_Director;
        PlayableAnimatorBlackboard m_Blackboard;
        AnimationGraph m_GraphController;

        AnimationGraph.Handler m_AnimatorHandler;
        AnimationGraph.Handler m_SequenceHandler;

        Coroutine m_SequenceFadeCoroutine;
        Context m_CurrentSequence;

        public bool IsInitialized { get { return m_GraphController != null; } }

        public Animator Animator
        {
            get
            {
                if (m_Animator == null)
                    m_Animator = GetComponent<Animator>();
                return m_Animator;
            }
        }
        Director Director
        {
            get
            {
                if (m_Director == null)
                    m_Director = GetComponent<Director>();
                return m_Director;
            }
        }

        PlayableAnimatorBlackboard Blackboard
        {
            get
            {
                if (m_Blackboard == null)
                    m_Blackboard = GetComponent<PlayableAnimatorBlackboard>();
                return m_Blackboard;
            }
        }
        public bool CanInterrupt { get { return !Blackboard.InterruptBlock.Blocked; } }
        public RestartData RestartData { get { return Blackboard.RestartData; } }
        public PlayableGraph Graph { get { return m_GraphController.Graph; } }
        public AnimationMixerPlayable SequenceMixer { get { return m_SequenceHandler.Mixer; } }

        private void OnValidate()
        {
            Blackboard.Bind(this);
            Director.AddBlackboard(Blackboard);
        }

        private void OnDisable()
        {
            Dispose();
        }

        private void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            Dispose();

            Blackboard.Bind(this);
            Director.AddBlackboard(Blackboard);

            m_GraphController = new AnimationGraph(name, Animator, 2);

            m_SequenceHandler = m_GraphController.CreateMixer(2);
            m_SequenceHandler.Weight = 1f;
            if (Application.isPlaying)
            {
                m_AnimatorHandler = m_GraphController.Add(m_AnimatorController);
                m_AnimatorHandler.Weight = 1f;
            }
        }

        public void Evaluate()
        {
            m_GraphController?.Evaluate();
        }

        public void Evaluate(float time)
        {
            m_GraphController?.Evaluate(time);
        }

        public void Play()
        {
            m_GraphController?.Play();
        }

        public void Stop()
        {
            m_GraphController?.Stop();
        }
        public void Pause()
        {
            m_GraphController?.Pause();
        }
        public void Resume()
        {
            m_GraphController?.Resume();
        }

        public void Dispose()
        {
            m_SequenceHandler?.Dispose();
            m_SequenceHandler = null;
            m_AnimatorHandler?.Dispose();
            m_AnimatorHandler = null;

            m_GraphController?.Dispose();
            m_GraphController = null;
        }

        public void SetSequenceWeight(float weight)
        {
            weight = Mathf.Clamp01(weight);
            m_GraphController.SetRootWeight(1f - weight);
        }

        public void SetAnimatorWeight(float weight)
        {
            weight = Mathf.Clamp01(weight);
            m_GraphController.SetRootWeight(weight);
        }

        public Context PlayAnimation(string stateName, float fadeDuration = 0.25f)
        {
            if (m_CurrentSequence != null)
            {
                m_CurrentSequence.SequenceContext.Interrupt();
            }

            var ctx = new Context(m_CurrentSequence);
            StartCoroutine(FadeAnimation(stateName, fadeDuration, () => {
                ctx.Complete();
            }));
            return ctx;
        }

        IEnumerator FadeAnimation(string stateName, float fadeDuration, System.Action onComplete)
        {
            m_AnimatorHandler.AnimatorController.CrossFade(stateName, fadeDuration);

            if (m_SequenceFadeCoroutine != null)
            {
                StopCoroutine(m_SequenceFadeCoroutine);
                m_SequenceFadeCoroutine = null;
            }

            var startSequenceWeight = m_GraphController.GetSequenceWeight();
            var time = 0f;
            while(time <= fadeDuration)
            {
                var t = time / fadeDuration;
                var sequenceWeight = Mathf.Lerp(startSequenceWeight, 0f, t);
                SetSequenceWeight(sequenceWeight);
                time += Time.deltaTime;
                yield return null;
            }

            SetSequenceWeight(0f);

            var stateInfo = m_AnimatorHandler.AnimatorController.GetCurrentAnimatorStateInfo(0);
            var waitDuration = stateInfo.length - fadeDuration;
            time = 0f;
            
            while (time <= waitDuration)
            {
                var t = time / waitDuration;
                time += Time.deltaTime;
                yield return null;
            }

            onComplete?.Invoke();
        }

        public Context PlaySequence(PlayableSequence sequence, float time = 0f, float fadeDuration = 0.5f)
        {
            var ctx = (PlayableSequenceContext)m_Director.Prepare(sequence, TickMode.Auto);
            m_Director.Play(time);

            m_CurrentSequence = new Context(ctx);
            ctx.OnChangeStatus += OnChangeSequenceState;

            FadeSequence(1f, fadeDuration);

            return m_CurrentSequence;
        }

        void OnChangeSequenceState(SequenceStatus state)
        {
            switch (state)
            {
                case SequenceStatus.Stoppped:
                    if(m_CurrentSequence != null && m_CurrentSequence.IsInterrupted)
                    {
                        SetSequenceWeight(0f);
                        m_CurrentSequence = null;
                        break;
                    }

                    FadeSequence(0f, 0.5f, () => {
                        var ctx = m_CurrentSequence;
                        m_CurrentSequence = null;
                        ctx?.Complete();
                    });
                    break;

                case SequenceStatus.Interrupted:
                    m_CurrentSequence?.Interupt();
                    break;
            }
        }

        void FadeSequence(float toSequenceWeight, float duration, System.Action onComplete = null)
        {
            if(m_SequenceFadeCoroutine != null)
            {
                StopCoroutine(m_SequenceFadeCoroutine);
                m_SequenceFadeCoroutine = null;
            }
            m_SequenceFadeCoroutine = StartCoroutine(_CrossFade(toSequenceWeight, duration, onComplete));
        }
        IEnumerator _CrossFade(float toSequenceWeight, float duration, System.Action onComplete)
        {
            var startWeight = m_GraphController.GetSequenceWeight();
            var time = 0f;
            while(time <= duration)
            {
                var t = time / duration;
                var weight = Mathf.Lerp(startWeight, toSequenceWeight, t);
                SetSequenceWeight(weight);
                time += Time.deltaTime;
                yield return null;
            }

            SetSequenceWeight(toSequenceWeight);

            m_SequenceFadeCoroutine = null;
            onComplete?.Invoke();
        }

        public class Context
        {
            public event System.Action OnCompleted;
            public event System.Action OnInterrupt;
            PlayableSequenceContext m_Ctx;
            Context m_Inner;

            internal PlayableSequenceContext SequenceContext { get { return m_Ctx; } }

            public bool IsCancled { get { return m_Inner == null ? m_IsCancled : m_Inner.IsCancled; } }
            public bool IsInterrupted { get { return m_Inner == null ? m_IsInterrupt : m_Inner.IsInterrupted; } }
            bool m_IsCancled;
            bool m_IsInterrupt;

            public Context(PlayableSequenceContext ctx)
            {
                m_Ctx = ctx;
            }
            public Context(Context inner)
            {
                m_Inner = inner;
            }

            public void Cancel()
            {
                m_IsCancled = true;
                m_Ctx?.Stop();
                m_Inner?.Cancel();
            }

            internal void Interupt()
            {
                m_IsInterrupt = true;
                m_Inner?.Interupt();
                OnInterrupt?.Invoke();
            }

            internal void Complete()
            {
                if (m_IsCancled)
                    return;

                m_Ctx?.Dispose();
                OnCompleted?.Invoke();
                OnCompleted = null;
            }
        }
    }
}