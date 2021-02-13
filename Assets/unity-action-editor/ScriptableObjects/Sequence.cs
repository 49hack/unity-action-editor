using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [CreateAssetMenu(menuName = "Action Editor/Sequence", fileName = "ActionSequence")]
    public class Sequence : ScriptableObject
    {
        [SerializeField] Track[] m_Tracks = new Track[0];
        [SerializeField] float m_Length;

        public static string PropNameTracks { get { return nameof(m_Tracks); } }
        public static string PropNameLength { get { return nameof(m_Length); } }

        public float Length { get { return m_Length; } }
    }
}