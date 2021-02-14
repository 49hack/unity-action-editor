using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [System.Serializable]
    public class Indicator
    {
        List<GUIContent> m_StepLabels;
        float m_StepFrameRate;
        List<Vector3> m_IndicatePointList = new List<Vector3>();
        Vector3[] m_IndicatePoints;
        Rect m_ControlRect;

        public float OnGUI(float totalFrame, float durationFrame, float currentFrame, float frameRate, float startFrame, float endFrame, System.Action<float, float> onFocus)
        {
            var e = Event.current;

            var rectTemp = GUILayoutUtility.GetRect(1f, 45f, GUILayout.ExpandWidth(true));
            var rect = new Rect(rectTemp.x, rectTemp.y, rectTemp.width - Utility.ScrollBarWidth, rectTemp.height);
            var headerRect = new Rect(rect.x + 4f, rect.y + 2f, Utility.HeaderWidth - 4f, rect.height - 4f);

            var isOutFrame = currentFrame < 0f || totalFrame < currentFrame;
            using(new Utility.ColorScope(isOutFrame ? Color.gray : GUI.color))
            using (new Utility.LabelWidthScope(64f))
            {
                var currentTime = 0f;
                var durationTime = 0f;
                if (frameRate != 0f)
                {
                    var recFrameRate = 1f / frameRate;
                    currentTime = currentFrame * recFrameRate;
                    durationTime = durationFrame * recFrameRate;
                }

                var secRect = new Rect(headerRect.x, headerRect.y, headerRect.width, headerRect.height * 0.5f);
                var newTimeCurrent = EditorGUI.DelayedFloatField(secRect, "Seconds", currentTime);
                if (newTimeCurrent != currentTime)
                {
                    newTimeCurrent = Mathf.Clamp(newTimeCurrent, 0f, durationTime);
                    currentTime = newTimeCurrent * frameRate;
                    onFocus?.Invoke(totalFrame, currentFrame);
                }

                var frameRect = new Rect(headerRect.x, headerRect.y + secRect.height, headerRect.width, headerRect.height * 0.5f);
                var newFrameCurrent = EditorGUI.DelayedIntField(frameRect, "Frame", (int)currentFrame);
                if (newFrameCurrent != (int)currentFrame)
                {
                    newFrameCurrent = Mathf.Clamp(newFrameCurrent, 0, (int)durationFrame);
                    currentFrame = newFrameCurrent;
                    onFocus?.Invoke(totalFrame, currentFrame);
                }
            }

            m_ControlRect = new Rect(
                headerRect.x + headerRect.width + Utility.Space,
                rect.y + 2f,
                rect.width - (headerRect.x + headerRect.width) - Utility.Space * 2f,
                rect.height - 4f
                );

            var halfHeight = m_ControlRect.height * 0.5f;

            var xMin = m_ControlRect.x;
            var xMax = m_ControlRect.xMax;
            var yMin = m_ControlRect.y;
            var yMax = m_ControlRect.yMax;

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            switch (e.type)
            {
                case EventType.Repaint:
                    {
                        GUI.skin.box.Draw(m_ControlRect, GUIContent.none, 0);

                        using(new Utility.HandlesColorScope(Color.white))
                        {
                            var intervalFrame = Utility.CalculateFrameInterval(startFrame, endFrame, xMin, xMax, 1f / Utility.IndicateInterval);
                            if(1 < intervalFrame)
                            {
                                var subIntervalFrame = Utility.CalculateFrameInterval(startFrame, endFrame, xMin, xMax, 1f / Utility.SubIndicateInterval);
                                Utility.Indicate(startFrame, endFrame, 0, Mathf.Max(subIntervalFrame, 1), xMin, xMax, (x, f) => {
                                    m_IndicatePointList.Add(new Vector3(x, yMax - 1f));
                                    m_IndicatePointList.Add(new Vector3(x, yMax));
                                });
                            }

                            var frameNum = Mathf.CeilToInt(totalFrame);
                            if (m_StepLabels == null)
                                m_StepLabels = new List<GUIContent>();
                            if(m_StepLabels.Count != frameNum || m_StepFrameRate != frameRate)
                            {
                                m_StepLabels.Clear();
                                m_StepFrameRate = frameRate;
                                for(int frameNo = 0; frameNo < frameNum; frameNo++)
                                {
                                    if(0f != frameRate)
                                    {
                                        var stepLabel = new GUIContent(string.Format("{0}{1}{2}", frameNo, System.Environment.NewLine, (frameNo / frameRate).ToString("F3")));
                                        m_StepLabels.Add(stepLabel);
                                    } else
                                    {
                                        var stepLabel = new GUIContent(string.Format("{0}{1}0.000", frameNo, System.Environment.NewLine));
                                        m_StepLabels.Add(stepLabel);
                                    }
                                }
                            }

                            Utility.Indicate(startFrame, endFrame, 0, Mathf.Max(intervalFrame, 1), xMin, xMax, (x, f) => {
                                m_IndicatePointList.Add(new Vector3(x, yMax - halfHeight));
                                m_IndicatePointList.Add(new Vector3(x, yMax));
                                GUI.Label(new Rect(x, yMin, 64f, m_ControlRect.height), m_StepLabels[f]);
                            });

                            var indicatePointNum = m_IndicatePointList.Count;
                            if(m_IndicatePoints == null || indicatePointNum != m_IndicatePoints.Length)
                            {
                                m_IndicatePoints = m_IndicatePointList.ToArray();
                            }
                            else
                            {
                                for(int i = 0; i < indicatePointNum; i++)
                                {
                                    m_IndicatePoints[i] = m_IndicatePointList[i];
                                }
                            }

                            Handles.DrawLines(m_IndicatePoints);
                            m_IndicatePointList.Clear();

                            using(new Utility.HandlesColorScope(Color.red))
                            {
                                if(startFrame <= currentFrame && endFrame >= currentFrame)
                                {
                                    float x = Utility.Remap(currentFrame, startFrame, endFrame, xMin, xMax);
                                    Handles.DrawLine(new Vector2(x, yMin), new Vector2(x, yMax));
                                }

                                if(durationFrame < endFrame)
                                {
                                    float x = Utility.Remap(durationFrame, startFrame, endFrame, xMin, xMax) + 1f;
                                    x = Mathf.Max(xMax, x);
                                    var r = new Rect(x, yMin, xMax - x, m_ControlRect.height);
                                    EditorGUI.DrawRect(r, Utility.DisableColor);
                                }
                            }
                        }
                    }
                    break;

                case EventType.MouseDown:
                    if(e.button == 0 && m_ControlRect.Contains(e.mousePosition))
                    {
                        GUIUtility.hotControl = controlId;
                        currentFrame = Utility.Remap(e.mousePosition.x, xMin, xMax, startFrame, endFrame);
                        var lastFrame = Mathf.Min(endFrame, durationFrame);
                        currentFrame = Mathf.Clamp(currentFrame, startFrame, lastFrame);
                        onFocus?.Invoke(totalFrame, currentFrame);
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if(e.button == 0 && controlId == GUIUtility.hotControl)
                    {
                        currentFrame = Utility.Remap(e.mousePosition.x, xMin, xMax, startFrame, endFrame);
                        var lastFrame = Mathf.Min(endFrame, durationFrame);
                        currentFrame = Mathf.Clamp(currentFrame, startFrame, lastFrame);
                        onFocus?.Invoke(totalFrame, currentFrame);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                case EventType.Ignore:
                    if(e.button == 0 && controlId == GUIUtility.hotControl)
                    {
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }
                    break;
            }

            return currentFrame;
        }
    }
}