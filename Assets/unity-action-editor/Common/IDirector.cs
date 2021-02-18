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

    public enum Status
    {
        Initial,
        Stoppped,
        Playing,
        Paused,
    }

    public interface IDirector
    {
        Status Status { get; }
        Sequence Sequence { get; }
        IReadOnlyList<Blackboard> Blackboard { get; }
        float CurrentTime { get; set; }
        float CurrentFrame { get; set; }
        float Length { get; }
        float TotalFrame { get; }
        void Prepare(Sequence sequence = null, TickMode mode = TickMode.Auto);
        void Play(float? time = null);
        void Stop();
        void Pause();
        void Resume();
        void Tick(float deltaTime);
        void Dispose();
    }
}
