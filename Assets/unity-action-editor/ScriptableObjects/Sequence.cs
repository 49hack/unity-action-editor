using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [CreateAssetMenu(menuName = "Action Editor/Sequence", fileName = "ActionSequence")]
    public class Sequence : ScriptableObject
    {
        [SerializeField] Track[] m_Tracks = new Track[0];
        [SerializeField] float m_TotalFrame = 120f;
        [SerializeField] float m_FrameRate = 60f;

        public static string PropNameTracks { get { return nameof(m_Tracks); } }
        public static string PropNameTotalFrame { get { return nameof(m_TotalFrame); } }
        public static string PropNameFrameRate { get { return nameof(m_FrameRate); } }

        public float TotalFrame { get { return m_TotalFrame; } }
        public float FrameRate { get { return m_FrameRate; } }

        public Runtime.SequenceContext CreateContext()
        {
            return new Runtime.SequenceContext(this, m_Tracks);
        }

        public void OnDispose()
        {
        }
    }
}