using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    using Runtime;

    public enum TickMode
    {
        Auto,
        Manual
    }

    public enum SequenceStatus
    {
        Initial,
        Stoppped,
        Interrupted,
        Playing,
        Paused,
    }

    public interface IDirector
    {
        SequenceStatus Status { get; }
        SequenceBehaviour Sequence { get; set; }
        IReadOnlyList<Blackboard> Blackboard { get; }
        float CurrentTime { get; set; }
        float CurrentFrame { get; set; }
        float Length { get; }
        float TotalFrame { get; }
        SequenceContext Prepare(SequenceBehaviour sequence = null, TickMode mode = TickMode.Auto);
        void Play(float? time = null);
        void Stop();
        void Pause();
        void Resume();
        void Tick(float deltaTime);
        void Dispose();
    }
}
