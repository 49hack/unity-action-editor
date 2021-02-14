﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [System.Serializable]
    public class SequenceEditor
    {
        [SerializeField] Window m_Owner;
        [SerializeField] Sequence m_Asset;
        [SerializeField] List<TrackEditor> m_TrackEditors = new List<TrackEditor>();
        [SerializeField] Vector2 m_Scroll;

        SerializedObject m_SerializedObject;

        Sequence Asset { get { return m_Asset; } }

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

        public SequenceEditor(Window owner, Sequence sequence)
        {
            m_Owner = owner;
            m_Asset = sequence;

            var propTracks = SerializedObject.FindProperty(Sequence.PropNameTracks);
            for(int i = 0; i < propTracks.arraySize; i++)
            {
                var item = propTracks.GetArrayElementAtIndex(i);
                var track = (Track)item.objectReferenceValue;
                var editor = CreateTrackEditor(track.GetType());
                editor.Initialize(this, track);
                m_TrackEditors.Add(editor);
            }
        }

        public void ChangeData()
        {
            m_Owner.ChangeData();
        }

        public void DrawSetting()
        {
            SerializedObject.Update();

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                var propTotalFrame = SerializedObject.FindProperty(Sequence.PropNameTotalFrame);
                propTotalFrame.floatValue = EditorGUILayout.DelayedFloatField("Total Frame", propTotalFrame.floatValue);

                var propFrameRate = SerializedObject.FindProperty(Sequence.PropNameFrameRate);
                propFrameRate.floatValue = EditorGUILayout.DelayedIntField("Frame Rate", (int)propFrameRate.floatValue);
            }

            SerializedObject.ApplyModifiedProperties();
        }

        public void Draw(Navigator navigator, float totalFrame, float currentFrame)
        {
            if (SerializedObject == null)
                return;

            using (var scroll = new EditorGUILayout.ScrollViewScope(m_Scroll))
            {
                m_Scroll = scroll.scrollPosition;

                for (int i = 0; i < m_TrackEditors.Count; i++)
                {
                    m_TrackEditors[i].Draw(navigator, totalFrame, currentFrame);
                }

                if (GUILayout.Button("Add Track", GUILayout.Width(Utility.HeaderWidth)))
                {
                    ShowAddTrackContextMenu();
                }
            }
        }

        void ShowAddTrackContextMenu()
        {
            var trackTypeList = Utility.GetSubClasses<Track>();

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

            var propTracks = SerializedObject.FindProperty(Sequence.PropNameTracks);
            var count = propTracks.arraySize;
            propTracks.InsertArrayElementAtIndex(count);
            propTracks.arraySize = count + 1;
            var propTrack = propTracks.GetArrayElementAtIndex(count);

            var track = ScriptableObject.CreateInstance(type);
            track.name = type.Name;

            var so = new SerializedObject(track);
            var propName = so.FindProperty(Track.PropNameTrackName);
            propName.stringValue = track.name;
            so.ApplyModifiedProperties();

            AssetDatabase.AddObjectToAsset(track, Asset);
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();

            propTrack.objectReferenceValue = track;

            var trackEditor = CreateTrackEditor(type);
            trackEditor.Initialize(this, (Track)track);
            m_TrackEditors.Add(trackEditor);

            SerializedObject.ApplyModifiedProperties();

            ChangeData();
        }

        TrackEditor CreateTrackEditor(System.Type type)
        {
            var customEditorType = GetCustomTrackEditor(type);
            if(customEditorType == null)
                return ScriptableObject.CreateInstance(typeof(TrackEditor)) as TrackEditor;

            return ScriptableObject.CreateInstance(customEditorType) as TrackEditor;
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

            var propTracks = SerializedObject.FindProperty(Sequence.PropNameTracks);
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