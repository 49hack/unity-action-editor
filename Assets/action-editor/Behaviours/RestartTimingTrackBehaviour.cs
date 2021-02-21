using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class RestartTimingTrackBehaviour : TrackBehaviour
    {
        [SerializeField] SharedRestartDataContext m_RestartData;

        public override void OnCreate(SequenceBehaviour sequence, IReadOnlyList<Blackboard> blackboards)
        {
            Blackboard.Bind(blackboards, m_RestartData);
        }

        public override void OnPlay()
        {
            m_RestartData.Value.Prohibit();
        }

        public override void OnDispose()
        {
            m_RestartData.Value.Prohibit();
        }
    }
}