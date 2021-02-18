using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [System.Serializable]
    public abstract class SharedValue
    {
        public static string PropNamePropertyName { get { return nameof(m_PropertyName); } }
        public static string PropNameValue { get { return SharedValue<object>.PropNameValue; } }

        [SerializeField] string m_PropertyName;

        public SharedValue() { }
        public SharedValue(string propertyName)
        {
            m_PropertyName = propertyName;
        }

        internal string PropertyName { get { return m_PropertyName; } }
        public abstract System.Type ValueType{ get; }
    }

    [System.Serializable]
    public abstract class SharedValue<T> : SharedValue
    {
        new public static string PropNameValue { get { return nameof(m_Value); } }

        [SerializeField] T m_Value;

        public T Value { get { return m_Value; } set { m_Value = value; } }
        public override System.Type ValueType { get { return typeof(T); } }

        public SharedValue() { }
        public SharedValue(string propertyName) : base(propertyName) { }
    }

    [System.Serializable]
    public class SharedValueContext<T>
    {
        public static string PropNamePropertyName { get { return nameof(m_PropertyName); } }

        [SerializeField] string m_PropertyName;

        SharedValue<T> m_Value;

        public string PropertyName { get { return m_PropertyName; } }

        public T Value
        {
            get
            {
                return m_Value == null ? default(T) : m_Value.Value;
            }
            set
            {
                Debug.Assert(m_Value != null, "Value is not bind.");
                m_Value.Value = value;
            }
        }

        public void Bind(SharedValue<T> value)
        {
            m_Value = value;
        }
    }
}