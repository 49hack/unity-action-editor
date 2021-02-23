using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class PlayableAnimatorBlackboard : Blackboard
    {
        public static string PropNameBlendableAnimator { get { return nameof(m_BlendableAnimator); } }
        public static string PropNameInterruptBlock { get { return nameof(m_InterruptBlock); } }
        public static string PropNameRestartData { get { return nameof(m_RestartData); } }

        [SerializeField] SharedBlendableAnimator m_BlendableAnimator = new SharedBlendableAnimator(PropNameBlendableAnimator);
        [SerializeField] SharedInterruptBlock m_InterruptBlock = new SharedInterruptBlock(PropNameInterruptBlock);
        [SerializeField] SharedRestartData m_RestartData = new SharedRestartData(PropNameRestartData);

        public InterruptBlock InterruptBlock { get { return m_InterruptBlock.Value; } }
        public RestartData RestartData { get { return m_RestartData.Value; } }

        protected override IReadOnlyList<SharedValue> Collect()
        {
            return new SharedValue[] {
                m_BlendableAnimator,
                m_InterruptBlock,
                m_RestartData
            };
        }

        public void Bind(PlayableAnimator animator)
        {
            m_BlendableAnimator.Value = animator;
        }
    }

    [System.Serializable]
    public class SharedBlendableAnimator : SharedValue<PlayableAnimator>
    {
        public SharedBlendableAnimator(string name) : base(name) { }
    }

    [System.Serializable]
    public class SharedBlendableAnimatorContext : SharedValueContext<PlayableAnimator>
    {
        public SharedBlendableAnimatorContext()
        {
            m_PropertyName = PlayableAnimatorBlackboard.PropNameBlendableAnimator;
            m_SharedType = SharedValueType.Blackboard;
        }
    }

    [System.Serializable]
    public class InterruptBlock
    {
        [SerializeField] bool m_IsBlock;
        public bool Blocked { get { return m_IsBlock; } }

        public void Change(bool isBlock)
        {
            m_IsBlock = isBlock;
        }
    }

    [System.Serializable]
    public class SharedInterruptBlock : SharedValue<InterruptBlock>
    {
        public SharedInterruptBlock(string name) : base(name) { }
    }

    [System.Serializable]
    public class SharedInterruptBlockContext : SharedValueContext<InterruptBlock>
    {
        public SharedInterruptBlockContext()
        {
            m_PropertyName = PlayableAnimatorBlackboard.PropNameInterruptBlock;
            m_SharedType = SharedValueType.Blackboard;
        }
    }

    [System.Serializable]
    public class RestartData
    {
        [SerializeField] float m_RestartTime;
        public float RestartTime { get { return m_RestartTime; } }
        public bool CanRestart { get { return m_RestartTime >= 0f; } }

        public void Permit(float time)
        {
            m_RestartTime = time;
        }

        public void Prohibit()
        {
            m_RestartTime = -1f;
        }
    }

    [System.Serializable]
    public class SharedRestartData : SharedValue<RestartData>
    {
        public SharedRestartData(string name) : base(name) { }
    }

    [System.Serializable]
    public class SharedRestartDataContext : SharedValueContext<RestartData>
    {
        public SharedRestartDataContext()
        {
            m_PropertyName = PlayableAnimatorBlackboard.PropNameRestartData;
            m_SharedType = SharedValueType.Blackboard;
        }
    }
}
