using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
namespace ActionEditor
{
    [CustomEditor(typeof(BindingHolder))]
    public class BindingHolderInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button(new GUIContent("自動設定", "このオブジェクト以下のGameObjectから自動収集します")))
            {
                AutoCollect((BindingHolder)target);
            }

            base.OnInspectorGUI();
        }

        void AutoCollect(BindingHolder root)
        {
            serializedObject.Update();

            var resourceProp = serializedObject.FindProperty(BindingHolder.PropNameResource);
            var components = root.gameObject.GetComponentsInChildren<Component>(true);

            var cache = new Dictionary<string, BindingEntry>();
            for(int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                var path = Utility.TransformToPath(component.transform, "", root.transform);
                path = "Root" + (string.IsNullOrEmpty(path) ? "" : "/") + path;

                if(!cache.TryGetValue(path, out var entry))
                {
                    entry = new BindingEntry();
                    entry.SetKey(path);
                    cache.Add(path, entry);
                }

                entry.AddObject(component);
            }

            var entriesProp = resourceProp.FindPropertyRelative(BindingResource.PropNameEntries);
            entriesProp.arraySize = 0;
            var index = 0;
            foreach(var pair in cache.Reverse())
            {
                var key = pair.Key;
                var entry = pair.Value;

                entriesProp.InsertArrayElementAtIndex(index);
                var item = entriesProp.GetArrayElementAtIndex(index);
                var nameProp = item.FindPropertyRelative(BindingEntry.PropNameKey);
                nameProp.stringValue = key;

                var objsProp = item.FindPropertyRelative(BindingEntry.PropNameObjects);
                objsProp.arraySize = 0;
                for (var ei = 0; ei < entry.Objects.Count; ei++)
                {
                    var obj = entry.Objects[ei];
                    objsProp.InsertArrayElementAtIndex(ei);
                    var objItem = objsProp.GetArrayElementAtIndex(ei);
                    objItem.objectReferenceValue = obj;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }


    }
}