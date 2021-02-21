using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace ActionEditor
{
    public static class Utility
    {
        public static float IndicateInterval = 48f;
        public static float SubIndicateInterval = 6f;
        public static float HeaderWidth = 250f;
        public static float ScrollBarWidth = 12f;
        public static float Space = 4f;
        public static Color DisableColor = new Color(0f, 0f, 0f, 0.25f);

        public static float Remap(float value, float min0, float max0, float min1, float max1)
        {
            return ToValue(min1, max1, ToRate(value, min0, max0));
        }

        public static float ToRate(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }
        public static float ToValue(float min, float max, float rate)
        {
            return rate * (max - min) + min;
        }

        public static void Indicate(float start, float end, int offset, int interval, float min, float max, System.Action<float, int> onIndicate)
        {
            var current = interval * Mathf.CeilToInt(start / interval) + offset;
            var last = interval * Mathf.CeilToInt(end / interval) + offset;
            while(current < last)
            {
                var x = Utility.Remap(current, start, end, min, max);
                onIndicate?.Invoke(x, current);
                current += interval;
            }
        }

        public static void FillTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color)
        {
            using (new HandlesColorScope(color))
            {
                Handles.BeginGUI();
                Handles.DrawAAConvexPolygon(new Vector3[] { p1, p2, p3 });
                Handles.EndGUI();
            }
        }

        public static void DrawWireRect(Rect rect, Color color)
        {
            using(new HandlesColorScope(color))
            {
                Handles.BeginGUI();

                var left = rect.x;
                var right = rect.xMax - 1f;
                float top = rect.y + 1f;
                var bottom = rect.yMax;
                var leftTop = new Vector3(left, top);
                var leftBottom = new Vector3(left, bottom);
                var rightTop = new Vector3(right, top);
                var rightBottom = new Vector3(right, bottom);
                Handles.DrawLine(leftBottom, leftTop);
                Handles.DrawLine(leftTop, rightTop);
                Handles.DrawLine(rightTop, rightBottom);
                Handles.DrawLine(rightBottom, leftBottom);

                Handles.EndGUI();
            }
        }

        public static (Rect left, Rect right) CalculateEdgeRects(Rect rect, float width)
        {
            var halfWidth = width * 0.5f;
            float dent = Mathf.Min(halfWidth, (rect.width - halfWidth) * 0.5f);
            var left = new Rect() { x = rect.x - halfWidth, y = rect.y, width = halfWidth + dent, height = rect.height };
            var right = new Rect() { x = (rect.x + rect.width) - dent, y = rect.y, width = halfWidth + dent, height = rect.height };
            return (left, right);
        }

        public static void DrawEdgeLabels(Rect rect, string leftLabel, string rightLabel)
        {
            var leftStyle = new GUIStyle(GUI.skin.label);
            var leftContent = new GUIContent(leftLabel);
            var leftSize = leftStyle.CalcSize(leftContent);

            var rightStyle = new GUIStyle(GUI.skin.label);
            var rightContent = new GUIContent(rightLabel);
            var rightSize = rightStyle.CalcSize(rightContent);

            if(rect.width > leftSize.x + rightSize.x)
            {
                leftStyle.alignment = TextAnchor.MiddleLeft;
                GUI.Label(rect, leftContent, leftStyle);

                rightStyle.alignment = TextAnchor.MiddleRight;
                GUI.Label(rect, rightContent, rightStyle);
                return;
            }

            if(rect.width > leftSize.x)
            {
                leftStyle.alignment = TextAnchor.MiddleLeft;
                GUI.Label(rect, leftContent, leftStyle);

                rightStyle.alignment = TextAnchor.MiddleLeft;
                GUI.Label(new Rect() { x = rect.x + rect.width, y = rect.y, width = rightSize.x, height = rect.height }, rightContent, rightStyle);
                return;
            }

            leftStyle.alignment = TextAnchor.MiddleRight;
            GUI.Label(new Rect() { x = rect.x - leftSize.x, y = rect.y, width = leftSize.x, height = rect.height }, leftContent, leftStyle);

            rightStyle.alignment = TextAnchor.MiddleLeft;
            GUI.Label(new Rect() { x = rect.x + rect.width, y = rect.y, width = rightSize.x, height = rect.height }, rightContent, rightStyle);
        }

        static readonly int[] FrameIntervals = new int[] { 6000, 4800, 2400, 1200, 600, 480, 240, 120, 60, 30, 15, 10, 5, 2, 1, 0 };
        public static int CalculateFrameInterval(float minFrame, float maxFrame, float min, float max, float recPixelInterval)
        {
            var splitCount = (max - min) * recPixelInterval;
            var minFrameInterval = (maxFrame - minFrame) / splitCount;
            var frameInterval = Mathf.CeilToInt(minFrameInterval);
            for(int i = 0; i < FrameIntervals.Length; i++)
            {
                if(FrameIntervals[i] < frameInterval)
                {
                    frameInterval = FrameIntervals[i - 1];
                    break;
                }
            }
            return frameInterval;
        }

        public static int IndexOf(SerializedProperty prop, Object obj)
        {
            for (int i = 0; i < prop.arraySize; i++)
            {
                var item = prop.GetArrayElementAtIndex(i);
                if (item.objectReferenceValue == obj)
                {
                    return i;
                }
            }

            return -1;
        }
        public static void RemoveAt(SerializedProperty prop, int index)
        {
            prop.DeleteArrayElementAtIndex(index);

            var count = prop.arraySize;
            for (int i = index; i < count - 1; i++)
            {
                var current = prop.GetArrayElementAtIndex(i);
                var next = prop.GetArrayElementAtIndex(i + 1);

                current.objectReferenceValue = next.objectReferenceValue;
            }
            prop.arraySize = count - 1;
        }

        public static void ForEach(SerializedObject so, System.Action<SerializedProperty> callback)
        {
            var iter = so.GetIterator();
            iter.NextVisible(true);
            while (iter.NextVisible(true))
            {
                var o = iter;
                callback?.Invoke(o);
            }
        }

        public static List<FieldInfo> CollectFields<T>(object obj)
        {
            List<FieldInfo> result = new List<FieldInfo>();
            if (obj == null)
                return result;

            var fieldInfos = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            var targetType = typeof(T);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                var fieldInfo = fieldInfos[i];
                var type = fieldInfo.FieldType;
                if (!targetType.IsAssignableFrom(type))
                    continue;

                result.Add(fieldInfo);
            }

            return result;
        }

        public static System.Type[] GetSubClasses<T>()
        {
            List<System.Type> result = new List<System.Type>();
            
            foreach(var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsSubclassOf(typeof(T)))
                        continue;

                    if (type.IsAbstract)
                        continue;

                    result.Add(type);
                }
            }
            return result.ToArray();
        }

        public static T GetAttribute<T>(System.Type type) where T : System.Attribute
        {
            return type.GetCustomAttribute<T>();
        }

        public static string TransformToPath(Transform transform, string path, Transform endpoint)
        {
            if (transform == endpoint)
                return path;

            path = transform.name + (string.IsNullOrEmpty(path) ? "" : "/") + path;

            if (transform.parent == null)
                return path;

            return TransformToPath(transform.parent, path, endpoint);
        }

        public class ColorScope : System.IDisposable
        {
            Color m_Cache;

            public ColorScope(Color color)
            {
                m_Cache = GUI.color;
                GUI.color = color;
            }

            public void Dispose()
            {
                GUI.color = m_Cache;
            }
        }

        public class LabelWidthScope : System.IDisposable
        {
            float m_Width;

            public LabelWidthScope(float width)
            {
                m_Width = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = width;
            }

            public void Dispose()
            {
                EditorGUIUtility.labelWidth = m_Width;
            }
        }

        public class HandlesColorScope : System.IDisposable
        {
            Color m_Cache;

            public HandlesColorScope(Color color)
            {
                m_Cache = Handles.color;
                Handles.color = color;
            }

            public void Dispose()
            {
                Handles.color = m_Cache;
            }
        }
    }
}