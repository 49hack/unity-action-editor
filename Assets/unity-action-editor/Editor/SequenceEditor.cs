using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [System.Serializable]
    public class SequenceEditor
    {
        [SerializeField] Window m_Owner;
        [SerializeField] SequenceBehaviour m_Asset;
        [SerializeField] Vector2 m_Scroll;

        SerializedObject m_SerializedObject;
        List<TrackEditor> m_TrackEditors;
        IReadOnlyList<Blackboard> m_BlackboardList;

        SequenceBehaviour Asset { get { return m_Asset; } }
        
        SerializedObject SerializedObject
        {
            get
            {
                if (m_SerializedObject == null)
                {
                    if (Asset == null)
                        return null;
                    m_SerializedObject = new SerializedObject(Asset);
                }

                return m_SerializedObject;
            }
        }

        public void Initialize(Window owner, SequenceBehaviour sequence)
        {
            m_Owner = owner;
            m_Asset = sequence;
            Enable();
        }

        public void Enable()
        {
            if (SerializedObject == null)
                return;

            if(m_TrackEditors == null)
                m_TrackEditors = new List<TrackEditor>();

            var propTracks = SerializedObject.FindProperty(SequenceBehaviour.PropNameTracks);
            for (int i = 0; i < propTracks.arraySize; i++)
            {
                var item = propTracks.GetArrayElementAtIndex(i);
                var track = (TrackBehaviour)item.objectReferenceValue;
                var editor = CreateTrackEditor(track);
                editor.OnChangeData += ChangeData;
                editor.OnRemoveTrack += RemoveTrack;
                m_TrackEditors.Add(editor);
            }
        }

        public void Disable()
        {
            if (m_TrackEditors != null)
            {
                for (int i = 0; i < m_TrackEditors.Count; i++)
                {
                    var editor = m_TrackEditors[i];
                    editor.Disable();
                }

                m_TrackEditors.Clear();
            }
        }

        public void Dispose()
        {
            Disable();
        }

        public void ChangeData()
        {
            SerializedObject.Update();
            SerializedObject.ApplyModifiedProperties();
            m_Owner.ChangeData();
        }

        public void DrawSetting()
        {
            SerializedObject.Update();

            using (new Utility.LabelWidthScope(80f))
            {
                var propTotalFrame = SerializedObject.FindProperty(SequenceBehaviour.PropNameTotalFrame);
                propTotalFrame.floatValue = EditorGUILayout.DelayedFloatField("Total Frame", propTotalFrame.floatValue);

                var propFrameRate = SerializedObject.FindProperty(SequenceBehaviour.PropNameFrameRate);
                propFrameRate.floatValue = EditorGUILayout.DelayedIntField("Frame Rate", (int)propFrameRate.floatValue);
            }

            SerializedObject.ApplyModifiedProperties();
        }

        public void Draw(Navigator navigator, float totalFrame, float currentFrame, IReadOnlyList<Blackboard> blackboards)
        {
            if (SerializedObject == null)
                return;

            m_BlackboardList = blackboards;

            using (var scroll = new EditorGUILayout.ScrollViewScope(m_Scroll))
            {
                m_Scroll = scroll.scrollPosition;

                for (int i = 0; i < m_TrackEditors.Count; i++)
                {
                    m_TrackEditors[i].Draw(navigator, totalFrame, currentFrame, m_BlackboardList);
                }

                if (GUILayout.Button("Add Track", GUILayout.Width(Utility.HeaderWidth)))
                {
                    ShowAddTrackContextMenu();
                }
            }
        }

        void ShowAddTrackContextMenu()
        {
            var trackTypeList = Utility.GetSubClasses<TrackBehaviour>();

            GenericMenu menu = new GenericMenu();

            for(int i = 0; i < trackTypeList.Length; i++)
            {
                var type = trackTypeList[i];
                var name = type.Name;
                var nameAttr = Utility.GetAttribute<MenuTitle>(type);
                if(nameAttr != null)
                {
                    name = nameAttr.Name;
                }
                menu.AddItem(new GUIContent(name), false, OnCreateTrack, type);
            }

            menu.ShowAsContext();
        }

        void OnCreateTrack(object trackType)
        {
            var type = (System.Type)trackType;

            SerializedObject.Update();

            var propTracks = SerializedObject.FindProperty(SequenceBehaviour.PropNameTracks);
            var count = propTracks.arraySize;
            propTracks.InsertArrayElementAtIndex(count);
            propTracks.arraySize = count + 1;
            var propTrack = propTracks.GetArrayElementAtIndex(count);

            var track = ScriptableObject.CreateInstance(type);
            track.name = type.Name;

            var so = new SerializedObject(track);
            var propName = so.FindProperty(TrackBehaviour.PropNameTrackName);
            propName.stringValue = track.name;
            so.ApplyModifiedProperties();

            AssetDatabase.AddObjectToAsset(track, Asset);
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();

            propTrack.objectReferenceValue = track;

            var trackEditor = CreateTrackEditor((TrackBehaviour)track);
            trackEditor.OnChangeData += ChangeData;
            trackEditor.OnRemoveTrack += RemoveTrack;
            m_TrackEditors.Add(trackEditor);

            SerializedObject.ApplyModifiedProperties();

            ChangeData();
        }

        TrackEditor CreateTrackEditor(TrackBehaviour track)
        {
            var customEditorType = GetCustomTrackEditor(track.GetType());
            if (customEditorType == null)
            {
                return TrackEditor.Create(typeof(TrackEditor), track);
            }

            return TrackEditor.Create(customEditorType, track);
        }

        System.Type GetCustomTrackEditor(System.Type type)
        {
            var editorTypeList = Utility.GetSubClasses<TrackEditor>();
            for(int i = 0; i < editorTypeList.Length; i++)
            {
                var editorType = editorTypeList[i];
                var cutomAttr = Utility.GetAttribute<CustomTrackEditor>(editorType);
                if (cutomAttr == null)
                    continue;

                if (cutomAttr.Target == type)
                    return editorType;
            }
            return null;
        }

        public void RemoveTrack(TrackEditor editor)
        {
            SerializedObject.Update();

            var propTracks = SerializedObject.FindProperty(SequenceBehaviour.PropNameTracks);
            var index = Utility.IndexOf(propTracks, editor.Asset);
            if(index >= 0)
            {
                Utility.RemoveAt(propTracks, index);
            }

            SerializedObject.ApplyModifiedProperties();

            for (int i = m_TrackEditors.Count - 1; i >= 0; i--)
            {
                var instance = m_TrackEditors[i];
                if (instance == editor)
                {
                    m_TrackEditors.RemoveAt(i);
                    break;
                }
            }

            Object.DestroyImmediate(editor.Asset, true);
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
        }
    }
}