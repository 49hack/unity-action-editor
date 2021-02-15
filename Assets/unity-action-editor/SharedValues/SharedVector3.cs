using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEditor
{
    [MenuTitle("Vector3")]
    public class SharedVector3 : SharedObject<Vector3>
    {
    }

    [System.Serializable]
    public class SharedVector3Context : SharedValue<Vector3>
    {
    }
}