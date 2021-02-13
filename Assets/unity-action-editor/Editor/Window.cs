using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    public class Window : EditorWindow
    {
        [MenuItem("Window/Action Editor")]
        public static void Create()
        {
            var window = (Window)GetWindow(typeof(Window));
            window.Show();
        }

        [SerializeField] Navigator m_Navigator = new Navigator();
        [SerializeField] Indicator m_Indicator = new Indicator();
        [SerializeField] float m_CurrentFrame = 20f;
        [SerializeField] Sequence m_Sequence;
        [SerializeField] SequenceEditor m_SequenceEditor;

        private void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_Sequence = (Sequence)EditorGUILayout.ObjectField("", m_Sequence, typeof(Sequence), false);
                if(check.changed)
                {
                    if(m_Sequence == null)
                    {
                        m_SequenceEditor = null;
                        return;
                    }
                    m_SequenceEditor = new SequenceEditor(m_Sequence);
                }
            }

            if (m_Sequence == null || m_SequenceEditor == null)
                return;

            const float TotalFrame = 120f;
            const float DurationFrame = 120f;
            m_CurrentFrame = m_Indicator.OnGUI(TotalFrame, DurationFrame, m_CurrentFrame, 60f, m_Navigator.MinFrame, m_Navigator.MaxFrame, m_Navigator.Focus);
            m_Navigator.OnGUI(TotalFrame, DurationFrame, m_CurrentFrame);

            m_SequenceEditor.OnGUI(m_Navigator, TotalFrame, m_CurrentFrame);
        }
    }
}