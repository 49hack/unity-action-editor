using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [System.Serializable]
    public class ClipBehaviourEditor
    {
        public static ClipBehaviourEditor Create(System.Type type, ClipBehaviour clip)
        {
            var editor = System.Activator.CreateInstance(type) as ClipBehaviourEditor;
            editor.Initialize(clip);
            return editor;
        }

        enum DragType { None, Min, Max, Range }

        [SerializeField] ClipBehaviour m_Clip;

        SerializedObject m_SerializedObject;
        DragType m_DragType;
        IReadOnlyList<Blackboard> m_BlackboardList;

        public event System.Action OnChangeData;
        public event System.Action<ClipBehaviourEditor> OnRemoveClip;

        #region Virtual
        protected virtual Color BackgroundColor { get { return new Color(0f, 0f, 0f, 0.25f); } }
        protected virtual void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            EditorGUI.DrawRect(rect, BackgroundColor);
        }

        #endregion // Virtual

        public ClipBehaviour Asset { get { return m_Clip; } }

        void Initialize(ClipBehaviour clip)
        {
            m_Clip = clip;
        }

        SerializedObject SerializedObject
        {
            get
            {
                if (m_SerializedObject == null)
                {
                    m_SerializedObject = new SerializedObject(Asset);
                }

                return m_SerializedObject;
            }
        }


        public float BeginFrame
        {
            get
            {
                var prop = SerializedObject.FindProperty(ClipBehaviour.PropNameBeginFrame);
                return prop.floatValue;
            }
            set
            {
                SerializedObject.Update();
                var prop = SerializedObject.FindProperty(ClipBehaviour.PropNameBeginFrame);
                prop.floatValue = value;
                SerializedObject.ApplyModifiedProperties();
            }
        }
        public float EndFrame
        {
            get
            {
                var prop = SerializedObject.FindProperty(ClipBehaviour.PropNameEndFrame);
                return prop.floatValue;
            }
            set
            {
                SerializedObject.Update();
                var prop = SerializedObject.FindProperty(ClipBehaviour.PropNameEndFrame);
                prop.floatValue = value;
                SerializedObject.ApplyModifiedProperties();
            }
        }

        public void Enable()
        {

        }

        public void Disable()
        {

        }

        public void Dispose()
        {

        }

        void ChangeData()
        {
            OnChangeData?.Invoke();
        }

        void OnClickDelete(object obj)
        {
            OnRemoveClip?.Invoke(this);
        }

        public void Draw(Rect viewRect, ClipViewInfo info, Navigator navigator, float totalFrame, float currentFrame, IReadOnlyList<Blackboard> blackboards)
        {
            if (Asset == null)
                return;
            if (SerializedObject == null)
                return;

            m_BlackboardList = blackboards;

            var beginFrame = BeginFrame;
            var endFrame = EndFrame;

            var fullRect = new Rect(info.Min, viewRect.y + 2f, info.Max - info.Min, viewRect.height - 4f);
            if (endFrame <= navigator.MinFrame || beginFrame >= navigator.MaxFrame)
            {
                return;
            }
            
            var edgeRect = Utility.CalculateEdgeRects(fullRect, 8f);

            EditorGUIUtility.AddCursorRect(edgeRect.left, MouseCursor.SplitResizeLeftRight);
            EditorGUIUtility.AddCursorRect(edgeRect.right, MouseCursor.SplitResizeLeftRight);

            var contentRect = new Rect(info.ContentMin + 2f, fullRect.y + 2f, info.ContentMax - info.ContentMin - 4f, fullRect.height - 4f);
            ClipViewUtility.DrawClip(fullRect, info, BackgroundColor);

            SerializedObject.Update();
            DrawContents(contentRect, SerializedObject);
            SerializedObject.ApplyModifiedProperties();

            var dragCtrlId = GUIUtility.GetControlID(FocusType.Passive);
            var e = Event.current;
            switch(e.type)
            {
                case EventType.MouseDown:
                    {
                        if (edgeRect.left.Contains(e.mousePosition))
                        {
                            m_DragType = DragType.Min;
                            GUIUtility.hotControl = dragCtrlId;
                            e.Use();
                            break;
                        }

                        if (edgeRect.right.Contains(e.mousePosition))
                        {
                            m_DragType = DragType.Max;
                            GUIUtility.hotControl = dragCtrlId;
                            e.Use();
                            break;
                        }

                        if (contentRect.Contains(e.mousePosition) && e.button == 1)
                        {
                            ShowContextMenu();
                            e.Use();
                            break;
                        }

                        if (contentRect.Contains(e.mousePosition))
                        {
                            Selection.objects = new Object[] { Asset };
                            m_DragType = DragType.Range;
                            GUIUtility.hotControl = dragCtrlId;
                            e.Use();
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (dragCtrlId == GUIUtility.hotControl)
                    {
                        switch (m_DragType)
                        {
                            case DragType.Min:
                                {
                                    var next = Utility.Remap(e.mousePosition.x, viewRect.xMin, viewRect.xMax, navigator.MinFrame, navigator.MaxFrame);
                                    BeginFrame = ClipViewUtility.Adjust(next, viewRect, navigator);
                                    BeginFrame = Mathf.Min(BeginFrame, EndFrame);
                                    BeginFrame = Mathf.Max(BeginFrame, info.StopMin);
                                }
                                break;

                            case DragType.Max:
                                {
                                    var next = Utility.Remap(e.mousePosition.x, viewRect.xMin, viewRect.xMax, navigator.MinFrame, navigator.MaxFrame);
                                    EndFrame = ClipViewUtility.Adjust(next, viewRect, navigator);
                                    EndFrame = Mathf.Max(BeginFrame, EndFrame);
                                    EndFrame = Mathf.Min(EndFrame, info.StopMax);
                                }
                                break;

                            case DragType.Range:
                                {
                                    var delta = Utility.Remap(e.delta.x, 0f, viewRect.width, 0f, navigator.Range);
                                    if (0f > delta)
                                    {
                                        if (0f > BeginFrame + delta)
                                        {
                                            delta = -BeginFrame;
                                        }
                                    }
                                    else
                                    {
                                        if (totalFrame < EndFrame + delta)
                                        {
                                            delta = totalFrame - EndFrame;
                                        }
                                    }
                                    BeginFrame += delta;
                                    EndFrame += delta;

                                    BeginFrame = Mathf.Max(BeginFrame, info.StopMin);
                                    EndFrame = Mathf.Min(EndFrame, info.StopMax);

                                    if(info.HasPrev)
                                    {
                                        EndFrame = Mathf.Max(EndFrame, info.StopMax2);
                                    }
                                    if(info.HasNext)
                                    {
                                        BeginFrame = Mathf.Min(BeginFrame, info.StopMin2);
                                    }
                                }
                                break;
                        }
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                case EventType.Ignore:
                    if (dragCtrlId == GUIUtility.hotControl)
                    {
                        BeginFrame = ClipViewUtility.Adjust(BeginFrame, viewRect, navigator);
                        EndFrame = ClipViewUtility.Adjust(EndFrame, viewRect, navigator);

                        m_DragType = DragType.None;
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }
                    break;
            }
        }

        void ShowContextMenu()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, OnClickDelete, null);

            menu.ShowAsContext();
        }

        protected void DrawContext(Rect position, SerializedProperty property, GUIContent label, System.Type type)
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                BlackboardEditorGUI.DrawContext(m_BlackboardList, position, property, label, type);
                if (check.changed)
                {
                    ChangeData();
                }
            }
        }
    }
}