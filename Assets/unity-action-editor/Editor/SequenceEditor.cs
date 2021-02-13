using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionEditor
{
    [System.Serializable]
    public class SequenceEditor
    {
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


        public SequenceEditor(Sequence sequence)
        {
            m_Asset = sequence;

            var trakcsProp = SerializedObject.FindProperty(Sequence.PropNameTracks);
            for (int i = 0; i < trakcsProp.arraySize; i++)
            {
                var trackProp = trakcsProp.GetArrayElementAtIndex(i);
                var trackEditor = new TrackEditor(this, (Track)trackProp.objectReferenceValue);
                m_TrackEditors.Add(trackEditor);
            }
        }

        public void OnGUI(Navigator navigator, float totalFrame, float currentFrame)
        {
            if (SerializedObject == null)
                return;

            using (var scroll = new EditorGUILayout.ScrollViewScope(m_Scroll))
            {
                m_Scroll = scroll.scrollPosition;

                for (int i = 0; i < m_TrackEditors.Count; i++)
                {
                    m_TrackEditors[i].OnGUI(navigator, totalFrame, currentFrame);
                }

                if (GUILayout.Button("Add Track"))
                {
                    AddTrack();
                }
            }
        }

        void AddTrack()
        {
            SerializedObject.Update();


            var propTracks = SerializedObject.FindProperty(Sequence.PropNameTracks);
            var count = propTracks.arraySize;
            propTracks.InsertArrayElementAtIndex(count);
            propTracks.arraySize = count + 1;
            var propTrack = propTracks.GetArrayElementAtIndex(count);

            var track = ScriptableObject.CreateInstance(typeof(Track));
            track.name = typeof(Track).Name;

            AssetDatabase.AddObjectToAsset(track, Asset);
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();

            propTrack.objectReferenceValue = track;

            var trackEditor = new TrackEditor(this, (Track)track);
            m_TrackEditors.Add(trackEditor);

            SerializedObject.ApplyModifiedProperties();
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