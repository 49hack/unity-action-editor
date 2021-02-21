using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor.Sample
{
    public class ActionPoint : MonoBehaviour
    {
        [SerializeField] PlayableSequence m_Sequence;

        PlayableSequence m_Instance;
        public PlayableSequence Sequence { get { return m_Instance; } }

        private void Awake()
        {
            m_Instance = Instantiate(m_Sequence);
        }

        private void OnDestroy()
        {
            Destroy(m_Instance);
        }
    }
}