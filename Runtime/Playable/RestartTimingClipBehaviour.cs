using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [ParentTrack(typeof(RestartTimingTrackBehaviour))]
    public class RestartTimingClipBehaviour : ClipBehaviour
    {
        [SerializeField] SharedRestartDataContext m_RestartData;

        bool m_IsInterrupted;

        public override void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            m_IsInterrupted = false;
            Blackboard.Bind(blackboards, m_RestartData);
        }

        public override void OnInterrupt(float clipTime, float absoluteTime)
        {
            m_IsInterrupted = true;
        }

        public override void OnBegin(float time, float absoluteTime)
        {
            if (m_IsInterrupted)
                return;

            m_RestartData.Value.Permit(absoluteTime);
        }

        public override void OnEnd(float time, float absoluteTime)
        {
            if (m_IsInterrupted)
                return;

            m_RestartData.Value.Prohibit();
        }

        public override void OnDispose()
        {
            m_IsInterrupted = false;
        }
    }
}