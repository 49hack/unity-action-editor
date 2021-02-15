using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class Blackborad : ScriptableObject
    {
        [SerializeField] SharedObject[] m_SharedObjects = new SharedObject[0];

        public static string PropNameSharedObjects { get { return nameof(m_SharedObjects); } }
        List<string> m_WorkList;
        
        public string[] GetPropertyNames(System.Type type)
        {
            if (m_WorkList == null)
                m_WorkList = new List<string>();

            m_WorkList.Clear();
            for(int i = 0; i < m_SharedObjects.Length; i++)
            {
                var obj = m_SharedObjects[i];
                if (type != null && obj.ValueType != type)
                    continue;

                m_WorkList.Add(m_SharedObjects[i].PropertyName);
            }

            return m_WorkList.ToArray();
        }

        public bool TryGetValue<T>(string name, out T value)
        {
            value = default(T);

            for(int i = 0; i < m_SharedObjects.Length; i++)
            {
                var sharedValue = m_SharedObjects[i].SharedValue;
                if (sharedValue.Name != name)
                    continue;
                if (sharedValue.Type != typeof(T) && !sharedValue.Type.IsInstanceOfType(typeof(T)))
                    continue;
                value = (T)sharedValue.Value;
                return true;
            }

            return false;
        }
    }
}