using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Sample
{
    public class CharacterBlackboard : Blackboard
    {
        [SerializeField] SharedTransform m_AttackLocator = new SharedTransform(nameof(m_AttackLocator));
        [SerializeField] SharedTransform m_Target = new SharedTransform(nameof(m_Target));

        protected override IReadOnlyList<SharedValue> Collect()
        {
            return new SharedValue[] {
                m_AttackLocator,
                m_Target
            };
        }
    }
}