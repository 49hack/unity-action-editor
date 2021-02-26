using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Playable/Interrupt Block")]
    [ParentTrack(typeof(InterruptBlockTrackBehaviour))]
    public class InterruptBlockClipBehaviour : ClipBehaviour
    {
        [SerializeField] SharedInterruptBlockContext m_Interrupt;

        public override void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            Blackboard.Bind(blackboards, m_Interrupt);
        }

        public override void OnBegin(float time, float absoluteTime, float duration)
        {
            m_Interrupt.Value.Change(true);
        }

        public override void OnEnd(float time, float absoluteTime, float duration)
        {
            m_Interrupt.Value.Change(false);
        }
    }
}