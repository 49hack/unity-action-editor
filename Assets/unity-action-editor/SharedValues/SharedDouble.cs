using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Double")]
    public class SharedDouble : SharedObject<double>
    {
    }

    [System.Serializable]
    public class SharedDoubleContext : SharedValue<float>
    {
    }
}