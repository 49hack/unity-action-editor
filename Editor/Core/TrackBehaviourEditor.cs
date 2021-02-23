using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ActionEditor
{
    [System.Serializable]
    public class TrackBehaviourEditor
    {
        public static TrackBehaviourEditor Create(System.Type editorType, TrackBehaviour track)
        {
            var editor = System.Activator.CreateInstance(editorType) as TrackBehaviourEditor;
            editor.Initialize(track);
            return editor;
        }

        [SerializeField] TrackBehaviour m_Track;

        SerializedObject m_SerializedObject;
        List<Vector3> m_IndicatePointList = new List<Vector3>();
        Vector3[] m_IndicatePoints;
        List<ClipBehaviourEditor> m_ClipEditors;
        IReadOnlyList<Blackboard> m_BlackboardList;
        ActionEditorTime m_EditorTime;

        public event System.Action OnChangeData;
        public event System.Action<TrackBehaviourEditor> OnRemoveTrack;

        protected float FrameRate { get { return m_EditorTime.FrameRate; } }

        #region Virtual
        protected virtual Color BackgroundColor { get { return new Color(0f, 0f, 0f, 0.5f); } }

        protected virtual void DrawContents(Rect rect, SerializedObject serializedObject)
        {
            var propName = serializedObject.FindProperty(TrackBehaviour.PropNameTrackName);
            var nameRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, EditorGUIUtility.singleLineHeight);
            propName.stringValue = EditorGUI.TextField(nameRect, propName.stringValue);
        }

        protected virtual void AddContextMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Delete"), false, OnClickDelete, null);
        }
        #endregion // Virtual

        public TrackBehaviour Asset { get { return m_Track; } }

        protected SerializedObject SerializedObject
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

        void Initialize(TrackBehaviour track)
        {
            m_Track = track;
            m_ClipEditors = new List<ClipBehaviourEditor>();
            Enable();
        }

        public void Enable()
        {
            if (SerializedObject == null)
                return;

            if (m_ClipEditors == null)
                m_ClipEditors = new List<ClipBehaviourEditor>();

            var propClips = SerializedObject.FindProperty(TrackBehaviour.PropNameClips);
            for (int i = 0; i < propClips.arraySize; i++)
            {
                var item = propClips.GetArrayElementAtIndex(i);
                var clip = (ClipBehaviour)item.objectReferenceValue;
                var editor = CreateClipEditor(clip);
                editor.OnRemoveClip += RemoveClip;
                editor.OnChangeData += ChangeData;
                m_ClipEditors.Add(editor);
            }
        }

        public void Disable()
        {
            for(int i = 0; i < m_ClipEditors.Count; i++)
            {
                var editor = m_ClipEditors[i];
                editor.Disable();
            }
            m_ClipEditors.Clear();
        }

        public void Dispose()
        {
            Disable();
        }

        public void ChangeData()
        {
            SerializedObject?.ApplyModifiedProperties();
            OnChangeData?.Invoke();
        }

        void OnClickDelete(object obj)
        {
            var dump = m_ClipEditors.ToArray();
            for (int i = 0; i < dump.Length; i++)
            {
                RemoveClip(dump[i]);
            }

            OnRemoveTrack?.Invoke(this);
        }

        public void Draw(ActionEditorTime editorTime, float totalFrame, float currentFrame, IReadOnlyList<Blackboard> blackboards)
        {
            if (Asset == null)
                return;

            if (SerializedObject == null)
                return;

            m_BlackboardList = blackboards;
            m_EditorTime = editorTime;

            var rect = GUILayoutUtility.GetRect(1f, 64f, GUILayout.ExpandWidth(true));

            SerializedObject.Update();

            OnGUITrack(rect);
            OnGUIClip(rect, editorTime, totalFrame, currentFrame);

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
                case EventType.MouseDown:
                    if (rect.Contains(e.mousePosition) && e.button == 0)
                    {
                        Selection.objects = new Object[] { Asset };
                    }
                    break;
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

        void OnGUIClip(Rect fullRect, ActionEditorTime editorTime, float totalFrame, float currentFrame)
        {
            var rect = new Rect(editorTime.Rect.x, fullRect.y + Utility.Space, editorTime.Rect.width, fullRect.height - Utility.Space * 2f);

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

                            var intervalFrame = Utility.CalculateFrameInterval(editorTime.MinFrame, editorTime.MaxFrame, xMin, xMax, 1f / 48f);
                            if (1 < intervalFrame)
                            {
                                var subIntervalFrame = Utility.CalculateFrameInterval(editorTime.MinFrame, editorTime.MaxFrame, xMin, xMax, 1f / 6f);
                                Utility.Indicate(editorTime.MinFrame, editorTime.MaxFrame, 0, Mathf.Max(subIntervalFrame, 1), xMin, xMax, (x, f) =>
                                {
                                    m_IndicatePointList.Add(new Vector3(x, yMin));
                                    m_IndicatePointList.Add(new Vector3(x, yMax));
                                });
                            }

                            Utility.Indicate(editorTime.MinFrame, editorTime.MaxFrame, 0, Mathf.Max(intervalFrame, 1), xMin, xMax, (x, f) => {
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
                            if (editorTime.MinFrame <= currentFrame && editorTime.MaxFrame >= currentFrame)
                            {
                                float x = Utility.Remap(currentFrame, editorTime.MinFrame, editorTime.MaxFrame, xMin, xMax);
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
                clipInfos.Add(new ClipViewInfo(current, prev, next, editorTime, totalFrame, rect));
            }
            for (int i = 0; i < m_ClipEditors.Count; i++)
            {
                m_ClipEditors[i].Draw(rect, clipInfos[i], editorTime, totalFrame, currentFrame, m_BlackboardList);
            }

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (rect.Contains(e.mousePosition) && e.button == 1)
                    {
                        var beginFrame = Utility.Remap(e.mousePosition.x, rect.xMin, rect.xMax, editorTime.MinFrame, editorTime.MaxFrame);
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

            var clipTypeList = Utility.GetSubClasses<ClipBehaviour>();
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

            var propClips = SerializedObject.FindProperty(TrackBehaviour.PropNameClips);
            propClips.InsertArrayElementAtIndex(index);
            var propClip = propClips.GetArrayElementAtIndex(index);

            var clip = (ClipBehaviour)ScriptableObject.CreateInstance(param.type);
            clip.name = param.type.Name;
            clip.PostCreate(param.beginFrame);


            SerializedObject.ApplyModifiedProperties();

            AssetDatabase.AddObjectToAsset(clip, Asset);
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();

            propClip.objectReferenceValue = clip;

            SerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();

            var editor = CreateClipEditor(clip);
            editor.OnRemoveClip += RemoveClip;
            editor.OnChangeData += ChangeData;
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

        ClipBehaviourEditor CreateClipEditor(ClipBehaviour clip)
        {
            var cutomEditor = GetCustomClipEditor(clip.GetType());
            if (cutomEditor == null)
                return ClipBehaviourEditor.Create(typeof(ClipBehaviourEditor), clip);

            return ClipBehaviourEditor.Create(cutomEditor, clip);
        }

        System.Type GetCustomClipEditor(System.Type type)
        {
            var editorTypeList = Utility.GetSubClasses<ClipBehaviourEditor>();
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
            var propClips = SerializedObject.FindProperty(TrackBehaviour.PropNameClips);
            for(int i = 0; i < propClips.arraySize; i++)
            {
                var item = propClips.GetArrayElementAtIndex(i);
                var clip = (ClipBehaviour)item.objectReferenceValue;
                if (clip == null)
                    continue;

                if(clip.BeginFrame > beginFrame)
                {
                    return i;
                }
            }

            return propClips.arraySize;
        }
        int FindIndex(ClipBehaviourEditor[] array, ClipBehaviour clip)
        {
            for(int i = 0; i < array.Length; i++)
            {
                if (array[i].Asset == clip)
                    return i;
            }
            return -1;
        }
        public List<ClipBehaviour> Sort(SerializedProperty propClips)
        {
            List<ClipBehaviour> list = new List<ClipBehaviour>();
            var count = propClips.arraySize;
            for(int i = 0; i < count; i++)
            {
                var item = propClips.GetArrayElementAtIndex(i);
                list.Add((ClipBehaviour)item.objectReferenceValue);
            }

            list.Sort((a, b) => { return Mathf.CeilToInt((b.BeginFrame - a.BeginFrame) * 1000f); });
            for (int i = 0; i < count; i++)
            {
                var item = propClips.GetArrayElementAtIndex(i);
                item.objectReferenceValue = list[i];
            }
            return list;
        }

        public void RemoveClip(ClipBehaviourEditor editor)
        {
            SerializedObject.Update();

            var propClips = SerializedObject.FindProperty(TrackBehaviour.PropNameClips);
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