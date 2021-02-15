using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ActionEditor
{
    [CreateAssetMenu(menuName = "Action Editor/Sequence", fileName = "ActionSequence")]
    public class Sequence : ScriptableObject, IHasBlackboard
    {
        [SerializeField] Track[] m_Tracks = new Track[0];
        [SerializeField] float m_TotalFrame = 120f;
        [SerializeField] float m_FrameRate = 60f;
        [SerializeField] Blackborad m_Blackboard;

        public static string PropNameTracks { get { return nameof(m_Tracks); } }
        public static string PropNameTotalFrame { get { return nameof(m_TotalFrame); } }
        public static string PropNameFrameRate { get { return nameof(m_FrameRate); } }
        public static string PropNameBlackboard { get { return nameof(m_Blackboard); } }

        public float TotalFrame { get { return m_TotalFrame; } }
        public float FrameRate { get { return m_FrameRate; } }
        public Blackborad Blackborad { get { return m_Blackboard; }}

        public Runtime.SequenceContext CreateContext()
        {
            return new Runtime.SequenceContext(this, m_Tracks);
        }

        public void OnDispose()
        {
        }
    }
}