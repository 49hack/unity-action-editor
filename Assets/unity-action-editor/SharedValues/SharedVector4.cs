using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Vector4")]
    public class SharedVector4 : SharedObject<Vector4>
    {
        
    }

    [System.Serializable]
    public class SharedVector4Context : SharedValue<Vector4>
    {
    }
}