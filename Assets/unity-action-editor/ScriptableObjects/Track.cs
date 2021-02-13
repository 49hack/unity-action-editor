using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class Track : ScriptableObject
    {
        [SerializeField] string m_TrackName;

        [SerializeField] Clip[] m_Clips = new Clip[0];

        public static string PropNameTrackName { get { return nameof(m_TrackName); } }
        public static string PropNameClips { get { return nameof(m_Clips); } }
    }
}
