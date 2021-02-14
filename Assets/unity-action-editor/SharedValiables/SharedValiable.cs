using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public interface ISharedValue
    {
        string Name { get; }
        object Value { get; set; }
        System.Type Type { get; }
    }

    public enum SharedValueType
    {
        Fixed,
        Flexible
    }

    public abstract class SharedObject : ScriptableObject
    {
        [SerializeField] protected string m_PropertyName;

        public static string PropNameValue { get { return SharedObject<float>.PropNameValue; } }
        public static string PropNamePropertyName { get { return nameof(m_PropertyName); } }

        public abstract ISharedValue SharedValue { get; }
    }

    public class SharedObject<T> : SharedObject
    {
        [SerializeField] T m_Value;
        new public static string PropNameValue { get { return nameof(m_Value); } }

        SharedValue<T>.Object m_SharedObject;

        public override ISharedValue SharedValue
        {
            get
            {
                if (m_SharedObject == null)
                {
                    m_SharedObject = SharedValue<T>.Create(m_PropertyName, m_Value);
                }
                m_SharedObject.Name = m_PropertyName;
                m_SharedObject.Value = m_Value;
                return m_SharedObject;
            }
        }
    }

    [System.Serializable]
    public abstract class SharedValue
    {
        [SerializeField] SharedValueType m_ValueType;
        [SerializeField] Blackborad m_Blackborad;

        public static string PropNameValueType { get { return nameof(m_ValueType); } }
        public static string PropNamePropertyName { get { return SharedValue<object>.PropNamePropertyName; } }
        public static string PropNameValue { get { return SharedValue<object>.PropNameValue; } }

        protected SharedValueType ValueType { get { return m_ValueType; } }
        protected Blackborad Blackboard { get { return m_Blackborad; } }
    }

    [System.Serializable]
    public class SharedValue<T> : SharedValue
    {
        public class Object : ISharedValue
        {
            string m_Name;
            T m_Value;
            public Object(string name, T value)
            {
                m_Name = name;
                m_Value = value;
            }

            public string Name { get { return m_Name; } set { m_Name = value; } }
            public T Value { get { return m_Value; } set { m_Value = value; } }
            object ISharedValue.Value { get { return m_Value; } set { m_Value = (T)value; } }
            public System.Type Type { get { return typeof(T); } }
        }

        public static Object Create(string name, T value)
        {
            return new Object(name, value);
        }

        new public static string PropNamePropertyName { get { return nameof(m_PropertyName); } }
        new public static string PropNameValue { get { return nameof(m_Value); } }

        [SerializeField] string m_PropertyName;
        [SerializeField] T m_Value;

        public bool HasValue
        {
            get
            {
                switch(ValueType)
                {
                    case SharedValueType.Fixed:
                        return m_Value != null;

                    case SharedValueType.Flexible:
                        if (Blackboard == null)
                            return false;
                        if(Blackboard.TryGetValue(m_PropertyName, out T _))
                        {
                            return true;
                        }
                        return false;
                }
                return false;
            }
        }

        public T Value
        {
            get
            {
                switch (ValueType)
                {
                    case SharedValueType.Fixed:
                        return m_Value;

                    case SharedValueType.Flexible:
                        if (Blackboard == null)
                            return default(T);

                        if (Blackboard.TryGetValue(m_PropertyName, out T value))
                        {
                            return default(T);
                        }

                        return value;
                }
                return default(T);
            }
        }
    }
}