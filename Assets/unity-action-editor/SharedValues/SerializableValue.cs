using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [System.Serializable]
    public class SerializableValue
    {
        public enum PropertyType
        {
            
        }

        [SerializeField] string m_ValueString;


        public object GetValue(System.Type type)
        {
            return null;
        }

        public void SetValue(object value)
        {
            
        }

        //PropertyType TypeToPropertyType(System.Type type)
        //{
        //    if(type == typeof(int))
        //    {

        //    }
        //}
    }
}