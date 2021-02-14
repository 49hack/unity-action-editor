﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ActionEditor
{
    [System.Serializable]
    public class TrackEditor : ScriptableObject
    {
        [SerializeField] SequenceEditor m_Owner;
        [SerializeField] Track m_Track;
        [SerializeField] List<ClipEditor> m_ClipEditors = new List<ClipEditor>();

        SerializedObject m_SerializedObject;
        List<Vector3> m_IndicatePointList = new List<Vector3>();
        Vector3[] m_IndicatePoints;

        #region Virtual
        protected virtual Color BackgroundColor { get { return new Color(0f, 0f, 0f, 0.5f); } }

        protected virtual void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            var propName = serializedObject.FindProperty(Track.PropNameTrackName);
            var nameRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, EditorGUIUtility.singleLineHeight);
            propName.stringValue = EditorGUI.TextField(nameRect, propName.stringValue);
        }

        protected virtual void AddContextMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Delete"), false, OnClickDelete, null);
        }
        #endregion // Virtual

        public Track Asset { get { return m_Track; } }

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

        public void Initialize(SequenceEditor owner, Track track)
        {
            m_Owner = owner;
            m_Track = track;

            var propClips = SerializedObject.FindProperty(Track.PropNameClips);
            for (int i = 0; i < propClips.arraySize; i++)
            {
                var item = propClips.GetArrayElementAtIndex(i);
                var clip = (Clip)item.objectReferenceValue;
                var editor = CreateClipEditor(clip.GetType());
                editor.Initialize(this, clip);
                m_ClipEditors.Add(editor);
            }
        }

        public void ChangeData()
        {
            m_Owner.ChangeData();
        }

        void OnClickDelete(object obj)
        {
            var dump = m_ClipEditors.ToArray();
            for (int i = 0; i < dump.Length; i++)
            {
                RemoveClip(dump[i]);
            }

            m_Owner.RemoveTrack(this);
        }

        public void Draw(Navigator navigator, float totalFrame, float currentFrame)
        {
            if (Asset == null)
                return;

            if (SerializedObject == null)
                return;

            var rect = GUILayoutUtility.GetRect(1f, 64f, GUILayout.ExpandWidth(true));

            SerializedObject.Update();

            OnGUITrack(rect);
            OnGUIClip(rect, navigator, totalFrame, currentFrame);

            SerializedObject.ApplyModifiedProperties();
        }

        void OnGUITrack(Rect fullRect)
        {
            var rect = new Rect(fullRect.x + Utility.Space, fullRect.y + Utility.Space, Utility.HeaderWidth - Utility.Space * 2f, fullRect.height - Utility.Space * 2f);

            EditorGUI.DrawRect(rect, BackgroundColor);
            DrawContents(rect, SerializedObject);

            var e = Event.current;
            switch (e.type)
            {
                case EventType.ContextClick:
                    if (rect.Contains(e.mousePosition))
                    {
                        GenericMenu menu = new GenericMenu();
                        AddContextMenu(menu);
                        menu.ShowAsContext();
                        e.Use();
                    }
                    break;
            }
        }

        void OnGUIClip(Rect fullRect, Navigator navigator, float totalFrame, float currentFrame)
        {
            var rect = new Rect(navigator.Rect.x, fullRect.y + Utility.Space, navigator.Rect.width, fullRect.height - Utility.Space * 2f);

            var e = Event.current;
            switch (e.type)
            {
                case EventType.Repaint:
                    {
                        GUI.skin.box.Draw(rect, GUIContent.none, 0);

                        var xMin = rect.x;
                        var xMax = rect.xMax;
                        var yMin = rect.y;
                        var yMax = rect.yMax;

                        using (new Utility.HandlesColorScope(Color.gray))
                        {
                            if (m_IndicatePointList == null)
                                m_IndicatePointList = new List<Vector3>();

                            var intervalFrame = Utility.CalculateFrameInterval(navigator.MinFrame, navigator.MaxFrame, xMin, xMax, 1f / 48f);
                            if (1 < intervalFrame)
                            {
                                var subIntervalFrame = Utility.CalculateFrameInterval(navigator.MinFrame, navigator.MaxFrame, xMin, xMax, 1f / 6f);
                                Utility.Indicate(navigator.MinFrame, navigator.MaxFrame, 0, Mathf.Max(subIntervalFrame, 1), xMin, xMax, (x, f) =>
                                {
                                    m_IndicatePointList.Add(new Vector3(x, yMin));
                                    m_IndicatePointList.Add(new Vector3(x, yMax));
                                });
                            }

                            Utility.Indicate(navigator.MinFrame, navigator.MaxFrame, 0, Mathf.Max(intervalFrame, 1), xMin, xMax, (x, f) => {
                                m_IndicatePointList.Add(new Vector3(x, yMin));
                                m_IndicatePointList.Add(new Vector3(x, yMax));
                            });

                            var indicatePointNum = m_IndicatePointList.Count;
                            if (m_IndicatePoints == null || indicatePointNum != m_IndicatePoints.Length)
                            {
                                m_IndicatePoints = m_IndicatePointList.ToArray();
                            }
                            else
                            {
                                for (int i = 0; i < indicatePointNum; i++)
                                {
                                    m_IndicatePoints[i] = m_IndicatePointList[i];
                                }
                            }

                            Handles.DrawLines(m_IndicatePoints);
                            m_IndicatePointList.Clear();
                        }

                        using (new Utility.HandlesColorScope(Color.red))
                        {
                            if (navigator.MinFrame <= currentFrame && navigator.MaxFrame >= currentFrame)
                            {
                                float x = Utility.Remap(currentFrame, navigator.MinFrame, navigator.MaxFrame, xMin, xMax);
                                Handles.DrawLine(new Vector2(x, fullRect.y), new Vector2(x, fullRect.yMax));
                            }
                        }
                    }
                    break;
            }

            var clipInfos = new List<ClipViewInfo>(m_ClipEditors.Count);
            for (int i = 0; i < m_ClipEditors.Count; i++)
            {
                var prev = (i - 1) >= 0 ? m_ClipEditors[i - 1].Asset : null;
                var current = m_ClipEditors[i].Asset;
                var next = (i + 1) < m_ClipEditors.Count ? m_ClipEditors[i + 1].Asset : null;
                clipInfos.Add(new ClipViewInfo(current, prev, next, navigator, totalFrame, rect));
            }
            for (int i = 0; i < m_ClipEditors.Count; i++)
            {
                m_ClipEditors[i].Draw(rect, clipInfos[i], navigator, totalFrame, currentFrame);
            }

            switch (e.type)
            {
                case EventType.ContextClick:
                    if (rect.Contains(e.mousePosition))
                    {
                        var beginFrame = Utility.Remap(e.mousePosition.x, rect.xMin, rect.xMax, navigator.MinFrame, navigator.MaxFrame);
                        ShowCreateClipContextMenu(beginFrame);
                        e.Use();
                    }
                    break;
            }
        }

        void ShowCreateClipContextMenu(float beginFrame)
        {
            var clipTypeList = GetClipTypeList();

            GenericMenu menu = new GenericMenu();

            for(int i = 0; i < clipTypeList.Length; i++)
            {
                var type = clipTypeList[i];
                var name = type.Name;
                var nameAttr = Utility.GetAttribute<MenuTitle>(type);
                if(nameAttr != null)
                {
                    name = nameAttr.Name;
                }

                menu.AddItem(new GUIContent(name), false, OnCreateClip, (type, beginFrame));
            }
            

            menu.ShowAsContext();
        }

        System.Type[] GetClipTypeList()
        {
            List<System.Type> result = new List<System.Type>();

            var clipTypeList = Utility.GetSubClasses<Clip>();
            for(int i = 0; i < clipTypeList.Length; i++)
            {
                var type = clipTypeList[i];
                var parentAttr = Utility.GetAttribute<ParentTrack>(type);
                if(parentAttr.Target != m_Track.GetType())
                {
                    continue;
                }

                result.Add(type);
            }

            return result.ToArray();
        }

        void OnCreateClip(object obj)
        {
            var param = ((System.Type type, float beginFrame))obj;

            SerializedObject.Update();

            var index = FindInsertIndex(param.beginFrame);

            var propClips = SerializedObject.FindProperty(Track.PropNameClips);
            propClips.InsertArrayElementAtIndex(index);
            var propClip = propClips.GetArrayElementAtIndex(index);

            var clip = (Clip)ScriptableObject.CreateInstance(param.type);
            clip.name = typeof(Clip).Name;
            clip.PostCreate(param.beginFrame);


            SerializedObject.ApplyModifiedProperties();

            AssetDatabase.AddObjectToAsset(clip, Asset);
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();

            propClip.objectReferenceValue = clip;

            SerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();

            var editor = CreateClipEditor(param.type);
            editor.Initialize(this, clip);
            if (index >= m_ClipEditors.Count)
            {
                m_ClipEditors.Add(editor);
            }
            else
            {
                m_ClipEditors.Insert(index, editor);
            }

            ChangeData();
        }

        ClipEditor CreateClipEditor(System.Type type)
        {
            var cutomEditor = GetCustomClipEditor(type);
            if (cutomEditor == null)
                return ScriptableObject.CreateInstance(typeof(ClipEditor)) as ClipEditor;

            return ScriptableObject.CreateInstance(cutomEditor) as ClipEditor;
        }

        System.Type GetCustomClipEditor(System.Type type)
        {
            var editorTypeList = Utility.GetSubClasses<ClipEditor>();
            for (int i = 0; i < editorTypeList.Length; i++)
            {
                var editorType = editorTypeList[i];
                var cutomAttr = Utility.GetAttribute<CustomClipEditor>(editorType);
                if (cutomAttr == null)
                    continue;

                if (cutomAttr.Target == type)
                    return editorType;
            }
            return null;
        }
        int FindInsertIndex(float beginFrame)
        {
            var propClips = SerializedObject.FindProperty(Track.PropNameClips);
            for(int i = 0; i < propClips.arraySize; i++)
            {
                var item = propClips.GetArrayElementAtIndex(i);
                var clip = (Clip)item.objectReferenceValue;
                if (clip == null)
                    continue;

                if(clip.BeginFrame > beginFrame)
                {
                    return i;
                }
            }

            return propClips.arraySize;
        }
        int FindIndex(ClipEditor[] array, Clip clip)
        {
            for(int i = 0; i < array.Length; i++)
            {
                if (array[i].Asset == clip)
                    return i;
            }
            return -1;
        }
        public List<Clip> Sort(SerializedProperty propClips)
        {
            List<Clip> list = new List<Clip>();
            var count = propClips.arraySize;
            for(int i = 0; i < count; i++)
            {
                var item = propClips.GetArrayElementAtIndex(i);
                list.Add((Clip)item.objectReferenceValue);
            }

            list.Sort((a, b) => { return Mathf.CeilToInt((b.BeginFrame - a.BeginFrame) * 1000f); });
            for (int i = 0; i < count; i++)
            {
                var item = propClips.GetArrayElementAtIndex(i);
                item.objectReferenceValue = list[i];
            }
            return list;
        }

        public void RemoveClip(ClipEditor editor)
        {
            SerializedObject.Update();

            var propClips = SerializedObject.FindProperty(Track.PropNameClips);
            var index = Utility.IndexOf(propClips, editor.Asset);
            if (index >= 0)
            {
                Utility.RemoveAt(propClips, index);
            }

            SerializedObject.ApplyModifiedProperties();

            for (int i = m_ClipEditors.Count - 1; i >= 0; i--)
            {
                var instance = m_ClipEditors[i];
                if (instance == editor)
                {
                    m_ClipEditors.RemoveAt(i);
                    break;
                }
            }

            Object.DestroyImmediate(editor.Asset, true);
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
        }
    }
}