using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [ParentSequence(typeof(PlayableSequence))]
    [MenuTitle("Playable/Interrupt Block")]
    public class InterruptBlockTrackBehaviour : TrackBehaviour
    {
        [SerializeField] SharedInterruptBlockContext m_Interrupt;

        public override void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            Blackboard.Bind(blackboards, m_Interrupt);
        }

        public override void OnPlay()
        {
            m_Interrupt.Value?.Change(false);
        }

        public override void OnStop()
        {
            m_Interrupt.Value?.Change(false);
        }

        public override void OnDispose()
        {
            m_Interrupt.Value?.Change(false);
        }
    }
}