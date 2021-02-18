using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    public abstract class Blackboard : MonoBehaviour
    {
        IReadOnlyList<SharedValue> m_Values;

        protected abstract IReadOnlyList<SharedValue> Collect();

        private void Awake()
        {
            m_Values = Collect();
        }

        public static void Bind<T>(IReadOnlyList<Blackboard> blackboards, SharedValueContext<T> context)
        {
            if (blackboards == null)
                return;

            for(int i = 0; i < blackboards.Count; i++)
            {
                if (blackboards[i] == null)
                    continue;

                if (blackboards[i].Bind(context))
                    return;
            }
        }

        internal bool Bind<T>(SharedValueContext<T> context)
        {
            if (m_Values == null)
                m_Values = Collect();

            for (int i = 0; i < m_Values.Count; i++)
            {
                var value = m_Values[i];
                if (value.PropertyName != context.PropertyName)
                    continue;
                if (value.ValueType != typeof(T))
                    continue;

                context.Bind((SharedValue<T>)value);
                return true;
            }

            return false;
        }
    }
}