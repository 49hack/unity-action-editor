using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public class BindingHolder : MonoBehaviour, IBindingProvider
    {
        public static string PropNameResource { get { return nameof(m_Resource); } }

        [SerializeField] BindingResource m_Resource = new BindingResource();

        public bool IsEnable { get { return m_Resource.IsEnable; } }

        public Object Find(string key, System.Type type, int index)
        {
            return m_Resource.Find(key, type, index);
        }

        public bool ToSerializeData(Object obj, out (string key, int index) result)
        {
            return m_Resource.ToSerializeData(obj, out result);
        }
    }
}