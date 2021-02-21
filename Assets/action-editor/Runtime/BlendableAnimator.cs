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
    [RequireComponent(typeof(BlendableAnimatorBlackboard))]
    public class BlendableAnimator : MonoBehaviour
    {
        [SerializeField] RuntimeAnimatorController m_AnimatorController;

        Animator m_Animator;
        Director m_Director;
        BlendableAnimatorBlackboard m_Blackboard;
        AnimationGraph m_GraphController;

        AnimationGraph.Handler m_AnimatorHandler;
        AnimationGraph.Handler m_SequenceHandler;

        Coroutine m_SequenceFadeCoroutine;

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

        BlendableAnimatorBlackboard Blackboard
        {
            get
            {
                if (m_Blackboard == null)
                    m_Blackboard = GetComponent<BlendableAnimatorBlackboard>();
                return m_Blackboard;
            }
        }

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

        public Context PlayAnimation(string stateName, float fadeDuration = 0.5f)
        {
            var ctx = new Context(null);
            StartCoroutine(FadeAnimation(stateName, fadeDuration, () => {
                ctx.Complete();
            }));
            return ctx;
        }

        IEnumerator FadeAnimation(string stateName, float fadeDuration, System.Action onComplete)
        {
            m_AnimatorHandler.AnimatorController.CrossFade(stateName, fadeDuration);

            yield return null;

            var stateInfo = m_AnimatorHandler.AnimatorController.GetCurrentAnimatorStateInfo(0);
            var duration = stateInfo.length;
            var time = 0f;

            while(time <= duration + fadeDuration)
            {
                time += Time.deltaTime;
                yield return null;
            }

            onComplete?.Invoke();
        }

        public Context PlaySequence(PlayableSequence sequence, float fadeDuration = 0.5f)
        {
            var ctx = m_Director.Prepare(sequence, TickMode.Auto);
            m_Director.Play(0f);

            var result = new Context(ctx);

            ctx.OnChangeStatus += (state) => {
                if (state != SequenceStatus.Stoppped)
                    return;
                FadeSequence(0f, 0.5f, () => {
                    result.Complete();
                    result = null;
                });
            };

            FadeSequence(1f, fadeDuration);

            return result;
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
            SequenceContext m_Ctx;

            public Context(SequenceContext ctx)
            {
                m_Ctx = ctx;
            }

            public void Stop()
            {
                m_Ctx?.Stop();
            }

            public void Complete()
            {
                m_Ctx?.Dispose();
                OnCompleted?.Invoke();
            }
        }
    }
}