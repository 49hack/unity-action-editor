using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Float")]
    public class SharedFloat : SharedObject<float>
    {
    }

    [System.Serializable]
    public class SharedFloatContext : SharedValue<float>
    {
    }
}