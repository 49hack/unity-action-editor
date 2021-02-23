using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public struct ClipViewInfo
    {
        public float ContentMin { get; private set; }
        public float ContentMax { get; private set; }
        public float Min { get; private set; }
        public float Max { get; private set; }

        public float StopMin { get; private set; }
        public float StopMax { get; private set; }
        public float StopMin2 { get; private set; }
        public float StopMax2 { get; private set; }

        public bool HasPrev { get; private set; }
        public bool HasNext { get; private set; }

        public ClipViewInfo(ClipBehaviour clip, ClipBehaviour prev, ClipBehaviour next, ActionEditorTime editorTime, float tolalFrame, Rect fullRect)
        {
            ContentMin = 0f;
            ContentMax = 0f;
            Min = 0f;
            Max = 0f;
            StopMin = 0f;
            StopMax = tolalFrame;
            StopMin2 = 0f;
            StopMax2 = tolalFrame;
            HasPrev = prev != null;
            HasNext = next != null;

            var value = Calculate(clip, editorTime);
            Min = value.min;
            Max = value.max;
            ContentMin = Min;
            ContentMax = Max;

            if(prev != null)
            {
                var prevValue = Calculate(prev, editorTime);
                if(prevValue.max > ContentMin)
                {
                    ContentMin = prevValue.max;
                }

                StopMin = ClipViewUtility.Adjust(editorTime.ToFrame(prevValue.min), fullRect, editorTime, 1);

                StopMax2 = ClipViewUtility.Adjust(editorTime.ToFrame(prevValue.max), fullRect, editorTime, 1);
            }

            if(next != null)
            {
                var nextValue = Calculate(next, editorTime);
                if (nextValue.min < ContentMax)
                {
                    ContentMax = nextValue.min;
                }

                StopMax = ClipViewUtility.Adjust(editorTime.ToFrame(nextValue.max), fullRect, editorTime, -1);
                StopMin2 = ClipViewUtility.Adjust(editorTime.ToFrame(nextValue.min), fullRect, editorTime, -1);
            }
        }

        static (float min, float max) Calculate(ClipBehaviour clip, ActionEditorTime editorTime)
        {
            var beginFrame = clip.BeginFrame;
            var endFrame = clip.EndFrame;

            beginFrame = Mathf.Max(beginFrame, editorTime.MinFrame);
            endFrame = Mathf.Min(endFrame, editorTime.MaxFrame);

            var min = editorTime.ToPosition(beginFrame);
            var max = editorTime.ToPosition(endFrame);

            return (min, max);
        }

    }

    public static class ClipViewUtility
    {
        public static void DrawClip(Rect fullRect, ClipViewInfo info, Color color)
        {
            using (new Utility.HandlesColorScope(color))
            {
                var left = info.ContentMin;
                var right = info.ContentMax;
                float top = fullRect.y;
                var bottom = fullRect.yMax;
                var leftTop = new Vector3(left, top);
                var leftBottom = new Vector3(left, bottom);
                var rightTop = new Vector3(right, top);
                var rightBottom = new Vector3(right, bottom);

                Utility.FillTriangle(leftBottom, leftTop, rightTop, color);
                Utility.FillTriangle(leftBottom, rightTop, rightBottom, color);

                if (info.Min < info.ContentMin)
                {
                    var leftBottom2 = new Vector3(info.Min, bottom);
                    Utility.FillTriangle(leftBottom2, leftTop, leftBottom, color);
                }

                if (info.Max > info.ContentMax)
                {
                    var rightBottom2 = new Vector3(info.Max, bottom);
                    Utility.FillTriangle(rightBottom, rightTop, rightBottom2, color);
                }
            }
        }

        public static float Adjust(float next, Rect viewRect, ActionEditorTime editorTime, int offset = 0)
        {
            var intervalFrame = Utility.CalculateFrameInterval(editorTime.MinFrame, editorTime.MaxFrame, viewRect.xMin, viewRect.xMax, 1f / Utility.SubIndicateInterval);
            var current = intervalFrame * Mathf.RoundToInt(next / intervalFrame) + offset;

            var adjusted = Utility.Remap(current, editorTime.MinFrame, editorTime.MaxFrame, viewRect.xMin, viewRect.xMax);
            next = Utility.Remap(adjusted, viewRect.xMin, viewRect.xMax, editorTime.MinFrame, editorTime.MaxFrame);

            return next;
        }
    }

}