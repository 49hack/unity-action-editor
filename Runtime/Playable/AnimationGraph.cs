using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace ActionEditor
{
    public class AnimationGraph
    {
        public enum Type
        {
            Animator,
            Sequence
        }

        public class Handler
        {
            public delegate void OnChangeWeightEvent(float weight);
            public delegate void OnRequestWeightChange(Handler handler, float weight);
            public delegate void OnDispose(Handler handler);

            public Type Type { get; private set; }
            public int InputIndex { get; private set; }
            public float Weight { get { return m_Weight; } set { ChangeWaight(value); } }
            public AnimationLayerMixerPlayable Mixer
            {
                get
                {
                    if (Type != Type.Sequence)
                        throw new System.InvalidOperationException("Only handler that type is Sequence has a mixer.");
                    return m_Mixer;
                }
            }
            public AnimatorControllerPlayable AnimatorController
            {
                get
                {
                    if (Type != Type.Animator)
                        throw new System.InvalidOperationException("Only handler that type is Animator has a AnimatorController.");
                    return m_AnimatorController;
                }
            }

            public event OnChangeWeightEvent OnChangeWeight;

            float m_Weight;
            OnRequestWeightChange onRequestWeightChange;
            OnDispose onDispose;
            AnimationLayerMixerPlayable m_Mixer;
            AnimatorControllerPlayable m_AnimatorController;

            public Handler(Type type, int inputIndex, float weight, OnRequestWeightChange waitChange, OnDispose dispose)
            {
                m_Weight = weight;
                Type = type;
                InputIndex = inputIndex;
                onRequestWeightChange = waitChange;
                onDispose = dispose;
            }

            void ChangeWaight(float value)
            {
                onRequestWeightChange?.Invoke(this, value);
            }
            
            public void Dispose()
            {
                onDispose?.Invoke(this);
            }

            internal void SetMixer(AnimationLayerMixerPlayable mixer)
            {
                m_Mixer = mixer;
            }
            internal void SetAnimatorController(AnimatorControllerPlayable controller)
            {
                m_AnimatorController = controller;
            }
            internal void ChangeInputIndex(int index)
            {
                InputIndex = index;
            }
            internal void ChangeWeightInternal(float weight)
            {
                m_Weight = weight;
                OnChangeWeight?.Invoke(weight);
            }
        }

        Animator m_Animator;
        PlayableGraph m_Graph;
        AnimationMixerPlayable m_RootMixer;
        AnimationMixerPlayable m_AnimatorControllerMixer;
        AnimationMixerPlayable m_SequenceMixer;
        int m_MaxMixingCount;
        AnimationPlayableOutput m_PlayableOutput;

        List<Handler> m_AnimatorHandlers = new List<Handler>();
        List<Handler> m_SequenceHandlers = new List<Handler>();

        public PlayableGraph Graph { get { return m_Graph; } }

        public AnimationGraph(string name, Animator animator, int maxMixingCount)
        {
            m_MaxMixingCount = maxMixingCount;
            m_Animator = animator;

            m_Graph = PlayableGraph.Create(name);
            m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            m_RootMixer = AnimationMixerPlayable.Create(m_Graph, 2, false);
            m_AnimatorControllerMixer = AnimationMixerPlayable.Create(m_Graph, maxMixingCount, false);
            m_SequenceMixer = AnimationMixerPlayable.Create(m_Graph, maxMixingCount, false);

            m_RootMixer.ConnectInput(0, m_AnimatorControllerMixer, 0, 1f);
            m_RootMixer.ConnectInput(1, m_SequenceMixer, 0, 0f);

            m_PlayableOutput = AnimationPlayableOutput.Create(m_Graph, "Animation", animator);
            m_PlayableOutput.SetSourcePlayable(m_RootMixer);

            m_Graph.Play();
        }

        public Handler Add(RuntimeAnimatorController controller)
        {
            m_Animator.runtimeAnimatorController = null;
            var controllerPlayable = AnimatorControllerPlayable.Create(m_Graph, controller);
            controllerPlayable.SetTime(0f);

            var index = m_AnimatorHandlers.Count;
            if (index >= m_MaxMixingCount)
                throw new System.InvalidOperationException(string.Format("More than the specified count of AnimatorController have been added. (max: {0})", m_MaxMixingCount));

            var Weight = 0f;
            m_AnimatorControllerMixer.ConnectInput(index, controllerPlayable, 0, Weight);

            var handler = new Handler(Type.Animator, index, Weight, OnChangeWeight, OnRemove);
            handler.SetAnimatorController(controllerPlayable);
            m_AnimatorHandlers.Add(handler);

            var target = m_PlayableOutput.GetTarget();
            if (target != null)
                target.Rebind();

            return handler;
        }

        public Handler CreateMixer(int inputCount)
        {
            var index = m_SequenceHandlers.Count;
            if (index >= m_MaxMixingCount)
                throw new System.InvalidOperationException(string.Format("More than the specified count of AnimationMixerPlayable have been added. (max: {0})", m_MaxMixingCount));

            var Weight = 0f;
            var mixer = AnimationLayerMixerPlayable.Create(m_Graph, inputCount);
            m_SequenceMixer.ConnectInput(index, mixer, 0, Weight);

            var handler = new Handler(Type.Sequence, index, Weight, OnChangeWeight, OnRemove);
            handler.SetMixer(mixer);
            m_SequenceHandlers.Add(handler);

            var target = m_PlayableOutput.GetTarget();
            if (target != null)
                target.Rebind();

            return handler;
        }

        public void SetRootWeight(float animatorWeight)
        {
            var weight = Mathf.Clamp01(animatorWeight);
            m_RootMixer.SetInputWeight(0, weight);
            m_RootMixer.SetInputWeight(1, 1f - weight);
        }

        public void SetRootWeight(int index, float weight)
        {
            weight = Mathf.Clamp01(weight);
            m_RootMixer.SetInputWeight(index, weight);
        }

        public float GetAnimatorWeight()
        {
            return m_RootMixer.GetInputWeight(0);
        }

        public float GetSequenceWeight()
        {
            return m_RootMixer.GetInputWeight(1);
        }

        public void Evaluate()
        {
            if (m_Graph.IsValid())
                m_Graph.Evaluate();
        }

        public void Evaluate(float time)
        {
            if (m_Graph.IsValid())
                m_Graph.Evaluate(time);
        }

        public void Play()
        {
            if (m_Graph.GetTimeUpdateMode() == DirectorUpdateMode.Manual)
            {
                m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
                m_Graph.Stop();
            }

            m_Graph.Play();
        }

        public void Stop()
        {
            m_Graph.Stop();
        }

        public void Pause()
        {
            m_Graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        }

        public void Resume()
        {
            m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            m_Graph.Stop();
            m_Graph.Play();
        }

        public void Dispose()
        {
            if (m_Graph.IsValid())
                m_Graph.Destroy();
        }

        AnimationMixerPlayable GetMixer(Type type)
        {
            if (type == Type.Animator)
                return m_AnimatorControllerMixer;
            return m_SequenceMixer;
        }
        List<Handler> GetHandlerList(Type type)
        {
            return type == Type.Animator ? m_AnimatorHandlers : m_SequenceHandlers; ;
        }

        void OnChangeWeight(Handler handler, float value)
        {
            var list = GetHandlerList(handler.Type);
            var mixer = GetMixer(handler.Type);
            var weight = Mathf.Clamp01(value);

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var isTarget = item == handler;
                var isCurrent = mixer.GetInputWeight(handler.InputIndex) > 0f;

                if(isTarget)
                {
                    handler.ChangeWeightInternal(weight);
                    mixer.SetInputWeight(handler.InputIndex, weight);
                    continue;
                }

                if(isCurrent)
                {
                    handler.ChangeWeightInternal(1f - weight);
                    mixer.SetInputWeight(handler.InputIndex, 1f - weight);
                    continue;
                }

                handler.ChangeWeightInternal(0f);
                mixer.SetInputWeight(handler.InputIndex, 0f);
            }
        }

        void OnRemove(Handler handler)
        {
            var list = GetHandlerList(handler.Type);

            if (!list.Contains(handler))
                return;

            var mixer = GetMixer(handler.Type);

            for (int i = 0; i < list.Count; i++)
            {
                mixer.DisconnectInput(list[i].InputIndex);
            }

            list.Remove(handler);

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                item.ChangeInputIndex(i);
                mixer.SetInputWeight(item.InputIndex, item.Weight);
            }
        }
    }
}