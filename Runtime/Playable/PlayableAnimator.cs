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
        public AnimationLayerMixerPlayable SequenceMixer { get { return m_SequenceHandler.Mixer; } }

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

            m_AnimatorHandler = m_GraphController.Add(m_AnimatorController);
            m_AnimatorHandler.Weight = 1f;

            SetSequenceWeight(0f);
            SetAnimatorWeight(1f);
        }

        internal void SetSequenceAvatarMask(AvatarMask mask)
        {
            m_GraphController?.SetSequenceAvatarMask(mask);
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
            m_GraphController.SetRootWeight(1, weight);
        }

        public void SetAnimatorWeight(float weight)
        {
            weight = Mathf.Clamp01(weight);
            m_GraphController.SetRootWeight(0, weight);
        }

        public Context PlayAnimation(int stateId, float fadeDuration = 0.25f)
        {
            if (m_CurrentSequence != null)
            {
                m_CurrentSequence.SequenceContext.Interrupt();
            }

            m_AnimatorHandler.AnimatorController.Play(stateId);

            var ctx = new Context(m_CurrentSequence);
            StartCoroutine(FadeAnimation(fadeDuration, () => {
                ctx.Complete();
            }));
            return ctx;
        }

        public Context CrossFadeAnimation(int stateId, float fadeDuration = 0.25f)
        {
            if (m_CurrentSequence != null)
            {
                m_CurrentSequence.SequenceContext.Interrupt();
            }

            m_AnimatorHandler.AnimatorController.CrossFade(stateId, fadeDuration);

            var ctx = new Context(m_CurrentSequence);
            StartCoroutine(FadeAnimation(fadeDuration, () => {
                ctx.Complete();
            }));
            return ctx;
        }

        public Context PlayAnimation(string stateName, float fadeDuration = 0.25f)
        {
            if (m_CurrentSequence != null)
            {
                m_CurrentSequence.SequenceContext.Interrupt();
            }

            m_AnimatorHandler.AnimatorController.Play(stateName);

            var ctx = new Context(m_CurrentSequence);
            StartCoroutine(FadeAnimation(fadeDuration, () => {
                ctx.Complete();
            }));
            return ctx;
        }

        public Context CrossFadeAnimation(string stateName, float fadeDuration = 0.25f)
        {
            if (m_CurrentSequence != null)
            {
                m_CurrentSequence.SequenceContext.Interrupt();
            }

            m_AnimatorHandler.AnimatorController.CrossFade(stateName, fadeDuration);

            var ctx = new Context(m_CurrentSequence);
            StartCoroutine(FadeAnimation(fadeDuration, () => {
                ctx.Complete();
            }));
            return ctx;
        }

        IEnumerator FadeAnimation(float fadeDuration, System.Action onComplete)
        {
            if (m_SequenceFadeCoroutine != null)
            {
                StopCoroutine(m_SequenceFadeCoroutine);
                m_SequenceFadeCoroutine = null;
            }

            var startSequenceWeight = m_GraphController.GetSequenceWeight();
            var time = 0f;
            while (time <= fadeDuration)
            {
                var t = time / fadeDuration;
                var sequenceWeight = Mathf.Lerp(startSequenceWeight, 0f, t);
                SetSequenceWeight(sequenceWeight);
                SetAnimatorWeight(1f - sequenceWeight);
                time += Time.deltaTime;
                yield return null;
            }

            SetSequenceWeight(0f);
            SetAnimatorWeight(1f);

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
            if (m_CurrentSequence != null)
            {
                m_CurrentSequence.SequenceContext.Interrupt();
                m_CurrentSequence.Interupt();
                m_CurrentSequence.Complete();
                m_CurrentSequence = null;
            }

            var ctx = (PlayableSequenceContext)m_Director.Prepare(sequence, TickMode.Auto);
            m_Director.Play(time);

            m_CurrentSequence = new Context(ctx, fadeDuration);
            ctx.OnChangeStatus += OnChangeSequenceState;
            ctx.OnUpdate += OnUpdateSequence;

            FadeSequence(1f, ctx.AnimatorWeight, fadeDuration);

            return m_CurrentSequence;
        }

        void OnUpdateSequence(SequenceContext ctx, float time)
        {
            if (m_CurrentSequence == null)
                return;

            if (m_CurrentSequence.IsEndFading)
                return;

            if (ctx.Length - m_CurrentSequence.EndFadeDuration <= time)
            {
                m_CurrentSequence.IsEndFading = true;
                FadeSequence(0f, 1f, m_CurrentSequence.EndFadeDuration, () => {
                    var cache = m_CurrentSequence;
                    m_CurrentSequence = null;
                    cache?.Complete();
                });
            }
        }

        void OnChangeSequenceState(SequenceStatus state)
        {
            switch (state)
            {
                case SequenceStatus.Stoppped:
                    {
                        while (true)
                        {
                            if (m_CurrentSequence == null)
                                break;
                            if (!m_CurrentSequence.IsInterrupted)
                                break;
                            if (m_CurrentSequence.IsEndFading)
                                break;
                            SetSequenceWeight(0f);
                            SetAnimatorWeight(1f);
                            break;
                        }
                        if (m_CurrentSequence != null && !m_CurrentSequence.IsEndFading)
                            m_CurrentSequence = null;
                    }
                    break;

                case SequenceStatus.Interrupted:
                    m_CurrentSequence?.Interupt();
                    break;
            }
        }

        void FadeSequence(float toSequenceWeight, float animatorWeight, float duration, System.Action onComplete = null)
        {
            if (m_SequenceFadeCoroutine != null)
            {
                StopCoroutine(m_SequenceFadeCoroutine);
                m_SequenceFadeCoroutine = null;
            }
            m_SequenceFadeCoroutine = StartCoroutine(_CrossFade(toSequenceWeight, animatorWeight, duration, onComplete));
        }
        IEnumerator _CrossFade(float toSequenceWeight, float toAnimatorWeight, float duration, System.Action onComplete)
        {
            var startSequenceWeight = m_GraphController.GetSequenceWeight();
            var startAnimatorWeight = m_GraphController.GetAnimatorWeight();
            var time = 0f;
            while (time <= duration)
            {
                var t = time / duration;
                var seqWeight = Mathf.Lerp(startSequenceWeight, toSequenceWeight, t);
                var animWeight = Mathf.Lerp(startAnimatorWeight, toAnimatorWeight, t);
                SetSequenceWeight(seqWeight);
                SetAnimatorWeight(animWeight);
                time += Time.deltaTime;
                yield return null;
            }

            SetSequenceWeight(toSequenceWeight);
            SetAnimatorWeight(toAnimatorWeight);

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
            public float AnimatorWeight { get { return m_Inner == null ? m_Ctx == null ? 0f : m_Ctx.AnimatorWeight : m_Inner.AnimatorWeight; } }
            public float EndFadeDuration { get; private set; }
            public bool IsEndFading { get; set; }
            bool m_IsCancled;
            bool m_IsInterrupt;

            public Context(PlayableSequenceContext ctx, float fadeOutDuration)
            {
                m_Ctx = ctx;
                EndFadeDuration = fadeOutDuration;
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
                Complete();
            }

            internal void Interupt()
            {
                m_IsInterrupt = true;
                m_Inner?.Interupt();
                OnInterrupt?.Invoke();
            }

            internal void Complete()
            {
                m_Ctx?.Dispose();
                m_Inner?.Complete();

                if (m_IsCancled)
                    return;

                OnCompleted?.Invoke();
                OnCompleted = null;
            }
        }
    }
}