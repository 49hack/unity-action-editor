using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ActionEditor
{
    public interface ISharedValue
    {
        string Name { get; }
        object Value { get; set; }
        System.Type Type { get; }
    }

    public interface IHasBlackboard
    {
        Blackborad Blackborad { get; }
    }

    public enum SharedValueType
    {
        Fixed,
        Blackboard
    }

    public abstract class SharedObject : ScriptableObject
    {
        [SerializeField] protected string m_PropertyName;

        public static string PropNameValue { get { return SharedObject<float>.PropNameValue; } }
        public static string PropNamePropertyName { get { return nameof(m_PropertyName); } }

        public string PropertyName { get { return m_PropertyName; } }
        public abstract ISharedValue SharedValue { get; }
        public abstract System.Type ValueType { get; }
    }

    public class SharedObject<T> : SharedObject
    {
        public class Handler : ISharedValue
        {
            System.Func<string> m_NameGetter;
            System.Action<T> m_ValueSetter;
            System.Func<T> m_ValueGetter;

            public Handler(System.Func<string> nameGetter, System.Action<T> setter, System.Func<T> getter)
            {
                m_NameGetter = nameGetter;
                m_ValueSetter = setter;
                m_ValueGetter = getter;
            }

            public string Name { get { return m_NameGetter(); }}
            public T Value { get { return m_ValueGetter(); } set { m_ValueSetter(value); } }
            object ISharedValue.Value { get { return Value; } set { Value = (T)value; } }
            public System.Type Type { get { return typeof(T); } }
        }

        [SerializeField] T m_Value;
        new public static string PropNameValue { get { return nameof(m_Value); } }

        Handler m_SharedObject;
        T m_TempValue;

        public override System.Type ValueType
        {
            get
            {
                return typeof(T);
            }
        }

        public override ISharedValue SharedValue
        {
            get
            {
                if (m_SharedObject == null)
                {
                    m_SharedObject = new Handler(GetName, SetValue, GetValue);
                }
                return m_SharedObject;
            }
        }

        string GetName()
        {
            return m_PropertyName;
        }
        T GetValue()
        {
            return m_Value;
        }
        void SetValue(T val)
        {
            m_Value = val;
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
        public static string PropNameBlackboard { get { return nameof(m_Blackborad); } }

        protected SharedValueType ValueType { get { return m_ValueType; } }
        protected Blackborad Blackboard { get { return m_Blackborad; } }
        public bool HasBlackboard { get { return m_Blackborad != null; } }
    }

    [System.Serializable]
    public class SharedValue<T> : SharedValue
    {
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

                    case SharedValueType.Blackboard:
                        if (Blackboard == null)
                            return false;
                        if(!Blackboard.TryGetValue(m_PropertyName, out T _))
                        {
                            return false;
                        }
                        return true;
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

                    case SharedValueType.Blackboard:
                        if (Blackboard == null)
                            return default(T);

                        if (!Blackboard.TryGetValue(m_PropertyName, out T value))
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