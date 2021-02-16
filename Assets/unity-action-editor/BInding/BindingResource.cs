using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public interface IBindingProvider
    {
        bool IsEnable { get; }
        Object Find(string key, System.Type type, int index);
        bool ToSerializeData(Object obj, out (string key, int index) result);
    }

    [System.Serializable]
    public class BindingResource : IBindingProvider
    {
        public static string PropNameEntries { get { return nameof(m_Entries); } }

        [SerializeField] BindingEntry[] m_Entries = new BindingEntry[0];

        public bool IsEnable { get { return true; } }

        public Object Find(string key, System.Type type, int index)
        {
            for (int i = 0; i < m_Entries.Length; i++)
            {
                var entry = m_Entries[i];
                if (entry.Key != key)
                    continue;

                var currentIndex = 0;
                for (int oi = 0; oi < entry.Objects.Count; oi++)
                {
                    var obj = entry.Objects[oi];
                    if (obj == null)
                        continue;

                    if (!type.IsAssignableFrom(obj.GetType()))
                        continue;

                    if (currentIndex != index)
                    {
                        currentIndex++;
                        continue;
                    }

                    return obj;
                }
            }

            return null;
        }

        public bool ToSerializeData(Object target, out (string key, int index) result)
        {
            result = ("", 0);

            if (target == null)
                return true;

            for (int ei = 0; ei < m_Entries.Length; ei++)
            {
                var currentIndex = 0;
                var entry = m_Entries[ei];
                for(int oi = 0; oi < entry.Objects.Count; oi++)
                {
                    var obj = entry.Objects[oi];
                    if (obj == null)
                        continue;

                    var objType = obj.GetType();
                    if(objType == target.GetType())
                    {
                        currentIndex++;
                    }
                    if (obj != target)
                    {
                        continue;
                    }

                    result = (entry.Key, currentIndex - 1);
                    return true;
                }
            }

            return false;
        }
    }

    [System.Serializable]
    public class BindingEntry
    {
        public static string PropNameKey { get { return nameof(m_Key); } }
        public static string PropNameObjects { get { return nameof(m_Objects); } }

        [SerializeField] string m_Key;
        [SerializeField] List<Object> m_Objects = new List<Object>();

        public string Key { get { return m_Key; } }
        public IReadOnlyList<Object> Objects { get { return m_Objects; } }

        public void SetKey(string key)
        {
            m_Key = key;
        }
        public void AddObject(Object obj)
        {
            if (m_Objects.Contains(obj))
                return;
            m_Objects.Add(obj);
        }
    }

    [System.Serializable]
    public class Binding
    {
        public static string PropNameKey { get { return nameof(m_Key); } }
        public static string PropNameIndex { get { return nameof(m_Index); } }

        [SerializeField] string m_Key;
        [SerializeField] int m_Index;

        public string Key { get { return m_Key; } }
        public int Index { get { return m_Index; } }
    }
}