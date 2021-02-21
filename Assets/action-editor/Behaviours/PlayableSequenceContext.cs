using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace ActionEditor.Runtime
{
    public class PlayableSequenceContext : SequenceContext
    {
        PlayableSequence Sequence { get { return (PlayableSequence)m_Sequence; } }

        AnimatorControllerPlayable m_ControllerPlayable;

        public PlayableSequenceContext(SequenceBehaviour sequence, TrackBehaviour[] tracks, IReadOnlyList<Blackboard> blackboards) : base(sequence, tracks, blackboards)
        {
        }
    }
}