using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace ActionEditor.Runtime
{
    public class PlayableSequenceContext : SequenceContext
    {
        public float AnimatorWeight { get; private set; }
        PlayableSequence Sequence { get { return (PlayableSequence)m_Sequence; } }

        public PlayableSequenceContext(SequenceBehaviour sequence, TrackBehaviour[] tracks, IReadOnlyList<Blackboard> blackboards) : base(sequence, tracks, blackboards)
        {
            var playableSequence = (PlayableSequence)sequence;
            AnimatorWeight = playableSequence.AnimatorWeight;
        }
    }
}