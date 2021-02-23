using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public enum SharedValueType
    {
        Fixed,
        Blackboard,
        Runtime
    }

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

    public abstract class SharedValueContext
    {
        public static string PropNameSharedType { get { return SharedValueContext<object>.PropNameSharedType; } }
        public static string PropNameFixedValue { get { return SharedValueContext<object>.PropNameFixedValue; } }
        public static string PropNamePropertyName { get { return SharedValueContext<object>.PropNamePropertyName; } }

        public abstract System.Type ValueType { get; }
    }

    [System.Serializable]
    public abstract class SharedValueContext<T> : SharedValueContext
    {
        new public static string PropNameSharedType { get { return nameof(m_SharedType); } }
        new public static string PropNameFixedValue { get { return nameof(m_FixedValue); } }
        new public static string PropNamePropertyName { get { return nameof(m_PropertyName); } }

        [SerializeField] protected SharedValueType m_SharedType = SharedValueType.Fixed;
        [SerializeField] protected string m_PropertyName;
        [SerializeField] protected T m_FixedValue;

        SharedValue<T> m_Value;


        public string PropertyName { get { return m_PropertyName; } }
        public override System.Type ValueType { get { return typeof(T); } }

        public T Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        public void Bind(SharedValue<T> value)
        {
            m_Value = value;
        }

        T GetValue()
        {
            switch(m_SharedType)
            {
                case SharedValueType.Blackboard:
                case SharedValueType.Runtime:
                    return m_Value == null ? default(T) : m_Value.Value;
            }
            return m_FixedValue;
        }

        void SetValue(T value)
        {
            switch (m_SharedType)
            {
                case SharedValueType.Blackboard:
                case SharedValueType.Runtime:
                    Debug.Assert(m_Value != null, "Value is not bind.");
                    m_Value.Value = value;
                    return;
            }
            m_FixedValue = value;
        }
    }
}