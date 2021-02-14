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
        [SerializeField] BlackboardEditor m_BlacboardEditor = new BlackboardEditor();
        [SerializeField] float m_CurrentFrame = 20f;
        [SerializeField] Sequence m_Sequence;
        [SerializeField] SequenceEditor m_SequenceEditor;
        [SerializeField] double m_LatestTickTime = 0f;
        Runtime.SequenceContext m_Context;

        private void OnEnable()
        {
            EditorApplication.update += OnUpdate;
        }
        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        private void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_Sequence = (Sequence)EditorGUILayout.ObjectField("Sequence Asset", m_Sequence, typeof(Sequence), false);
                if(check.changed)
                {
                    if(m_Sequence == null)
                    {
                        m_SequenceEditor = null;
                        return;
                    }
                    m_SequenceEditor = new SequenceEditor(this, m_Sequence);
                }
            }

            if (m_Sequence == null || m_SequenceEditor == null)
                return;

            if(m_Context == null)
                m_Context = m_Sequence.CreateContext();

            // Sequence Setting
            m_SequenceEditor.DrawSetting();

            m_BlacboardEditor.Draw(this, m_Sequence);

            DrawPlayer();

            m_CurrentFrame = m_Indicator.OnGUI(m_Sequence.TotalFrame, m_Sequence.TotalFrame, m_CurrentFrame, m_Sequence.FrameRate, m_Navigator.MinFrame, m_Navigator.MaxFrame, Focus);
            m_Navigator.OnGUI(m_Sequence.TotalFrame, m_Sequence.TotalFrame, m_CurrentFrame);

            m_SequenceEditor.Draw(m_Navigator, m_Sequence.TotalFrame, m_CurrentFrame);
        }

        void Focus(float totalFrame, float focusFrame)
        {
            m_Navigator?.Focus(totalFrame, focusFrame);
            if(m_Context != null)
                m_Context.Current = focusFrame / m_Sequence.FrameRate;
        }
        void DrawPlayer()
        {
            bool isPlaying = m_Context != null && m_Context.IsPlaying;

            using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
            {
                if(GUILayout.Button("<<", EditorStyles.toolbarButton))
                {
                    m_CurrentFrame = 0;
                    if(m_Context != null)
                        m_Context.Current = m_CurrentFrame / m_Sequence.FrameRate;
                }
                if (GUILayout.Button("<", EditorStyles.toolbarButton))
                {
                    m_CurrentFrame = Mathf.Max(m_CurrentFrame - 1, 0);
                    if (m_Context != null)
                        m_Context.Current = m_CurrentFrame / m_Sequence.FrameRate;
                }

                if(isPlaying)
                {
                    if (GUILayout.Button("Stop", EditorStyles.toolbarButton))
                    {
                        m_Context.Stop();
                    }
                } else
                {
                    if (GUILayout.Button("Play", EditorStyles.toolbarButton))
                    {
                        m_Context.Play(m_Context.Current);
                        m_LatestTickTime = EditorApplication.timeSinceStartup;
                    }
                }
                if (GUILayout.Button(">", EditorStyles.toolbarButton))
                {
                    m_CurrentFrame = Mathf.Min(m_CurrentFrame + 1, m_Sequence.TotalFrame);
                    if (m_Context != null)
                        m_Context.Current = m_CurrentFrame / m_Sequence.FrameRate;
                }
                if (GUILayout.Button(">>", EditorStyles.toolbarButton))
                {
                    m_CurrentFrame = m_Sequence.TotalFrame;
                    if (m_Context != null)
                        m_Context.Current = m_CurrentFrame / m_Sequence.FrameRate;
                }
            }
        }

        void OnUpdate()
        {
            if (m_Context == null)
                return;
            if (!m_Context.IsPlaying)
                return;

            var delta = EditorApplication.timeSinceStartup - m_LatestTickTime;
            m_Context?.Tick((float)delta);
            m_CurrentFrame = m_Context.Current * m_Sequence.FrameRate;
            m_LatestTickTime = EditorApplication.timeSinceStartup;
            Repaint();
        }

        public void ChangeData()
        {
            if (m_Context == null)
                return;

            var isPlaying = m_Context.IsPlaying;
            var current = m_Context.Current;
            m_Context = m_Sequence.CreateContext();
            if(isPlaying)
            {
                m_Context.Play(current);
            }
        }

        public void ChangeBlackboard(Blackborad blackboard)
        {
            AssetDatabase.AddObjectToAsset(blackboard, m_Sequence);
            EditorUtility.SetDirty(m_Sequence);
            AssetDatabase.SaveAssets();
        }
    }
}