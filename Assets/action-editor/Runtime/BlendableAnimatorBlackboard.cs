using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class BlendableAnimatorBlackboard : Blackboard
    {
        public static string PropNameBlendableAnimator { get { return nameof(m_BlendableAnimator); } }
        [SerializeField] SharedBlendableAnimator m_BlendableAnimator = new SharedBlendableAnimator(PropNameBlendableAnimator);

        protected override IReadOnlyList<SharedValue> Collect()
        {
            return new SharedValue[] {
                m_BlendableAnimator
            };
        }

        public void Bind(BlendableAnimator animator)
        {
            m_BlendableAnimator.Value = animator;
        }
    }

    [System.Serializable]
    public class SharedBlendableAnimator : SharedValue<BlendableAnimator>
    {
        public SharedBlendableAnimator(string name) : base(name) { }
    }
    [System.Serializable]
    public class SharedBlendableAnimatorContext : SharedValueContext<BlendableAnimator>
    {
        public SharedBlendableAnimatorContext()
        {
            m_PropertyName = BlendableAnimatorBlackboard.PropNameBlendableAnimator;
            m_SharedType = SharedValueType.Blackboard;
        }
    }
}
