using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Sample
{
    public class SampleBlackboard : Blackboard
    {
        [SerializeField] SharedInt m_IntValue = new SharedInt(nameof(m_IntValue));
        [SerializeField] SharedFloat m_FloatValue = new SharedFloat(nameof(m_FloatValue));
        [SerializeField] SharedVector2 m_Vector2Value = new SharedVector2(nameof(m_Vector2Value));
        [SerializeField] SharedSample m_Sample = new SharedSample(nameof(m_Sample));
        [SerializeField] SharedAnimator m_Animator = new SharedAnimator(nameof(m_Animator));

        [SerializeField] SharedAnimatorContext m_Test = new SharedAnimatorContext();

        protected override IReadOnlyList<SharedValue> Collect()
        {
            return new SharedValue[] {
                m_IntValue,
                m_FloatValue,
                m_Vector2Value,
                m_Sample,
                m_Animator
            };
        }
    }

    [System.Serializable]
    public class SharedSample : SharedValue<SampleClass> { public SharedSample(string name) : base(name) { } }

    [System.Serializable]
    public class SampleClass
    {
        [SerializeField] string m_Name;
        [SerializeField] int m_SampleValue;
    }
}