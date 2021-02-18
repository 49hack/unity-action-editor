using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Sample
{
    public class SampleBlackboard : Blackboard
    {
        [SerializeField] SharedAnimator m_Animator = new SharedAnimator(nameof(m_Animator));

        protected override IReadOnlyList<SharedValue> Collect()
        {
            return new SharedValue[] {
                m_Animator
            };
        }
    }
}