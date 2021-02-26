using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    public class Window : EditorWindow
    {
        static System.Action s_OnRefresh;

        public static void Refresh()
        {
            s_OnRefresh?.Invoke();
        }

        enum State
        {
            Disable,
            NeedPrepare,
            Playable,
            NoSequence,
            EditOnly,
        }
        [MenuItem("Window/Action Editor")]
        public static void Create()
        {
            var window = (Window)GetWindow(typeof(Window));
            window.Show();
        }

        [SerializeField] Navigator m_Navigator = new Navigator();
        [SerializeField] Indicator m_Indicator = new Indicator();
        [SerializeField] BlackboardEditor m_BlackboardEditor = new BlackboardEditor();
        [SerializeField] SequenceBehaviourEditor m_SequenceEditor;
        [SerializeField] double m_LatestTickTime = 0f;
        IDirector m_Director;
        ActionEditorTime m_Timeline;

        private void OnEnable()
        {
            EditorApplication.update += OnUpdate;
            EditorApplication.playModeStateChanged += OnPlayModeState;
            s_OnRefresh += OnRefresh;
            m_SequenceEditor?.Enable();
            ChangeDirector(null);
            OnSelect(Selection.activeObject);
        }
        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            s_OnRefresh -= OnRefresh;
            m_SequenceEditor?.Disable();
            ChangeDirector(null);
        }

        private void OnDestroy()
        {
            ChangeDirector(null);
            m_SequenceEditor?.Dispose();
        }

        void OnSelectionChange()
        {
            var selected = Selection.activeObject;
            OnSelect(selected);
        }

        public void OnRefresh()
        {
            ChangeDirector(m_Director);
        }

        void OnPlayModeState(PlayModeStateChange state)
        {
            switch(state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    ChangeDirector(null);
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    OnSelect(Selection.activeObject);
                    break;
            }
        }


        void OnSelect(Object selected)
        {
            if (Application.isPlaying)
                return;

            if (selected is GameObject go)
            {
                if (string.IsNullOrEmpty(go.scene.name))
                {
                    ChangeDirector(null);
                    return;
                }

                var director = go.GetComponent<IDirector>();
                ChangeDirector(director);
                return;
            }

            if (selected is SequenceBehaviour sequence)
            {
                ChangeDirector(EditorDirector.Create(sequence));
                return;
            }
        }

        void ChangeDirector(IDirector director)
        {
            if(m_Director != null)
            {
                m_Director.Dispose();
                m_Director = null;
            }

            if (m_SequenceEditor != null)
            {
                m_SequenceEditor.Dispose();
                m_SequenceEditor = null;
            }

            m_Director = director;

            Repaint();
        }

        private void OnGUI()
        {
            var state = CheckState();

            if (state == State.Disable)
                return;

            var sequence = m_Director.Sequence;
            var nextSequence = (SequenceBehaviour)EditorGUILayout.ObjectField("Sequence Asset", sequence, typeof(SequenceBehaviour), false);
            if (sequence != nextSequence)
            {
                m_Director.Sequence = nextSequence;
                m_SequenceEditor = null;
                ChangeData();
            }

            if (state == State.NoSequence)
            {
                EditorGUILayout.HelpBox("Assign a sequence asset into Director", MessageType.Warning);
                return;
            }

            if(state == State.NeedPrepare)
            {
                m_Director?.Prepare(mode: TickMode.Manual);
            }

            if (m_SequenceEditor == null)
            {
                m_SequenceEditor = new SequenceBehaviourEditor();
                m_SequenceEditor.Initialize(this, m_Director.Sequence);
            }

            m_SequenceEditor.DrawSetting(m_Director.Blackboard);

            m_BlackboardEditor.Draw(m_Director.Blackboard);

            // Tool bar
            if (m_SequenceEditor != null)
            {
                var style = EditorStyles.toolbar;
                using (new EditorGUILayout.HorizontalScope(style, GUILayout.ExpandWidth(true)))
                {
                    DrawPlayer();

                    EditorGUILayout.Space();

                    m_SequenceEditor.DrawToolBar();

                    GUILayout.FlexibleSpace();

                    if(GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                    {
                        m_SequenceEditor = null;
                        ChangeData();
                        return;
                    }
                }
            }

            m_Navigator.OnGUI(m_Director.TotalFrame, m_Director.TotalFrame, m_Director.CurrentFrame);
            m_Director.CurrentFrame = m_Indicator.OnGUI(m_Director.TotalFrame, m_Director.TotalFrame, m_Director.CurrentFrame, m_Director.Sequence.FrameRate, m_Navigator.MinFrame, m_Navigator.MaxFrame, Focus);

            if (m_Timeline == null)
                m_Timeline = new ActionEditorTime(m_Navigator, m_Indicator);

            m_SequenceEditor.Draw(m_Timeline, m_Director.TotalFrame, m_Director.CurrentFrame, m_Director.Blackboard);
        }

        void Focus(float totalFrame, float focusFrame)
        {
            m_Navigator?.Focus(totalFrame, focusFrame);

            if(m_Director != null)
                m_Director.CurrentFrame = focusFrame;
        }
        void DrawPlayer()
        {
            using (new EditorGUI.DisabledGroupScope(CheckState() != State.Playable))
            {
                bool isPlaying = m_Director.Status == SequenceStatus.Playing;

                const float ButtonWidth = 50f;
                if (GUILayout.Button("<<", EditorStyles.toolbarButton, GUILayout.Width(ButtonWidth)))
                {
                    m_Director.CurrentFrame = 0;
                }
                if (GUILayout.Button("<", EditorStyles.toolbarButton, GUILayout.Width(ButtonWidth)))
                {
                    m_Director.CurrentFrame = Mathf.Max(m_Director.CurrentFrame - 1, 0);
                }

                if (isPlaying)
                {
                    if (GUILayout.Button("Stop", EditorStyles.toolbarButton, GUILayout.Width(ButtonWidth)))
                    {
                        m_Director.Stop();
                    }
                }
                else
                {
                    if (GUILayout.Button("Play", EditorStyles.toolbarButton, GUILayout.Width(ButtonWidth)))
                    {
                        m_Director.Play();
                        m_LatestTickTime = EditorApplication.timeSinceStartup;
                    }
                }
                if (GUILayout.Button(">", EditorStyles.toolbarButton, GUILayout.Width(ButtonWidth)))
                {
                    m_Director.CurrentFrame = Mathf.Min(m_Director.CurrentFrame + 1, m_Director.TotalFrame);
                }
                if (GUILayout.Button(">>", EditorStyles.toolbarButton, GUILayout.Width(ButtonWidth)))
                {
                    m_Director.CurrentFrame = m_Director.TotalFrame;
                }
            }
        }

        State CheckState()
        {
            if (m_Director == null)
                return State.Disable;

            if (m_Director.Sequence == null)
                return State.NoSequence;

            if (m_Director.Status == SequenceStatus.Initial)
                return State.NeedPrepare;

            if(m_Director is EditorDirector)
                return State.EditOnly;

            return State.Playable;
        }

        void OnUpdate()
        {
            Repaint();

            if (m_Director == null)
                return;

            if (m_Director.Status != SequenceStatus.Playing)
                return;

            var delta = EditorApplication.timeSinceStartup - m_LatestTickTime;
            m_Director.Tick((float)delta);
            m_LatestTickTime = EditorApplication.timeSinceStartup;
            Repaint();
        }

        public void ChangeData()
        {
            if (m_Director == null)
                return;

            var isPlaying = m_Director.Status == SequenceStatus.Playing;
            var current = m_Director.CurrentTime;
            m_Director.Prepare(mode: TickMode.Manual);
            if (isPlaying)
            {
                m_Director.Play(current);
            }
        }
    }
}