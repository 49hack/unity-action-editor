using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class Blackborad : ScriptableObject
    {
        [SerializeField] SharedObject[] m_SharedObjects = new SharedObject[0];

        public static string PropNameSharedObjects { get { return nameof(m_SharedObjects); } }

        List<ISharedValue> m_ShareValueList = new List<ISharedValue>();

        public void Prepare()
        {
            m_ShareValueList.Clear();

            for(int i = 0; i < m_SharedObjects.Length; i++)
            {
                m_ShareValueList.Add(m_SharedObjects[i].SharedValue);
            }
        }

        public bool TryGetValue<T>(string name, out T value)
        {
            value = default(T);

            for (int i = 0; i < m_ShareValueList.Count; i++)
            {
                var sharedValue = m_ShareValueList[i];
                if (sharedValue.Name != name)
                    continue;
                if (!sharedValue.Type.IsInstanceOfType(typeof(T)))
                    continue;
                value = (T)sharedValue.Value;
                return true;
            }

            return false;
        }
    }
}