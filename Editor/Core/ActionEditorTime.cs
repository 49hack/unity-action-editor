using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class ActionEditorTime
    {
        Navigator m_Navigator;
        Indicator m_Indicator;

        public ActionEditorTime(Navigator navigator, Indicator indicator)
        {
            m_Navigator = navigator;
            m_Indicator = indicator;
        }

        public float MaxFrame { get { return m_Navigator.MaxFrame; } }
        public float MinFrame { get { return m_Navigator.MinFrame; } }
        public float Range { get { return MaxFrame - MinFrame; } }
        public float MinPosition { get { return m_Navigator.Rect.xMin; } }
        public float MaxPosition { get { return m_Navigator.Rect.xMax; } }
        public float Width { get { return m_Navigator.Rect.width; } }
        public float FrameWidth { get { return Width / Range; } }
        public Rect Rect { get { return m_Navigator.Rect; } }
        public float FrameRate { get { return m_Indicator.FrameRate; } }

        public float ToFrame(float position)
        {
            return Utility.Remap(position, m_Navigator.Rect.xMin, m_Navigator.Rect.xMax, MinFrame, MaxFrame);
        }

        public float ToPosition(float frame)
        {
            return Utility.Remap(frame, MinFrame, MaxFrame, m_Navigator.Rect.xMin, m_Navigator.Rect.xMax);
        }
    }
}