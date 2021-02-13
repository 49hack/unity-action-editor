using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [System.Serializable]
    public class Navigator
    {
        const double DubleClickInterval = 0.25f;
        const float DoubleClipDistance = 2f;
        const float FocusMerginFrame = 0.2f;

        enum DragType { None, Min, Max, Range}

        [SerializeField] float m_MinFrame;
        [SerializeField] float m_MaxFrame;
        [SerializeField] float m_SavedMinFrame;
        [SerializeField] float m_SavedMaxFrame;

        Rect m_BackRect;
        DragType m_DragType;
        double m_PressedTime;
        Vector2 m_PressedPosition;

        public float MaxFrame { get { return m_MaxFrame; } }
        public float MinFrame { get { return m_MinFrame; } }
        public float Range { get { return MaxFrame - MinFrame; } }
        public float MinPosition { get { return m_BackRect.xMin; } }
        public float MaxPosition { get { return m_BackRect.xMax; } }
        public float Width { get { return m_BackRect.width; } }
        public float FrameWidth { get { return Width / Range; } }
        public Rect Rect { get { return m_BackRect; } }

        public float ToFrame(float position)
        {
            return Utility.Remap(position, m_BackRect.xMin, m_BackRect.xMax, MinFrame, MaxFrame);
        }
        public float ToPosition(float frame)
        {
            return Utility.Remap(frame, MinFrame, MaxFrame, m_BackRect.xMin, m_BackRect.xMax);
        }

        public void OnGUI(float totalFrame, float durationFrame, float currentFrame)
        {
            if(m_MinFrame == m_MaxFrame)
            {
                m_MinFrame = 0f;
                m_MaxFrame = totalFrame;
            }

            var rect = GUILayoutUtility.GetRect(1f, 24f, GUILayout.ExpandWidth(true));
            m_BackRect = new Rect(rect.x, rect.y, rect.width - Utility.ScrollBarWidth, rect.height);
            m_BackRect.x += Utility.HeaderWidth + Utility.Space;
            m_BackRect.width -= Utility.HeaderWidth + Utility.Space * 2f;
            m_BackRect.y += 2f;
            m_BackRect.height -= 4f;

            var minRange = m_BackRect.width * 0.01f;
            minRange = Mathf.Min(minRange, totalFrame);

            m_MinFrame = Mathf.Clamp(m_MinFrame, 0f, totalFrame);
            m_MaxFrame = Mathf.Clamp(m_MaxFrame, m_MinFrame + minRange, totalFrame);

            if(totalFrame < m_MaxFrame)
            {
                var over = m_MaxFrame - totalFrame;
                m_MaxFrame -= over;
                m_MinFrame -= over;
            }

            var xMin = m_BackRect.xMin;
            var xMax = m_BackRect.xMax;
            var minPos = Utility.Remap(m_MinFrame, 0f, totalFrame, xMin, xMax);
            var maxPos = Utility.Remap(m_MaxFrame, 0f, totalFrame, xMin, xMax);

            var rangeRect = new Rect(minPos, m_BackRect.y, maxPos - minPos, m_BackRect.height);
            var edgeRect = Utility.CalculateEdgeRects(rangeRect, 8f);

            EditorGUIUtility.AddCursorRect(edgeRect.left, MouseCursor.SplitResizeLeftRight);
            EditorGUIUtility.AddCursorRect(edgeRect.right, MouseCursor.SplitResizeLeftRight);

            var dragCtrlId = GUIUtility.GetControlID(FocusType.Passive);
            var e = Event.current;
            switch(e.type)
            {
                case EventType.Repaint:
                    {
                        GUI.skin.box.Draw(m_BackRect, GUIContent.none, 0);
                        using(new Utility.ColorScope(new Color(0f, 0f, 0f, 0.5f)))
                        {
                            GUI.skin.box.Draw(rangeRect, GUIContent.none, 0);
                        }
                        Utility.DrawEdgeLabels(rangeRect, m_MinFrame.ToString("F1"), m_MaxFrame.ToString("F1"));

                        if(currentFrame >= 0f && totalFrame >= currentFrame)
                        {
                            var rate = currentFrame / totalFrame;
                            var x = m_BackRect.width * rate + m_BackRect.x;
                            Handles.color = Color.red;
                            Handles.DrawLine(new Vector2(x, m_BackRect.y), new Vector2(x, m_BackRect.max.y));
                        }

                        {
                            var x = Utility.Remap(durationFrame, 0f, totalFrame, xMin, xMax) + 1f;
                            x = Mathf.Max(xMin, x);
                            var r = new Rect(x, m_BackRect.yMin, xMax - x, m_BackRect.height);
                            EditorGUI.DrawRect(r, Utility.DisableColor);
                        }
                    }
                    break;

                case EventType.MouseDown:
                    {
                        if(edgeRect.left.Contains(e.mousePosition))
                        {
                            m_DragType = DragType.Min;
                            GUIUtility.hotControl = dragCtrlId;
                            e.Use();
                            break;
                        }

                        if(edgeRect.right.Contains(e.mousePosition))
                        {
                            m_DragType = DragType.Max;
                            GUIUtility.hotControl = dragCtrlId;
                            e.Use();
                            break;
                        }

                        if(rangeRect.Contains(e.mousePosition))
                        {
                            m_DragType = DragType.Range;
                            var time = EditorApplication.timeSinceStartup;

                            if(DubleClickInterval > time - m_PressedTime && DoubleClipDistance > Vector2.Distance(m_PressedPosition, e.mousePosition))
                            {
                                if(m_MinFrame != 0f || totalFrame != m_MaxFrame)
                                {
                                    m_SavedMinFrame = m_MinFrame;
                                    m_SavedMaxFrame = m_MaxFrame;
                                    m_MinFrame = 0f;
                                    m_MaxFrame = totalFrame;
                                } else
                                {
                                    m_MinFrame = m_SavedMinFrame;
                                    m_MaxFrame = m_SavedMaxFrame;
                                }
                            }
                            else
                            {
                                m_PressedTime = time;
                                m_PressedPosition = e.mousePosition;
                            }
                            GUIUtility.hotControl = dragCtrlId;
                            e.Use();
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (dragCtrlId == GUIUtility.hotControl)
                    {
                        switch(m_DragType)
                        {
                            case DragType.Min:
                                {
                                    m_MinFrame = Utility.Remap(e.mousePosition.x, xMin, xMax, 0f, totalFrame);
                                    m_MinFrame = Mathf.Clamp(m_MinFrame, 0f, m_MaxFrame - minRange);
                                }
                                break;

                            case DragType.Max:
                                {
                                    m_MaxFrame = Utility.Remap(e.mousePosition.x, xMin, xMax, 0f, totalFrame);
                                    m_MaxFrame = Mathf.Clamp(m_MaxFrame, m_MinFrame + minRange, totalFrame);
                                }
                                break;

                            case DragType.Range:
                                {
                                    var delta = Utility.Remap(e.delta.x, 0f, m_BackRect.width, 0f, totalFrame);
                                    if (0f > delta)
                                    {
                                        if (0f > m_MinFrame + delta)
                                        {
                                            delta = -m_MinFrame;
                                        }
                                    }
                                    else
                                    {
                                        if (totalFrame < m_MaxFrame + delta)
                                        {
                                            delta = totalFrame - m_MaxFrame;
                                        }
                                    }
                                    m_MinFrame += delta;
                                    m_MaxFrame += delta;
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
                        m_DragType = DragType.None;
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }
                    break;
            }
        }

        public void Focus(float totalFrame, float focusFrame)
        {
            var slide = 0f;
            if(focusFrame < m_MinFrame)
            {
                var target = Mathf.Clamp(focusFrame - FocusMerginFrame, 0f, totalFrame);
                slide = target - m_MinFrame;
            }
            else if(focusFrame > m_MaxFrame)
            {
                var target = Mathf.Clamp(focusFrame + FocusMerginFrame, 0f, totalFrame);
                slide = target - m_MaxFrame;
            }
            m_MinFrame += slide;
            m_MaxFrame += slide;
        }
    }
}